using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConveyorMovement : MonoBehaviour
{
    public GameObject entry, exit;
    public ConveyorMovement nextConveyor;
    private List<ConveyorItem> itemsOnConveyor = new List<ConveyorItem>();
    public float speed =1.0f;
    private float pathDistance = 0.0f;
    private Vector3 beltDirection;
    
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
                ConveyorItem currentItem = itemsOnConveyor[i];
                itemsOnConveyor.RemoveAt(i);

                if(nextConveyor == null) { currentItem.gameObject.GetComponent<Rigidbody>().isKinematic = false; continue;}

                currentItem.distanceTravelled = currentItem.distanceTravelled - pathDistance;
                currentItem.currentConveyor = nextConveyor.gameObject;
                nextConveyor.itemsOnConveyor.Add(currentItem);
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
        other.transform.position = Vector3.Lerp(entry.transform.position, exit.transform.position, t);

        itemsOnConveyor.Add(item);
    }
}
