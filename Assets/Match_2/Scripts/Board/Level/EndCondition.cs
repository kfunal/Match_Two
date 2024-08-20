using UnityEngine;

[System.Serializable]
public class EndCondition
{
    [SerializeField] private string name;
    [SerializeField] private ElementType elementType;
    [SerializeField] private int amount;
    [SerializeField] private Sprite sprite;
    [SerializeField] private bool completed;

    private int currentAmount;

    public ElementType ElementType => elementType;
    public int Amount => amount;
    public int CurrentAmount => currentAmount;
    public Sprite Sprite => sprite;
    public bool Completed => completed;

    public EndCondition(ElementType _type, Sprite _sprite, int _amount)
    {
        elementType = _type;
        amount = _amount;
        sprite = _sprite;
        completed = false;
    }

    public void ResetCompleted()
    {
        completed = false;
        currentAmount = amount;
    }

    public void DecreaseAmount(int _amount)
    {
        if (completed)
            return;

        currentAmount -= _amount;

        if (currentAmount <= 0)
            completed = true;
    }
}
