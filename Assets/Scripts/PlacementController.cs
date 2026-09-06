using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class PlacementController : MonoBehaviour
{
    [SerializeField] private ItemInstance currentItem;
    public ItemInstance CurrentItem => currentItem;

    // 예상 배치도를 보여주는 프리뷰 구현용 필드
    [Tooltip("프리뷰를 보여줄 아이템")]
    private GameObject preview;
    public GameObject Preview => preview;
    [SerializeField] private Material previewMaterial;
    private Collider previewCollider;
    // 프리뷰 높이를 저장한 뒤, 바닥에서 높이/2만큼 올려 바닥에 붙은 것처럼 보이게 하기
    private float previewHeightOffset;

    // 이미 배치 중인데 다시 배치하려 하는지 확인
    private bool isPlacing;
    public bool IsPlacing => isPlacing;

    [SerializeField] private Camera placementCamera;
    [SerializeField] private LayerMask placementSurfaceMask;
    [Tooltip("플레이어가 실제로 물건을 놓을 수 있는 거리")]
    [SerializeField] private float maxPlacementDistance = 20f;
    [Tooltip("카메라가 표면을 찾는 거리")]
    [SerializeField] private float raycastDistance = 50f;

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

        int previewLayer = LayerMask.NameToLayer("PlacementPreview");
        preview.layer = previewLayer; // preview의 mesh가 여러개면 모두 해야함!!

        previewCollider = preview.GetComponent<Collider>();
        previewCollider.enabled = false;

        Renderer previewRenderer = preview.GetComponent<Renderer>();
        previewRenderer.material = previewMaterial;
        // bounds로 물체를 둘러싸는 박스의 크기를 계산, extents.y로 중심부터 박스까지의 거리(높이/2) 찾기

        // preview의 mesh가 여러 개일 경우
        // Renderer[] renderers = preview.GetComponentsInChildren<Renderer>();
        //
        // foreach (Renderer previewRenderer in renderers) previewRenderer.Material = previewMaterial;

        previewHeightOffset = previewRenderer.bounds.extents.y;



        isPlacing = true;
    }

    private void UpdatePreviewPosition()
    {
        Ray ray = placementCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);

        bool hasHit = Physics.Raycast(ray, out RaycastHit hit, raycastDistance, placementSurfaceMask, QueryTriggerInteraction.Ignore);

        if (!hasHit)
        {
            preview.SetActive(false);
            return;
        }

        preview.SetActive(true);
        preview.transform.position = hit.point;
        float distanceBetweenRayAndPlayer = Vector3.Distance(transform.position, hit.point);
        bool isInRange = distanceBetweenRayAndPlayer <= maxPlacementDistance;

        if (!isInRange)
        {
            preview.SetActive(false);
            return;
        }

        preview.SetActive(true);

        preview.transform.position = hit.point + Vector3.up * previewHeightOffset;
    }
}
