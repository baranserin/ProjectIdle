using UnityEngine;
using UnityEngine.UI;

public class CounterClickButton : MonoBehaviour
{
    public IncomeManager IncomeManager;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnCounterClicked);
    }

    void OnCounterClicked()
    {
        if (IncomeManager.Instance != null)
        {
            IncomeManager.Instance.AddMoney(IncomeManager.income);

            // Optional: Add animation, sound, or feedback here
            Debug.Log("Clicked the counter! +" + IncomeManager.income);
        }
    }
}
