using System.Collections.Generic;
using UnityEngine;

public class UpgradeConfig : ScriptableObject
{
    public string Name;
    public List<UpgradeCost> UpgradeCost;
    public GameObject Prefab;
    public Sprite Icon;
}