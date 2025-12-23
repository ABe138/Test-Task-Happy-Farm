using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHarvester : MonoBehaviour
{
    [SerializeField] private HarvestingConfig _config;
    [SerializeField] private LayerMask _harvestLayer;

    public Action OnHarvestSwingTriggered;

    private bool _harvestingActive = false;
    private float _nextSwingTime = 0f;
    private Coroutine _autoHarvestCoroutine;

    private const float CheckInterval = 0.15f;

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
            bool hasNearby = HasNearbyHarvestable();
            if (hasNearby && !_harvestingActive)
            {
                StartHarvesting();
            }
            else if (!hasNearby && _harvestingActive)
            {
                StopHarvesting();
            }

            if (_harvestingActive && Time.time >= _nextSwingTime)
            {
                // Trigger a swing cycle
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

    private void StartHarvesting()
    {
        _harvestingActive = true;
        _nextSwingTime = Time.time;
    }

    private void StopHarvesting()
    {
        _harvestingActive = false;
    }

    private IEnumerator PerformSwing()
    {
        // Play trigger (animation should also set a hit event ideally, but we use timing here)
        OnHarvestSwingTriggered?.Invoke();

        // Wait until swingTime when hit should occur
        yield return new WaitForSeconds(_config.swingTime);

        // Perform arc harvest at moment of impact
        DoArcHarvest();

        // Optionally wait until animation end — handled by cooldown
    }

    private void DoArcHarvest()
    {
        var scythe = DataManager.Instance.EquippedScythe;
        var list = ObjectsScanner.FindObjectsInArc<IHarvestable>(transform.position, transform.forward, scythe.Reach, scythe.ArcHalfAngle, _harvestLayer);

        var harvested = new HashSet<IHarvestable>();
        foreach (var h in list)
        {
            if (h == null) continue;
            if (harvested.Contains(h)) continue;
            bool consumed = h.Harvest(transform.position);
            if (consumed) harvested.Add(h);
        }
    }

    // Optional public hook to force a manual swing (e.g., player pressed button)
    public void ForceSwing()
    {
        if (Time.time < _nextSwingTime) return;
        StartCoroutine(PerformSwing());
        _nextSwingTime = Time.time + _config.swingCooldown;
    }
}
