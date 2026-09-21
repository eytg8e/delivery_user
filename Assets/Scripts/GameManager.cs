using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlacementController placementController;
    public PlacementController PlacementController
    {
        get => placementController;
        set => PlacementController = value;
    }
    [SerializeField] private PlayerCarry playerCarry;
    public PlayerCarry PlayerCarry
    {
        get => playerCarry;
        set => playerCarry = value;
    }
    [SerializeField] private PlayerInteraction playerInteraction;
    public PlayerInteraction PlayerInteraction
    {
        get => playerInteraction;
        set => playerInteraction = value;
    }

    [SerializeField] private PlayerController playerController;
    public PlayerController PlayerController
    {
        get => playerController;
        set => playerController = value;
    }
    [SerializeField] private GoalChecker goalChecker;
    public GoalChecker GoalChecker
    {
        get => goalChecker;
        set => goalChecker = value;
    }
    [SerializeField] private StageController stageController;
    public StageController StageController
    {
        get => stageController;
        set => stageController = value;
    }
    [SerializeField] private UIController uiController;
    public UIController UIController
    {
        get => uiController;
        set => uiController = value;
    }

    [SerializeField] private ItemInstance currentUITarget;
    public ItemInstance CurrentUITarget
    {
        get => currentUITarget;
        set => currentUITarget = value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        goalChecker.CompletedListUpdated += SetDeliveryStateUIValue;
        playerInteraction.CurrentTargetChanged += GetCurrentUITarget;
        playerInteraction.CurrentTargetChanged += SetDeliveryDataValue;

        playerInteraction.CurrentTargetChanged += SetControlStateUIValue;
        playerInteraction.ItemChanged += SetControlStateUIValue;

        SetDeliveryStateUIValue();
        uiController.UpdateControlPanelUI();
    }

    void OnDisable()
    {
        goalChecker.CompletedListUpdated -= SetDeliveryStateUIValue;
        playerInteraction.CurrentTargetChanged -= GetCurrentUITarget;
        playerInteraction.CurrentTargetChanged -= SetDeliveryDataValue;

        playerInteraction.CurrentTargetChanged -= SetControlStateUIValue;
        playerInteraction.ItemChanged -= SetControlStateUIValue;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void GetCurrentUITarget()
    {
        if (playerCarry.CurrentItem == null) currentUITarget = playerInteraction.CurrentTarget;
        else currentUITarget = playerCarry.CurrentItem;

        if (currentUITarget == null)
        {
            uiController.UpdateControlPanelUI();
            uiController.UpdatePackedItemStateUI();
            return;
        }
    }

    // 현재 배달한 상태, GoalChecker가 쓰인다
    void SetDeliveryStateUIValue()
    {
        int currentNum = goalChecker.currentNum;
        int goalNum = goalChecker.RequiredItems.Length;
        bool[] completedList = goalChecker.CompletedList;

        uiController.UpdateDeliveryStateUI(currentNum, goalNum, completedList);
    }

    // 좌측 하단 컨트롤 패널, CurrentUITarget의 특성과 PlayerCarry 상태에 따라 달라진다
    void SetControlStateUIValue()
    {
        if (currentUITarget == null) return;

        uiController.UpdateControlPanelUI();
    }

    // 플레이어가 들고 있는 아이템의 배달 정보, 우측 하단 UI에 넣을 내용.
    void SetDeliveryDataValue()
    {
        if (currentUITarget == null) return;

        String receiver = "To. " + currentUITarget.DeliveryData.ReceiverName;
        String category = "Not assigned yet";
        switch (currentUITarget.ItemData.Category)
        {
            case ItemCategory.Electronics:
                category = "Electronics";
                break;
            case ItemCategory.Household:
                category = "Housewares";
                break;
            case ItemCategory.Sports:
                category = "Sports";
                break;
            case ItemCategory.Unknown:
                category = "Unknown";
                break;
        }

        String destination = currentUITarget.DeliveryData.Destination;

        uiController.UpdatePackedItemStateUI(receiver, category, destination);

    }
}
