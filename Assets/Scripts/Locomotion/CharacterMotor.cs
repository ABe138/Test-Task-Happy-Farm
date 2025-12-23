using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{
    [SerializeField] private LocomotionConfig config;
    [SerializeField] private CharacterController characterController;

    public Action<bool> OnGroundedChanged;
    public Action<Vector3> OnVelocityChanged;

    ILocomotionInput inputProvider;
    Vector3 velocity;
    float verticalVel;
    bool wasGrounded;

    private void Start()
    {
        inputProvider = GetComponent<ILocomotionInput>();
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // Input
        Vector2 moveInput = inputProvider?.Move ?? Vector2.zero;   // x = A/D (west/east), y = W/S (north/south)
        bool sprint = inputProvider?.Sprint ?? false;
        Quaternion? targetRot = inputProvider?.TargetRotation;

        // Horizontal target velocity (WORLD-space): map input directly to world axes (no transform)
        float targetSpeed = sprint ? config.sprintSpeed : config.walkSpeed;
        Vector3 inputDirWorld = new Vector3(moveInput.x, 0f, moveInput.y);    // X = west(-)/east(+), Z = south(-)/north(+)
        inputDirWorld = Vector3.ClampMagnitude(inputDirWorld, 1f);
        Vector3 targetVelXZ = inputDirWorld * targetSpeed;

        // Smooth accel/decel
        Vector3 currentVelXZ = new Vector3(velocity.x, 0f, velocity.z);
        float accel = (targetVelXZ.magnitude > currentVelXZ.magnitude) ? config.acceleration : config.deceleration;
        Vector3 newVelXZ = Vector3.MoveTowards(currentVelXZ, targetVelXZ, accel * dt);

        // Rotation: face movement direction (top-down). Only rotate when horizontal movement magnitude exceeds threshold.
        Vector3 moveDirection = new Vector3(newVelXZ.x, 0f, newVelXZ.z);
        float moveSpeed = moveDirection.magnitude;
        if (moveSpeed > config.faceThreshold)
        {
            Vector3 forward = moveDirection.normalized;
            Quaternion desired = Quaternion.LookRotation(forward, Vector3.up);

            float maxDeg = config.turnSpeed * dt;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desired, maxDeg);
        }
        else if (targetRot.HasValue)
        {
            // If nearly stopped, optionally follow input-provided rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot.Value, 20f * dt);
        }

        // Simple gravity snap to keep grounded behaviour stable
        bool grounded = characterController.isGrounded;
        if (grounded && verticalVel < 0f) verticalVel = -1f;
        verticalVel += config.gravity * dt;
        verticalVel = Mathf.Max(verticalVel, config.terminalVelocity);

        velocity = new Vector3(newVelXZ.x, verticalVel, newVelXZ.z);

        characterController.Move(velocity * dt);

        bool nowGrounded = characterController.isGrounded;
        if (nowGrounded != wasGrounded) OnGroundedChanged?.Invoke(nowGrounded);
        wasGrounded = nowGrounded;

        OnVelocityChanged?.Invoke(velocity);
    }

    public Vector3 GetVelocity() => velocity;
}
