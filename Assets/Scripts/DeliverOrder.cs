using UnityEngine;

public class DeliverOrder
{
    private DeliveryData deliveryData;
    public DeliveryData DeliveryData => deliveryData;
    private bool isDelivered;
    public bool IsDelivered
    {
        get => isDelivered;
        set => isDelivered = value;
    }
}
