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

        if (collider.gameObject.GetComponent<Rigidbody>() == null) return;

        Rigidbody rigidbody = collider.gameObject.GetComponent<Rigidbody>();

        if (rigidbody.isKinematic || collider.gameObject.GetComponent<ItemInstance>() == playerCarry.CurrentItem) return;

        Vector3 velocity = rigidbody.linearVelocity;
        velocity.y = -velocity.y * bounceRatio;
        if (velocity.y < -maxBounceSpeed) velocity.y = -maxBounceSpeed;
        else if (velocity.y > maxBounceSpeed) velocity.y = maxBounceSpeed;
        rigidbody.linearVelocity = velocity;
    }
}
