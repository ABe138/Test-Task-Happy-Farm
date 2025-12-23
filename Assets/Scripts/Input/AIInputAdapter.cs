using UnityEngine;

public class AIInputAdapter : MonoBehaviour, ILocomotionInput
{
    public Transform target;
    public float stopDistance = 1f;

    Vector2 _move;
    bool _sprint;
    Quaternion? _targetRot;

    public Vector2 Move => _move;
    public Quaternion? TargetRotation => _targetRot;
    public bool Sprint => _sprint;

    void Update()
    {
        if (target == null) { _move = Vector2.zero; _targetRot = null; return; }

        Vector3 dir = (target.position - transform.position);
        dir.y = 0f;
        float dist = dir.magnitude;
        if (dist < stopDistance) { _move = Vector2.zero; _targetRot = null; return; }

        Vector3 local = transform.InverseTransformDirection(dir.normalized);

        _move = new Vector2(local.x, local.z);
        _targetRot = Quaternion.LookRotation(new Vector3(dir.normalized.x, 0f, dir.normalized.z));
        _sprint = false;
    }
}