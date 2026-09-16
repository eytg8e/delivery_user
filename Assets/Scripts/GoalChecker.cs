using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoalChecker : MonoBehaviour
{
    [SerializeField] private ItemInstance[] requiredItems;
    private HashSet<ItemInstance> goalIns = new();

    [SerializeField] private StageController stageController;

    private void OnTriggerEnter(Collider collider)
    {
        ItemInstance triggeredItem = collider.gameObject.GetComponentInParent<ItemInstance>();
        if (requiredItems.Contains(triggeredItem))
        {
            goalIns.Add(triggeredItem);
            CheckClear();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        ItemInstance triggeredItem = collider.gameObject.GetComponentInParent<ItemInstance>();
        if (requiredItems.Contains(triggeredItem))
        {
            goalIns.Remove(triggeredItem);
        }
    }

    private void CheckClear()
    {
        Debug.Log(goalIns);
        foreach (ItemInstance requiredItem in requiredItems)
        {
            if (!goalIns.Contains(requiredItem)) return;
        }
        stageController.ClearStage();
    }
}
