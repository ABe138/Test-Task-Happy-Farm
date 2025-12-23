using System.Linq;
using UnityEngine;

public class ScytheWorkbench : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _flashEffect;

    public bool Interact()
    {
        var dataManager = DataManager.Instance;
        var currentLevel = dataManager.Data.ScytheLevel;
        var maxLevel = dataManager.ScytheUpgradeConfigs.Count - 1;
        if (currentLevel < maxLevel) 
        {
            var nextLevel = currentLevel + 1;
            var nextUpgrade = dataManager.ScytheUpgradeConfigs[nextLevel];
            if (dataManager.TryWithdrawCurrency(nextUpgrade.UpgradeCost.ToDictionary(c => c.Currency.Id, c => c.Cost))) 
            {
                dataManager.SetScytheLevel(nextLevel);
                _flashEffect.gameObject.SetActive(false);
                _flashEffect.gameObject.SetActive(true);
                return true;
            }
        }
        return false;
    }
}
