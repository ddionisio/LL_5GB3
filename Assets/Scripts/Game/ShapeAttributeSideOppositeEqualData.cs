using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// At least X number of sides opposite are equal
/// </summary>
[CreateAssetMenu(fileName = "shapeAttributeSideOppositeEqual", menuName = "Game/Shape Attribute/Side Opposite Equal")]
public class ShapeAttributeSideOppositeEqualData : ShapeAttributeData {
    public int count = 1; //at least 1

    public override bool Evaluate(ShapeProfile shapeProfile) {
        int equalCount = 0;

        var sides = shapeProfile.sideLengths;
        int hCount = sides.Length / 2;

        //compare opposite sides
        for(int i = 0; i < hCount; i++) {
            var sideA = Mathf.RoundToInt(sides[i]);
            var sideB = Mathf.RoundToInt(sides[i + hCount]);

            if(sideA == sideB)
                equalCount++;
        }

        return equalCount >= count;
    }
}
