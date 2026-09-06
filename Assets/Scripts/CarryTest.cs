using UnityEngine;
using UnityEngine.InputSystem;

public class CarryTest : MonoBehaviour
{
    [SerializeField] private PlayerCarry playerCarry;
    [SerializeField] private ItemInstance testItem;

    private void Update()
    {
        if(Keyboard.current.digit1Key.wasPressedThisFrame) playerCarry.Pickup(testItem);
        if(Keyboard.current.digit2Key.wasPressedThisFrame) playerCarry.BeginPlacement();
    }
}
