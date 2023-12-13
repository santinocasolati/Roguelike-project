using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseGun : MonoBehaviour
{
    public GameObject bulletPrefab;

    private Transform barrelTransform;
    private Transform bulletParent;

    public float shootDelay;
    public float switchDelay = 0.25f;
    public float bulletHitMissDistance = 25f;
    public float shakeDuration = 0.1f;

    private bool canShoot = false;
    private bool isShooting = false;

    private PlayerInput playerInput;

    private InputAction shootAction;

    private int recoilAnimation;

    private float currentDelay;

    private void Start()
    {
        barrelTransform = PlayerController.instance.barrelTransform;
        bulletParent = PlayerController.instance.bulletParent;

        playerInput = PlayerController.instance.playerInput;
        shootAction = playerInput.actions["Shoot"];
        recoilAnimation = Animator.StringToHash("PistolShootRecoil");

        shootAction.performed += _ => Shoot(true);
        shootAction.canceled += _ => Shoot(false);

        Invoke(nameof(HandleWeaponSwitch), switchDelay);
    }

    private void OnDisable()
    {
        shootAction.performed -= _ => Shoot(true);
        shootAction.canceled -= _ => Shoot(false);
    }

    private void HandleWeaponSwitch()
    {
        canShoot = true;
    }

    public virtual void Shoot(bool state) { }

    public void Shooting(bool state)
    {
        isShooting = state;

        if (!state)
        {
            currentDelay = shootDelay;
        }
    }

    public void ShootGun()
    {
        if (!canShoot) return;

        RaycastHit hit;

        GameObject bullet = GameObject.Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        BulletController bulletController = bullet.GetComponent<BulletController>();

        if (Physics.Raycast(PlayerController.instance.cameraTransform.position, PlayerController.instance.cameraTransform.forward, out hit, Mathf.Infinity, 0))
        {
            bulletController.target = hit.point;
            bulletController.hit = true;
        }
        else
        {
            bulletController.target = PlayerController.instance.cameraTransform.position + PlayerController.instance.cameraTransform.forward * bulletHitMissDistance;
            bulletController.hit = false;
        }

        PlayerController.instance.animator.CrossFade(recoilAnimation, 0.15f);
        PlayerController.instance.GenerateShake(new Vector3(0, -0.01f, 0.1f), shakeDuration);
    }

    public void ShootUpdate()
    {
        if (isShooting)
        {
            if (currentDelay > shootDelay)
            {
                currentDelay = 0;
                ShootGun();
            }
        }

        currentDelay += Time.deltaTime;
    }
}
