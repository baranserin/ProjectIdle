using UnityEngine;

[CreateAssetMenu(fileName = "EventConfig", menuName = "Events/Event Config")]
public class EventConfig : ScriptableObject
{
    public string eventName;

    public bool isInstantReward;
    public double rewardAmount; // Anl�k para �d�l�

    public bool isTimedMultiplier;
    public float multiplier = 2f;   // �arpan
    public float duration = 30f;    // S�re (saniye)

    public string description;
}
