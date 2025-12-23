using UnityEngine;

public class HarvestingAnimationHandler : MonoBehaviour
{
    [SerializeField] private PlayerHarvester _harvester;
    [SerializeField] private PlayerCollector _collector;
    [SerializeField] private Animator _animator;

    void OnEnable()
    {
        _harvester.OnHarvestSwingTriggered += TriggerHarvestSwing;
        _collector.OnGatheringChanged += HandleGathering;
    }

    void OnDisable()
    {
        _harvester.OnHarvestSwingTriggered -= TriggerHarvestSwing;
        _collector.OnGatheringChanged -= HandleGathering;
    }

    void HandleGathering(bool gathering) => _animator?.SetBool("Gathering", gathering);

    void TriggerHarvestSwing() => _animator?.SetTrigger("Swing");
}