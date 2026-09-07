using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.WSA;

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

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
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
    }

    private void Move()
    {
        if (characterController.isGrounded && playerVelocity.y < 0) playerVelocity.y = 0f;

        // 입력 받기
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = Vector3.ClampMagnitude(move, 1f);

        if (move != Vector3.zero) transform.forward = move;

        // 점프하기
        if (jumpAction.action.WasPressedThisFrame() && characterController.isGrounded)
        {
            if (playerCarry.CurrentItem == null) playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityScale);
            else Debug.Log("You can't jump while carrying item!");
        }

        // 중력 적용
        playerVelocity.y += gravityScale * Time.deltaTime;

        // 수평, 수직 이동 합치기
        Vector3 finalMove = (move * moveSpeed) + (playerVelocity.y * Vector3.up) + externalMovement;
        characterController.Move(finalMove * Time.deltaTime);

        externalMovement = Vector3.zero;
    }

    private void PickupOnOff()
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

        if (isFloating) playerVelocity.y = playerVelocity.y * 0.05f;
        else playerVelocity.y += gravityScale * Time.deltaTime;
    }
}
