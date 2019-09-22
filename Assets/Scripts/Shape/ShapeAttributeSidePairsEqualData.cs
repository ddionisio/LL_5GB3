using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// At least X number of side pairs (adjacent) are equal.
/// </summary>
[CreateAssetMenu(fileName = "shapeAttributeSidePairsEqual", menuName = "Game/Shape Attribute/Side Pairs Equal")]
public class ShapeAttributeSidePairsEqualData : ShapeAttributeData {
    public int count = 2;

    private struct Pair {
        public int indA;
        public int indB;
    }

    private M8.CacheList<Pair> mPairs;

    public override bool Evaluate(ShapeProfile shapeProfile) {
        var equalCount = 0;

        var sides = shapeProfile.sideLengths;

        if(sides.Length % 2 != 0)
            return false;

        var hSideCount = sides.Length / 2;
        if(mPairs == null || mPairs.Count != hSideCount)
            mPairs = new M8.CacheList<Pair>(hSideCount);
        else
            mPairs.Clear();

        for(int i = 0; i < sides.Length; i++) {
            //make sure these are not already paired
            var indA = i;
            if(IsPaired(indA))
                continue;

            var indB = i < sides.Length - 1 ? i + 1 : 0;
            if(IsPaired(indB))
                continue;

            

            var sideLenA = Mathf.RoundToInt(sides[indA]);
            var sideLenB = Mathf.RoundToInt(sides[indB]);

            if(sideLenA == sideLenB) {
                mPairs.Add(new Pair { indA=indA, indB=indB });
                equalCount++;
            }
        }

        return equalCount >= count;
    }

    private bool IsPaired(int index) {
        for(int i = 0; i < mPairs.Count; i++) {
            var pair = mPairs[i];
            if(pair.indA == index || pair.indB == index)
                return true;
        }

        return false;
    }
}
