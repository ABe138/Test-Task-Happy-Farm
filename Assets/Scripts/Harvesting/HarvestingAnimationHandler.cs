using UnityEngine;

public class HarvestingAnimationHandler : MonoBehaviour
{
    [SerializeField] private PlayerHarvester _harvester;
    [SerializeField] private Animator _animator;

    void OnEnable()
    {
        _harvester.OnHarvestSwingTriggered += TriggerHarvestSwing;
    }

    void OnDisable()
    {
        _harvester.OnHarvestSwingTriggered -= TriggerHarvestSwing;
    }

    void TriggerHarvestSwing() => _animator?.SetTrigger("Swing");
}