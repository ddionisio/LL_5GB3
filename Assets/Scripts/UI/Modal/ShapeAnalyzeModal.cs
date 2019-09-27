using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public enum Mode {
        None,
        PickCategories,
        Score
    }

    [Header("Templates")]
    public Transform cacheRoot;

    public ShapeAnalyzeCategoryWidget categoryWidgetTemplate;

    [Header("Config")]
    public float evaluateDelay = 0.5f; //delay per category

    [Header("Display")]
    public Text descText;
    public Shapes2D.Shape shapeSolid;
    public Shapes2D.Shape shapeOutline;
    public Transform shapeMeasureRoot; //put angle and side measure display here
    public Transform shapeMeasureDetailRoot; //put angle degree and side values here

    public RectTransform categoryArea;
    public Transform categoryContainer;
    public GameObject categoryHighlightGO;

    public AnimatorEnterExit categoryPickBase;
    public Transform categoryPickContainer;

    public AnimatorEnterExit scoreBase;
    public M8.UI.Texts.TextCounter scoreCounter;

    public AnimatorEnterExit nextBase;

    private bool mShapeUseSolid;
    private MeasureDisplayFlag mMeasureDisplayFlags;
    private ShapeProfile mShapeProfile;

    private const int shapeCategoryCapacity = 8;
    private const int shapeAttributeCapacity = 32;

    private M8.CacheList<ShapeCategoryData> mShapeCategories = new M8.CacheList<ShapeCategoryData>(shapeCategoryCapacity); //categories that fit for shape
    private M8.CacheList<ShapeCategoryData> mShapeCategoriesPlaced = new M8.CacheList<ShapeCategoryData>(shapeCategoryCapacity); //categories that were placed
    private M8.CacheList<ShapeAttributeData> mShapeAttributes = new M8.CacheList<ShapeAttributeData>(shapeAttributeCapacity);
    
    private M8.CacheList<ShapeAnalyzeCategoryWidget> mShapeCategoryWidgetCache = new M8.CacheList<ShapeAnalyzeCategoryWidget>(shapeCategoryCapacity);

    private M8.CacheList<ShapeAnalyzeCategoryWidget> mShapeCategoryWidgetActivePlaced = new M8.CacheList<ShapeAnalyzeCategoryWidget>(shapeCategoryCapacity);
    private M8.CacheList<ShapeAnalyzeCategoryWidget> mShapeCategoryWidgetActivePicks = new M8.CacheList<ShapeAnalyzeCategoryWidget>(shapeCategoryCapacity);

    private System.Text.StringBuilder mDescStrBuff = new System.Text.StringBuilder();

    private Mode mCurMode = Mode.None;

    private Coroutine mRout;

    public void Next() {
        switch(mCurMode) {
            case Mode.PickCategories:
                mCurMode = Mode.Score;
                mRout = StartCoroutine(DoScore());
                break;

            case Mode.Score:
                mCurMode = Mode.None;

                scoreBase.PlayExit();
                nextBase.PlayExit();

                Close();
                break;
        }
    }

    void M8.IModalPop.Pop() {
        CancelRout();

        DefaultActiveDisplay();
    }

    void M8.IModalPush.Push(M8.GenericParams parms) {

        mShapeUseSolid = false;
        mMeasureDisplayFlags = MeasureDisplayFlag.None;
        mShapeProfile = null;

        mShapeCategories.Clear();
        mShapeAttributes.Clear();

        //clear placements
        ClearCategoryPicks();
        ClearCategoryPlaced();

        ShapeCategoryData[] shapes = null;

        if(parms != null) {
            if(parms.ContainsKey(parmUseSolid))
                mShapeUseSolid = parms.GetValue<bool>(parmUseSolid);

            if(parms.ContainsKey(parmMeasureDisplayFlags))
                mMeasureDisplayFlags = parms.GetValue<MeasureDisplayFlag>(parmMeasureDisplayFlags);

            if(parms.ContainsKey(parmShapeProfile))
                mShapeProfile = parms.GetValue<ShapeProfile>(parmShapeProfile);

            if(parms.ContainsKey(parmShapes))
                shapes = parms.GetValue<ShapeCategoryData[]>(parmShapes);
        }

        //determine which categories fit the shape profile
        if(mShapeProfile != null && shapes != null) {
            for(int i = 0; i < shapes.Length; i++) {
                if(shapes[i].Evaluate(mShapeProfile))
                    mShapeCategories.Add(shapes[i]);
            }
        }

        //generate categories
        if(shapes != null) {
            for(int i = 0; i < shapes.Length; i++) {
                var category = shapes[i];

                //add category to pick area
                if(mShapeCategoryWidgetCache.Count > 0) {
                    var widget = mShapeCategoryWidgetCache.RemoveLast();

                    widget.Setup(category);
                    widget.isInputEnabled = true;

                    widget.transform.SetParent(categoryPickContainer, false);

                    mShapeCategoryWidgetActivePicks.Add(widget);
                }
            }
        }

        //fill up attributes
        for(int i = 0; i < mShapeCategories.Count; i++) {
            var category = mShapeCategories[i];

            for(int j = 0; j < category.attributes.Length; j++) {
                var attr = category.attributes[j];

                if(!mShapeAttributes.Exists(attr))
                    mShapeAttributes.Add(attr);
            }
        }

        //generate description
        mDescStrBuff.Clear();

        for(int i = 0; i < mShapeAttributes.Count; i++) {
            var attr = mShapeAttributes[i];

            mDescStrBuff.Append("· ");
            mDescStrBuff.Append(M8.Localize.Get(attr.textRef));

            if(i < mShapeAttributes.Count - 1)
                mDescStrBuff.Append('\n');
        }

        descText.text = mDescStrBuff.ToString();

        DefaultActiveDisplay();

        ApplyShape();

        categoryPickBase.gameObject.SetActive(true);
        categoryPickBase.PlayEnter();

        mCurMode = Mode.PickCategories;
    }

    void Awake() {
        //generate category widget cache        
        for(int i = 0; i <shapeCategoryCapacity; i++) {
            var inst = Instantiate(categoryWidgetTemplate, cacheRoot);

            inst.dragCallback += OnCategoryDrag;
            inst.dragEndCallback += OnCategoryDragEnd;
            inst.dragCancelCallback += OnCategoryDragCancel;

            mShapeCategoryWidgetCache.Add(inst);
        }
        categoryWidgetTemplate.gameObject.SetActive(false);

        //generate angle display cache

        //generate measure length display cache

        //generate text cache
    }

    void OnCategoryDrag(ShapeAnalyzeCategoryWidget widget, PointerEventData eventData) {
        //check if widget is within category panel
        bool isInCategoryPanel = IsInCategoryPanel(widget.dragRoot);

        categoryHighlightGO.SetActive(isInCategoryPanel);
    }

    void OnCategoryDragEnd(ShapeAnalyzeCategoryWidget widget, PointerEventData eventData) {
        categoryHighlightGO.SetActive(false);

        bool isInCategoryPanel = IsInCategoryPanel(widget.dragRoot);
        if(isInCategoryPanel) {
            //add to placed
            if(!mShapeCategoryWidgetActivePlaced.Exists(widget)) {
                mShapeCategoryWidgetActivePicks.Remove(widget);

                widget.transform.SetParent(categoryContainer, false);

                mShapeCategoryWidgetActivePlaced.Add(widget);

                //show next if first time
                if(mShapeCategoryWidgetActivePlaced.Count == 1) {                    
                    if(!nextBase.gameObject.activeSelf || !nextBase.isEntering) {
                        CancelRout();
                        nextBase.gameObject.SetActive(true);
                        nextBase.PlayEnter();
                    }
                }
            }
        }
        else {
            //add to picks
            if(!mShapeCategoryWidgetActivePicks.Exists(widget)) {
                mShapeCategoryWidgetActivePlaced.Remove(widget);

                widget.transform.SetParent(categoryPickContainer, false);

                mShapeCategoryWidgetActivePicks.Add(widget);

                //hide next if no categories picked
                if(mShapeCategoryWidgetActivePlaced.Count == 0) {
                    if(nextBase.gameObject.activeSelf && !nextBase.isExiting) {
                        CancelRout();
                        mRout = StartCoroutine(DoNextHide());
                    }
                }
            }
        }
    }

    void OnCategoryDragCancel(ShapeAnalyzeCategoryWidget widget) {
        categoryHighlightGO.SetActive(false);
    }

    IEnumerator DoScore() {
        //lock inputs
        for(int i = 0; i < mShapeCategoryWidgetActivePicks.Count; i++)
            mShapeCategoryWidgetActivePicks[i].isInputEnabled = false;

        for(int i = 0; i < mShapeCategoryWidgetActivePlaced.Count; i++)
            mShapeCategoryWidgetActivePlaced[i].isInputEnabled = false;

        //hide next
        yield return nextBase.PlayExitWait();
        nextBase.gameObject.SetActive(false);        

        //hide pick category
        yield return categoryPickBase.PlayExitWait();
        categoryPickBase.gameObject.SetActive(false);

        //clear out picks
        ClearCategoryPicks();

        int correctCount = 0;
        int wrongCount = 0;

        var wait = new WaitForSeconds(evaluateDelay);

        mShapeCategoriesPlaced.Clear();

        //evaluate placed categories
        for(int i = 0; i < mShapeCategoryWidgetActivePlaced.Count; i++) {
            var widget = mShapeCategoryWidgetActivePlaced[i];
            var category = widget.data;

            //check if matched
            if(mShapeCategories.Exists(category)) {
                widget.correctGO.SetActive(true);
                correctCount++;

                mShapeCategoriesPlaced.Add(category);
            }
            else {
                widget.errorGO.SetActive(true);
                wrongCount++;
            }

            yield return wait;
        }

        //add missed categories
        for(int i = 0; i < mShapeCategories.Count; i++) {
            var category = mShapeCategories[i];
            if(!mShapeCategoriesPlaced.Exists(category)) {
                if(mShapeCategoryWidgetCache.Count > 0) {
                    var widget = mShapeCategoryWidgetCache.RemoveLast();

                    widget.Setup(category);
                    widget.missGO.SetActive(true);

                    widget.transform.SetParent(categoryContainer, false);

                    mShapeCategoryWidgetActivePlaced.Add(widget);

                    yield return wait;
                }
            }
        }

        //compute score
        float score = Mathf.Clamp01((float)correctCount / mShapeCategories.Count) * GameData.instance.scoreShape;
        score -= wrongCount * GameData.instance.scorePenalty;
        if(score < 0f)
            score = 0f;

        if(mShapeProfile) {
            mShapeProfile.score = Mathf.RoundToInt(score);

            scoreCounter.count = mShapeProfile.score;
        }

        //show score
        scoreBase.gameObject.SetActive(true);
        yield return scoreBase.PlayEnterWait();

        //show next
        nextBase.gameObject.SetActive(true);
        nextBase.PlayEnter();

        mRout = null;
    }

    IEnumerator DoNextHide() {
        yield return nextBase.PlayExitWait();

        nextBase.gameObject.SetActive(false);

        mRout = null;
    }

    private void CancelRout() {
        if(mRout != null) {
            StopCoroutine(mRout);
            mRout = null;
        }
    }

    private bool IsInCategoryPanel(RectTransform src) {
        var panelRect = categoryArea.rect;
        panelRect.center = categoryArea.TransformPoint(panelRect.center);

        var srcRect = src.rect;
        srcRect.center = src.TransformPoint(srcRect.center);

        return srcRect.Overlaps(panelRect);
    }

    private void ApplyShape() {
        shapeSolid.gameObject.SetActive(mShapeUseSolid);
        shapeOutline.gameObject.SetActive(!mShapeUseSolid);

        //apply
    }

    private void ClearCategoryPlaced() {
        for(int i = 0; i < mShapeCategoryWidgetActivePlaced.Count; i++) {
            var widget = mShapeCategoryWidgetActivePlaced[i];
            widget.transform.SetParent(cacheRoot);

            mShapeCategoryWidgetCache.Add(widget);
        }

        mShapeCategoryWidgetActivePlaced.Clear();
    }

    private void ClearCategoryPicks() {
        for(int i = 0; i < mShapeCategoryWidgetActivePicks.Count; i++) {
            var widget = mShapeCategoryWidgetActivePicks[i];
            widget.transform.SetParent(cacheRoot);

            mShapeCategoryWidgetCache.Add(widget);
        }

        mShapeCategoryWidgetActivePicks.Clear();
    }

    private void DefaultActiveDisplay() {
        categoryHighlightGO.SetActive(false);
        categoryPickBase.gameObject.SetActive(false);
        scoreBase.gameObject.SetActive(false);
        nextBase.gameObject.SetActive(false);
    }
}
