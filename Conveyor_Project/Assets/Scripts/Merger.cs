using System.Collections;
using System.Collections.Generic;
using UnityEngine;

  /// <summary>
    /// Handles merging items from two conveyors onto a single conveyor using FIFO Queue
    /// Outputs based on split time variable and just teleports the item dropping it onto the next conveyor
    /// </summary>
public class Merger : MonoBehaviour
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
                obj.TryGetComponent<ConveyorItem>(out ConveyorItem item);
                item.currentConveyor = null;
                obj.transform.position = placeable.exits[0].transform.position;
                obj.SetActive(true);
            }
        }
    }

    public void HandleEntryTrigger(Collider other)
    {
        if(other.TryGetComponent<ConveyorItem>(out _))
        {
            other.gameObject.SetActive(false);
            items.Enqueue(other.gameObject);
        }
    }
}
