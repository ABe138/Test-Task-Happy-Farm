using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIPriceView : MonoBehaviour
{
    [SerializeField] RectTransform _coinsLayout;
    [SerializeField] UICoinView _coinViewPrefab;

    private Dictionary<string, UICoinView> _activeCoinViews = new Dictionary<string, UICoinView>();

    public void ClearViews()
    {
        foreach (var view in _activeCoinViews)
        {
            view.Value.gameObject.SetActive(false);
        }
    }

    public void UpdatePrice(string coinId, int value) 
    {
        var view = GetActiveCoinView(coinId);
        view.UpdateQuantity(value);
        view.gameObject.SetActive(true);
    }

    private UICoinView GetActiveCoinView(string coinId)
    {
        if (_activeCoinViews.ContainsKey(coinId)) return _activeCoinViews[coinId];
        var newView = PoolingManager.Instance.Pool(_coinViewPrefab, _coinsLayout, Vector3.zero, Quaternion.identity);
        _activeCoinViews.Add(coinId, newView);
        newView.SetIcon(DataManager.Instance.CollectableObjectConfigs.FirstOrDefault(obj => obj.Id == coinId).InventoryIcon);
        return newView;
    }
}
