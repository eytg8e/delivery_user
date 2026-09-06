using UnityEngine;
using UnityEngine.InputSystem;

public class CarryTest : MonoBehaviour
{
    [SerializeField] private PlayerCarry playerCarry;
    [SerializeField] private PlacementController placementController;
    [SerializeField] private ItemInstance testItem;

    private void Update()
    {
        if (Keyboard.current.leftCtrlKey.wasPressedThisFrame || Keyboard.current.rightCtrlKey.wasPressedThisFrame)
        {
            if (!testItem.IsCarried)
            {
                playerCarry.Pickup(testItem);
                playerCarry.BeginPlacement();
            }
            else placementController.Place();
        }
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame || Keyboard.current.rightShiftKey.wasPressedThisFrame)
        {
            // if (testItem.IsCarried) playerCarry.BeginPlacement();
            if (!testItem.ItemState.IsPacked) testItem.Pack();
            else testItem.Unpack();
        }

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            // if (testItem.IsCarried) playerCarry.BeginPlacement();
            if (!testItem.ItemState.IsPacked)
            {
                if (!testItem.ItemState.IsActive) testItem.Activate();
                else testItem.Deactivate();
            }
            else Debug.Log("Can't activate the packed item!");
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame && placementController.IsPlacing)
        {

        }
    }
}
