using UnityEngine;
/// <summary>
/// Stores information for each individual port
/// </summary>
public class Port : MonoBehaviour
{
    [HideInInspector] public bool isOccupied;
    public Port connectedPort;
    public Placeable Owner => GetComponentInParent<Placeable>();
}
