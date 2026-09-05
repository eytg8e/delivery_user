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
    Bounce
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public float weight;
    public ItemCategory category;
    public ItemFeature[] features;
    public bool canPack;
    public bool canActivate;
}