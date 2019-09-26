using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeProfile : MonoBehaviour {
    [Header("Data")]
    public Shapes2D.Shape shape;
    public float sideScale = 1f;

    public Vector2[] points {
        get {
            if(mPoints == null)
                ComputeAttributes();

            return mPoints;
        }
    }

    public float[] sideLengths {
        get {
            if(mSideLengths == null)
                ComputeAttributes();

            return mSideLengths;
        }
    }

    public Vector2[] sideDirs {
        get {
            if(mSideDirs == null)
                ComputeAttributes();

            return mSideDirs;
        }
    }

    public float[] angles {
        get {
            if(mAngles == null)
                ComputeAttributes();

            return mAngles;
        }
    }

    public bool isCollected { get; private set; }

    public int score { get; set; }

    private Vector2[] mPoints;
    private float[] mSideLengths;
    private Vector2[] mSideDirs;
    private float[] mAngles;

    public void Collect() {
        isCollected = true;
    }

    public void ComputeAttributes() {
        Vector2 s = shape.transform.localScale;
        var settings = shape.settings;
        var count = settings.polyVertices.Length;

        //generate points
        mPoints = new Vector2[count];

        for(int i = 0; i < count; i++) {
            var vert = settings.polyVertices[i];
            mPoints[i] = vert * s;
        }

        //generate lengths
        if(count > 1) {
            mSideLengths = new float[count];
            mSideDirs = new Vector2[count];

            for(int i = 0; i < count; i++) {
                var pt1 = mPoints[i];

                Vector2 pt2;
                if(i < count - 1)
                    pt2 = mPoints[i + 1];
                else
                    pt2 = mPoints[0];

                var dpos = pt2 - pt1;
                var len = dpos.magnitude;

                mSideDirs[i] = dpos / len;
                mSideLengths[i] = len * sideScale;
            }
        }
        else //fail-safe
            mSideLengths = new float[0];

        //generate angles
        if(count > 2) {
            mAngles = new float[count];

            for(int i = 0; i < count; i++) {
                var pt1 = mPoints[i];

                Vector2 pt2, pt3;

                if(i > 0)
                    pt2 = mPoints[i - 1];
                else
                    pt2 = mPoints[count - 1];

                if(i < count - 1)
                    pt3 = mPoints[i + 1];
                else
                    pt3 = mPoints[0];

                var dir1 = (pt2 - pt1).normalized;
                var dir2 = (pt3 - pt1).normalized;

                var angle = Vector2.SignedAngle(dir2, dir1);
                if(angle < 0f)
                    angle = 360f + angle;

                mAngles[i] = angle;
            }
        }
        else //fail-safe
            mAngles = new float[0];
    }

    void Awake() {
        ComputeAttributes();
    }
}
