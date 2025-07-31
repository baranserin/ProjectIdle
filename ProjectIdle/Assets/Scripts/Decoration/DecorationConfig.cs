using UnityEngine;

[CreateAssetMenu(fileName = "DecorationConfig", menuName = "Decoration/DecorationConfig")]
public class DecorationConfig : ScriptableObject
{
    public string itemName;
    public int upgradeCost;
    public float itemMultiplier;
    public GameObject decorationPrefab;   // G�r�nt�lenecek obje
    public RectTransform spawnPoint;          // Objeyi yerle�tirece�imiz sahnedeki nokta
}