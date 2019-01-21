using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyExplorer
{
    public class BezierCurve : MonoBehaviour {
        public Vector3[] points;

        public Vector3 GetPoint (float t)
        {
            return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], t));
        }
    }
}
