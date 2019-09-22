using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// At least X number of angles are equal to opposite.
/// </summary>
[CreateAssetMenu(fileName = "shapeAttributeAngleEqualOpposite", menuName = "Game/Shape Attribute/Angle Equal Opposite")]
public class ShapeAttributeAngleEqualOppositeData : ShapeAttributeData {

    public int count = 1; //at least 1

    public override bool Evaluate(ShapeProfile shapeProfile) {
        int equalCount = 0;

        var angles = shapeProfile.angles;
        int hCount = angles.Length / 2;

        //compare opposite sides
        for(int i = 0; i < hCount; i++) {
            var angleA = Mathf.RoundToInt(angles[i]);
            var angleB = Mathf.RoundToInt(angles[i + hCount]);

            if(angleA == angleB)
                equalCount++;
        }

        return equalCount >= count;
    }
}
