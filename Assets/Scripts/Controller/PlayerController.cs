using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Player Component bindings")]
    [SerializeField] private CharacterController characterController;
    public CharacterController CharacterController => characterController;
    [SerializeField] private PlayerCarry playerCarry;
    [SerializeField] private PlacementController placementController;
    [SerializeField] private PlayerInteraction playerInteraction;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference rotateAction;


    [Header("Player stats")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 90.0f;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float gravityScale = -9.8f;
    [SerializeField] private Vector3 playerVelocity;
    [Tooltip("Glide 아이템 없을 때 바람에 밀려나는 정도")]
    [SerializeField, Range(0f, 1f)] private float normalWindMultiplier = 0.1f;
    [Tooltip("바람 등에 의해 받은 외력")]
    [SerializeField] private Vector3 externalMovement = Vector3.zero;

    public bool isInWind = false;

    public float jumpBuffer = 0.12f;
    public float jumpBufferCounter = 0f;
    public float coyoteTime = 0.12f;
    public float coyoteTimeCounter = 0f;

    private Vector3 finalMove;
    [SerializeField] private int itemCollisionTestMask;
    private float collisionOffset = 0.1f;

    private BoxCollider itemCollisionTest;
    private Vector3 itemWorldCenter;
    private Vector3 scale;
    private Vector3 halfExtents;
    private Quaternion itemRotation;

    private Vector3 moveDelta;
    private Vector3 horizontalDelta;

    private float distance;
    private Vector3 direction;

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        rotateAction.action.performed += RotateItem;
        rotateAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        rotateAction.action.performed -= RotateItem;
        rotateAction.action.Disable();
    }

    private void Start()
    {
        itemCollisionTestMask = LayerMask.GetMask("PlacementSurface", "Obstacle");
    }

    void Update()
    {
        // 이동하기
        Move();

        // 키입력으로 아이템 집기, 포장하기, 활성화하기
        if (Keyboard.current.leftCtrlKey.wasPressedThisFrame || Keyboard.current.rightCtrlKey.wasPressedThisFrame)
        {
            PickupOnOff();
        }
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame || Keyboard.current.rightShiftKey.wasPressedThisFrame)
        {
            PackUnpack();
        }

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ActiveOnOff();
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void Move()
    {
        if (characterController.isGrounded && playerVelocity.y < 0) playerVelocity.y = -2f;

        // 입력 받기
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);

        // 회전하기
        if (move != Vector3.zero)
        {
            move = Vector3.ClampMagnitude(move, 1f);
            IsRotateBlocked(move);
        }

        // 점프하기

        // 점프 버퍼
        if (jumpAction.action.WasPressedThisFrame())
        {
            jumpBufferCounter = jumpBuffer;
        }

        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // 코요테 타임
        if (characterController.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }

        else
        {
            coyoteTimeCounter -= Time.deltaTime;

        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            float carryingJumpHeight = jumpHeight;
            if (playerCarry.CurrentItem != null)
            {
                float mass = playerCarry.CurrentItem.ItemData.Weight / 5f;
                carryingJumpHeight = jumpHeight - mass;
                if (carryingJumpHeight < 0f) carryingJumpHeight = 0f;
            }

            playerVelocity.y = Mathf.Sqrt(carryingJumpHeight * -2.0f * gravityScale);

            // 점프 버퍼, 코요테 타임 초기화
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        // 중력 적용
        playerVelocity.y += gravityScale * Time.deltaTime;

        // 수평, 수직 이동 합치기
        finalMove = (move * moveSpeed) + (playerVelocity.y * Vector3.up) + externalMovement;

        if (move != Vector3.zero && IsStuck())
        {
            finalMove.x = 0f;
            finalMove.z = 0f;
        }

        characterController.Move(finalMove * Time.deltaTime);

        externalMovement = Vector3.zero;
    }

    private bool IsStuck()
    {
        if (playerCarry.CurrentItem == null) return false;

        setItemCollisionTest();
        setItemMoveTest();

        if (direction == Vector3.zero || distance < 0.0001f) return false;

        bool isBlocked = Physics.BoxCast(itemWorldCenter, halfExtents, direction, out RaycastHit hit, itemRotation, distance + collisionOffset, itemCollisionTestMask, QueryTriggerInteraction.Collide);

        return isBlocked;
    }

    private bool IsRotateBlocked(Vector3 move)
    {
        Quaternion targetRotation = Quaternion.LookRotation(move);
        Quaternion nextRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        bool rotationBlocked = false;

        if (playerCarry.CurrentItem == null) rotationBlocked = false;

        else
        {
            setItemCollisionTest();

            // 현재 위치에서 다음 회전방향까지 가기 위해 필요한 회전값
            Quaternion rotationDelta = nextRotation * Quaternion.Inverse(transform.rotation);

            Vector3 centerOffset = itemWorldCenter - transform.position;
            Vector3 nextCenterOffset = rotationDelta * centerOffset;

            Vector3 nextWorldCenter = transform.position + nextCenterOffset;

            Quaternion nextItemRotation = rotationDelta * itemRotation;

            rotationBlocked = Physics.CheckBox(nextWorldCenter, halfExtents, nextItemRotation, itemCollisionTestMask, QueryTriggerInteraction.Collide);
        }

        if (!rotationBlocked) transform.rotation = nextRotation;

        return rotationBlocked;
    }

    private void setItemCollisionTest()
    {
        if (playerCarry.CurrentItem == null) return;

        itemCollisionTest = playerCarry.CurrentItem.ItemCollider as BoxCollider;
        itemWorldCenter = itemCollisionTest.transform.TransformPoint(itemCollisionTest.center);
        scale = itemCollisionTest.transform.lossyScale;
        halfExtents = Vector3.Scale(itemCollisionTest.size * 0.5f, new Vector3(scale.x, scale.y, scale.z));
        itemRotation = itemCollisionTest.transform.rotation;
    }

    private void setItemMoveTest()
    {
        moveDelta = finalMove * Time.deltaTime;
        horizontalDelta = new Vector3(moveDelta.x, 0f, moveDelta.z);
        distance = horizontalDelta.magnitude;
        direction = horizontalDelta.normalized;
    }

    public void RotateItem(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (playerCarry.CurrentItem != null)
        {
            playerCarry.CurrentItem.transform.Rotate(input.y * 90f, input.x * 90f, 0f);
        }

        else if (playerInteraction.CurrentTarget != null)
        {
            playerInteraction.CurrentTarget.transform.Rotate(input.y * 90f, input.x * 90f, 0f);
        }
    }

    public void PickupOnOff()
    {
        if (playerCarry.CurrentItem != null) placementController.Place();
        {
            if (playerInteraction.CurrentTarget != null)
            {
                playerInteraction.PickupTarget();
            }
            else Debug.Log("There's nothing to pickup");
        }
    }

    private void PackUnpack()
    {
        if (playerCarry.CurrentItem == null && playerInteraction.CurrentTarget == null) Debug.Log("There's nothing to pack/unpack");
        playerInteraction.PackTarget();
    }

    private void ActiveOnOff()
    {
        // if (testItem.IsCarried) playerCarry.BeginPlacement();
        if (playerCarry.CurrentItem == null && playerInteraction.CurrentTarget == null) Debug.Log("There's nothing to on/off");
        playerInteraction.ActivateTarget();
    }

    public void AddExternalMovement(Vector3 movement)
    {
        bool hasActiveGlider = playerCarry.CurrentItem != null && playerCarry.CurrentItem.HasActiveFeature(ItemFeature.Glide);
        bool isFloating = isInWind && hasActiveGlider;

        // 글라이딩 아이템이 있으면 빠르게 밀린다
        if (hasActiveGlider) externalMovement += movement;

        //글라이딩 아이템이 없으면 느리게 밀린다
        else externalMovement += movement * normalWindMultiplier;

        if (isFloating) playerVelocity.y = 0f;
        // else playerVelocity.y += gravityScale * Time.deltaTime;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        ItemInstance item = hit.collider.gameObject.GetComponentInParent<ItemInstance>();
        if (item != null && item.ItemData.Features.Contains(ItemFeature.Bounce))
        {
            float maxBounceSpeed = item.gameObject.GetComponentInChildren<BounceFeature>().MaxBounceSpeed;

            Vector3 wallNorm = hit.normal;
            wallNorm = wallNorm.normalized;

            float reflectForce = Vector3.Dot(playerVelocity, wallNorm);
            if (reflectForce > 0f) return; //이미 빠져나오는 중

            float bounceRatio = item.gameObject.GetComponentInChildren<BounceFeature>().BounceRatio;

            Vector3 reflectedVelocity = playerVelocity - (1f + bounceRatio) * reflectForce * wallNorm;

            playerVelocity = Vector3.ClampMagnitude(reflectedVelocity, maxBounceSpeed);
        }
    }
}
