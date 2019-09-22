using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// At least X number of sides are inequal
/// </summary>
[CreateAssetMenu(fileName = "shapeAttributeSideInequal", menuName = "Game/Shape Attribute/Side Inequal")]
public class ShapeAttributeSideInequalData : ShapeAttributeData {
    public int count = 3;
    public bool isAll;

    public override bool Evaluate(ShapeProfile shapeProfile) {
        int maxEqualCount = 0;

        var sides = shapeProfile.sideLengths;
        for(int i = 0; i < sides.Length; i++) {
            var _count = 0;

            var sideCheckA = Mathf.RoundToInt(sides[i]);

            for(int j = 0; j < sides.Length; j++) {
                if(j == i) continue;

                var sideCheckB = Mathf.RoundToInt(sides[j]);

                if(sideCheckA == sideCheckB)
                    _count++;
            }

            if(_count > maxEqualCount)
                maxEqualCount = _count;
        }

        if(isAll)
            return maxEqualCount == 0;

        return sides.Length - maxEqualCount >= count;
    }
}
