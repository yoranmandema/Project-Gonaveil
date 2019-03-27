using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScreenDebug : MonoBehaviour
{
    class PrintMessage {
        public string message;
        public float lifeTime;
        public Color color;

        public float startTime;
    }

    public static OnScreenDebug Instance { get; private set; }

    private static List<PrintMessage> messages = new List<PrintMessage>();
    private static GUIStyle style;

    private static void Initialise () {
        style = new GUIStyle {
            fontSize = 16
        };
    }

    private void Awake () {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogError("Too many OnScreenDebug instances in the scene.");
        }

        Initialise();
    }

    public static void Print(string message) {
        Print(message, Color.white, Time.deltaTime);
    }

    public static void Print (string message, Color color) {
        Print(message, color, Time.deltaTime);
    }

    public static void Print(string message, float lifeTime) {
        Print(message, Color.white, lifeTime);
    }

    public static void Print (string message, Color color, float lifeTime) {
        if (Instance == null) {
            var find = FindObjectOfType<OnScreenDebug>();

            if (find == null) {
                Debug.LogError("Please place an OnScreenDebug instance in the scene.");
                return;
            }
            else {
                Instance = find;

                Initialise();
            }
        }

        messages.Add(new PrintMessage {
            message = message,
            lifeTime = lifeTime,
            color = color,
            startTime = Time.realtimeSinceStartup
            });
    }

    void OnGUI () {
        for (var i = messages.Count - 1; i > 0; i--) {
            var message = messages[i];
            var rect = new Rect(10, 10 + (i - 0) * 20, 100, 20);
            var oldColor = style.normal.textColor;

            style.normal.textColor = message.color;

            GUI.Label(rect, message.message, style);

            style.normal.textColor = oldColor;

            if (Time.realtimeSinceStartup - message.startTime >= message.lifeTime) messages.RemoveAt(i);
        }
    }
}
