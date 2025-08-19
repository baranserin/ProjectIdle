using UnityEngine;
using UnityEngine.UI; 


public class alpha_hit_test : MonoBehaviour
{

    void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }

    public void OnCounterClicked()
    {
        Debug.Log("Counter clicked!");

    }

}