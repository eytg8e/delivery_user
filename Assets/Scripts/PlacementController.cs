using UnityEngine;

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

    // 이미 배치 중인데 다시 배치하려 하는지 확인
    private bool isPlacing;
    public bool IsPlacing => isPlacing;

    [SerializeField] private Camera placementCamera;
    [SerializeField] private LayerMask placementSurfaceMask;
    [SerializeField] private float maxPlacementDistance = 5f;
    
    public void BeginPlacement(ItemInstance item)
    {
        if(isPlacing || item == null) return;

        GameObject visualSource = currentItem.CurrentVisual;
        currentItem = item;
        preview = Instantiate(visualSource);

        int previewLayer = LayerMask.NameToLayer("PlacementPreview");
        preview.layer = previewLayer; // preview의 mesh가 여러개면 모두 해야함!!

        previewCollider = preview.GetComponent<Collider>();
        previewCollider.enabled = false;

        Renderer previewRenderer = preview.GetComponent<Renderer>();
        previewRenderer.material = previewMaterial;

        // preview의 mesh가 여러 개일 경우
        // Renderer[] renderers = preview.GetComponentsInChildren<Renderer>();
        //
        // foreach (Renderer previewRenderer in renderers) previewRenderer.Material = previewMaterial;

        isPlacing = true;
    }
}
