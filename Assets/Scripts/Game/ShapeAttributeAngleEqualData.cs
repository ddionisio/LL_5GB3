﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// At least X number of angles are equal
/// </summary>
[CreateAssetMenu(fileName = "shapeAttributeAngleEqual", menuName = "Game/Shape Attribute/Angle Equal")]
public class ShapeAttributeAngleEqualData : ShapeAttributeData {
    public int count = 2; //ensure it is at least 2 for this to make sense

    public override bool Evaluate(ShapeProfile shapeProfile) {
        int maxEqualCount = 0;

        var angles = shapeProfile.angles;
        for(int i = 0; i < angles.Length; i++) {
            var _count = 1;

            var angleCheckA = Mathf.RoundToInt(angles[i]);

            for(int j = 0; j < angles.Length; j++) {
                if(j == i) continue;

                var angleCheckB = Mathf.RoundToInt(angles[j]);

                if(angleCheckA == angleCheckB)
                    _count++;
            }

            if(_count > maxEqualCount)
                maxEqualCount = _count;
        }

        return maxEqualCount >= count;
    }
}
