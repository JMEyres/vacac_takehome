using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    public Transform groundAnchor;
    public List<Port> entries = new List<Port>();
    public List<Port> exits = new List<Port>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        foreach(Port e in entries)
        {Gizmos.color = Color.green; Gizmos.DrawSphere(e.transform.position, 0.15f);}
        foreach(Port x in exits)
        {Gizmos.color = Color.red; Gizmos.DrawSphere(x.transform.position, 0.15f);}
    }
}
