using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlacementController : MonoBehaviour
{
    public List<GameObject> beltPrefabs = new List<GameObject>();
    public List<GameObject> spawnerPrefabs = new List<GameObject>();
    public List<ConveyorMovement> placedConveyors = new List<ConveyorMovement>();
    public List<Spawner> placedSpawners = new List<Spawner>();
    public float yawStep = 15.0f;
    public float snapRadius = 1.0f;
    private GameObject currentPreview;
    float currentYaw = 0.0f;
    private ConveyorMovement prevConveyor, nextConveyor, currentConveyor;
    private Spawner currentSpawner;
    int selectedIndex = -1;
    bool isSpawner = false;

    [SerializeField] private LayerMask groundLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        placedConveyors.AddRange(FindObjectsByType<ConveyorMovement>());
        placedSpawners.AddRange(FindObjectsByType<Spawner>());

        foreach (ConveyorMovement belt in placedConveyors)
        {
            if (belt.nextConveyor != null)
            {
                belt.nextConveyor.prevConveyor = belt;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame){ 
            SelectBelt(0);
        }
        if(Keyboard.current != null && Keyboard.current.digit2Key.wasPressedThisFrame){
            SelectBelt(1);
        }
        if(Keyboard.current != null && Keyboard.current.digit3Key.wasPressedThisFrame){
            SelectBelt(2);
        }
        if(Keyboard.current != null && Keyboard.current.digit4Key.wasPressedThisFrame){
            SelectBelt(3);
        }
        if(Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame){
            selectedIndex = -1;
        }

        if (selectedIndex == -1)
        {
            if (currentPreview != null)
            {
                Destroy(currentPreview);
                currentPreview = null;
                currentConveyor = null;
                currentSpawner = null;
            }
            return;
        }

        if(currentPreview == null)
        {
            if (!isSpawner) currentPreview = Instantiate(beltPrefabs[selectedIndex]);
            else currentPreview = Instantiate(spawnerPrefabs[selectedIndex]);
            currentPreview.transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);
            foreach (Collider col in currentPreview.GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }
            if(currentPreview.TryGetComponent<ConveyorMovement>(out currentConveyor)){currentConveyor.enabled = false;}
            else if (currentPreview.TryGetComponent<Spawner>(out currentSpawner)){currentSpawner.enabled = false;}
        }

        float scrollY = Mouse.current != null ? Mouse.current.scroll.ReadValue().y : 0f;

        if(scrollY > 0f){currentYaw += yawStep; currentYaw %= 360f;}
        if(scrollY < 0f){currentYaw -= yawStep; currentYaw %= 360f;}

        currentPreview.transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);

        Ray cursorPos = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(currentConveyor != null)
        {   
            if (Physics.Raycast(cursorPos, out RaycastHit hit, 100f, groundLayer))
            {
                currentPreview.transform.position = hit.point;
                currentPreview.transform.position += currentPreview.transform.position - currentConveyor.groundAnchor.transform.position;
            }
            
            prevConveyor = null;
            nextConveyor = null;
            float closestDist = snapRadius;

            foreach(ConveyorMovement belt in placedConveyors)
            {
                if (belt.nextConveyor == null)
                { 
                    float dist = HorizontalDistance(currentConveyor.entry.transform.position, belt.exit.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        prevConveyor = belt;
                        nextConveyor = null;
                    }
                }
                if (belt.prevConveyor == null)
                {
                    float dist2 = HorizontalDistance(currentConveyor.exit.transform.position, belt.entry.transform.position);
                    if (dist2 < closestDist)
                    {
                        closestDist = dist2;
                        nextConveyor = belt;
                        prevConveyor = null;
                    }
                }
            }

            if(prevConveyor != null) {currentPreview.transform.position += prevConveyor.exit.transform.position - currentConveyor.entry.transform.position;}
            if(nextConveyor != null) {currentPreview.transform.position += nextConveyor.entry.transform.position - currentConveyor.exit.transform.position;}

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            if(Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
                GameObject spawnedBelt = Instantiate(beltPrefabs[selectedIndex], currentPreview.transform.position, currentPreview.transform.rotation);
                ConveyorMovement newConveyor = spawnedBelt.GetComponent<ConveyorMovement>();
                if (prevConveyor != null)
                {
                    prevConveyor.nextConveyor = newConveyor;
                    newConveyor.prevConveyor = prevConveyor;
                }
                if (nextConveyor != null)
                {
                    newConveyor.nextConveyor = nextConveyor;
                    nextConveyor.prevConveyor = newConveyor;
                }
                if (newConveyor != null) placedConveyors.Add(newConveyor);
                selectedIndex = -1;
                currentYaw = 0.0f;
            }
        }

        else
        {
            if (Physics.Raycast(cursorPos, out RaycastHit hit, 100f, groundLayer))
            {
                currentPreview.transform.position = hit.point;
                currentPreview.transform.position += currentPreview.transform.position - currentSpawner.ground.transform.position;
            }
            
            prevConveyor = null;
            nextConveyor = null;
            float closestDist = snapRadius;

            foreach(ConveyorMovement belt in placedConveyors)
            {
                if (belt.prevConveyor == null)
                {
                    float dist2 = HorizontalDistance(currentSpawner.exit.transform.position, belt.entry.transform.position);
                    if (dist2 < closestDist)
                    {
                        closestDist = dist2;
                        nextConveyor = belt;
                        prevConveyor = null;
                    }
                }
            }
            if(nextConveyor != null) {currentPreview.transform.position += nextConveyor.entry.transform.position - currentSpawner.exit.transform.position;}

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            if(Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
                GameObject spawnedSpawner = Instantiate(spawnerPrefabs[selectedIndex], currentPreview.transform.position, currentPreview.transform.rotation);
                Spawner newSpawner = spawnedSpawner.GetComponent<Spawner>();
                if (nextConveyor != null)
                {
                    newSpawner.nextConveyor = nextConveyor;
                    
                }
                if (newSpawner != null) placedSpawners.Add(newSpawner);
                selectedIndex = -1;
                currentYaw = 0.0f;
            }
        }
    }

    public void SelectBelt(int index)
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
            currentConveyor = null;
            currentSpawner = null;
        }
        currentYaw = 0.0f;
        selectedIndex = index; 
        isSpawner = false;
    }

    public void SelectSpawner(int index)
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
            currentConveyor = null;
            currentSpawner = null;
        }
        currentYaw = 0.0f;
        selectedIndex = index; 
        isSpawner = true;
    }

    float HorizontalDistance(Vector3 a, Vector3 b)
    {
        return Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
    }
}
