using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{

    private InputSystemControl inputControl;
    private void Start()
    {
        lockMouse = true;
        inputControl = new InputSystemControl();
        inputControl.Enable();
        inputControl.Player.Enable();
    }

    private void Update()
    {
        if (Input.mousePresent)
        {
            MouseControlUpdate();
        }
    }

    private bool lockMouse;
    private void MouseControlUpdate()
    {

        lockMouse = !inputControl.Player.KeyAlt.inProgress;

        Cursor.lockState = lockMouse ? CursorLockMode.Locked : CursorLockMode.None;
        

        if (lockMouse)
        {

            var d = Input.mousePositionDelta;
            var current = transform.rotation.eulerAngles;

            current += new Vector3(-d.y, d.x) * 0.1f;
            transform.rotation=Quaternion.Euler(current);


            if (inputControl.Player.KeyW.inProgress)
            {
                var hand = transform.forward;
                transform.Translate(hand.normalized * (Time.deltaTime * 10f), Space.World);
            }
            else if(inputControl.Player.KeyS.inProgress)
            {
                var back = transform.rotation * Vector3.back;
                transform.Translate(back.normalized * (Time.deltaTime * 10f), Space.World);
            }
            
        }
        
        
    }

    private void OnDrawGizmos()
    {
        var t = transform;
        var p = t.position;
        Gizmos.DrawLine(p, p + t.forward);
    }
}
