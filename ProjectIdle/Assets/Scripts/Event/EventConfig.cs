using UnityEngine;

public enum EventType { InstantReward, TimedMultiplier }

[CreateAssetMenu(fileName = "EventConfig", menuName = "Events/Event Config")]
public class EventConfig : ScriptableObject
{
    [Header("Temel")]
    public string eventName;
    public string description;
    public Sprite icon;

    [Header("Falling Button")]
    public GameObject fallingButtonPrefab; // UI Button prefab�
    public float fallSpeed = 200f;         // d��me h�z� (px/s)

    [Header("Etki")]
    public EventType eventType = EventType.InstantReward;

    [Tooltip("InstantReward i�in miktar, TimedMultiplier i�in ba�lang�� �arpan�")]
    public double rewardAmount = 50;

    [Tooltip("TimedMultiplier �arpan de�eri")]
    public float multiplier = 2f;

    [Tooltip("TimedMultiplier s�resi (saniye)")]
    public float duration = 15f;
}
