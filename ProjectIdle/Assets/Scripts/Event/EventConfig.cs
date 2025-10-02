using UnityEngine;

public enum EventType
{
    InstantReward,      // Tek seferlik para
    TimedMultiplier     // S�reli �arpan
}

[CreateAssetMenu(fileName = "EventConfig", menuName = "Events/Event Config")]
public class EventConfig : ScriptableObject
{
    public string eventName;
    public string description;
    public Sprite icon;

    [Header("Event Tipi")]
    public EventType eventType;

    [Header("Falling Button Ayarlar�")]
    public GameObject fallingButtonPrefab;
    public float fallSpeed = 200f;
    public float lifeTime = 5f;

    [Header("Instant Reward")]
    public double rewardAmount; // sadece InstantReward i�in ge�erli

    [Header("Timed Multiplier")]
    public float multiplier = 2f;
    public float duration = 30f;
}
