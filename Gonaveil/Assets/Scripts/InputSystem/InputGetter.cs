using UnityEngine;
using System.Collections;

public abstract class InputGetter : MonoBehaviour
{
    bool initialised;

    public void Init()
    {

        initialised = true;
    }

    public void UpdateKeyStates(int key)
    {

    }

    protected bool GetButton(int key)
    {
        return false;
    }

    protected bool GetButtonDown(int key)
    {
        return false;
    }

    protected bool GetButtonUp(int key)
    {
        return false;
    }

    protected float GetAxis(int axis)
    {
        return 0;
    }

    protected float GetAxisRaw(int axis)
    {
        return 0;
    }

}
