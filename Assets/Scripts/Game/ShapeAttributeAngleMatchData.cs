using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// X amount of angles must match given value
/// </summary>
[CreateAssetMenu(fileName = "shapeAttributeAngleMatch", menuName = "Game/Shape Attribute/Angle Match")]
public class ShapeAttributeAngleMatchData : ShapeAttributeData {
    public int angle;
    public int count = 1;

    public override bool Evaluate(ShapeProfile shapeProfile) {
        int matchCount = 0;

        for(int i = 0; i < shapeProfile.angles.Length; i++) {
            var iAngle = Mathf.RoundToInt(shapeProfile.angles[i]);
            if(iAngle == angle)
                matchCount++;
        }

        return matchCount >= count;
    }
}
