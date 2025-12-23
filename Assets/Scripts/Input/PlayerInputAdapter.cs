using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputAdapter : MonoBehaviour, ILocomotionInput
{
    Vector2 move;
    bool sprint;
    Quaternion? targetRot;

    public Vector2 Move => move;
    public Quaternion? TargetRotation => targetRot;
    public bool Sprint => sprint;

    void OnMove(InputValue v) => move = v.Get<Vector2>();
    void OnSprint(InputValue v) => sprint = v.isPressed;
}
