using UnityEngine;
using UnityEngine.UI; 


public class alpha_hit_test : MonoBehaviour
{
    public Button otherButton;

    void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }

    public void OnCounterClicked()
    {
        Debug.Log("Counter clicked!");

    }
    public void OnClickA()
    {
        Debug.Log("Object A clicked!");

        // Simulate clicking the other button
        if (otherButton != null)
            otherButton.onClick.Invoke();
    }
}