using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig/Harvestable")]
public class HarvestableConfig : ScriptableObject
{
    public int Durability;
    public float RegrowDuration;
    public float ProduceSpread;
    public CollectableConfig Produce;
}