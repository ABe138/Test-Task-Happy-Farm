using System.Collections;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private InteractingConfig _config;
    [SerializeField] private LayerMask _interactLayer;

    private Coroutine _autoHarvestCoroutine;

    private const float CheckInterval = 0.15f;

    private void OnEnable()
    {
        _autoHarvestCoroutine = StartCoroutine(AutoInteractLoop());
    }

    private void OnDisable()
    {
        if (_autoHarvestCoroutine != null) StopCoroutine(_autoHarvestCoroutine);
    }

    private IEnumerator AutoInteractLoop()
    {
        while (true)
        {
            var interactables = ObjectsScanner.FindObjectsInRange<IInteractable>(transform.position, _config.scanRadius, _interactLayer);
            foreach (var interactable in interactables)
            {
                interactable.Interact();
            }
            yield return new WaitForSeconds(CheckInterval);
        }
    }
}
