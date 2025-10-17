using UnityEngine;

public enum EventType
{
    InstantReward,
    TimedMultiplier
}

[CreateAssetMenu(fileName = "EventConfig", menuName = "Event/New Event")]
public class EventConfig : ScriptableObject
{
    public string eventName;
    public string description;

    [Header("Event Type")]
    public EventType eventType;

    [Header("Instant Reward")]
    public double rewardAmount;

    [Header("Timed Multiplier")]
    public float multiplier = 2f;
    public float duration = 5f;

    [Header("Fall Settings")]
    public float fallSpeed = 200f;

    [Header("UI")]
    public Sprite icon;                  // ✅ Eklendi
    public Sprite[] animationFrames;     // 🔥 Çok kareli sprite animasyonu

    [Header("Prefab Reference")]
    public GameObject fallingButtonPrefab;
}
