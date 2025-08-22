using UnityEngine;

public class AcousticGuitarSound : MonoBehaviour
{
    public AudioSource backgroundMusic; // Oyun müziði
    public AudioSource effectSound;     // Butona basýnca çalacak ses

    public void OnButtonClick()
    {
        if (effectSound != null)
        {
            // Müzik o anda çalýyorsa duraklat
            if (backgroundMusic != null && backgroundMusic.isPlaying)
            {
                backgroundMusic.Pause();
                StartCoroutine(ResumeMusicAfterEffect());
            }

            // Efekt sesini çal
            effectSound.Play();
        }
    }

    private System.Collections.IEnumerator ResumeMusicAfterEffect()
    {
        // Efektin süresini bekle
        yield return new WaitForSeconds(effectSound.clip.length);

        // Müzik o anda duraklatýlmýþsa devam ettir
        if (backgroundMusic != null && backgroundMusic.time > 0f)
        {
            backgroundMusic.UnPause();
        }
    }
}
