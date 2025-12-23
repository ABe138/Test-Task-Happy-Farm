using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig/CollectableObject")]
public class CollectableObjectConfig : ScriptableObject
{
    public string Id;
    public string Name;
    public int Value;
    public CollectableItem CollectableItemPrefab;
    public Sprite InventoryIcon;
}
