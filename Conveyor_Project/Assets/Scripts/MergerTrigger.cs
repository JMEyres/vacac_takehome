using UnityEngine;

public class MergerTrigger : MonoBehaviour
{
    public Merger merger;
    void OnTriggerEnter(Collider other)
    {
        merger.HandleEntryTrigger(other);
    }
}
