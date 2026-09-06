using UnityEngine;

[CreateAssetMenu(fileName = "DeliveryData", menuName = "Scriptable Objects/DeliveryData")]
public class DeliveryData : ScriptableObject
{
    [SerializeField] private ItemData itemData;
    public ItemData ItemData => itemData;
    [SerializeField] private string receiverName;
    public string ReceiverName => receiverName;
    [SerializeField] private string destination;
    public string Destination => destination;
    [SerializeField] private string[] handlingInfo;
    public string[] HandlingInfo => handlingInfo;
    [SerializeField] private Vector3 packageSize;
    public Vector3 PackageSize => packageSize;
}
