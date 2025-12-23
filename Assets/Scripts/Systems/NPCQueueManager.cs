using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCQueueManager : MonoBehaviour
{
    public static NPCQueueManager Instance { get; private set; }

    [SerializeField] private NPCBrain _npcPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _purchasePoint;
    [SerializeField] private Transform _despawnPoint;

    private List<NPCBrain> _npcsInQueue;

    private Coroutine _updateAICoroutine;
    private Coroutine _spawnNPCCoroutine;

    private const float UpdateInterval = 0.15f;
    private const float SpawnInterval = 2f;
    private const int SpawnLimit = 5;

    public PurchaseOrder PendingPurchaseOrder { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _npcsInQueue = new List<NPCBrain>();
    }

    private void Start()
    {
        _spawnNPCCoroutine = StartCoroutine(SpawnNPCCoroutine());
    }

    private void OnDestroy()
    {
        if (_spawnNPCCoroutine != null) StopCoroutine(_spawnNPCCoroutine);
    }

    private IEnumerator SpawnNPCCoroutine()
    {
        while (true)
        {
            if (_npcsInQueue.Count > SpawnLimit)
            {
                yield return new WaitForSeconds(1);
            }
            else
            {
                var position = _spawnPoint.position;
                var newNPC = PoolingManager.Instance.Pool(_npcPrefab, null, _spawnPoint.position + Vector3.up * 0.05f, Quaternion.identity);
                _npcsInQueue.Add(newNPC);

                newNPC.AddStateToQueue(new FollowQueueState(newNPC));
                newNPC.AddStateToQueue(new MoveState(newNPC, _purchasePoint.position));
                newNPC.AddStateToQueue(new MakePurchaseState(newNPC, new (UnityEngine.Random.Range(0, 2).ToString(), UnityEngine.Random.Range(1, 10))));
                newNPC.AddStateToQueue(new MoveState(newNPC, _despawnPoint.position));
                newNPC.AddStateToQueue(new Despawn(newNPC));

                yield return new WaitForSeconds(SpawnInterval);
            }
        }
    }

    public int MyPositionInQueue(NPCBrain npc)
    {
        return _npcsInQueue.IndexOf(npc);
    }

    public NPCBrain GetNPCInFront(NPCBrain npc)
    {
        var myPos = MyPositionInQueue(npc);
        if (myPos > 0) 
        {
            return _npcsInQueue[myPos - 1];
        }
        return null;
    }

    public void LeaveQueue(NPCBrain npc)
    {
        if (_npcsInQueue.Contains(npc)) _npcsInQueue.Remove(npc);
    }

    public void SetPendingPurchaseOrder(PurchaseOrder purchaseOrder) 
    {
        //show speech bubble
        PendingPurchaseOrder = purchaseOrder;
    }

    public bool TryCompletePendingOrder() 
    {
        if (PendingPurchaseOrder != null && !PendingPurchaseOrder.Satisfied) 
        {
            var dataManager = DataManager.Instance;
            if (dataManager.TryWithdrawItems(PendingPurchaseOrder.ItemId, PendingPurchaseOrder.Quantity)) 
            {
                dataManager.AddCurrency(PendingPurchaseOrder.ItemId, dataManager.GetItemValue(PendingPurchaseOrder.ItemId) * PendingPurchaseOrder.Quantity);
                PendingPurchaseOrder.MarkAsComplete();
                return true;
            }
        }
        return false;
    }

    public void DiscardPendingOrder() 
    {
        //hide speech bubble
        PendingPurchaseOrder = null;
    }
}

public class PurchaseOrder 
{
    public string ItemId;
    public int Quantity;
    public DateTime StartTime;

    public bool Satisfied { get; private set; }

    public bool MarkAsComplete() => Satisfied = true;
}
