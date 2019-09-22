using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeProfile))]
public class ShapeProfileInspector : Editor {

    private bool mShapeCategoryFoldout;

    private static ShapeCategoryData[] mShapeCategories;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        M8.EditorExt.Utility.DrawSeparator();

        var dat = target as ShapeProfile;

        var shape = dat.shape ? dat.shape : dat.GetComponent<Shapes2D.Shape>();
        if(shape) {
            var settings = shape.settings;
            var verts = settings.polyVertices;

            Vector2 scale = shape.transform.localScale;

            GUILayout.Label("Sides");
            if(verts.Length > 1) {
                for(int i = 0; i < verts.Length; i++) {
                    var vert1 = verts[i] * scale;

                    Vector2 vert2;

                    if(i < verts.Length - 1)
                        vert2 = verts[i + 1] * scale;
                    else
                        vert2 = verts[0] * scale;

                    var length = (vert2 - vert1).magnitude * dat.sideScale;
                    var iLength = Mathf.RoundToInt(length);

                    EditorGUILayout.LabelField(i.ToString(), iLength.ToString());
                }
            }

            GUILayout.Label("Angles");
            if(verts.Length > 2) {
                for(int i = 0; i < verts.Length; i++) {
                    var vert1 = verts[i] * scale;

                    Vector2 vert2, vert3;

                    if(i > 0)
                        vert2 = verts[i - 1] * scale;
                    else
                        vert2 = verts[verts.Length - 1] * scale;

                    if(i < verts.Length - 1)
                        vert3 = verts[i + 1] * scale;
                    else
                        vert3 = verts[0] * scale;

                    var dir1 = (vert2 - vert1).normalized;
                    var dir2 = (vert3 - vert1).normalized;

                    var angle = Vector2.SignedAngle(dir2, dir1);
                    if(angle < 0f)
                        angle = 360f + angle;

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
                    dat.ComputeAttributes();
                    RefreshShapeCategories();
                    mShapeCategoryFoldout = true;
                    Repaint();
                }
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
