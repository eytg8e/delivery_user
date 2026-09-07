using UnityEngine;
using UnityEngine.InputSystem;

public class CarryTest : MonoBehaviour
{
    [SerializeField] private PlayerCarry playerCarry;
    [SerializeField] private PlacementController placementController;
    [SerializeField] private ItemInstance testItem;

    private void Update()
    {
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            if (!testItem.IsCarried)
            {
                playerCarry.Pickup(testItem);
            }
            else placementController.Place();
        }
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            // if (testItem.IsCarried) playerCarry.BeginPlacement();
            if (!testItem.ItemState.IsPacked) testItem.Pack();
            else testItem.Unpack();
        }

        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            // if (testItem.IsCarried) playerCarry.BeginPlacement();
            if (!testItem.ItemState.IsPacked)
            {
                if (!testItem.ItemState.IsActive) testItem.Activate();
                else testItem.Deactivate();
            }
            else Debug.Log("Can't activate the packed item!");
        }
    }
}
