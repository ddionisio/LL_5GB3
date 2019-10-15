using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeOpenDetailProxy : MonoBehaviour {

    public ShapeCategoryData shape;

    private M8.GenericParams mParms = new M8.GenericParams();

    public void Invoke() {
        mParms[ShapeCategoryDetailModal.parmShapeCategoryData] = shape;

        M8.ModalManager.main.Open(GameData.instance.modalShapeCategoryDetail, mParms);
    }
}
