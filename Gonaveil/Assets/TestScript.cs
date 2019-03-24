using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestScript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        EventManager.StartListening("Charging", TestListen);
    }
    public float TestVal;

    void TestListen(float eventArg)
    {
        TestVal = eventArg;
    }
}
