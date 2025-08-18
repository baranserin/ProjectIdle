using UnityEngine;
using UnityEngine.UI;

public class CounterClickButton : MonoBehaviour
{
    public IncomeManager IncomeManager;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnCounterClicked);
    }

    public void OnCounterClicked()
    {
        if (IncomeManager.Instance != null)
        {
            IncomeManager.Instance.AddMoney(IncomeManager.clickValue);

        }
    }
}
