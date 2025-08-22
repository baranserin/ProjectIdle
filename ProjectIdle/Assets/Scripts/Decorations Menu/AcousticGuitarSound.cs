using UnityEngine;

public class AcousticGuitarSound : MonoBehaviour
{
    public AudioSource backgroundMusic; // Oyun m�zi�i
    public AudioSource effectSound;     // Butona bas�nca �alacak ses

    public void OnButtonClick()
    {
        if (effectSound != null)
        {
            // M�zik o anda �al�yorsa duraklat
            if (backgroundMusic != null && backgroundMusic.isPlaying)
            {
                backgroundMusic.Pause();
                StartCoroutine(ResumeMusicAfterEffect());
            }

            // Efekt sesini �al
            effectSound.Play();
        }
    }

    private System.Collections.IEnumerator ResumeMusicAfterEffect()
    {
        // Efektin s�resini bekle
        yield return new WaitForSeconds(effectSound.clip.length);

        // M�zik o anda duraklat�lm��sa devam ettir
        if (backgroundMusic != null && backgroundMusic.time > 0f)
        {
            backgroundMusic.UnPause();
        }
    }
}
