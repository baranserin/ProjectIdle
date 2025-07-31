using UnityEngine;

[CreateAssetMenu(fileName = "DecorationConfig", menuName = "Decoration/DecorationConfig")]
public class DecorationConfig : ScriptableObject
{
    public string itemName;
    public int upgradeCost;
    public float itemMultiplier;

    public GameObject decorationPrefab; // Görsel prefab
    public Transform spawnPoint;        // Konum (sahnede atanacak)
}