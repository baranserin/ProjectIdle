using UnityEngine;

[CreateAssetMenu(fileName = "EventConfig", menuName = "Events/Event Config")]
public class EventConfig : ScriptableObject
{
    public string eventName;

    public bool isInstantReward;
    public double rewardAmount; // Anlýk para ödülü

    public bool isTimedMultiplier;
    public float multiplier = 2f;   // Çarpan
    public float duration = 30f;    // Süre (saniye)

    public string description;
}
