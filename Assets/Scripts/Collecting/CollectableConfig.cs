using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig/Collectable")]
public class CollectableConfig : ScriptableObject
{
    public string Id;
    public string Name;
    public int Value;
    public CollectableItem CollectableItemPrefab;
    public Sprite InventoryIcon;
}
