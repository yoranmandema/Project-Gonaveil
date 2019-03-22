using UnityEngine;
using System.Collections;
using KeyInput;

public class TestScript : InputGetter
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetButtonDown((int)Key.Jump);
    }
}
