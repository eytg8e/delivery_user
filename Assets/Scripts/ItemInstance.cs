using System;
using System.Linq;
using UnityEngine;

public class ItemInstance : MonoBehaviour
{
    [Header("Itemdata & ItemState")]
    [SerializeField] private ItemData itemData;
    public ItemData ItemData => itemData;
    [SerializeField] private ItemState itemState;
    public ItemState ItemState => itemState;

    [Header("Rigidbody & Collider")]
    [SerializeField] private Rigidbody itemRigidbody;
    public Rigidbody ItemRigidbody => itemRigidbody;
    [SerializeField] private Collider itemCollider;
    public Collider ItemCollider => itemCollider;

    [Header("Mesh & Outline")]
    [SerializeField] private GameObject packedMesh;
    public GameObject PackedMesh => packedMesh;
    [SerializeField] private GameObject packedOutline;
    public GameObject PackedOutline => packedOutline;
    [SerializeField] private GameObject openedMesh;
    public GameObject OpenedMesh => openedMesh;
    [SerializeField] private GameObject openedOutline;
    public GameObject OpenedOutline => openedOutline;
    // 활성화됐을 때 보여줄 게임오브젝트
    [SerializeField] private GameObject activatedMesh;
    public GameObject ActivatedMesh => activatedMesh;
    [SerializeField] private GameObject activatedOutline;
    public GameObject ActivatedOutline => activatedOutline;

    // 현재 아이템 GameObject를 저장
    [SerializeField] private GameObject currentVisual;
    public GameObject CurrentVisual => currentVisual;

    [Header("Feature")]
    [SerializeField] private GameObject blowArea;

    //운반 중인지 상태를 확인하는 bool
    [SerializeField] private bool isCarried;
    public bool IsCarried => isCarried;

    // 외곽선 활성화 여부 저장
    [SerializeField] private bool isHighlighted;
    public bool IsHighlighted => isHighlighted;

    // 아이템 상태 변화 알림
    public event Action VisualChanged;
    public event Action ItemDestroyed;


    // 첫 시작 때 Inspector에서 설정한 상태와 맞추기
    private void Awake()
    {
        ApplyState();
    }

    // 게임 내에서 포장하는 행동.
    public void Pack()
    {
        if (!itemData.CanPack) return;

        itemState.IsPacked = true;
        itemState.IsActive = false;

        ApplyState();
    }

    public void Unpack()
    {
        if (!itemData.CanPack) return;
        // if (itemData.CanOnlyCarriedWithPacked)
        // {
        //     ItemDestroyed?.Invoke();
        //     Destroy(gameObject);
        //     return;
        // }

        itemState.IsPacked = false;
        itemState.IsActive = false;

        ApplyState();
    }

    public void Activate()
    {
        if (!itemData.CanActivate)
        {
            Debug.Log("You can't activate this item!");
            return;
        }
        if (itemState.IsPacked)
        {
            Debug.Log("You can't activate packed item!");
            return;
        }

        itemState.IsActive = true;

        ApplyState();
    }

    public void Deactivate()
    {
        if (!itemData.CanActivate) return;
        if (itemState.IsPacked) return;

        itemState.IsActive = false;

        ApplyState();
    }

    public void SetHighlighted(bool value)
    {
        isHighlighted = value;

        ApplyState();
    }

    // 실제 아이템의 bool, mesh, collider를 받아온 상태로 바꾼다.
    private void ApplyState()
    {
        GameObject previousVisual = currentVisual;

        bool showPacked = itemState.IsPacked;
        bool showActivated = !itemState.IsPacked && itemState.IsActive;
        bool showOpened = !itemState.IsPacked && !itemState.IsActive;

        packedMesh.SetActive(showPacked);
        openedMesh.SetActive(showOpened);
        if (activatedMesh != null) activatedMesh.SetActive(showActivated);

        // Rigidbody, Collider 변경
        if (showPacked)
        {
            itemCollider = packedMesh.GetComponent<Collider>();
            currentVisual = packedMesh;
            packedOutline.SetActive(isHighlighted);
        }

        else if (showActivated)
        {
            itemCollider = activatedMesh.GetComponent<Collider>();
            currentVisual = activatedMesh;
            activatedOutline.SetActive(isHighlighted);
        }

        else
        {
            itemCollider = openedMesh.GetComponent<Collider>();
            currentVisual = openedMesh;
            openedOutline.SetActive(isHighlighted);
        }

        itemCollider.enabled = !isCarried;
        if (blowArea != null) blowArea.SetActive(showActivated);

        if (previousVisual != currentVisual) VisualChanged?.Invoke(); // ?를 붙이면 이벤트를 구독한 것이 있을 때만 작동한다

    }

    #region playerCarry에게 운반 상태를 받았을 때 상태 전환
    public bool BeginCarry()
    {
        if (itemData.CarryCondition == CarryCondition.PackedOnly && !itemState.IsPacked) return false;

        isCarried = true;

        int carryLayer = LayerMask.NameToLayer("CarriedItem");


        SetLayer(gameObject, carryLayer);

        itemRigidbody.isKinematic = true;


        ApplyState();

        return true;
    }

    public void EndCarry()
    {
        isCarried = false;

        int stackLayer = LayerMask.NameToLayer("Stackable");

        SetLayer(gameObject, stackLayer);

        ApplyState();

        itemRigidbody.isKinematic = false;
    }

    #endregion

    #region 내려놓으면 지형지물처럼, 운반 상태면 영향을 받지 않도록 레이어 조정하기
    private void SetLayer(GameObject item, int layer)
    {
        item.layer = layer;

        foreach (Transform child in item.transform)
        {
            SetLayer(child.gameObject, layer);
        }
    }

    #endregion

    #region 특정 feature가 있는지 확인하는 함수
    public bool HasActiveFeature(ItemFeature feature)
    {
        if (itemData.Features.Contains(ItemFeature.Glide) && itemState.IsActive) return true;
        return false;
    }

    #endregion
}
