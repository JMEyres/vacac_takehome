using TMPro;
using UnityEngine;
/// <summary>
/// Handles updating the deposit count UI
/// </summary>
public class DepositUI : MonoBehaviour
{
    public int cansDeposited = 0, boxesDeposited = 0;
    public TextMeshProUGUI canCount, boxCount;

    // Update is called once per frame
    void Update()
    {
        canCount.text = cansDeposited.ToString("D3");
        boxCount.text = boxesDeposited.ToString("D3");
    }
}
