using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHarvester : MonoBehaviour
{
    [SerializeField] private HarvestingConfig _config;
    [SerializeField] private LayerMask _harvestLayer;
    [SerializeField] private HarvestingSlashEffect _slashEffectPrefab;

    public Action OnHarvestSwingTriggered;

    private float _nextSwingTime = 0f;
    private Coroutine _autoHarvestCoroutine;

    private const float CheckInterval = 0.1f;
    private const float SlashAnimationDuration = 0.25f;

    private void Start()
    {
        _autoHarvestCoroutine = StartCoroutine(AutoHarvestLoop());
    }

    private void OnDestroy()
    {
        if (_autoHarvestCoroutine != null) StopCoroutine(_autoHarvestCoroutine);
    }

    private IEnumerator AutoHarvestLoop()
    {
        while (true)
        {
            var hasNearby = HasNearbyHarvestable();

            if (hasNearby && Time.time >= _nextSwingTime)
            {
                StartCoroutine(PerformSwing());
                _nextSwingTime = Time.time + _config.swingCooldown;
            }

            yield return new WaitForSeconds(CheckInterval);
        }
    }

    private bool HasNearbyHarvestable()
    {
        var scythe = DataManager.Instance.EquippedScythe;
        var list = ObjectsScanner.FindObjectsInArc<IHarvestable>(transform.position, transform.forward, scythe.Reach, scythe.ArcHalfAngle, _harvestLayer);
        return list.Count > 0;
    }

    private IEnumerator PerformSwing()
    {
        OnHarvestSwingTriggered?.Invoke();
        yield return new WaitForSeconds(_config.swingTime);
        DoArcHarvest();
    }

    private void DoArcHarvest()
    {
        var scythe = DataManager.Instance.EquippedScythe;

        var slashEffect = PoolingManager.Instance.Pool(_slashEffectPrefab, null, transform.position, Quaternion.LookRotation(transform.forward));
        slashEffect.Animate(scythe.ArcHalfAngle * 2f, scythe.Reach, SlashAnimationDuration);

        var list = ObjectsScanner.FindObjectsInArc<IHarvestable>(transform.position, transform.forward, scythe.Reach, scythe.ArcHalfAngle, _harvestLayer);

        var harvested = new HashSet<IHarvestable>();
        foreach (var h in list)
        {
            if (h == null) continue;
            if (harvested.Contains(h)) continue;
            var consumed = h.Harvest(transform.position);
            if (consumed) harvested.Add(h);
        }
    }

    public void ForceSwing()
    {
        if (Time.time < _nextSwingTime) return;
        StartCoroutine(PerformSwing());
        _nextSwingTime = Time.time + _config.swingCooldown;
    }
}
