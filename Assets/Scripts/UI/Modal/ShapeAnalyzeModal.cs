using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeAnalyzeModal : M8.ModalController, M8.IModalPush, M8.IModalPop {
    public const string parmUseSolid = "useSolid";
    public const string parmMeasureDisplayFlags = "measureDisplayFlags"; //MeasureDisplayFlag mask
    public const string parmShapeProfile = "shape"; //ShapeProfile
    public const string parmShapes = "shapes"; //ShapeCategoryData[]

    [System.Flags]
    public enum MeasureDisplayFlag {
        None,
        Angle = 0x1,
        Length = 0x2,
    }

    [Header("Display")]
    public Shapes2D.Shape shapeSolid;
    public Shapes2D.Shape shapeOutline;
    public Transform shapeMeasureRoot;
    public Transform shapeMeasureDetailRoot;

    public GameObject categoryGO;
    public Transform categoryContainer;

    public AnimatorEnterExit categoryPickBase;
    public Transform categoryPickContainer;

    public AnimatorEnterExit scoreBase;
    public M8.UI.Texts.TextCounter scoreCounter;

    public AnimatorEnterExit nextBase;

    private ShapeProfile mShapeProfile;
    private M8.CacheList<ShapeCategoryData> mShapeCategories; //categories that fit for shape

    public void Next() {

    }

    void M8.IModalPop.Pop() {
        DefaultDisplay();
    }

    void M8.IModalPush.Push(M8.GenericParams parms) {

        DefaultDisplay();
    }

    private void DefaultDisplay() {
        categoryPickBase.gameObject.SetActive(false);
        scoreBase.gameObject.SetActive(false);
        nextBase.gameObject.SetActive(false);
    }
}
