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
    public GameObject fallingButtonPrefab; // UI Button prefabý
    public float fallSpeed = 200f;         // düþme hýzý (px/s)

    [Header("Etki")]
    public EventType eventType = EventType.InstantReward;

    [Tooltip("InstantReward için miktar, TimedMultiplier için baþlangýç çarpaný")]
    public double rewardAmount = 50;

    [Tooltip("TimedMultiplier çarpan deðeri")]
    public float multiplier = 2f;

    [Tooltip("TimedMultiplier süresi (saniye)")]
    public float duration = 15f;
}
