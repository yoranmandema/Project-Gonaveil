using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    public float sensitivity = 1f;

    float mouseY;

    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        transform.parent.Rotate(new Vector3(0,Input.GetAxis("Mouse X"),0));
        mouseY -= Input.GetAxis("Mouse Y");
        mouseY = Mathf.Clamp(mouseY, -90, 90);
        transform.localEulerAngles = new Vector3(mouseY, 0, 0);
    }
}
