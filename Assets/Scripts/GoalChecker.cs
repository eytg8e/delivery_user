using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GoalChecker : MonoBehaviour
{
    [SerializeField] private ItemInstance[] requiredItems;
    public ItemInstance[] RequiredItems => requiredItems;
    [SerializeField] private bool[] completedList;
    public bool[] CompletedList => completedList;
    private HashSet<ItemInstance> goalIns = new();
    public HashSet<ItemInstance> GoalIns => goalIns;

    public event Action CompletedListUpdated;

    public int currentNum = 0;

    [SerializeField] private StageController stageController;

    void Awake()
    {
        completedList = new bool[requiredItems.Length];
        UpdateCompletedList();
    }

    private void OnTriggerEnter(Collider collider)
    {
        ItemInstance triggeredItem = collider.gameObject.GetComponentInParent<ItemInstance>();
        if (requiredItems.Contains(triggeredItem))
        {
            goalIns.Add(triggeredItem);
            currentNum += 1;
            UpdateCompletedList();
            CheckClear();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        ItemInstance triggeredItem = collider.gameObject.GetComponentInParent<ItemInstance>();
        if (requiredItems.Contains(triggeredItem))
        {
            goalIns.Remove(triggeredItem);
            currentNum -= 1;
            UpdateCompletedList();
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

    private void UpdateCompletedList()
    {
        int i = 0;
        foreach (ItemInstance requiredItem in requiredItems)
        {
            if (goalIns.Contains(requiredItem)) completedList[i] = true;
            else completedList[i] = false;
            i++;
        }

        CompletedListUpdated?.Invoke();
    }
}
