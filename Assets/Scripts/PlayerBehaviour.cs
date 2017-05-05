using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

	public float speed;
	private float _moveStep = 1;
	private bool _isMoving;
	private bool _isColliding;
	private Vector3 _targetPos;
	private Vector3 _prevPos;

	// Use this for initialization
	void Start () {
		this.transform.localScale = new Vector3 (4f, 4f, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (this._isMoving) {
			var currentSpeed = speed * Time.deltaTime;
			var route = _targetPos - this.transform.localPosition;
			var motionVector = route.normalized * currentSpeed;
			var hitsTarget = ((this._targetPos - this.transform.localPosition).magnitude <= currentSpeed);
			if (hitsTarget) {
				this.transform.localPosition = _targetPos;
				this._isMoving = false;
			} else {
				this.transform.Translate (motionVector);
			}
		} else {
			if (Input.GetKey (KeyCode.LeftArrow)) {
				this.Move (new Vector3(_moveStep * -1, 0, 0));
			} else if (Input.GetKey (KeyCode.RightArrow)) {
				this.Move (new Vector3(_moveStep, 0, 0));
			}
			if (Input.GetKey (KeyCode.UpArrow)) {
				this.Move (new Vector3(0, _moveStep, 0));
			} else if (Input.GetKey (KeyCode.DownArrow)) {
				this.Move (new Vector3(0, _moveStep * -1, 0));
			}
		}
	}

	void Move (Vector3 direction) {
		this._prevPos = this.transform.localPosition;
		this._targetPos = this.transform.localPosition + direction;
		this._isMoving = true;
	}

	void OnCollisionEnter2D (Collision2D col)
	{
		this.transform.localPosition = this._prevPos;
		this._isMoving = false;
	}
}
