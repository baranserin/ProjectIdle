using UnityEngine;
using UnityEngine.UI; // Eðer UI elementleriyle çalýþýyorsan gerekli

public class LambaKontrol : MonoBehaviour
{
    [Header("Sprite Ayarlarý")]
    public Sprite lambaAcikResmi;   
    public Sprite lambaKapaliResmi; 

    private SpriteRenderer spriteRenderer;
    private bool acikMi = false; 

    void Start()
    {
     
        spriteRenderer = GetComponent<SpriteRenderer>();

   
        spriteRenderer.sprite = lambaKapaliResmi;
        acikMi = false;
    }

    public void LambaDurumunuDegistir()
    {
    
        acikMi = !acikMi;

        if (acikMi)
        {
            spriteRenderer.sprite = lambaAcikResmi;
        }
        else
        {
            spriteRenderer.sprite = lambaKapaliResmi;
        }
    }
}