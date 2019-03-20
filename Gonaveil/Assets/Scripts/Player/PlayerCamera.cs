using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    public float sensitivity = 1f;
    public float velocityRollSmoothing = 0.5f;
    public float velocityRollMultiplier = 0.25f;
    public float maxVelocityRoll = 4f;
    public PlayerMovement playerMovement;

    float mouseY;
    float roll;

    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        transform.parent.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
        mouseY -= Input.GetAxis("Mouse Y");
        mouseY = Mathf.Clamp(mouseY, -90, 90);

        var desiredRoll = transform.InverseTransformDirection(playerMovement.velocity).x * -velocityRollMultiplier;

        if (velocityRollSmoothing != 0) {
            roll += (desiredRoll - roll) * Time.deltaTime * (1 / velocityRollSmoothing);
        } else {
            roll = desiredRoll;
        }

        transform.localEulerAngles = new Vector3(mouseY, 0, roll);
    }
}
