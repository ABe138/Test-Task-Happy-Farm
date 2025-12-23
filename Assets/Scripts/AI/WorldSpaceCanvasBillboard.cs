using TMPro;
using UnityEngine;

public class WorldSpaceCanvasBillboard : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    //private void Update()
    //{
    //    transform.LookAt(
    //        _camera.transform,
    //        Vector3.up
    //    );
    //}

    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;

    void LateUpdate()
    {
        Transform cameraTransform = _camera.transform;
        Vector3 lookRotation = cameraTransform.rotation.eulerAngles;

        // Optionally lock specific axes
        if (lockX) lookRotation.x = transform.rotation.eulerAngles.x;
        if (lockY) lookRotation.y = transform.rotation.eulerAngles.y;
        if (lockZ) lookRotation.z = transform.rotation.eulerAngles.z;

        transform.rotation = Quaternion.Euler(lookRotation);
    }
}
