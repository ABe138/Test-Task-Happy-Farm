using UnityEngine;

public class CounterDesk : MonoBehaviour, IInteractable
{
    [SerializeField] private UIInventoryView _pendingPurchaseView;
    [SerializeField] private GameObject _noRequestView;
    [SerializeField] private GameObject _flashEffect;

    private void Start()
    {
        NPCQueueManager.Instance.PendingRequestUpdated += OnPendingRequestUpdated;

        OnPendingRequestUpdated();
    }

    private void OnPendingRequestUpdated()
    {
        _pendingPurchaseView.ClearViews();

        var request = NPCQueueManager.Instance.PendingPurchaseOrder;
        if (request != null && !request.Satisfied)
        {
            _pendingPurchaseView.UpdateItemQuantity(request.ItemId, request.Quantity);
            _noRequestView.gameObject.SetActive(false);
        }
        else
        {
            _noRequestView.gameObject.SetActive(true);
        }
    }

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
