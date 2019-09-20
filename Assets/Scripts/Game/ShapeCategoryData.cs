using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "shape", menuName = "Game/Shape Category")]
public class ShapeCategoryData : ScriptableObject {
    [M8.Localize]
    public string textRef;

    public ShapeAttributeData[] attributes;

    public bool Evaluate(ShapeProfile shapeProfile) {
        for(int i = 0; i < attributes.Length; i++) {
            var attr = attributes[i];
            if(!attr.Evaluate(shapeProfile))
                return false;
        }

        return true;
    }
}
