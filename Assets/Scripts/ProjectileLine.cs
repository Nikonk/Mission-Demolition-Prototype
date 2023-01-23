using System.Collections.Generic;
using UnityEngine;

public class ProjectileLine : MonoBehaviour {
    static public ProjectileLine S;

    [Header("Set in Inspector")]
    public float                minDist = .1f;
     
    private LineRenderer       _line;
    private GameObject         _poi;
    private List<Vector3>      _points;

    public GameObject poi {
        get {
            return _poi;
        }
        set {
            _poi = value;
            if (_poi != null) {
                _line.enabled = false;
                _points = new List<Vector3>();
                AddPoint();
            }
        }
    }

    public Vector3 lastPoint {
        get {
            if (_points == null) {
                return Vector3.zero;
            }
            return _points[_points.Count-1];
        }
    }

    private void Awake() {
        S = this;
        _line = GetComponent<LineRenderer>();
        _line.enabled = false;
        _points = new List<Vector3>();
    }

    private void FixedUpdate() {
        if (poi == null) {
            if (FollowCam.POI != null) {
                if (FollowCam.POI.tag == "Projectile") {
                    poi = FollowCam.POI;
                } else {
                    return;
                }
            } else {
                return;
            }
        }
        AddPoint();
        if (FollowCam.POI == null) {
            poi = null;
        }
    }

    public void Clear() {
        poi = null;
        _line.enabled = false;
        _points = new List<Vector3>();
    }

    public void AddPoint() {
        Vector3 pt = poi.transform.position;
        if (_points.Count > 0 && (pt - lastPoint).magnitude < minDist) {
            return;
        }
        if (_points.Count == 0) {
            Vector3 launchPosDiff = pt - Slingshot.LAUNCH_POS;
            _points.Add(pt + launchPosDiff);
            _points.Add(pt);
            _line.positionCount = 2;
            _line.SetPosition(0, _points[0]);
            _line.SetPosition(1, _points[1]);
            _line.enabled = true;
        } else {
            _points.Add(pt);
            _line.positionCount = _points.Count;
            _line.SetPosition(_points.Count-1, lastPoint);
            _line.enabled = true;
        }
    }
}
