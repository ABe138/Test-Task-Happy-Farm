using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{
    [SerializeField] private LocomotionConfig config;
    [SerializeField] private CharacterController characterController;

    public Action<bool> OnGroundedChanged;
    public Action<Vector3> OnVelocityChanged;

    private ILocomotionInput _inputProvider;
    private Vector3 _velocity;
    private float _verticalVel;
    private bool _wasGrounded;

    private void Start()
    {
        _inputProvider = GetComponent<ILocomotionInput>();
    }

    void Update()
    {
        var dt = Time.deltaTime;

        var moveInput = _inputProvider?.Move ?? Vector2.zero;

        var targetSpeed = config.walkSpeed;
        var inputDirWorld = new Vector3(moveInput.x, 0f, moveInput.y);
        inputDirWorld = Vector3.ClampMagnitude(inputDirWorld, 1f);
        var targetVelXZ = inputDirWorld * targetSpeed;

        var currentVelXZ = new Vector3(_velocity.x, 0f, _velocity.z);
        var accel = (targetVelXZ.magnitude > currentVelXZ.magnitude) ? config.acceleration : config.deceleration;
        var newVelXZ = Vector3.MoveTowards(currentVelXZ, targetVelXZ, accel * dt);

        var moveDirection = new Vector3(newVelXZ.x, 0f, newVelXZ.z);
        var moveSpeed = moveDirection.magnitude;
        if (moveSpeed > config.faceThreshold)
        {
            var forward = moveDirection.normalized;
            var desired = Quaternion.LookRotation(forward, Vector3.up);

            var maxDeg = config.turnSpeed * dt;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desired, maxDeg);
        }

        var grounded = characterController.isGrounded;
        if (grounded && _verticalVel < 0f) _verticalVel = -1f;
        _verticalVel += config.gravity * dt;
        _verticalVel = Mathf.Max(_verticalVel, config.terminalVelocity);

        _velocity = new Vector3(newVelXZ.x, _verticalVel, newVelXZ.z);

        characterController.Move(_velocity * dt);

        var nowGrounded = characterController.isGrounded;
        if (nowGrounded != _wasGrounded) OnGroundedChanged?.Invoke(nowGrounded);
        _wasGrounded = nowGrounded;

        OnVelocityChanged?.Invoke(_velocity);
    }
}
