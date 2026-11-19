using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform exit, ground, spawn;
    public GameObject item;
    public ConveyorMovement nextConveyor;

    public float spawnTime = 1.0f;
    float timer = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

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
