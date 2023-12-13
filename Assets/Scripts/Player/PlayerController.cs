using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [SerializeField] private Transform aimTarget;
    public Transform barrelTransform;
    public Transform bulletParent;

    public Animator animator;

    [SerializeField] private float aimDistance = 10f;
    [SerializeField] private float animationSmoothTime = 0.1f;
    [SerializeField] private float animationPlayTransition = 0.15f;
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 10f;

    [SerializeField] private CinemachineImpulseSource impulseSource;
    public float shakeDuration;
    public Vector3 shakeImpulse;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    public PlayerInput playerInput;
    public Transform cameraTransform;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction runAction;

    private int moveXParamId;
    private int moveZParamId;
    private int moveMultiplier;
    private int jumpAnimation;

    private Vector2 currentAnimationBlend;
    private Vector2 animationVelocity;

    private float running = 1;

    private void Awake()
    {
        instance = this;

        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        runAction = playerInput.actions["Run"];

        cameraTransform = Camera.main.transform;

        moveXParamId = Animator.StringToHash("MoveX");
        moveZParamId = Animator.StringToHash("MoveZ");
        moveMultiplier = Animator.StringToHash("MoveMultiplier");
        jumpAnimation = Animator.StringToHash("Jump");
    }

    public void GenerateShake(Vector3 impulse, float stopTime)
    {
        impulseSource.GenerateImpulse(impulse);
        Invoke(nameof(StopShake), stopTime);
    }

    private void StopShake()
    {
        impulseSource.GenerateImpulse(Vector3.zero);
    }

    void Update()
    {
        aimTarget.position = cameraTransform.position + cameraTransform.forward * aimDistance;

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        if (runAction.IsPressed() && groundedPlayer)
        {
            running = 1.5f;
        } else
        {
            running = 1f;
        }
        animator.SetFloat(moveMultiplier, running);

        Vector2 input = moveAction.ReadValue<Vector2>();

        currentAnimationBlend = Vector2.SmoothDamp(currentAnimationBlend, input, ref animationVelocity, animationSmoothTime);

        Vector3 move = new Vector3(currentAnimationBlend.x, 0, currentAnimationBlend.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;
        controller.Move(move * running * Time.deltaTime * playerSpeed);

        animator.SetFloat(moveXParamId, currentAnimationBlend.x);
        animator.SetFloat(moveZParamId, currentAnimationBlend.y);

        if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            animator.CrossFade(jumpAnimation, animationPlayTransition);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}