using UnityEngine;

public class BlowFeature : MonoBehaviour
{
    [SerializeField] private float windForce = 20f;
    [SerializeField] private int blowableLayer;
    [SerializeField] private int playerLayer;
    [SerializeField] private float playerPushSpeed = 300f;

    [SerializeField] private PlayerController playerController;


    private Rigidbody ownerRigidbody;

    private void Awake()
    {
        ownerRigidbody = GetComponentInParent<Rigidbody>();
        blowableLayer = LayerMask.NameToLayer("Stackable");
        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnDisable()
    {
        playerController.isInWind = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        bool isPlayer = collider.gameObject.layer == playerLayer;
        if (isPlayer) playerController.isInWind = true;
    }

    private void OnTriggerStay(Collider collider)
    {
        bool isPlayer = collider.gameObject.layer == playerLayer;
        if (isPlayer)
        {
            // OnTrigger류는 프레임마다 재생되는 것이 아니라서 Time.deltaTime을 사용하지 않는다
            Vector3 windMovement = transform.right * playerPushSpeed * Time.fixedDeltaTime;
            playerController.AddExternalMovement(windMovement);
        }

        bool isBlowable = collider.gameObject.layer == blowableLayer;
        if (!isBlowable) return;

        Rigidbody targetRigidbody = collider.attachedRigidbody;

        if (targetRigidbody == null) return;
        if (targetRigidbody == ownerRigidbody) return;

        targetRigidbody.AddForce(gameObject.transform.right * windForce, ForceMode.Force);
    }

    private void OnTriggerExit(Collider collider)
    {
        bool isPlayer = collider.gameObject.layer == playerLayer;
        if (isPlayer) playerController.isInWind = false;
    }


    void Update()
    {

    }
}
