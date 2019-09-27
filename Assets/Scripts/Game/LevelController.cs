using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : GameModeController<LevelController> {
    [Header("Data")]
    public ShapeCategoryData[] shapeCategories;

    public bool analyzeIsShapeSolid;
    [M8.EnumMask]
    public ShapeAnalyzeModal.MeasureDisplayFlag analyzeMeasureDisplay;

    [Header("Interactions")]
    public Spaceship spaceship;
    public CursorCollector cursor;

    public ActionMode actionMode {
        get { return mActionMode; }
        set {
            if(mActionMode != value) {
                mActionMode = value;
                actionChangedCallback?.Invoke();
            }
        }
    }
                
    public ShapeProfile[] shapes {
        get {
            if(mShapes == null)
                InitShapes();

            return mShapes;
        }
    }

    public M8.CacheList<ShapeProfile> shapesCollected {
        get {
            if(mShapesCollected == null)
                InitShapes();

            return mShapesCollected;
        }
    }

    public ShapeProfile lastShapeCollected {
        get {
            if(mShapesCollected == null || mShapesCollected.Count == 0)
                return null;

            return mShapesCollected[mShapesCollected.Count - 1];
        }
    }

    public event System.Action actionChangedCallback;
    public event System.Action<ShapeProfile> shapeCollectedCallback;

    private ActionMode mActionMode = ActionMode.None;
    private ShapeProfile[] mShapes;
    private M8.CacheList<ShapeProfile> mShapesCollected;

    private M8.GenericParams mAnalyzeParms = new M8.GenericParams();

    public void CollectShape(ShapeProfile shapeProfile) {
        if(shapeProfile.isCollected) //fail-safe
            return;

        shapeProfile.Collect();

        shapesCollected.Add(shapeProfile);

        shapeCollectedCallback?.Invoke(shapeProfile);

        actionMode = ActionMode.Collect;
    }

    public void AnalyzeLastCollectedShape() {
        actionMode = ActionMode.Analyze;

        mAnalyzeParms[ShapeAnalyzeModal.parmUseSolid] = analyzeIsShapeSolid;
        mAnalyzeParms[ShapeAnalyzeModal.parmMeasureDisplayFlags] = analyzeMeasureDisplay;
        mAnalyzeParms[ShapeAnalyzeModal.parmShapeProfile] = lastShapeCollected;
        mAnalyzeParms[ShapeAnalyzeModal.parmShapes] = shapeCategories;

        M8.ModalManager.main.Open(GameData.instance.modalShapeAnalyze, mAnalyzeParms);
    }

    private void InitShapes() {
        var gos = GameObject.FindGameObjectsWithTag(GameData.instance.tagShape);

        var shapeList = new List<ShapeProfile>();

        for(int i = 0; i < gos.Length; i++) {
            var go = gos[i];
            var shape = go.GetComponent<ShapeProfile>();
            if(shape)
                shapeList.Add(shape);
        }

        mShapes = shapeList.ToArray();

        mShapesCollected = new M8.CacheList<ShapeProfile>(mShapes.Length);
    }
}
