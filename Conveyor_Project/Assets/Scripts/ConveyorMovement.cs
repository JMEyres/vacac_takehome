using System.Collections.Generic;
using System.IO;
using UnityEngine;
/// <summary>
/// Handles item movement/handoff between conveyors
/// When an item hit the collision trigger initially it gets stored in a list on the conveyor, and moved linearly from entry-exit
/// When an item hits the end of the belt it is transferred to the next connected conveyor if there is one, otherwise its physics are re enabled and it drops off the end
/// </summary>
public class ConveyorMovement : MonoBehaviour
{
    public GameObject entry, exit, groundAnchor;
    public ConveyorMovement nextConveyor, prevConveyor;
    public List<ConveyorItem> itemsOnConveyor = new List<ConveyorItem>();
    public float speed =1.0f;
    private float pathDistance = 0.0f;
    private Vector3 beltDirection;
    private Placeable placeable;
    void Awake()
    {
        placeable = GetComponent<Placeable>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pathDistance = Vector3.Distance(entry.transform.position, exit.transform.position); // get total distance between entry & exit
        beltDirection = (exit.transform.position - entry.transform.position).normalized; // gives worldspace direction of the belt

        // Snap the connected piece into place by aligning its entry port directly onto this exit port
        if(nextConveyor != null){ 
            Vector3 offset = exit.transform.position - nextConveyor.entry.transform.position;
            nextConveyor.gameObject.transform.position += offset;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Iterate backwards so items can be safely removed mid-loop upon reaching the exit
        for(int i = itemsOnConveyor.Count-1; i >= 0; i--)
        {
            // Move the items based on set speed
            itemsOnConveyor[i].distanceTravelled +=Time.deltaTime * speed;
            
            // If the item reaches the end handle outcome
            if(itemsOnConveyor[i].distanceTravelled >= pathDistance) { 
                Port connectedExit = placeable.exits[0].connectedPort;
                ConveyorItem currentItem = itemsOnConveyor[i];
                itemsOnConveyor.RemoveAt(i);

                // If there is no connection re enable item physics
                if(connectedExit == null) { currentItem.gameObject.GetComponent<Rigidbody>().isKinematic = false; continue;}

                // If connected machine is a conveyor assign the item to it otherwise just re enable physics
                if (connectedExit.Owner.TryGetComponent(out ConveyorMovement nextBelt))
                {
                    // Carry over leftover travel distance so movement stays seamless across belt seams
                    currentItem.distanceTravelled = currentItem.distanceTravelled - pathDistance;
                    currentItem.currentConveyor = nextBelt.gameObject;
                    nextBelt.itemsOnConveyor.Add(currentItem);
                }
                else
                {
                    currentItem.gameObject.GetComponent<Rigidbody>().isKinematic = false; 
                    continue;
                }
            }
            // If item on conveyor actually move its position based on its distance travelled on the belt
            else
            {
                float t = itemsOnConveyor[i].distanceTravelled/pathDistance;
                itemsOnConveyor[i].gameObject.transform.position = Vector3.Lerp(entry.transform.position, exit.transform.position, t);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(!other.TryGetComponent(out ConveyorItem item)) return;
        // Ignore items already claimed by another belt segment
        if(item.currentConveyor != null) return;

        item.currentConveyor = gameObject;
        item.distanceTravelled = Mathf.Clamp(Vector3.Dot(item.transform.position - entry.transform.position, beltDirection), 0f, pathDistance) ; // get the current distance along the belt for the item

        // Hand control over from physics simulation to deterministic belt interpolation
        if (other.attachedRigidbody != null) other.attachedRigidbody.isKinematic = true;

        float t = item.distanceTravelled/pathDistance;
        other.transform.position = Vector3.Lerp(entry.transform.position, exit.transform.position, t); // set the position relative to the distance along the belt

        itemsOnConveyor.Add(item); // add to conveyors list
    }
}
