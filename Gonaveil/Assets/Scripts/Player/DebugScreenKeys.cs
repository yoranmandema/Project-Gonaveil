using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScreenKeys : MonoBehaviour
{
	public Sprite keyUpSprite;
	public Sprite keyDownSprite;
	public Image wKey;
	public Image aKey;
	public Image sKey;
	public Image dKey;
	public Image ctrlKey;
	public Image spaceKey;
	public Image mouse0Key;
	public Image mouse1Key;
	public Image mouseLeft;
	public Image mouseRight;

	private Color Cyan = new Color(0, 255, 255);
	private Color Red = new Color(255, 0, 0);
	private Color White = new Color(255, 255, 255);

	private void Update() {
		wKey.sprite = Input.GetKey(KeyCode.W) ? keyDownSprite : keyUpSprite;
		wKey.color = Input.GetKey(KeyCode.W) ? Cyan : White;

		aKey.sprite = Input.GetKey(KeyCode.A) ? keyDownSprite : keyUpSprite;
		aKey.color = Input.GetKey(KeyCode.A) ? Cyan : White;

		sKey.sprite = Input.GetKey(KeyCode.S) ? keyDownSprite : keyUpSprite;
		sKey.color = Input.GetKey(KeyCode.S) ? Cyan : White;

		dKey.sprite = Input.GetKey(KeyCode.D) ? keyDownSprite : keyUpSprite;
		dKey.color = Input.GetKey(KeyCode.D) ? Cyan : White;

		spaceKey.sprite = Input.GetKey(KeyCode.Space) ? keyDownSprite : keyUpSprite;
		spaceKey.color = Input.GetKey(KeyCode.Space) ? Cyan : White;

		ctrlKey.sprite = Input.GetKey(KeyCode.LeftControl) ? keyDownSprite : keyUpSprite;
		ctrlKey.color = Input.GetKey(KeyCode.LeftControl) ? Cyan : White;

		mouse0Key.enabled = Input.GetKey(KeyCode.Mouse0);
		mouse0Key.color = Input.GetKey(KeyCode.Mouse0) ? Red : White;

		mouse1Key.enabled = Input.GetKey(KeyCode.Mouse1);
		mouse1Key.color = Input.GetKey(KeyCode.Mouse1) ? Red : White;

		mouseLeft.enabled = Input.GetAxis("Mouse X") < 0f;
		mouseRight.enabled = Input.GetAxis("Mouse X") > 0f;
	}
}
