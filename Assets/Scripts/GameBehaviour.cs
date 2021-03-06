﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBehaviour : MonoBehaviour {

	public Transform wall;
	public Transform background;
	public Transform player;
	public Text scoreText;
	public float width;
	public float height;
	public float speed;
	public float interval;
	public int scoreStep;
	public RawImage directionArrow;

	private List<List<List<Transform>>> _grid;
	private bool _isMoving;
	private int _removeX, _removeY, _lowX, _highX, _lowY, _highY, _score;
	private Vector3 _targetPos;
	private int _direction;

	// Use this for initialization
	void Start () {
		StartGame ();
	}
	
	// Update is called once per frame
	void Update () {
		if (this._isMoving) {
			var currentSpeed = speed * Time.deltaTime;
			var route = _targetPos - this.transform.position;
			var motionVector = route.normalized * currentSpeed;
			var hitsTarget = ((this._targetPos - this.transform.position).magnitude <= currentSpeed);
			if (hitsTarget) {
				this.transform.position = _targetPos;
				this._isMoving = false;
				if (this._removeX >= 0) {
					foreach (List<List<Transform>> item in this._grid) {
						foreach (Transform t in item[this._removeX]) {
							Destroy (t.gameObject);
						}
						item.RemoveAt (this._removeX);
					}
					this._removeX = -1;
				} else if (this._removeY >= 0) {
					foreach (List<Transform> item in this._grid[this._removeY]) {
						foreach (Transform t in item) {
							Destroy (t.gameObject);
						}
					}
					this._grid.RemoveAt (this._removeY);
					this._removeY = -1;
				}
				_score += scoreStep;
				_direction = (int)Mathf.Floor (Random.value * 4);
			} else {
				this.transform.Translate (motionVector);
			}
		}
		UpdateTextLabels ();
	}

	void UpdateTextLabels () {
		scoreText.text = _score.ToString ();
		switch (_direction) {
		case 0:
			directionArrow.transform.rotation = Quaternion.AngleAxis (0, new Vector3(0, 0, 1f));
			break;
		case 1:
			directionArrow.transform.rotation = Quaternion.AngleAxis (180, new Vector3(0, 0, 1f));
			break;
		case 2:
			directionArrow.transform.rotation = Quaternion.AngleAxis (90, new Vector3(0, 0, 1f));
			break;
		case 3:
			directionArrow.transform.rotation = Quaternion.AngleAxis (270, new Vector3(0, 0, 1f));
			break;
		}
	}

	void DestroyChildren (Transform t) {
		foreach (Transform c in t) {
			DestroyChildren (c);
			Destroy (c.gameObject);
		}
	}

	public bool IsMoving() {
		return _isMoving;
	}

	public void StartGame() {
		_lowX = -1;
		_lowY = -1;
		_highX = (int)width;
		_highY = (int)height;
		_removeX = -1;
		_removeY = -1;
		_isMoving = false;
		_score = 0;
		_direction = (int)Mathf.Floor (Random.value * 4);

		this.transform.localPosition = new Vector3 (0, 0, 0);

		PlacePlayer ();
		DrawBorder ();

		_grid = new List<List<List<Transform>>> ();
		for (var y = 0; y < height; y++) {
			var row = new List<List<Transform>> ();
			for (var x = 0; x < width; x++) {
				row.Add (SetSquare (x, y));
			}
			_grid.Add (row);
		}
		InvokeRepeating("MovePlayground", interval, interval);
	}

	public void ResetGame() {
		CancelInvoke ();
		DestroyChildren (this.transform);
		StartGame ();
	}

	void PlacePlayer () {
		var myPlayer = Instantiate (player);
		myPlayer.parent = this.transform;
		myPlayer.localPosition = new Vector3 (0, 0, -2f);
	}

	void DrawBorder () {
		var positions = new Vector3[] {
			new Vector3((width / 2f) - .5f, -.5f, -1f),
			new Vector3(-1 * (width / 2f) - .5f, -.5f, -1f),
			new Vector3(-.5f, (height / 2f) - .5f, -1f),
			new Vector3(-.5f, -1 * (height / 2f) - .5f, -1f)
		};
		var scales = new Vector3[] {
			new Vector3 (.1f, height, 0),
			new Vector3 (.1f, height, 0),
			new Vector3 (width, .1f, 0),
			new Vector3 (width, .1f, 0)
		};
		var bgPositions = new Vector3[] {
			new Vector3 (0, (height / 2f) + ((Screen.height / 2)), -.5f),
			new Vector3 (0, (height / -2f) - ((Screen.height / 2) + 1f), -.5f),
			new Vector3 ((width / 2f) + ((Screen.width / 2) - .5f), 0, -.5f),
			new Vector3 ((width / -2f) - ((Screen.width / 2) + .5f), 0, -.5f),
		};
		for (var i = 0; i < 4; i++) {
			var myBg = Instantiate (background);
			myBg.position = bgPositions [i];
			myBg.localScale = new Vector3 (Screen.width, Screen.height, 0);
			myBg.parent = this.transform.parent;

			var myWall = Instantiate (wall);
			myWall.position = positions[i];
			myWall.localScale = scales[i];
			myWall.parent = this.transform.parent;
			myWall.gameObject.tag = "FieldBorder";
		}
	}

	List<Transform> SetSquare (int x, int y) {
		var cell = new List<Transform> ();
		var walls = Mathf.Floor (Random.value * 2);
		for (var i = 0; i < walls; i++) {
			var myWall = Instantiate (wall);
			myWall.localScale = new Vector3(.1f, 1f, 1f);
			myWall.parent = this.transform;
			var rotation = Mathf.Floor(Random.value * 2);
			var position = Mathf.Floor(Random.value * 2);
			myWall.Rotate (0, 0, 90 * rotation);
			myWall.gameObject.tag = "Wall";
			if (rotation == 0) {
				myWall.localPosition = new Vector3 (
					(float)(((position > 0) ? x - .5 : x + .5f) - (width / 2f)), 
					(float)(y - (height / 2)), 
					0);
			} else {
				myWall.localPosition = new Vector3 (
					(float)(x - (width / 2)), 
					(float)(((position > 0) ? y - .5f : y + .5f) - (height / 2f)), 
					0);
			}
			cell.Add (myWall);
		}
		return cell;
	}

	void MovePlayground () {
		this._removeX = -1;
		this._removeY = -1;
		if (_direction == 0) {
			AddRow (false);
			this._targetPos = new Vector3 (
				this.transform.position.x,
				this.transform.position.y + 1f,
				0);
		} else if (_direction == 1) {
			AddRow (true);
			this._targetPos = new Vector3 (
				this.transform.position.x,
				this.transform.position.y - 1f,
				0);
		} else if (_direction == 2) {
			AddCol (true);
			this._targetPos = new Vector3 (
				this.transform.position.x - 1f,
				this.transform.position.y,
				0);
		} else if (_direction == 3) {
			AddCol (false);
			this._targetPos = new Vector3 (
				this.transform.position.x + 1f,
				this.transform.position.y,
				0);
		}
		this._isMoving = true;
	}

	void AddCol (bool right) {
		for (var i = 0; i < this.height; i++) {
			if (right) {
				_grid[i].Add(SetSquare (this._highX, this._lowY + 1 + i));
			} else {
				_grid[i].Insert(0, SetSquare (this._lowX, this._lowY + 1 + i));
			}
		}
		if (right) {
			this._removeX = 0;
			this._lowX++;
			this._highX++;
		} else {
			this._removeX = this._grid[0].Count - 1;
			this._lowX--;
			this._highX--;
		}
	}

	void AddRow (bool bottom) {
		var row = new List<List<Transform>> ();
		for (var i = 0; i < this.width; i++) {
			if (bottom) {
				row.Add(SetSquare (this._lowX + 1 + i, this._highY));
			} else {
				row.Add(SetSquare (this._lowX + 1 + i, this._lowY));
			}
		}
		if (bottom) {
			_grid.Add (row);
			this._removeY = 0;
			this._lowY++;
			this._highY++;
		} else {
			_grid.Insert (0, row);
			this._removeY = this._grid.Count - 1;
			this._lowY--;
			this._highY--;
		}
	}
}
