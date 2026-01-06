using System.Collections;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private InteractingConfig _config;
    [SerializeField] private LayerMask _interactLayer;

    private Coroutine _autoInteractCoroutine;

    private const float CheckInterval = 0.15f;

    private void OnEnable()
    {
        _autoInteractCoroutine = StartCoroutine(AutoInteractLoop());
    }

    private void OnDisable()
    {
        if (_autoInteractCoroutine != null) StopCoroutine(_autoInteractCoroutine);
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
