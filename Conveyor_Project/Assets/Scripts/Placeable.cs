using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Stores information for every placeable object, with an option for debug gizmos
/// </summary>
public class Placeable : MonoBehaviour
{
    public Transform groundAnchor;
    public List<Port> entries = new List<Port>();
    public List<Port> exits = new List<Port>();

    void OnDrawGizmos()
    {
        foreach(Port e in entries)
        {Gizmos.color = Color.green; Gizmos.DrawSphere(e.transform.position, 0.15f);}
        foreach(Port x in exits)
        {Gizmos.color = Color.red; Gizmos.DrawSphere(x.transform.position, 0.15f);}
    }
}
