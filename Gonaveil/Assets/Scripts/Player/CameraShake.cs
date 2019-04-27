using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
	public Transform WeaponHolder;
	public float rotationMultiplier = 15f;
    public float debugAmplitude = 1f;
    public float debugFrequency = 10f;
	[Range(0.1f, 10f)]
	public float debugDuration = 1f;

    public void Shake(float amplitude = 1f, float frequency = 10f, float duration = 1f, float falloff = 0.5f) {
        StartCoroutine(ShakeRoutine(debugAmplitude, debugFrequency, debugDuration, falloff));
    }

    private IEnumerator ShakeRoutine(float amplitude, float frequency, float duration, float falloff) {
        Vector3 originalPos = transform.localPosition;
        Vector3 originalRot = transform.localRotation.eulerAngles;
		Vector3 lastPos = Vector3.zero;

        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            float sampleFloat = Time.time * frequency;
			//frequency *= (1 - falloff);

            var posOffset = (new Vector3(
                Mathf.PerlinNoise(sampleFloat, 0.2f),
                Mathf.PerlinNoise(sampleFloat, 0.4f),
				Mathf.PerlinNoise(sampleFloat, 0.7f)) - new Vector3(0.5f, 0.5f, 0.5f)) * amplitude;

            var rotOffset = (new Vector3(
				Mathf.PerlinNoise(sampleFloat, 0.25f),
                Mathf.PerlinNoise(sampleFloat, 0.5f),
				Mathf.PerlinNoise(sampleFloat, 0.75f)) - new Vector3(0.5f, 0.5f, 0.5f)) * amplitude * rotationMultiplier;

            var newPos = originalPos + new Vector3(posOffset.x, posOffset.y, 0);
            transform.localPosition = newPos;
			WeaponHolder.localPosition -= posOffset * 0.1f;
		
			WeaponHolder.Rotate(new Vector3(0, rotOffset.y, rotOffset.z));
            transform.Rotate(new Vector3(rotOffset.x, rotOffset.y, rotOffset.z));

			lastPos = transform.localPosition;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

		StartCoroutine(MoveCamera(duration * 0.25f, lastPos, originalPos));
    }

	private IEnumerator MoveCamera(float duration, Vector3 fromPos, Vector3 toPos) {
		float elapsed = 0f;
		while (elapsed <= duration) {
			transform.localPosition = Vector3.Lerp(fromPos, toPos, elapsed / duration);
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
