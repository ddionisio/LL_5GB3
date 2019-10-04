using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeProfile))]
public class ShapeProfileInspector : Editor {

    private bool mShapeCategoryFoldout = true;

    private static ShapeCategoryData[] mShapeCategories;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        M8.EditorExt.Utility.DrawSeparator();

        var dat = target as ShapeProfile;

        dat.RefreshScale();
        dat.ComputeAttributes();

        GUILayout.Label("Sides");
        var sides = dat.sideLengths;
        if(sides.Length > 1) {
            for(int i = 0; i < sides.Length; i++) {
                var length = sides[i];
                var iLength = Mathf.RoundToInt(length);

                EditorGUILayout.LabelField(i.ToString(), string.Format("{0} ({1})", iLength, length));
            }
        }

        GUILayout.Label("Angles");
        var angles = dat.angles;
        if(angles.Length > 2) {
            for(int i = 0; i < angles.Length; i++) {
                var angle = angles[i];
                var iAngle = Mathf.RoundToInt(angle);

                EditorGUILayout.LabelField(i.ToString(), iAngle.ToString());
            }
        }

        mShapeCategoryFoldout = EditorGUILayout.Foldout(mShapeCategoryFoldout, "Categories");
        if(mShapeCategoryFoldout) {
            //grab shape categories
            if(mShapeCategories == null)
                RefreshShapeCategories();

            //determine which category this shape fits in, and display
            for(int i = 0; i < mShapeCategories.Length; i++) {
                var cat = mShapeCategories[i];
                if(!cat) continue;

                if(cat.Evaluate(dat))
                    GUILayout.Label("- " + cat.label);
            }

            if(GUILayout.Button("Refresh")) {
                RefreshShapeCategories();
                mShapeCategoryFoldout = true;
                Repaint();
            }
        }
    }

    private void RefreshShapeCategories() {
        var guids = AssetDatabase.FindAssets("t:ShapeCategoryData");

        mShapeCategories = new ShapeCategoryData[guids.Length];

        for(int i = 0; i < guids.Length; i++) {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            mShapeCategories[i] = AssetDatabase.LoadAssetAtPath<ShapeCategoryData>(path);
        }
    }
}
