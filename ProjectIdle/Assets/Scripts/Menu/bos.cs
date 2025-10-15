using UnityEngine;
using UnityEngine.EventSystems; // Bu sat�r �ok �nemli!

public class ClickTester : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(gameObject.name + " objesine t�kland�! T�klama alg�land�!");
    }
}