using UnityEngine;
/// <summary>
/// Handles spawning items by instantiating the item at the spawn points location
/// </summary>
public class Spawner : MonoBehaviour
{
    public Transform exit, ground, spawn;
    public GameObject item;
    public ConveyorMovement nextConveyor;

    public float spawnTime = 1.0f;
    float timer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if(item == null) return;
        timer += Time.deltaTime;

        if(timer >= spawnTime)
        {
            timer = 0.0f;
            Instantiate(item, spawn.transform.position, spawn.transform.rotation);
        }
    }
}
