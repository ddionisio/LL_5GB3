using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "shape", menuName = "Game/Shape Category")]
public class ShapeCategoryData : ScriptableObject {
    /// <summary>
    /// Used for editor
    /// </summary>
    public string label;

    [M8.Localize]
    public string textRef;

    public ShapeCategoryData parentCategory;

    public ShapeAttributeData[] attributes;

    public bool Evaluate(ShapeProfile shapeProfile) {
        if(parentCategory) {
            if(!parentCategory.Evaluate(shapeProfile))
                return false;
        }

        for(int i = 0; i < attributes.Length; i++) {
            var attr = attributes[i];
            if(!attr.Evaluate(shapeProfile))
                return false;
        }

        return true;
    }
}
