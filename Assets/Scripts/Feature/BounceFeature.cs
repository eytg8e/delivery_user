using UnityEngine;

public class BounceFeature : MonoBehaviour
{
    [SerializeField] private ItemInstance item;
    [SerializeField] private float bounceRatio = 1.2f;
    public float BounceRatio => bounceRatio;
    [SerializeField] private float maxBounceSpeed = 10f;
    public float MaxBounceSpeed => maxBounceSpeed;

    [SerializeField] private PlayerCarry playerCarry;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame

    void OnCollisionEnter(Collision collider)
    {
        if (item.ItemState.IsPacked) return;

        Rigidbody rigidbody = collider.rigidbody;

        ItemInstance otherItem = collider.collider.GetComponentInParent<ItemInstance>();

        if (rigidbody == null || rigidbody.isKinematic) return;

        if (otherItem != null && otherItem == playerCarry.CurrentItem) return;

        Vector3 incomingVelocity = collider.relativeVelocity;
        Vector3 itemNorm = -collider.contacts[0].normal; // 반대일수도?

        float reflectForce = Vector3.Dot(incomingVelocity, itemNorm);

        if (reflectForce > 0f) return;

        Vector3 reflectedVelocity = incomingVelocity - (1f + bounceRatio) * reflectForce * itemNorm;
        rigidbody.linearVelocity = Vector3.ClampMagnitude(reflectedVelocity, maxBounceSpeed);
    }
}
