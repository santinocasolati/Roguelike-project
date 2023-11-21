using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotation : MonoBehaviour
{
    public Transform followTarget;
    public Vector2 sensitivity;

    private void Update()
    {
        Vector2 mouseDelta = GameManager.instance.playerInputs.Player.MouseLook.ReadValue<Vector2>();

        //Rotation on X angle
        Vector3 xRot = transform.rotation.eulerAngles;
        xRot.y += mouseDelta.x * sensitivity.x;
        transform.rotation = Quaternion.Euler(xRot);

        //Rotation on Y angle
        Vector3 yRot = followTarget.rotation.eulerAngles;
        yRot.x -= mouseDelta.y * sensitivity.y;

        //Clamp the Y rotation
        if (mouseDelta.y < 0)
        {
            if (yRot.x > 45 && yRot.x < 344)
            {
                yRot.x = 45;
            }
        } else if (mouseDelta.y > 0)
        {
            if (yRot.x < 344 && yRot.x > 45)
            {
                yRot.x = 344;
            }
        }

        followTarget.rotation = Quaternion.Euler(yRot);
    }
}
