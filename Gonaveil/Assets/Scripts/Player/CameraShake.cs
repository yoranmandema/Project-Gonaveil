using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    public float debugMagnitude = 1f;

    public void Shake(float magnitude) {
        StartCoroutine(DoShake(debugMagnitude));
    }

    private IEnumerator DoShake(float magnitude) {
        Vector3 originalPos = transform.localPosition;
        Vector3 originalRot = transform.localEulerAngles;
        float elapsedTime = 0.0f;
        float duration = Mathf.Clamp(Mathf.Pow(1.1f, magnitude) -0.5f, 0.2F, 2.5f);
        print(duration);

        while (elapsedTime < duration) {
            float x = originalPos.x + Random.Range(-1f, 1f) * magnitude;
            float y = originalPos.y + Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(x, y, originalPos.z);

            var rotationOffset = new Vector3(Random.Range(-10f, 10f) * magnitude, Random.Range(-10f, 10f) * magnitude, Random.Range(-10f, 10f) * magnitude);
            transform.localEulerAngles += rotationOffset;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
        transform.localEulerAngles = originalRot;
    }

    // Temporary debugging related shit below
    private void Update() {
        if (Input.GetKeyDown(KeyCode.X)) {
            Shake(debugMagnitude);
        }
    }
}
