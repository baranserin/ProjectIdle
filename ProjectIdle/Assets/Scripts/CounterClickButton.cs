using UnityEngine;
using UnityEngine.UI;

public class CounterClickButton : MonoBehaviour
{
    public double clickValue = 1;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnCounterClicked);
    }

    void OnCounterClicked()
    {
        if (IncomeManager.Instance != null)
        {
            IncomeManager.Instance.AddMoney(clickValue);

            // Optional: Add animation, sound, or feedback here
            Debug.Log("Clicked the counter! +" + clickValue);
        }
    }
}
