using System;
using UnityEngine;

public abstract class BaseState
{
    protected NPCBrain _npc;

    public BaseState(NPCBrain npc)
    {
        _npc = npc;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public abstract bool Update();
}

public class MoveState : BaseState
{
    private Vector3 _targetPosition;

    private const float StoppingDistance = 1f;

    public MoveState(NPCBrain npc, Vector3 targetPosition) : base(npc)
    {
        _targetPosition = targetPosition;
    }

    public override bool Update()
    {
        var dir = (_targetPosition - _npc.transform.position);
        dir.y = 0f;
        var dist = dir.magnitude;
        if (dist < StoppingDistance)
        {
            _npc.CreateLocomotionInput(Vector2.zero, null);
            return true;
        }
        else
        {
            var norm = dir.normalized;
            _npc.CreateLocomotionInput(new Vector2(norm.x, norm.z), Quaternion.LookRotation(new Vector3(norm.x, 0f, norm.z)));
            return false;
        }
    }
}

public class FollowState : BaseState
{
    private Transform _targetTransform;

    private const float StoppingDistance = 3f;

    public FollowState(NPCBrain npc, Transform targetTransform) : base(npc)
    {
        _targetTransform = targetTransform;
    }

    public override bool Update()
    {
        var targetPosition = _targetTransform.position;
        var dir = (targetPosition - _npc.transform.position);
        dir.y = 0f;
        var dist = dir.magnitude;
        if (dist < StoppingDistance)
        {
            _npc.CreateLocomotionInput(Vector2.zero, null);
            return true;
        }
        else
        {
            var norm = dir.normalized;
            _npc.CreateLocomotionInput(new Vector2(norm.x, norm.z), Quaternion.LookRotation(new Vector3(norm.x, 0f, norm.z)));
            return false;
        }
    }
}

public class FollowQueueState : BaseState
{
    private const float StoppingDistance = 3f;

    public FollowQueueState(NPCBrain npc) : base(npc)
    {

    }

    public override bool Update()
    {
        var npcInFront = NPCQueueManager.Instance.GetNPCInFront(_npc);
        if (npcInFront != null)
        {
            var targetPosition = npcInFront.transform.position;
            var dir = (targetPosition - _npc.transform.position);
            dir.y = 0f;
            var dist = dir.magnitude;
            if (dist < StoppingDistance)
            {
                _npc.CreateLocomotionInput(Vector2.zero, null);
            }
            else
            {
                var norm = dir.normalized;
                _npc.CreateLocomotionInput(new Vector2(norm.x, norm.z), Quaternion.LookRotation(new Vector3(norm.x, 0f, norm.z)));
            }
            return false;
        }
        return true;
    }
}

public class MakePurchaseState : BaseState
{
    private (string id, int q) _order;

    private const float OrderWaitTime = 20f;

    public MakePurchaseState(NPCBrain npc, (string, int) order) : base(npc)
    {
        _order = order;
    }

    public override void Enter()
    {
        base.Enter();
        NPCQueueManager.Instance.SetPendingPurchaseOrder(new PurchaseOrder
        {
            ItemId = _order.id,
            Quantity = _order.q,
            StartTime = DateTime.Now
        });
    }

    public override bool Update()
    {
        var pendingOrder = NPCQueueManager.Instance.PendingPurchaseOrder;
        if (pendingOrder != null)
        {
            if (pendingOrder.StartTime.AddSeconds(OrderWaitTime) > DateTime.Now)
                return pendingOrder.Satisfied;
            else
                NPCQueueManager.Instance.DiscardPendingOrder();
        }
        return true;
    }

    public override void Exit()
    {
        base.Exit();
        NPCQueueManager.Instance.LeaveQueue(_npc);
    }
}

public class Despawn : BaseState
{
    public Despawn(NPCBrain npc) : base(npc)
    {

    }

    public override bool Update()
    {
        return true;
    }

    public override void Exit()
    {
        base.Exit();
        PoolingManager.Instance.Release(_npc);
    }
}

