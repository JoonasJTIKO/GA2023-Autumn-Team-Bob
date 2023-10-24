using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class Bezier : MonoBehaviour
    {
        public Vector3[] points;

        public int CurveCount
        {
            get { return (points.Length - 1) / 3; }
        }

        public void Reset()
        {
            points = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f)
            };
        }

        public Vector3 GetPoint(float t)
        {
            if (t >= 1f)
            {
                t = 1f;
            }

            return Vector3.Lerp(Vector3.Lerp(points[0], points[1], t), Vector3.Lerp(points[1], points[2], t), t);
            
        }

        public Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            t = Mathf.Clamp01(t);
            return 2f * (1f - t) * (p1 - p0) + 2f * t * (p2 - p1);
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }

        public Vector3 GetVelocity(float t)
        {
            if (t >= 1f)
            {
                t = 1f;
            }

            return transform.TransformPoint(GetFirstDerivative(points[0], points[1], points[2], t)) - transform.position;
        }
    }
}
