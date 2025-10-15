using UnityEngine;
using UnityEngine.EventSystems; // Bu satýr çok önemli!

public class ClickTester : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(gameObject.name + " objesine týklandý! Týklama algýlandý!");
    }
}