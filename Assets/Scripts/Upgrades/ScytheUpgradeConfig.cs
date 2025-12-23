using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig/Upgrade/Scythe")]
public class ScytheUpgradeConfig : ScriptableObject
{
    public string Name;
    public List<UpgradeCost> UpgradeCost;
    public float Reach;
    public float ArcHalfAngle;
    public GameObject ScythePrefab;
    public Sprite ScytheIcon;
}
