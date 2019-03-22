using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KeyInput;

public class InputController : MonoBehaviour
{
    public List<InputGetter> inputList = new List<InputGetter>();

    string[] keyList;
    string[] axisList;
    string[] axisKeyList;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);


        int error;
        LoadKeyConfig(out keyList, out axisList, out axisKeyList, out error);
        if(error != 0)
        {
            LoadDefault(out keyList, out axisList, out axisKeyList);
            SaveKeyConfig(keyList, axisList, axisKeyList, out error);
        }
    }

    void LoadDefault(out string[] key, out string[] axis, out string[] axisKey)
    {
        //string[] list = new string[System.Enum.GetNames(typeof(Key)).Length];
        //list[Key.];

        key = new string[0];
        axis = new string[0];
        axisKey = new string[0];
    }

    void LoadKeyConfig(out string[] key, out string[] axis, out string[] axisKey, out int error)
    {
        key = new string[0];
        axis = new string[0];
        axisKey = new string[0];
        error = 0;
    }

    void SaveKeyConfig(string[] key, string[] axis, string[] axisKey, out int error)
    {
        error = 0;
    }

    void UpdateKeyStates()
    {

    }

    void SendKeyStates()
    {

    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateKeyStates();
        SendKeyStates();
    }
}
