using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputAdapter : MonoBehaviour, ILocomotionInput
{
    public Vector2 Move => _move;

    private Vector2 _move;

    void OnMove(InputValue v) => _move = v.Get<Vector2>();
}
