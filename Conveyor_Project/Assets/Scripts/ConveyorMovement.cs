using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

        if(nextConveyor != null){ 
            Vector3 offset = exit.transform.position - nextConveyor.entry.transform.position;
            nextConveyor.gameObject.transform.position += offset;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = itemsOnConveyor.Count-1; i >= 0; i--)
        {
            itemsOnConveyor[i].distanceTravelled +=Time.deltaTime * speed;

            if(itemsOnConveyor[i].distanceTravelled >= pathDistance) { 
                Port connectedExit = placeable.exits[0].connectedPort;
                ConveyorItem currentItem = itemsOnConveyor[i];
                itemsOnConveyor.RemoveAt(i);

                if(connectedExit == null) { currentItem.gameObject.GetComponent<Rigidbody>().isKinematic = false; continue;}

                if (connectedExit.Owner.TryGetComponent(out ConveyorMovement nextBelt))
                {
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
        if(item.currentConveyor != null) return;

        item.currentConveyor = gameObject;
        item.distanceTravelled = Mathf.Clamp(Vector3.Dot(item.transform.position - entry.transform.position, beltDirection), 0f, pathDistance) ; // get the current distance along the belt for the item
        if (other.attachedRigidbody != null) other.attachedRigidbody.isKinematic = true;

        float t = item.distanceTravelled/pathDistance;
        other.transform.position = Vector3.Lerp(entry.transform.position, exit.transform.position, t); // set the position relative to the distance along the belt

        itemsOnConveyor.Add(item); // add to conveyors list
    }
}
