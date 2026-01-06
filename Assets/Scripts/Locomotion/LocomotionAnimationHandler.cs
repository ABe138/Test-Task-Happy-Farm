using UnityEngine;

public class LocomotionAnimationHandler : MonoBehaviour
{
    [SerializeField] private CharacterMotor motor;
    [SerializeField] private Animator animator;

    void OnEnable()
    {
        motor.OnVelocityChanged += HandleVelocity;
        motor.OnGroundedChanged += HandleGround;
    }

    void OnDisable()
    {
        motor.OnVelocityChanged -= HandleVelocity;
        motor.OnGroundedChanged -= HandleGround;
    }

    void HandleVelocity(Vector3 velocity)
    {
        var speed = new Vector3(velocity.x, 0f, velocity.z).magnitude;
        animator?.SetFloat("Speed", speed);
    }

    void HandleGround(bool grounded) => animator?.SetBool("Grounded", grounded);
}