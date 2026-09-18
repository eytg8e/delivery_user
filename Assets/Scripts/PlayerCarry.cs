using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlacementController))]
public class PlayerCarry : MonoBehaviour
{
    [SerializeField] private Transform carryPoint;
    [SerializeField] private PlacementController placementController;
    public PlacementController PlacementController => placementController;

    private ItemInstance currentItem;
    public ItemInstance CurrentItem
    {
        get => currentItem;
        set => currentItem = value;
    }

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

        bool canPickup = item.BeginCarry();

        if (!canPickup)
        {
            Debug.Log("Can't pickup Packedonly Item!");
            return;
        }

        currentItem = item;

        currentItem.transform.SetParent(carryPoint.transform);
        currentItem.transform.localPosition = new Vector3(0f, -0.5f, 0f);
        if (currentItem.ItemData.Features.Contains(ItemFeature.Glide)) currentItem.transform.localRotation = Quaternion.identity;

        currentItem.gameObject.layer = LayerMask.NameToLayer("CarriedItem");

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

        currentItem.gameObject.layer = LayerMask.NameToLayer("Stackable");

        currentItem = null;
    }

    public void Cancel()
    {
        placementController.Cancel();
        currentItem.transform.SetParent(null, true);
        currentItem.EndCarry();
        currentItem.gameObject.layer = LayerMask.NameToLayer("Stackable");
        currentItem = null;
    }

}
