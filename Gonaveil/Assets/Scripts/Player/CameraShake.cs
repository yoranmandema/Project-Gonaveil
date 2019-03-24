using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
	public float rotationMultiplier = 15f;
    public float debugAmplitude = 1f;
    public float debugFrequency = 10f;
	[Range(0.1f, 10f)]
	public float debugDuration = 1f;

    public void Shake(float amplitude = 1f, float frequency = 10f, float duration = 1f) {
        StartCoroutine(ShakeRoutine(debugAmplitude, debugFrequency, debugDuration));
    }

    private IEnumerator ShakeRoutine(float amplitude, float frequency, float duration) {
        Vector3 originalPos = transform.localPosition;
        Vector3 originalRot = transform.localEulerAngles;

		Vector3 lastPos = Vector3.zero;
		Vector3 lastRot = Vector3.zero;

        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            float sampleFloat = Time.time * frequency;

            var posOffset = (new Vector2(
                Mathf.PerlinNoise(sampleFloat, 0.33f),
                Mathf.PerlinNoise(sampleFloat, 0.66f)) - new Vector2(0.5f, 0.5f)) * amplitude;

            var rotOffset = (new Vector2(
				Mathf.PerlinNoise(sampleFloat, 0.5f),
                Mathf.PerlinNoise(sampleFloat, 0.75f)) - new Vector2(0.5f, 0.5f)) * amplitude * rotationMultiplier;

            var newPos = new Vector2(originalPos.x, originalPos.y) + posOffset;
            transform.localPosition = new Vector3(newPos.x, newPos.y, originalPos.z);

			var newRot = new Vector2(originalRot.y, originalRot.z) + rotOffset;
            transform.localEulerAngles += new Vector3(originalRot.x, newRot.x, newRot.y);

			lastPos = transform.localPosition;
			lastPos = transform.localEulerAngles;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

		StartCoroutine(MoveCamera(duration * 0.5f, lastPos, lastRot, originalPos, originalRot));
        // transform.localPosition = originalPos;
        // transform.localEulerAngles = originalRot;
    }

	private IEnumerator MoveCamera(float duration, Vector3 fromPos, Vector3 fromRot, Vector3 toPos, Vector3 toRot) {
		float elapsed = 0f;
		while (elapsed <= duration) {
			transform.localPosition = Vector3.Lerp(fromPos, toPos, elapsed / duration);
			//transform.localEulerAngles = Vector3.Lerp(fromRot, toRot, elapsed / duration);
			print($"{elapsed / duration} - {Vector3.Lerp(fromPos, toPos, elapsed / duration)}");

			elapsed += Time.deltaTime;
			yield return null;
		}
	}

    // Temporary debugging related shit below
    private void Update() {
        if (Input.GetKeyDown(KeyCode.X)) {
            Shake(debugAmplitude, debugFrequency);
        }
    }
}
