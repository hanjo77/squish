using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

	public float speed;
	public RuntimeAnimatorController animUp;
	public RuntimeAnimatorController animDown;
	public RuntimeAnimatorController animLeft;
	public RuntimeAnimatorController animRight;

	private float _moveStep = 1;
	private bool _isMoving;
	private bool _isColliding;
	private Vector3 _targetPos;
	private Vector3 _prevPos;
	private bool _touchesBorder;
	private GameBehaviour _game;
	private Animator _spriteAnimator;

	// Use this for initialization
	void Start () {
		_game = this.transform.parent.gameObject.GetComponent<GameBehaviour> ();
		_spriteAnimator = this.gameObject.GetComponentInChildren<Animator> ();
		_spriteAnimator.Stop ();
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
				_spriteAnimator.runtimeAnimatorController = animLeft;
				this.Move (new Vector3 (_moveStep * -1, 0, 0));
			} else if (Input.GetKey (KeyCode.RightArrow)) {
				_spriteAnimator.runtimeAnimatorController = animRight;
				this.Move (new Vector3 (_moveStep, 0, 0));
			} else if (Input.GetKey (KeyCode.UpArrow)) {
				_spriteAnimator.runtimeAnimatorController = animUp;
				this.Move (new Vector3 (0, _moveStep, 0));
			} else if (Input.GetKey (KeyCode.DownArrow)) {
				_spriteAnimator.runtimeAnimatorController = animDown;
				this.Move (new Vector3 (0, _moveStep * -1, 0));
			} else {
				_spriteAnimator.runtimeAnimatorController = null;
				_spriteAnimator.Stop ();
			}
			if (!this._touchesBorder || !_game.IsMoving ()) {
				this.transform.localPosition = new Vector3 (
					Mathf.Round (this.transform.localPosition.x),
					Mathf.Round (this.transform.localPosition.y),
					Mathf.Round (this.transform.localPosition.z));
				this._prevPos = this.transform.localPosition;
			}
		}
	}

	void Move (Vector3 direction) {
		this._prevPos = this.transform.localPosition;
		this._targetPos = this.transform.localPosition + direction;
		this._isMoving = true;
		_spriteAnimator.Play ("Entry", 0);
	}

	void OnCollisionEnter2D (Collision2D col)
	{
		this.transform.localPosition = this._prevPos;
		this._isMoving = false;
		if (col.gameObject.tag == "FieldBorder") {
			this._touchesBorder = true;
		}
		if (this._touchesBorder && (col.gameObject.tag == "Wall")) {
			_game.ResetGame ();
		}
	}

	void OnCollisionExit2D (Collision2D col)
	{
		if (col.gameObject.tag == "FieldBorder") {
			this._touchesBorder = false;
		}
	}
}
