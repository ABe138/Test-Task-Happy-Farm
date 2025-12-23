using System;
using System.Collections;
using UnityEngine;

public class PlayerCollector : MonoBehaviour, ICollector
{
    [SerializeField] private CollectConfig _config;
    [SerializeField] private LayerMask _collectLayer;

    private Coroutine _autoCollectCoroutine;

    private const float CheckInterval = 0.15f;

    public Action<bool> OnGatheringChanged;

    public Transform CollectorTransform => transform;

    private void OnEnable()
    {
        _autoCollectCoroutine = StartCoroutine(AutoCollectLoop());
    }

    private void OnDisable()
    {
        if (_autoCollectCoroutine != null) StopCoroutine(_autoCollectCoroutine);
        OnGatheringChanged?.Invoke(false);
    }

    private IEnumerator AutoCollectLoop()
    {
        while (true)
        {
            var collectables = ObjectsScanner.FindObjectsInRange<ICollectable>(transform.position, _config.scanRadius, _collectLayer);
            foreach (var collectable in collectables)
            {
                collectable.Collect(this);
            }
            //OnGatheringChanged?.Invoke(collectables.Count > 0);
            yield return new WaitForSeconds(CheckInterval);
        }
    }

    public bool Consume(ICollectable collectable)
    {
        var dataManager = DataManager.Instance;
        if (dataManager.InventoryCurrentLoad < dataManager.InventoryCapacity)
        {
            DataManager.Instance.AddItem(collectable.Id, 1);
            return true;
        }
        return false;
    }
}
