using UnityEngine;
using System.Collections.Generic;

public class bezierPt{
	public GameObject _pp;
	public GameObject _cp1;
	public GameObject _cp2;

	public bezierPt _next;

	public float _length;
	public List<Vector3> _pts;

	public bezierPt() {
		_pp = null;
		_cp1 = null;
		_cp2 = null;

		_next = null;

		_length = 0.0f;
		//_pts = new List<Vector3>();
	}

	public void setupMirror() {
		_cp1.GetComponent<drag>()._center = _pp.transform;
		_cp1.GetComponent<drag>()._mirror = _cp2.transform;
		_cp2.GetComponent<drag>()._center = _pp.transform;
		_cp2.GetComponent<drag>()._mirror = _cp1.transform;
	}
	public void setupDrag() {
		if (_cp1 != null)
			_pp.GetComponent<drag>()._move1 = _cp1.transform;
		if (_cp2 != null)
			_pp.GetComponent<drag>()._move2 = _cp2.transform;
	}

	public void calculate() {
		Vector3 a = _pp.transform.position;
		Vector3 b = _cp2.transform.position;
		Vector3 c = _next._cp1.transform.position;
		Vector3 d = _next._pp.transform.position;

		Vector3 pt = new Vector3();
		pt = a;

		_pts.Add(pt);

		Vector3 prev = pt;
		int step = 20;

		for (int i = 1; i < step; ++i) {

			float t = (float)i / (float)step;

			Vector3 ab;
			Vector3 bc;
			Vector3 cd;
			Vector3 abc;
			Vector3 bcd;

			ab = a + (b - a) * t;
			bc = b + (c - b) * t;
			cd = c + (d - c) * t;

			abc = ab + (bc - ab) * t;
			bcd = bc + (cd - bc) * t;

			pt = abc + (bcd - abc) * t;

			_length += (pt - prev).magnitude;
			prev = pt;

			_pts.Add(pt);
        }

		_length += (d - prev).magnitude;

	}

}

public class control : MonoBehaviour {

	public bool _held = false;
	public GameObject _prefab = null;

	private List<bezierPt> _segments = new List<bezierPt>();
	public List<Vector3> _pts = new List<Vector3>();

	public LineRenderer _line;


	void Start () {
		_segments.Add(new bezierPt());
		_segments.Add(new bezierPt());

		_segments[0]._next = _segments[1];
		_segments[0]._pts = _pts;
		_segments[1]._pts = _pts;

		_line = GetComponent<LineRenderer>();

	}
	
	void Update () {
		if(Input.GetMouseButtonDown(0) && !_held){
			Vector3 mpos = Input.mousePosition;
			mpos.z = 10.0f;
			mpos = Camera.main.ScreenToWorldPoint(mpos);

			bezierPt bzp1 = _segments[_segments.Count - 2];
			bezierPt bzp2 = _segments[_segments.Count - 1];

			if (bzp1._pp == null) {
				bzp1._pp = Instantiate(_prefab, mpos, Quaternion.identity) as GameObject;
			} else {
				bzp2._pp = Instantiate(_prefab, mpos, Quaternion.identity) as GameObject;

				Vector3 dir = bzp2._pp.transform.position - bzp1._pp.transform.position;

				bzp1._cp2 = Instantiate(_prefab, bzp1._pp.transform.position + dir * 0.3333f, Quaternion.identity) as GameObject;
				bzp2._cp1 = Instantiate(_prefab, bzp1._pp.transform.position + dir * 0.6666f, Quaternion.identity) as GameObject;

				bzp1._cp2.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				bzp2._cp1.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

				bzp1.setupDrag();
				bzp2.setupDrag();

				if (_segments.Count > 2) {
					bzp1.setupMirror();
                }

				_segments.Add(new bezierPt());
				_segments[_segments.Count - 1]._pts = _pts;

				bzp2._next = _segments[_segments.Count - 1];
			}
			
		}

		if (Input.GetKey(KeyCode.A)) {
			_pts.Clear();
			for (int i = 0; i < _segments.Count - 2; ++i) {
				_segments[i].calculate();
			}

			_line.SetVertexCount(_pts.Count);

			for (int i = 0; i < _pts.Count; ++i) {
				_line.SetPosition(i, _pts[i]);
			}
		}
	}
}
