using TMPro;
using UnityEngine;

public class DepositUI : MonoBehaviour
{
    public int cansDeposited = 0, boxesDeposited = 0;
    public TextMeshProUGUI canCount, boxCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        canCount.text = cansDeposited.ToString("D3");
        boxCount.text = boxesDeposited.ToString("D3");
    }
}
