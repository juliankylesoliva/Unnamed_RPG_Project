using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharCtrl : MonoBehaviour
{
    /* https://www.youtube.com/watch?v=bH33Qvhvl40 */

    public bool enableMovement = true;
    public float moveSpeed = 10.0f;
    public float rotateSpeed = 10.0f;
    
    void Update()
    {
        if(enableMovement)
        {
            DoRotate();
            DoMove();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void DoRotate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        transform.Rotate(0, Input.GetAxis("Mouse X") * Time.deltaTime * rotateSpeed, 0);
    }

    private void DoMove()
    {
        if(Input.GetKey(KeyCode.W))
        {
            float speed = moveSpeed;

            if(Input.GetKey(KeyCode.LeftShift))
            {
                speed *= 2.0f;
            }

            Vector3 newPos = transform.position + (transform.rotation * Vector3.forward * Time.deltaTime * speed);
            NavMeshHit hit;

            bool isValidMove = NavMesh.SamplePosition(newPos, out hit, 1.0f, NavMesh.AllAreas);

            if(isValidMove)
            {
                transform.position = hit.position;
            }
        }
    }
}
