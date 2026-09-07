using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private PlayerCarry playerCarry;
    [SerializeField] private Transform playerRoot;

    [SerializeField] private List<ItemInstance> foundItems;

    [SerializeField] private ItemInstance currentTarget;
    public ItemInstance CurrentTarget => currentTarget;


    private void OnTriggerEnter(Collider collider)
    {
        ItemInstance foundItem = collider.gameObject.GetComponentInParent<ItemInstance>();
        if (foundItems.Contains(foundItem)) return;
        foundItems.Add(foundItem);
    }

    private void OnTriggerExit(Collider collider)
    {
        ItemInstance foundItem = collider.gameObject.GetComponentInParent<ItemInstance>();
        if (foundItems.Contains(foundItem)) foundItems.Remove(foundItem);
    }

    private void UpdateInteractionTarget()
    {
        if (playerCarry.CurrentItem != null) ClearInteractionTarget(); // 들고 있는 아이템이 있으면 currentTarget을 없앤다
        else
        {
            ItemInstance target = currentTarget;    // 기존 currentTarget을 저장한다
            ItemInstance newTarget = FindClosestItem();
            if (newTarget != target)
            {
                if (currentTarget != null) ClearInteractionTarget();
                if (newTarget != null) SetInteractionTarget(newTarget);
            }
        }
    }

    private ItemInstance FindClosestItem()
    {
        ItemInstance target = null;
        float minDistance = float.PositiveInfinity;
        foreach (ItemInstance foundItem in foundItems)
        {
            float distance = Vector3.Distance(foundItem.transform.position, playerRoot.position);
            if (distance < minDistance)
            {
                target = foundItem;
                minDistance = distance;
            }
        }

        return target;
    }

    private void ClearInteractionTarget()
    {
        if (currentTarget == null) return;

        currentTarget.SetHighlighted(false);
        currentTarget = null;
    }

    private void SetInteractionTarget(ItemInstance newTarget)
    {
        currentTarget = newTarget;
        currentTarget.SetHighlighted(true);
    }

    public void PickupTarget()
    {
        // 상호작용 가능 물체가 없으면 그냥 종료
        if (currentTarget == null) return;
        // 이미 들고 있는 물체가 있어도 그냥 종료
        if (playerCarry.CurrentItem != null) return;

        currentTarget.SetHighlighted(false);

        // playerCarry가 currentTarget을 줍는다
        playerCarry.Pickup(currentTarget);

        // 줍기에 성공했으면
        if (playerCarry.CurrentItem != null) ClearInteractionTarget();
    }

    public void PackTarget()
    {
        ItemInstance interactionTarget = null;
        if (playerCarry.CurrentItem != null) interactionTarget = playerCarry.CurrentItem;
        else if (currentTarget != null) interactionTarget = currentTarget;

        if (interactionTarget == null) return;

        if (!interactionTarget.ItemState.IsPacked) interactionTarget.Pack();
        else interactionTarget.Unpack();
    }

    // public void UnpackTarget()
    // {
    //     ItemInstance interactionTarget = null;
    //     if (playerCarry.CurrentItem != null) interactionTarget = playerCarry.CurrentItem;
    //     else if (currentTarget != null) interactionTarget = currentTarget;

    //     if (interactionTarget == null) return;

    //     interactionTarget.Unpack();
    // }

    public void ActivateTarget()
    {
        ItemInstance interactionTarget = null;
        if (playerCarry.CurrentItem != null) interactionTarget = playerCarry.CurrentItem;
        else if (currentTarget != null) interactionTarget = currentTarget;

        if (interactionTarget == null) return;

        if (interactionTarget.ItemState.IsActive) interactionTarget.Deactivate();
        else interactionTarget.Activate();
    }

    // public void DeactivateTarget()
    // {
    //     ItemInstance interactionTarget = null;
    //     if (playerCarry.CurrentItem != null) interactionTarget = playerCarry.CurrentItem;
    //     else if (currentTarget != null) interactionTarget = currentTarget;

    //     if (interactionTarget == null) return;

    //     interactionTarget.Deactivate();
    // }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // 매 프레임마다 인터랙션 할 수 있는 타겟이 있는지 확인한다
    void Update()
    {
        UpdateInteractionTarget();
    }
}
