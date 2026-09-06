using UnityEngine;

[RequireComponent(typeof(PlacementController))]
public class PlayerCarry : MonoBehaviour
{
    [SerializeField] private Transform carryPoint;
    [SerializeField] private PlacementController placementController;

    private ItemInstance currentItem;
    public ItemInstance CurrentItem => currentItem;

    // placementcontroller가 준비되었는지 확인한다
    private void Awake()
    {
        placementController = GetComponent<PlacementController>();

        if (placementController == null) enabled = false;
    }


    // PlayerInteraction에서 받아온 ItemInstance를 가지고 집어올린다
    public void Pickup(ItemInstance item)
    {
        if (item == null || currentItem != null) return;

        item.BeginCarry();

        currentItem = item;

        currentItem.transform.SetParent(carryPoint.transform);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;

        placementController.BeginPlacement(currentItem);
    }

    public void BeginPlacement()
    {
        if (currentItem == null || placementController.IsPlacing) return;
        placementController.BeginPlacement(currentItem);
    }

    public void Place(Vector3 position, Quaternion rotation)
    {
        if (currentItem == null || !placementController.IsPlacing) return;

        currentItem.transform.SetParent(null, true);
        currentItem.transform.SetPositionAndRotation(position, rotation);
        currentItem.EndCarry();

        currentItem = null;
    }

}
