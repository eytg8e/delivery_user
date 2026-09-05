using UnityEngine;

public class ItemState
{
    private bool isPacked;
    public bool IsPacked
    {
        get => isPacked;
        set => isPacked = value;
    }
    private bool isActive;
    public bool IsActive
    {
        get => isActive;
        set => isActive = value;
    }
}
