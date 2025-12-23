using UnityEngine;

public class CounterDesk : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _flashEffect;

    public bool Interact()
    {
        var success = NPCQueueManager.Instance.TryCompletePendingOrder();
        if (success) 
        {
            _flashEffect.gameObject.SetActive(false);
            _flashEffect.gameObject.SetActive(true);
        }
        return success;
    }
}
