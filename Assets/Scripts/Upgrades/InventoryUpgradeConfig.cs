using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig/Upgrade/Inventory")]
public class InventoryUpgradeConfig : ScriptableObject
{
    public string Name;
    public List<UpgradeCost> UpgradeCost;
    public int Capacity;
    public GameObject BackpackPrefab;
    public Sprite BackpackIcon;
}
