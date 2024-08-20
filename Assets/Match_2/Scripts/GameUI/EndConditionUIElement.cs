using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EndConditionUIElement
{
    [SerializeField] private ElementType _type;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private GameObject parentObject;

    public void Init(EndCondition _endCondition)
    {
        _type = _endCondition.ElementType;
        image.sprite = _endCondition.Sprite;
        amountText.SetText(_endCondition.Amount.ToString());
        parentObject.SetActive(true);
    }

    public void UpdateAmount(int _newAmount)
    {
        if (_newAmount < 0)
            return;

        amountText.SetText(_newAmount.ToString());
    }
}