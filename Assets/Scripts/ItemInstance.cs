using System;
using UnityEngine;

public class ItemInstance : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    public ItemData ItemData => itemData;
    [SerializeField] private ItemState itemState = new ItemState();
    public ItemState ItemState => itemState;

    [SerializeField] private Rigidbody itemRigidbody;
    public Rigidbody ItemRigidbody => itemRigidbody;
    [SerializeField] private Collider itemCollider;
    public Collider ItemCollider => itemCollider;

    [SerializeField] private GameObject packedMesh;
    public GameObject PackedMesh => packedMesh;
    [SerializeField] private GameObject openedMesh;
    public GameObject OpenedMesh => openedMesh;
    // 활성화됐을 때 보여줄 게임오브젝트
    [SerializeField] private GameObject activatedMesh;
    public GameObject ActivatedMesh => activatedMesh;

    //운반 중인지 상태를 확인하는 bool
    [SerializeField] private bool isCarried;
    public bool IsCarried => isCarried;

    // 현재 아이템 GameObject를 저장
    [SerializeField] private GameObject currentVisual;
    public GameObject CurrentVisual => currentVisual;

    // 아이템 상태 변화 알림
    public event Action VisualChanged;


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

        itemState.IsPacked = false;
        itemState.IsActive = false;

        ApplyState();
    }

    public void Activate()
    {
        if (!itemData.CanActivate) return;
        if (itemState.IsPacked) return;

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
        }

        else if (showActivated)
        {
            itemCollider = activatedMesh.GetComponent<Collider>();
            currentVisual = activatedMesh;
        }

        else
        {
            itemCollider = openedMesh.GetComponent<Collider>();
            currentVisual = openedMesh;
        }

        itemCollider.enabled = !isCarried;

        if (previousVisual != currentVisual) VisualChanged?.Invoke(); // ?를 붙이면 이벤트를 구독한 것이 있을 때만 작동한다

    }

    #region playerCarry에게 운반 상태를 받았을 때 상태 전환
    public void BeginCarry()
    {
        isCarried = true;

        int carryLayer = LayerMask.NameToLayer("CarriedItem");

        SetLayer(gameObject, carryLayer);

        itemRigidbody.isKinematic = true;

        ApplyState();
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
}
