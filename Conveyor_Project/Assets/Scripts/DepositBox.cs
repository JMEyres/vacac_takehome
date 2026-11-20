using UnityEngine;
/// <summary>
/// Simply destroys the item when it hits the collider and iterates deposited count
/// </summary>
public class DepositBox : MonoBehaviour
{
    private DepositUI depositUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
       depositUI = FindAnyObjectByType<DepositUI>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Can"))
        {
            depositUI.cansDeposited++;
        }

        if(other.gameObject.CompareTag("Box"))
        {
            depositUI.boxesDeposited++;
        }
        Destroy(other.gameObject);
    }
}
