using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "shapeAttributeSide", menuName = "Game/Shape Attribute/Side Count")]
public class ShapeAttributeSideData : ShapeAttributeData {
    public int count;

    public override bool Evaluate(ShapeProfile shapeProfile) {
        return shapeProfile.sideLengths.Length == count;
    }
}
