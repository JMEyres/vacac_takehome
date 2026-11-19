using UnityEngine;

public class Port : MonoBehaviour
{
    [HideInInspector] public bool isOccupied;
    public Port connectedPort;
    public Placeable Owner => GetComponentInParent<Placeable>();
}
