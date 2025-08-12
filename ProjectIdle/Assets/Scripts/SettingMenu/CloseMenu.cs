using UnityEngine;

public class CloseMenu : MonoBehaviour
{
    public GameObject onClickClose;
    void Start()
    {

    }


    void Update()
    {

    }


    public void ClickClose()
    {
        if (onClickClose.activeSelf)
        {
            onClickClose.SetActive(false);
        }
    }
}