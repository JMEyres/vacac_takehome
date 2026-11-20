using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
/// <summary>
/// Manages ghost preview instantiation, ground anchor alignment, port snapping, and runtime placement/linking of modular factory components.
/// </summary>
public class PlacementController : MonoBehaviour
{
    public List<GameObject> prefabs = new List<GameObject>();
    public List<Placeable> placedObjects = new List<Placeable>();
    public float yawStep = 15.0f;
    public float snapRadius = 1.0f;
    private GameObject currentPreview;
    float currentYaw = 0.0f;
    private Placeable currentPlaceable;
    private Port currentPort, targetPort;
    int selectedIndex = -1;
    int snapIndex = -1;
    bool snappedToExit = false;

    [SerializeField] private LayerMask groundLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Register pre-placed scene objects and link their port references so runtime pieces can snap to them
        placedObjects.AddRange(FindObjectsByType<Placeable>());

        foreach (Placeable obj in placedObjects)
        {
            if (obj.TryGetComponent<ConveyorMovement>(out ConveyorMovement belt) && belt.nextConveyor != null)
            {
                Placeable nextPlaceable = belt.nextConveyor.GetComponent<Placeable>();
                if (nextPlaceable != null && obj.exits.Count > 0 && nextPlaceable.entries.Count > 0)
                {
                    obj.exits[0].isOccupied = true;
                    obj.exits[0].connectedPort = nextPlaceable.entries[0];

                    nextPlaceable.entries[0].isOccupied = true;
                    nextPlaceable.entries[0].connectedPort = obj.exits[0];
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame){ 
            SelectPlaceable(0);
        }
        if(Keyboard.current != null && Keyboard.current.digit2Key.wasPressedThisFrame){
            SelectPlaceable(1);
        }
        if(Keyboard.current != null && Keyboard.current.digit3Key.wasPressedThisFrame){
            SelectPlaceable(2);
        }
        if(Keyboard.current != null && Keyboard.current.digit4Key.wasPressedThisFrame){
            SelectPlaceable(3);
        }
        if(Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame){
            selectedIndex = -1;
        }

        if (selectedIndex == -1)
        {
            if (currentPreview != null)
            {
                Destroy(currentPreview);
                currentPreview = null;
                currentPlaceable = null;
            }
            return;
        }

        if(currentPreview == null)
        {
            // Disable colliders and machine logic on the ghost preview so it doesn't interfere with raycasts or active items
            currentPreview = Instantiate(prefabs[selectedIndex]);
            currentPreview.transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);
            currentPlaceable = currentPreview.GetComponent<Placeable>();
            foreach (Collider col in currentPreview.GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }

            foreach (MonoBehaviour script in currentPreview.GetComponents<MonoBehaviour>())
            {
                if (script != currentPlaceable) script.enabled = false;
            }
            
        }

        float scrollY = Mouse.current != null ? Mouse.current.scroll.ReadValue().y : 0f;

        if(scrollY > 0f){currentYaw += yawStep; currentYaw %= 360f;}
        if(scrollY < 0f){currentYaw -= yawStep; currentYaw %= 360f;}

        currentPreview.transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);

        Ray cursorPos = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Offset by groundAnchor to sit flush on the floor
        if (Physics.Raycast(cursorPos, out RaycastHit hit, 100f, groundLayer))
        {
            currentPreview.transform.position = hit.point;
            currentPreview.transform.position += currentPreview.transform.position - currentPlaceable.groundAnchor.transform.position;
        }
        
        currentPort = null;
        targetPort = null;
        snappedToExit = false;
        snapIndex = -1;
        float closestDist = snapRadius;

        // Find the closest open port to snap to (checks both entry-to-exit and exit-to-entry)
        foreach(Placeable obj in placedObjects)
        {
            for (int i = 0; i < currentPlaceable.entries.Count; i++)
            {
                foreach (Port theirExit in obj.exits)
                {
                    if(!theirExit.isOccupied)
                    {
                        float dist = HorizontalDistance(currentPlaceable.entries[i].transform.position, theirExit.transform.position);
                         if (dist < closestDist)
                        {
                            closestDist = dist;
                            currentPort = currentPlaceable.entries[i];
                            targetPort = theirExit;
                            snapIndex = i;
                            snappedToExit = false;
                        }
                    }
                }
            }
            for (int i = 0; i < currentPlaceable.exits.Count; i++)
            {
                foreach (Port theirEntry in obj.entries)
                {
                    if (!theirEntry.isOccupied)
                    {
                        float dist = HorizontalDistance(currentPlaceable.exits[i].transform.position, theirEntry.transform.position);
                         if (dist < closestDist)
                        {
                            closestDist = dist;
                            currentPort = currentPlaceable.exits[i];
                            targetPort = theirEntry;
                            snapIndex = i;
                            snappedToExit = true;
                        }
                    }
                }
            }
        }

        if(currentPort != null) {currentPreview.transform.position += targetPort.transform.position - currentPort.transform.position;}

        // Prevent clicking toolbar UI buttons from accidentally placing an object underneath the cursor
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if(Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
            GameObject spawnedPlaceable = Instantiate(prefabs[selectedIndex], currentPreview.transform.position, currentPreview.transform.rotation);
            Placeable newPlaceable = spawnedPlaceable.GetComponent<Placeable>();
            // Bind port references on the newly spawned instance and mark both sockets occupied
            if(currentPort != null)
            {
                Port newPort = snappedToExit ? newPlaceable.exits[snapIndex] : newPlaceable.entries[snapIndex];
                newPort.isOccupied = true;
                targetPort.isOccupied = true;

                newPort.connectedPort = targetPort;
                targetPort.connectedPort = newPort;
            }

            if (newPlaceable != null) placedObjects.Add(newPlaceable);
            selectedIndex = -1;
            currentYaw = 0.0f;
        }
        
    }

    public void SelectPlaceable(int index)
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
            currentPlaceable = null;
        }
        currentYaw = 0.0f;
        selectedIndex = index; 
    }
    
    // Compare XZ plane distance so incline/decline ports at different heights still snap cleanly
    float HorizontalDistance(Vector3 a, Vector3 b)
    {
        return Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
    }
}
