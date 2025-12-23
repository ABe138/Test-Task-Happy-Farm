using UnityEngine;

public interface ILocomotionInput
{
    Vector2 Move { get; }
    Quaternion? TargetRotation { get; }

    bool Sprint { get; }
}
