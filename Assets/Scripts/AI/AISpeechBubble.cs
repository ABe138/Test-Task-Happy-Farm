using TMPro;
using UnityEngine;

public class AISpeechBubble : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _speechText;

    private void Update()
    {
        transform.LookAt(
            Camera.main.transform,
            Vector3.up
        );
    }

    public void ShowMessage(string text)
    {
        _speechText.text = text;
    }
}
