using UnityEngine;

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
        if(other.gameObject.tag == "Can")
        {
            depositUI.cansDeposited++;
        }

        if(other.gameObject.tag == "Box")
        {
            depositUI.boxesDeposited++;
        }
        Destroy(other.gameObject);
    }
}
