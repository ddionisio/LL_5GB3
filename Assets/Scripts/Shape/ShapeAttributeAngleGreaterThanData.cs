using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// At least X number of angles are greater than value
/// </summary>
[CreateAssetMenu(fileName = "shapeAttributeAngleGreaterThan", menuName = "Game/Shape Attribute/Angle Greater Than")]
public class ShapeAttributeAngleGreaterThanData : ShapeAttributeData {
    public int angle;
    public int count = 1;

    public override bool Evaluate(ShapeProfile shapeProfile) {
        int matchCount = 0;

        var angles = shapeProfile.angles;
        for(int i = 0; i < angles.Length; i++) {
            var iAngle = Mathf.RoundToInt(angles[i]);
            if(iAngle > angle)
                matchCount++;
        }

        return matchCount >= count;
    }
}
