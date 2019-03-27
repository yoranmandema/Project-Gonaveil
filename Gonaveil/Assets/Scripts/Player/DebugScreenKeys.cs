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

	private void Update() {
		wKey.sprite = Input.GetKey(KeyCode.W) ? keyDownSprite : keyUpSprite;
		aKey.sprite = Input.GetKey(KeyCode.A) ? keyDownSprite : keyUpSprite;
		sKey.sprite = Input.GetKey(KeyCode.S) ? keyDownSprite : keyUpSprite;
		dKey.sprite = Input.GetKey(KeyCode.D) ? keyDownSprite : keyUpSprite;
		spaceKey.sprite = Input.GetKey(KeyCode.Space) ? keyDownSprite : keyUpSprite;
		ctrlKey.sprite = Input.GetKey(KeyCode.LeftControl) ? keyDownSprite : keyUpSprite;
		mouse0Key.enabled = Input.GetKey(KeyCode.Mouse0);
		mouse1Key.enabled = Input.GetKey(KeyCode.Mouse1);
		mouseLeft.enabled = Input.GetAxis("Mouse X") < 0f;
		mouseRight.enabled = Input.GetAxis("Mouse X") > 0f;
	}
}
