using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : GameModeController<LevelController> {
    [Header("Data")]
    public ShapeCategoryData[] shapeCategories;

    public ShapeProfile[] shapes {
        get {
            if(mShapes == null) {
                var gos = GameObject.FindGameObjectsWithTag(GameData.instance.tagShape);

                var shapeList = new List<ShapeProfile>();

                for(int i = 0; i < gos.Length; i++) {
                    var go = gos[i];
                    var shape = go.GetComponent<ShapeProfile>();
                    if(shape)
                        shapeList.Add(shape);
                }

                mShapes = shapeList.ToArray();
            }

            return mShapes;
        }
    }

    private ShapeProfile[] mShapes;
}
