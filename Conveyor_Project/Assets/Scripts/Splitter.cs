using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles splitting and sorting one conveyor belt into two
/// </summary>
public class Splitter : MonoBehaviour
{
    private Queue<GameObject> items = new Queue<GameObject>();
    public float splitTime = 0.5f;
    float timer = 0.0f;

    private Placeable placeable;

    void Awake()
    {
        placeable = GetComponent<Placeable>();
    }

    // Update is called once per frame
    void Update()
    {
        timer+= Time.deltaTime;

        if(timer >= splitTime)
        {
            timer = 0.0f;
            if (items.Count > 0) 
            {
                GameObject obj = items.Dequeue();
                // Check the item tag, in future this would be better as an id to handle more items and split automatically
                if(obj.CompareTag("Can"))
                {  
                    obj.TryGetComponent<ConveyorItem>(out ConveyorItem item);
                    item.currentConveyor = null;
                    obj.transform.position = placeable.exits[0].transform.position;
                    obj.SetActive(true);
                }

                if(obj.CompareTag("Box"))
                {  
                    obj.TryGetComponent<ConveyorItem>(out ConveyorItem item);
                    item.currentConveyor = null;
                    obj.transform.position = placeable.exits[1].transform.position;
                    obj.SetActive(true);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
        items.Enqueue(other.gameObject);
    }
}
