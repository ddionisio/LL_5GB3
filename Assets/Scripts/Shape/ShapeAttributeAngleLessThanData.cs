using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// At least X number of angles are less than value
/// </summary>
[CreateAssetMenu(fileName = "shapeAttributeAngleLessThan", menuName = "Game/Shape Attribute/Angle Less Than")]
public class ShapeAttributeAngleLessThanData : ShapeAttributeData {
    public int angle;
    public int count = 1;
    public bool isAll;

    public override bool Evaluate(ShapeProfile shapeProfile) {
        int matchCount = 0;

        var angles = shapeProfile.angles;
        for(int i = 0; i < angles.Length; i++) {
            var iAngle = Mathf.RoundToInt(angles[i]);
            if(iAngle < angle)
                matchCount++;
        }

        if(isAll)
            return matchCount == angles.Length;

        return matchCount >= count;
    }
}
