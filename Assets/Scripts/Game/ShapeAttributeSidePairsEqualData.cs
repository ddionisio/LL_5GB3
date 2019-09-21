using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// At least X number of side pairs (adjacent) are equal.
/// </summary>
[CreateAssetMenu(fileName = "shapeAttributeSideOppositeEqual", menuName = "Game/Shape Attribute/Side Opposite Equal")]
public class ShapeAttributeSidePairsEqualData : ShapeAttributeData {
    public int count = 2;

    public override bool Evaluate(ShapeProfile shapeProfile) {
        var equalCount = 0;

        var sides = shapeProfile.sideLengths;
        for(int i = 0; i < sides.Length; i++) {
            var sideLenA = Mathf.RoundToInt(sides[i]);

            int sideLenB;
            if(i < sides.Length - 1)
                sideLenB = Mathf.RoundToInt(sides[i + 1]);
            else
                sideLenB = Mathf.RoundToInt(sides[0]);

            if(sideLenA == sideLenB)
                equalCount++;
        }

        return equalCount >= count;
    }
}
