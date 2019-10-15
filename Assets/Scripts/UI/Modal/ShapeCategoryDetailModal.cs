using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeCategoryDetailModal : M8.ModalController, M8.IModalPush {

    public const string parmShapeCategoryData = "dat"; //ShapeCategoryData

    public ShapeCategoryInfoWidget categoryWidget;

    void M8.IModalPush.Push(M8.GenericParams parms) {
        ShapeCategoryData dat = null;

        if(parms != null) {
            if(parms.ContainsKey(parmShapeCategoryData))
                dat = parms.GetValue<ShapeCategoryData>(parmShapeCategoryData);
        }

        if(dat) {
            categoryWidget.Setup(dat);
        }
    }
}