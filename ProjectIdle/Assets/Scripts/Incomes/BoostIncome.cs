using UnityEngine;

public class BoostIncome : MonoBehaviour
{
    public static BoostIncome Instance;

    [Header("Instant Cash")]
    public float amount = 1000f;

    [Header("Boost")]
    public float boostDuration = 15f;
    private float boostTimer = 0f;
    private bool boostActive = false;

    [Header("Cooldown")]
    public float cooldownDuration = 15f;  // ⏱️ Boost kullanıldıktan sonra bekleme süresi
    private float cooldownTimer = 0f;
    private bool isOnCooldown = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (boostActive)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                boostActive = false;
                isOnCooldown = true;
                cooldownTimer = cooldownDuration;
                Debug.Log("🔻 Boost sona erdi. Cooldown başladı.");
            }
        }

        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                Debug.Log("✅ Cooldown bitti. Boost tekrar kullanılabilir.");
            }
        }
    }

    public void InstantCash()
    {
        if (IncomeManager.Instance != null)
        {
            IncomeManager.Instance.totalMoney += amount;
            IncomeManager.Instance.UpdateUI();
            Debug.Log("💰 Anında para verildi.");
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
            Debug.Log($"⏳ Boost beklemede. Kalan: {cooldownTimer:F1} sn");
            return;
        }

        boostActive = true;
        boostTimer = boostDuration;
        Debug.Log($"🚀 Boost başladı! Süre: {boostDuration}s");
    }

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
