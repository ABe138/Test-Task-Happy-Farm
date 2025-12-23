using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICoinView : MonoBehaviour
{
    [SerializeField] private Image _itemIconImage; 
    [SerializeField] private TextMeshProUGUI _itemQuantityText;

    public void SetIcon(Sprite icon) 
    {
        _itemIconImage.sprite = icon;
    }

    public void UpdateQuantity(int newValue) 
    {
        _itemQuantityText.text = $"x{newValue}";
    }
}