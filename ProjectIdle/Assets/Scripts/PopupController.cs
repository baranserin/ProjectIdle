using UnityEngine;

public class PopupController : MonoBehaviour
{
    public void OpenPopup()
    {
        gameObject.SetActive(true);  // this object is DarkOverlay
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
