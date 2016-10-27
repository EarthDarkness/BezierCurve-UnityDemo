using UnityEngine;
using System.Collections;

public class drag : MonoBehaviour {
	control _ctrl;

	public Transform _mirror = null;
	public Transform _center = null;

	public Transform _move1 = null;
	public Transform _move2 = null;

	void Start(){
		_ctrl = GameObject.Find("CTRL").GetComponent<control>();
	}
	
	void Update(){

	}

	void OnMouseDown() {
		_ctrl._held = true;
	}
	void OnMouseUp() {
		_ctrl._held = false;
	}
	void OnMouseDrag() {
		Vector3 mpos = Input.mousePosition;
		mpos.z = 10.0f;
		mpos = Camera.main.ScreenToWorldPoint(mpos);

		Vector3 dif = mpos - transform.position;

		transform.position = mpos;

		if (_mirror != null && _center != null) {
			Vector3 dir = transform.position - _center.position;
			_mirror.position = _center.position - dir.normalized * (_mirror.position - _center.position).magnitude;
		}
		if (_move1 != null) {
			_move1.Translate(dif);
		}
		if (_move2 != null) {
			_move2.Translate(dif);
		}
	}
}
