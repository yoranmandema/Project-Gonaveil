using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    public float sensitivity = 1f;

    void Update()
    {
        transform.parent.Rotate(new Vector3(0,Input.GetAxis("Mouse X"),0));
        transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0, 0), Space.Self);
    }
}
