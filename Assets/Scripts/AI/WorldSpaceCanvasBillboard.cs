using TMPro;
using UnityEngine;

public class WorldSpaceCanvasBillboard : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    void LateUpdate()
    {
        var cameraTransform = _camera.transform;
        var lookRotation = cameraTransform.rotation.eulerAngles;

        transform.rotation = Quaternion.Euler(lookRotation);
    }
}
