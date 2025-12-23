using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCBrain : MonoBehaviour, ILocomotionInput
{
    public List<BaseState> StatesQueue = new List<BaseState>();

    public Vector2 Move => _move;

    public Quaternion? TargetRotation => _targetRotation;

    public bool Sprint => false;

    private Vector2 _move;
    private Quaternion? _targetRotation;

    private void Update()
    {
        if (StatesQueue.Count > 0)
        {
            if (StatesQueue.First().Update())
            {
                StatesQueue[0].Exit();
                StatesQueue.RemoveAt(0);
                if (StatesQueue.Any()) StatesQueue[0].Enter();
            }
        }
    }

    public void AddStateToQueue(BaseState state)
    {
        StatesQueue.Add(state);
    }

    public void ClearStatesQueue() 
    {
        StatesQueue.Clear();
    }

    public void CreateLocomotionInput(Vector3 move, Quaternion? rotation)
    {
        _move = move;
        _targetRotation = rotation;
    }
}
