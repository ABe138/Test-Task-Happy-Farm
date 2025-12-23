using System.Linq;
using UnityEngine;

public abstract class Workbench : MonoBehaviour, IInteractable
{
    [SerializeField] private UIPriceView _upgradePriceView;
    [SerializeField] private GameObject _maxLevelView;
    [SerializeField] private GameObject _flashEffect;

    protected DataManager _dataManager;

    protected abstract int CurrentLevel { get; }
    protected abstract int MaxLevel { get; }
    protected int NextLevel => CurrentLevel + 1;

    protected abstract UpgradeConfig NextUpgrade { get; }

    void Start()
    {
        _dataManager = DataManager.Instance;

        _maxLevelView.SetActive(false);

        UpdateUpgradePrice();
    }

    public bool Interact()
    {
        if (CurrentLevel < MaxLevel)
        {
            if (_dataManager.TryWithdrawCurrency(NextUpgrade.UpgradeCost.ToDictionary(c => c.Currency.Id, c => c.Cost)))
            {
                SetCurrentUpgradeLevel(NextLevel);
                UpdateUpgradePrice();

                _flashEffect.gameObject.SetActive(false);
                _flashEffect.gameObject.SetActive(true);

                return true;
            }
        }
        return false;
    }

    private void UpdateUpgradePrice()
    {
        _upgradePriceView.ClearViews();
        if (CurrentLevel < MaxLevel)
        {
            var costDictionary = NextUpgrade.UpgradeCost.ToDictionary(c => c.Currency.Id, c => c.Cost);
            foreach (var cost in costDictionary)
            {
                _upgradePriceView.UpdatePrice(cost.Key, cost.Value);
            }
        }
        else
        {
            _upgradePriceView.gameObject.SetActive(false);
            _maxLevelView.SetActive(true);
        }
    }

    protected abstract void SetCurrentUpgradeLevel(int level);
}