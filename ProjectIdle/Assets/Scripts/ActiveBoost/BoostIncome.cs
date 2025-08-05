using UnityEngine;

public class BoostIncome : MonoBehaviour
{
    public IncomeManager incomeManager; // 👈 Sahneden atanmalı

    [Header("Instant Cash")]
    public float amount = 1000f;

    [Header("Boost")]
    public float boostDuration = 15f;

    private bool boostActive = false;
    private float boostTimer = 0f;

    public static BoostIncome Instance;

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
                Debug.Log("🔻 Boost sona erdi.");
            }
        }
    }

    public void InstantCash()
    {
        incomeManager.totalMoney += amount;
        Debug.Log($"💰 Instant Cash verildi: {amount}");
        incomeManager.UpdateUI(); // UI hemen güncellensin
    }

    public void ActivateBoost()
    {
        if (!boostActive)
        {
            boostActive = true;
            boostTimer = boostDuration;
            Debug.Log($"🚀 Boost aktif! Süre: {boostDuration}s");
        }
    }

    public bool IsBoostActive()
    {
        return boostActive;
    }
}
