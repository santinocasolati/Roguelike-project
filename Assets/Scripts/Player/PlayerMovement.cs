using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1f;

    void Update()
    {
        Vector2 moveInputs = GameManager.instance.playerInputs.Player.Movement.ReadValue<Vector2>();

        Vector3 localMove = new Vector3(moveInputs.x, 0, moveInputs.y);
        Vector3 worldMove = transform.TransformDirection(localMove);

        if (worldMove != Vector3.zero)
        {
            transform.position += worldMove * moveSpeed * Time.deltaTime;
        }
    }
}
