using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeDetector : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public UpgradeCardManager cardManager;
    private Vector2 startPos;

    public void OnDrag(PointerEventData eventData)
    {
        if (startPos == Vector2.zero)
            startPos = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 endPos = eventData.position;
        float deltaX = endPos.x - startPos.x;

        if (Mathf.Abs(deltaX) > 100f) // swipe threshold
        {
            if (deltaX > 0)
                cardManager.PreviousCard();
            else
                cardManager.NextCard();
        }

        startPos = Vector2.zero;
    }
}
