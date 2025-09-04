using UnityEngine;

public class BoostIncome : MonoBehaviour
{
    public static BoostIncome Instance;

    [Header("UI")]
    public GameObject boostButton;  // Inspector’dan butonu sürükle-bırak

    [Header("Instant Cash")]
    public float amount = 1000f;

    [Header("Boost")]
    public float boostDuration = 15f;
    private float boostTimer = 0f;
    private bool boostActive = false;

    [Header("Cooldown")]
    public float cooldownDuration = 15f;
    private float cooldownTimer = 0f;
    private bool isOnCooldown = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (boostButton) boostButton.SetActive(true);
    }

    void Update()
    {
        if (boostActive)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                boostActive = false;
                StartCooldown();
            }
        }

        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                Debug.Log("✅ Cooldown bitti. Buton tekrar açıldı.");
                if (boostButton) boostButton.SetActive(true);
            }
        }
    }

    public void ActivateBoost()
    {
        if (boostActive)
        {
            Debug.Log("⚠️ Boost zaten aktif.");
            return;
        }

        if (isOnCooldown)
        {
            Debug.Log($"⏳ Boost beklemede: {cooldownTimer:F1} sn");
            return;
        }

        boostActive = true;
        boostTimer = boostDuration;
        Debug.Log("🚀 Boost başladı!");

        if (boostButton) boostButton.SetActive(false);
    }

    private void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = cooldownDuration;
        Debug.Log("🔻 Boost bitti, cooldown başladı.");
        if (boostButton) boostButton.SetActive(false);
    }

    // --- EKLENECEK PUBLIC YARDIMCI METOTLAR ---
    public bool IsBoostActive()
    {
        return boostActive;
    }

    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }

    public float GetCooldownRemaining()
    {
        return isOnCooldown ? cooldownTimer : 0f;
    }
}

