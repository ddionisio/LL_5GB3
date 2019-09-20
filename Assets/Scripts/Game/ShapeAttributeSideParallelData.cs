using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// At least X number of sides opposite are parallel
/// </summary>
[CreateAssetMenu(fileName = "shapeAttributeSideParallel", menuName = "Game/Shape Attribute/Side Parallel")]
public class ShapeAttributeSideParallelData : ShapeAttributeData {

    public int count = 1; //at least 1

    public override bool Evaluate(ShapeProfile shapeProfile) {
        int parallelCount = 0;

        var sideDirs = shapeProfile.sideDirs;
        int hCount = sideDirs.Length / 2;

        //compare opposite sides
        for(int i = 0; i < hCount; i++) {
            var dirA = sideDirs[i];
            var dirB = sideDirs[i + hCount];

            var angle = Vector2.Angle(dirA, dirB);
            var iAngle = Mathf.RoundToInt(angle);
            if(iAngle == 0 || iAngle == 180)
                parallelCount++;
        }

        return parallelCount >= count;
    }
}
