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
    void Awake()
    {

    }

    void OnDestroy()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SetCurrentUITarget();
    }

    void SetCurrentUITarget()
    {
        if (playerCarry.CurrentItem == null) currentUITarget = playerInteraction.CurrentTarget;
        else currentUITarget = playerCarry.CurrentItem;

        if (currentUITarget == null) return;

        SetItemStateUI();
    }

    void SetItemStateUI()
    {
        // if (currentUITarget.ItemState.IsPacked)
        // {
        //     uiController.ShowPackedItemStateUI();
        // }

        // else
        // {
        //     uiController.ShowUnPackedItemStateUI();
        // }
    }

    void SetWeightHUD()
    {
        uiController.UpdateWeightHUD(currentUITarget);
    }
}
