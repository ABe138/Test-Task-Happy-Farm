using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig/Locomotion")]
public class LocomotionConfig : ScriptableObject
{
    public float walkSpeed = 4f;
    public float acceleration = 30f;
    public float deceleration = 30f;
    public float gravity = -24f;
    public float terminalVelocity = -50f;
    public float groundCheckDistance = 0.1f;
    public float turnSpeed = 720f;
    public float faceThreshold = 0.1f;
}
