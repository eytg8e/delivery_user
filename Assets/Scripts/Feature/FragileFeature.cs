using UnityEngine;
using System.Collections;
using System.Linq;

public enum IsGrounded
{
    supported,
    airborne
}

public class FragileFeature : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ItemInstance item;
    [SerializeField] private BoxCollider itemCollider;

    [SerializeField] private LayerMask rayCastLayerMask;

    private float maxHeight = 0f;
    private float endHeight = 0f;

    private float currentDistance = 0f;

    [SerializeField] private float packedThreshold = 10f;
    [SerializeField] private float openedThreshold = 6.3f;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Quaternion startRotation;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private bool startPacked;
    [SerializeField] private bool startActivated;

    [SerializeField] private ParticleSystem particle;
    [SerializeField] private float scatteringTime = 0f;
    [SerializeField] private float duration = 0.5f;

    private bool fallDown = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rayCastLayerMask = LayerMask.GetMask("PlacementSurface", "Stackable", "Obstacle");
        startPosition = transform.position;
        startRotation = transform.rotation;
        startScale = transform.localScale;

        startPacked = item.ItemState.IsPacked;
        startActivated = item.ItemState.IsActive;
    }

    // Update is called once per frame
    // void Update()
    // {

    // }

    void FixedUpdate()
    {
        // if (CheckIfFragile())
        // {
        //     itemCollider = item.CurrentVisual.GetComponent<BoxCollider>();
        //     // item이 회전해서 땅 밑을 뚫고 가도 괜찮게 0.05f로 넣는다.
        //     Vector3 rayOrigin = itemCollider.transform.TransformPoint(itemCollider.center);
        //     rayOrigin.y = Mathf.Max(0.02f, itemCollider.bounds.min.y + 0.02f);
        //     if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hitInfo, 40f, rayCastLayerMask, QueryTriggerInteraction.Collide))
        //     {
        //         if (hitInfo.distance > currentDistance)
        //         {
        //             currentDistance = hitInfo.distance;
        //             maxHeight = itemCollider.bounds.min.y + 0.02f;
        //         }

        //         if (hitInfo.distance < 0.05f)
        //         {
        //             endHeight = itemCollider.bounds.min.y + 0.02f;
        //             fallDown = true;
        //         }

        //         // Debug.Log($"CurrentDistance: {currentDistance}, maxHeight: {maxHeight}, endHeight = {endHeight}");
        //     }

        //     if (fallDown)
        //     {
        //         if (item.ItemState.IsPacked)
        //         {
        //             if (currentDistance >= packedThreshold) BreakDown();
        //         }

        //         else
        //         {
        //             if (currentDistance >= openedThreshold) BreakDown();
        //         }

        //         ClearState();
        //     }
        // }
    }

    void OnCollisionEnter(Collision collision)
    {
        ItemInstance otherItem = collision.collider.GetComponentInParent<ItemInstance>();
        if (item == gameManager.PlayerCarry.CurrentItem || item.gameObject.layer == LayerMask.NameToLayer("Scattering")) return;
        if (otherItem != null && otherItem.ItemData != null && otherItem.ItemData.Features.Contains(ItemFeature.Bounce)) return;

        Vector3 collisionNormal = collision.GetContact(0).normal;
        float collisionVelocity = Mathf.Abs(Vector3.Dot(collision.relativeVelocity, collisionNormal));

        if (item.ItemState.IsPacked && collisionVelocity >= packedThreshold) BreakDown();
        else if (!item.ItemState.IsPacked && collisionVelocity >= openedThreshold) BreakDown();
    }

    private bool CheckIfFragile()
    {
        bool isStackable = gameObject.layer == LayerMask.NameToLayer("Stackable");
        bool isNotKinematic = !gameObject.GetComponent<Rigidbody>().isKinematic;
        bool isNotInHand = gameObject.GetComponent<ItemInstance>() != gameManager.PlayerCarry.CurrentItem;

        if (isStackable && isNotKinematic && isNotInHand) return true;
        else return false;
    }

    private void ClearState()
    {
        fallDown = false;
        currentDistance = 0f;
        maxHeight = 0f;
        endHeight = 0f;
    }

    private void BreakDown()
    {
        // 부서지는 연출 후 씬 생성 당시의 첫 위치로 이동
        if (item.gameObject.layer != LayerMask.NameToLayer("Scattering"))
        {
            particle.Play();
            StartCoroutine("Disappear");
        }
    }

    IEnumerator Disappear()
    {
        item.gameObject.layer = LayerMask.NameToLayer("Scattering");
        item.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        while (scatteringTime <= duration)
        {
            scatteringTime += Time.deltaTime;
            transform.localScale = startScale * Mathf.Lerp(1f, 0f, scatteringTime / duration);
            yield return null;
        }
        transform.position = startPosition;
        transform.rotation = startRotation;
        transform.localScale = startScale;

        item.ItemState.IsPacked = startPacked;
        item.ItemState.IsActive = startActivated;

        item.gameObject.layer = LayerMask.NameToLayer("Stackable");
        item.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        scatteringTime = 0f;
    }
}
