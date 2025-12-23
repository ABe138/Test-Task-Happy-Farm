using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIInventoryView : MonoBehaviour
{
    [SerializeField] RectTransform _coinsLayout;
    [SerializeField] UIItemView _itemViewPrefab;

    private Dictionary<string, UIItemView> _activeItemViews = new Dictionary<string, UIItemView>();

    public void ClearViews() 
    {
        foreach (var view in _activeItemViews)
        {
            view.Value.gameObject.SetActive(false);
        }
    }

    public void UpdateItemQuantity(string itemId, int quantity)
    {
        var view = GetActiveItemView(itemId);
        view.UpdateQuantity(quantity);
        view.gameObject.SetActive(quantity > 0);
    }

    private UIItemView GetActiveItemView(string itemId)
    {
        if (_activeItemViews.ContainsKey(itemId)) return _activeItemViews[itemId];
        var newView = PoolingManager.Instance.Pool(_itemViewPrefab, _coinsLayout, Vector3.zero, Quaternion.identity);
        _activeItemViews.Add(itemId, newView);
        newView.SetIcon(DataManager.Instance.CollectableObjectConfigs.FirstOrDefault(obj => obj.Id == itemId).InventoryIcon);
        return newView;
    }
}