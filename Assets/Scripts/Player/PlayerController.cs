using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private Transform barrelTransform;
    [SerializeField] private Transform bulletParent;
    [SerializeField] private Transform aimTarget;

    [SerializeField] private Animator animator;

    [SerializeField] private float bulletHitMissDistance = 25f;
    [SerializeField] private float animationSmoothTime = 0.1f;
    [SerializeField] private float animationPlayTransition = 0.15f;
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float aimDistance = 10f;

    [SerializeField] private CinemachineImpulseSource impulseSource;
    public float shakeDuration;
    public Vector3 shakeImpulse;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private PlayerInput playerInput;
    private Transform cameraTransform;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;
    private InputAction runAction;

    private int moveXParamId;
    private int moveZParamId;
    private int moveMultiplier;
    private int jumpAnimation;
    private int recoilAnimation;

    private Vector2 currentAnimationBlend;
    private Vector2 animationVelocity;

    private float running = 1;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];
        runAction = playerInput.actions["Run"];

        cameraTransform = Camera.main.transform;

        moveXParamId = Animator.StringToHash("MoveX");
        moveZParamId = Animator.StringToHash("MoveZ");
        moveMultiplier = Animator.StringToHash("MoveMultiplier");
        jumpAnimation = Animator.StringToHash("Jump");
        recoilAnimation = Animator.StringToHash("PistolShootRecoil");
    }

    private void OnEnable()
    {
        shootAction.performed += _ => ShootGun();
    }

    private void OnDisable()
    {
        shootAction.performed -= _ => ShootGun();
    }

    private void ShootGun()
    {
        RaycastHit hit;

        GameObject bullet = GameObject.Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        BulletController bulletController = bullet.GetComponent<BulletController>();

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, 0))
        {
            bulletController.target = hit.point;
            bulletController.hit = true;
        } else
        {
            bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance;
            bulletController.hit = false;
        }

        animator.CrossFade(recoilAnimation, animationPlayTransition);

        impulseSource.GenerateImpulse(shakeImpulse);
        Invoke(nameof(StopShake), shakeDuration);
    }

    private void StopShake()
    {
        impulseSource.GenerateImpulse(Vector2.zero);
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