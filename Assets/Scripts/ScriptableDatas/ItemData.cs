using UnityEngine;

public enum ItemCategory
{
    [InspectorName("생활용품")] Household,
    [InspectorName("스포츠용품")] Sports,
    [InspectorName("가전제품")] Electronics,
    [InspectorName("알 수 없음")] Unknown
}

public enum ItemFeature
{
    Blow,
    Glide,
    Bounce,
    Scatter
}

public enum CarryCondition
{
    AnyState,
    PackedOnly
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    public string ItemName => itemName;
    [SerializeField] private float weight;
    public float Weight => weight;
    [SerializeField] private ItemCategory category;
    public ItemCategory Category => category;
    [SerializeField] private ItemFeature[] features;
    public ItemFeature[] Features => features;
    [SerializeField] private bool canPack;
    public bool CanPack => canPack;

    [SerializeField] private bool canActivate;
    public bool CanActivate => canActivate;

    [SerializeField] private CarryCondition carryCondition;
    public CarryCondition CarryCondition => carryCondition;
}

