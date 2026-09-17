using Unity.VisualScripting;
using UnityEngine;

public class ScatterFeature : MonoBehaviour
{
    [SerializeField] private ItemInstance item;
    [SerializeField] private PlayerCarry playerCarry;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        if (item == playerCarry.CurrentItem && !item.ItemState.IsPacked)
        {
            playerCarry.PlacementController.Place();
        }
    }
}
