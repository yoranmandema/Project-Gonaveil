using UnityEngine;

public static class InputManager
{
    private static bool axisLock, buttonLock;

    public static void LockAxis()
    {
        axisLock = true;
    }

    public static void UnlockAxis()
    {
        axisLock = false;
    }

    public static void LockKeys()
    {
        buttonLock = true;
    }

    public static void UnlockKeys()
    {
        buttonLock = false;
    }

    public static bool GetButton(string buttonName)
    {
        return !buttonLock && Input.GetButton(buttonName);
    }

    public static bool GetButtonDown(string buttonName)
    {
        return !buttonLock && Input.GetButtonDown(buttonName);
    }

    public static bool GetButtonUp(string buttonName)
    {
        return !buttonLock && Input.GetButtonUp(buttonName);
    }

    public static float GetAxis(string axisName)
    {
        if (axisLock) return 0;
        return Input.GetAxis(axisName);
    }

    public static float GetAxisRaw(string axisName)
    {
        if (axisLock) return 0;
        return Input.GetAxisRaw(axisName);
    }
}
