using UnityEngine;
/// <summary>
/// Simple relay class to handle multiple trigger colliders on mergers
/// </summary>
public class MergerTrigger : MonoBehaviour
{
    public Merger merger;
    void OnTriggerEnter(Collider other)
    {
        merger.HandleEntryTrigger(other);
    }
}
