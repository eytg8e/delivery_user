using NUnit.Framework;
using UnityEngine;

[System.Serializable]
public class ItemState
{
    [SerializeField] private bool isPacked;
    public bool IsPacked
    {
        get => isPacked;
        set => isPacked = value;
    }
    [SerializeField] private bool isActive;
    public bool IsActive
    {
        get => isActive;
        set => isActive = value;
    }
}
