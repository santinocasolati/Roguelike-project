using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    public Transform container;
    public Transform leftHandGrip;
    public Transform rightHandGrip;
    public Transform barrel;
    public Weapon[] weaponPrefabs;

    public float switchTime = 0.25f;

    private Weapon currentWeapon;
    private GameObject currentModel;

    private PlayerInput playerInput;

    private InputAction weapon1;
    private InputAction weapon2;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        weapon1 = playerInput.actions["Weapon1"];
        weapon2 = playerInput.actions["Weapon2"];

        weapon1.performed += _ => SwitchWeapon(0);
        weapon2.performed += _ => SwitchWeapon(1);

        SwitchWeapon(0);
    }

    void SwitchWeapon(int weaponIndex)
    {
        if (currentWeapon != null && currentWeapon == weaponPrefabs[weaponIndex]) return;

        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        currentWeapon = weaponPrefabs[weaponIndex];

        currentModel = Instantiate(currentWeapon.model, container);
        currentModel.transform.GetChild(0).gameObject.SetActive(false);
        WeaponData weaponData = currentModel.GetComponent<WeaponData>();

        StartCoroutine(SetGripPositions(leftHandGrip, rightHandGrip, barrel, weaponData));
    }

    IEnumerator SetGripPositions(Transform leftGrip, Transform rightGrip, Transform newBarrel, WeaponData weaponData)
    {
        float elapsedTime = 0f;

        newBarrel.localPosition = weaponData.barrelPosition;

        while (elapsedTime < switchTime)
        {
            leftGrip.localPosition = Vector3.Lerp(leftGrip.localPosition, weaponData.leftGripPosition, elapsedTime / switchTime);
            rightGrip.localPosition = Vector3.Lerp(rightGrip.localPosition, weaponData.rightGripPosition, elapsedTime / switchTime);

            leftGrip.localRotation = Quaternion.Lerp(leftGrip.localRotation, Quaternion.Euler(weaponData.leftGripRotation), elapsedTime / switchTime);
            rightGrip.localRotation = Quaternion.Lerp(rightGrip.localRotation, Quaternion.Euler(weaponData.rightGripRotation), elapsedTime / switchTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentModel.transform.GetChild(0).gameObject.SetActive(true);
    }
}

[System.Serializable]
public class Weapon
{
    public GameObject model;
}