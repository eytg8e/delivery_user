using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class PlacementController : MonoBehaviour
{
    [SerializeField] private ItemInstance currentItem;
    public ItemInstance CurrentItem => currentItem;

    [SerializeField] private PlayerCarry playerCarry;

    // 예상 배치도를 보여주는 프리뷰 구현용 필드
    [Tooltip("프리뷰를 보여줄 아이템")]
    [SerializeField] private GameObject preview;
    public GameObject Preview => preview;
    [SerializeField] private Material previewMaterial;
    [SerializeField] private Material canPlaceMaterial;
    [SerializeField] private Material cannotPlaceMaterial;

    private Collider previewCollider;
    // 프리뷰 높이를 저장한 뒤, 바닥에서 높이/2만큼 올려 바닥에 붙은 것처럼 보이게 하기
    private Renderer previewRenderer;
    private float previewHeightOffset;

    // 최대 배치 가능 높이 위쪽 및 아래쪽 방향
    [SerializeField] private float maxStackHeight = 3f;
    public float MaxStackHeight
    {
        get => maxStackHeight;
        set => maxStackHeight = value;
    }
    [SerializeField] private float maxDropDepth = 5f;

    // 이미 배치 중인데 다시 배치하려 하는지 확인
    private bool isPlacing;
    public bool IsPlacing => isPlacing;

    // 배치 가능 여부 저장
    [SerializeField] private bool canPlace;
    public bool CanPlace => canPlace;

    [SerializeField] private LayerMask placementSurfaceMask;


    private void Awake()
    {
        playerCarry = GetComponent<PlayerCarry>();
    }

    private void Update()
    {
        if (!isPlacing) return;

        UpdatePreviewPosition();
    }

    public void BeginPlacement(ItemInstance item)
    {
        if (isPlacing || item == null) return;

        currentItem = item;
        GameObject visualSource = currentItem.CurrentVisual;
        preview = Instantiate(visualSource);
        currentItem.VisualChanged += UpdatePreviewVisual; // 현재 아이템의 visualchanged 이벤트 발생 시 다음 함수를 발생시킨다

        int previewLayer = LayerMask.NameToLayer("PlacementPreview");
        SetLayer(preview, previewLayer); // preview의 mesh가 여러 개일 때도 모두 레이어를 적용시키는 함수

        previewRenderer = preview.GetComponentInChildren<Renderer>();
        previewCollider = preview.GetComponent<Collider>();
        SetMaterial(preview, previewMaterial);
        // bounds로 물체를 둘러싸는 박스의 크기를 계산, extents.y로 중심부터 박스까지의 거리(높이/2) 찾기

        previewHeightOffset = previewRenderer.bounds.extents.y;

        isPlacing = true;

        previewCollider.enabled = false;
    }

    public void Place()
    {
        UpdatePreviewPosition();

        if (canPlace)
        {
            playerCarry.Place(preview.transform.position, preview.transform.rotation);

            currentItem.VisualChanged -= UpdatePreviewVisual;
            Destroy(preview);

            preview = null;
            previewRenderer = null;
            currentItem = null;
            isPlacing = false;
            canPlace = false;
        }

        else
        {
            Debug.Log("Cannot place at here!");
        }
    }

    public void Cancel()
    {
        currentItem.VisualChanged -= UpdatePreviewVisual;
        Destroy(preview);

        preview = null;
        previewRenderer = null;
        currentItem = null;
        isPlacing = false;
        canPlace = false;
    }

    private void UpdatePreviewPosition()
    {
        if (currentItem == null)
        {
            Destroy(preview);
            return;
        }
        preview.transform.SetPositionAndRotation(currentItem.CurrentVisual.transform.position, currentItem.CurrentVisual.transform.rotation);

        Vector3 castCenter = previewRenderer.bounds.center + Vector3.up * maxStackHeight;
        Vector3 halfExtents = previewRenderer.bounds.extents;

        // 위쪽에서 아래로 훑고 내려와서 충돌한 위치에 배치한다
        Vector3 startPreviewPosition = preview.transform.position + Vector3.up * maxStackHeight;

        float castDistance = maxStackHeight + maxDropDepth;

        bool hasHit = Physics.BoxCast(castCenter, halfExtents, Vector3.down, out RaycastHit hit, Quaternion.identity, castDistance, placementSurfaceMask, QueryTriggerInteraction.Ignore);

        preview.SetActive(true);

        // Boxcast에 맞은 게 없거나 배치 예정 위치가 다른 오브젝트로 막혀있다면 배치 불가, 아니면 배치 가능
        if (!hasHit)
        {
            canPlace = false;
            // previewRenderer.material = cannotPlaceMaterial;
            SetMaterial(preview, cannotPlaceMaterial);
            return;
        }

        preview.transform.position = startPreviewPosition + Vector3.down * hit.distance;

        bool isBlocked = IsBlocked(hit.collider);
        canPlace = !isBlocked;

        if (canPlace)
        {
            SetMaterial(preview, canPlaceMaterial);
            // previewRenderer.material = canPlaceMaterial;
        }
        else
        {
            SetMaterial(preview, cannotPlaceMaterial);
            // previewRenderer.material = cannotPlaceMaterial;
        }
    }

    private void UpdatePreviewVisual()
    {
        if (preview != null)
        {
            preview.SetActive(false);
            Destroy(preview);
        }

        GameObject visualSource = currentItem.CurrentVisual;
        preview = Instantiate(visualSource);
        SetLayer(preview, LayerMask.NameToLayer("PlacementPreview"));

        previewRenderer = preview.GetComponentInChildren<Renderer>(true);
        previewCollider = preview.GetComponentInChildren<Collider>(true);
        previewCollider.enabled = false;

    }

    private bool IsBlocked(Collider collider)
    {
        Collider[] overlaps = Physics.OverlapBox(previewRenderer.bounds.center, previewRenderer.bounds.extents, Quaternion.identity, placementSurfaceMask, QueryTriggerInteraction.Ignore);

        foreach (Collider overlap in overlaps)
        {
            if (overlap == collider) continue;

            return true;
        }
        return false;
    }


    #region ItemInstance처럼 프리뷰의 모든 오브젝트를 프리뷰 전용 Carried 레이어로 옮기기
    private void SetLayer(GameObject item, int layer)
    {
        item.layer = layer;

        foreach (Transform child in item.transform)
        {
            SetLayer(child.gameObject, layer);
        }
    }

    private void SetMaterial(GameObject item, Material previewMaterial)
    {
        item.GetComponentInChildren<Renderer>().material = previewMaterial;

        foreach (Transform child in item.transform)
        {
            SetMaterial(child.gameObject, previewMaterial);
        }
    }

    #endregion

}
