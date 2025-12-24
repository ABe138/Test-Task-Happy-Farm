using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig/PurchaseRequest")]
public class PurchaseRequestConfig : ScriptableObject
{
    public CollectableConfig RequestCollectable;
    public int MinRequest;
    public int MaxRequest;

    public string ItemId => RequestCollectable.Id;

    public int RollRequestValue => Random.Range(MinRequest, MaxRequest + 1);
}