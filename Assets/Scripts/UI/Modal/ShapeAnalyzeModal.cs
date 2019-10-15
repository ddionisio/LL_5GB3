using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShapeAnalyzeModal : M8.ModalController, M8.IModalPush, M8.IModalPop, M8.IModalActive {
    public const string parmUseSolid = "useSolid";
    public const string parmIsDragInstruct = "dragInstruct";
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

    public Image angleTemplate;
    public RectTransform lengthTemplate;
    public Text measureTextTemplate;

    public ShapeAnalyzeCategoryWidget categoryWidgetTemplate;

    [Header("Config")]
    public float evaluateDelay = 0.5f; //delay per category
    public float shapeSizeMax;

    public float angleTextOfs = 24f;
    public float measureLengthDisplayOfs = 1f;
    public float lengthTextOfs = 30f;

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

    [Header("Drag Instruct")]
    public DragToGuideWidget dragGuide;
    public GameObject dragInstructGO;

    [Header("SFX")]
    [M8.SoundPlaylist]
    public string sfxEnter;
    [M8.SoundPlaylist]
    public string sfxCorrect;
    [M8.SoundPlaylist]
    public string sfxWrong;
    [M8.SoundPlaylist]
    public string sfxMiss;

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

    private const int measureDisplayCapacity = 4;

    private M8.CacheList<Image> mAngleWidgetActives = new M8.CacheList<Image>(measureDisplayCapacity);
    private M8.CacheList<Image> mAngleWidgetCache = new M8.CacheList<Image>(measureDisplayCapacity);

    private M8.CacheList<RectTransform> mLengthWidgetActives = new M8.CacheList<RectTransform>(measureDisplayCapacity);
    private M8.CacheList<RectTransform> mLengthWidgetCache = new M8.CacheList<RectTransform>(measureDisplayCapacity);

    private M8.CacheList<Text> mTextActives = new M8.CacheList<Text>(measureDisplayCapacity*2);
    private M8.CacheList<Text> mTextCache = new M8.CacheList<Text>(measureDisplayCapacity*2);

    private System.Text.StringBuilder mDescStrBuff = new System.Text.StringBuilder();

    private Mode mCurMode = Mode.None;

    private bool mIsDragInstruct;
    private ShapeAnalyzeCategoryWidget mDragInstructTarget;
    private Coroutine mDragInstructRout;
    private bool mIsDragInstructApplied;

    private Coroutine mRout;

    public void Next() {
        switch(mCurMode) {
            case Mode.PickCategories:
                mCurMode = Mode.Score;
                ApplyDragInstruct();
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

    void M8.IModalActive.SetActive(bool aActive) {
        if(aActive) {
            ApplyDragInstruct();
        }
    }

    void M8.IModalPop.Pop() {
        CancelRout();

        if(mIsDragInstruct) {
            if(mDragInstructRout != null) {
                StopCoroutine(mDragInstructRout);
                mDragInstructRout = null;
            }

            dragGuide.Hide();

            mIsDragInstructApplied = true;
        }

        DefaultActiveDisplay();

        ClearCategoryPicks();
        ClearMeasureDisplays();
        ClearCategoryPlaced();
    }

    void M8.IModalPush.Push(M8.GenericParams parms) {

        mShapeUseSolid = false;
        mMeasureDisplayFlags = MeasureDisplayFlag.None;
        mShapeProfile = null;

        mIsDragInstruct = false;

        mShapeCategories.Clear();
        mShapeAttributes.Clear();

        //clear placements
        ClearCategoryPicks();
        ClearMeasureDisplays();
        ClearCategoryPlaced();

        ShapeCategoryData[] shapes = null;

        if(parms != null) {
            if(parms.ContainsKey(parmUseSolid))
                mShapeUseSolid = parms.GetValue<bool>(parmUseSolid);

            if(parms.ContainsKey(parmMeasureDisplayFlags))
                mMeasureDisplayFlags = parms.GetValue<MeasureDisplayFlag>(parmMeasureDisplayFlags);

            if(!mIsDragInstructApplied) { //only show drag instruction once
                if(parms.ContainsKey(parmIsDragInstruct))
                    mIsDragInstruct = parms.GetValue<bool>(parmIsDragInstruct);
            }

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
                    widget.isDragEnabled = true;

                    widget.transform.SetParent(categoryPickContainer, false);

                    mShapeCategoryWidgetActivePicks.Add(widget);

                    //pick a target for drag instruct
                    if(mIsDragInstruct && !mDragInstructTarget) {
                        if(mShapeCategories.Exists(category))
                            mDragInstructTarget = widget;
                    }
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

        M8.SoundPlaylist.instance.Play(sfxEnter, false);
    }

    void Awake() {
        shapeSolid.Configure();
        shapeOutline.Configure();

        //generate category widget cache        
        for(int i = 0; i < mShapeCategoryWidgetCache.Capacity; i++) {
            var inst = Instantiate(categoryWidgetTemplate, cacheRoot);

            inst.dragCallback += OnCategoryDrag;
            inst.dragEndCallback += OnCategoryDragEnd;
            inst.dragCancelCallback += OnCategoryDragCancel;

            mShapeCategoryWidgetCache.Add(inst);
        }
        categoryWidgetTemplate.gameObject.SetActive(false);

        //generate measure display caches
        for(int i = 0; i < mAngleWidgetCache.Capacity; i++) {
            var angleWidget = Instantiate(angleTemplate, cacheRoot);
            mAngleWidgetCache.Add(angleWidget);
        }

        angleTemplate.gameObject.SetActive(false);

        //generate length display caches
        for(int i = 0; i < mLengthWidgetCache.Capacity; i++) {
            var itm = Instantiate(lengthTemplate, cacheRoot);
            mLengthWidgetCache.Add(itm);
        }

        lengthTemplate.gameObject.SetActive(false);

        //generate text cache
        for(int i = 0; i < mTextCache.Capacity; i++) {
            var itm = Instantiate(measureTextTemplate, cacheRoot);
            mTextCache.Add(itm);
        }

        measureTextTemplate.gameObject.SetActive(false);
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

        ApplyDragInstruct();
    }

    void OnCategoryDragCancel(ShapeAnalyzeCategoryWidget widget) {
        categoryHighlightGO.SetActive(false);

        if(mDragInstructTarget == widget)
            ApplyDragInstruct();
    }

    IEnumerator DoScore() {
        //lock inputs, enabled clicks on placed
        for(int i = 0; i < mShapeCategoryWidgetActivePicks.Count; i++)
            mShapeCategoryWidgetActivePicks[i].isDragEnabled = false;

        for(int i = 0; i < mShapeCategoryWidgetActivePlaced.Count; i++) {
            mShapeCategoryWidgetActivePlaced[i].isDragEnabled = false;
            mShapeCategoryWidgetActivePlaced[i].isClickEnabled = true;
        }

        //hide next
        yield return nextBase.PlayExitWait();
        nextBase.gameObject.SetActive(false);        

        //hide pick category
        yield return categoryPickBase.PlayExitWait();
        categoryPickBase.gameObject.SetActive(false);

        //clear out picks
        ClearCategoryPicks();

        //ClearMeasureDisplays();

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

                M8.SoundPlaylist.instance.Play(sfxCorrect, false);
            }
            else {
                widget.errorGO.SetActive(true);
                wrongCount++;

                M8.SoundPlaylist.instance.Play(sfxWrong, false);
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
                    widget.isClickEnabled = true;
                    widget.missGO.SetActive(true);

                    widget.transform.SetParent(categoryContainer, false);

                    mShapeCategoryWidgetActivePlaced.Add(widget);

                    M8.SoundPlaylist.instance.Play(sfxMiss, false);

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

        M8.SoundPlaylist.instance.Play(sfxEnter, false);

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
        if(!mShapeProfile)
            return;

        shapeSolid.gameObject.SetActive(mShapeUseSolid);
        shapeOutline.gameObject.SetActive(!mShapeUseSolid);

        var shape = mShapeUseSolid ? shapeSolid : shapeOutline;

        //set the proper aspect ratio
        var s = mShapeProfile.scale;
        if(s.x > s.y) {
            var hRatio = s.y / s.x;

            var rTrans = shape.transform as RectTransform;
            var size = rTrans.sizeDelta;
            size.x = shapeSizeMax;
            size.y = shapeSizeMax * hRatio;
            rTrans.sizeDelta = size;
        }
        else {
            var vRatio = s.x / s.y;

            var rTrans = shape.transform as RectTransform;
            var size = rTrans.sizeDelta;
            size.x = shapeSizeMax * vRatio;
            size.y = shapeSizeMax;
            rTrans.sizeDelta = size;
        }

        //copy points
        var settings = shape.settings;

        settings.polyVertices = mShapeProfile.shape.settings.polyVertices;

        if(mMeasureDisplayFlags != MeasureDisplayFlag.None) {
            var vtxPts = shape.GetPolygonWorldVertices();

            //show angle displays
            if((mMeasureDisplayFlags & MeasureDisplayFlag.Angle) != MeasureDisplayFlag.None) {
                var angles = mShapeProfile.angles;
                var dirs = mShapeProfile.sideDirs;

                for(int i = 0; i < angles.Length; i++) {
                    Vector2 pt0 = i > 0 ? vtxPts[i - 1] : vtxPts[vtxPts.Length - 1];
                    Vector2 pt1 = vtxPts[i];
                    Vector2 pt2 = i < vtxPts.Length - 1 ? vtxPts[i + 1] : vtxPts[0];

                    Vector2 dir0 = (pt0 - pt1).normalized;
                    Vector2 dir1 = dirs[i];

                    //angle
                    if(mAngleWidgetCache.Count > 0) {
                        var angleWidget = mAngleWidgetCache.RemoveLast();
                        angleWidget.transform.SetParent(shapeMeasureRoot, false);
                        mAngleWidgetActives.Add(angleWidget);
                                                
                        angleWidget.transform.position = vtxPts[i];
                        angleWidget.transform.up = dir1;

                        var a = angleWidget.transform.eulerAngles; //prevent gimbal lock
                        a.x = a.y = 0f;
                        angleWidget.transform.eulerAngles = a;

                        angleWidget.fillAmount = angles[i] / 360f;
                    }

                    //text
                    if(mTextCache.Count > 0) {
                        var textWidget = mTextCache.RemoveLast();
                        textWidget.transform.SetParent(shapeMeasureDetailRoot, false);
                        mTextActives.Add(textWidget);

                        textWidget.text = Mathf.RoundToInt(angles[i]).ToString() + '°';
                        textWidget.transform.position = pt1 + Vector2.Lerp(dir0, dir1, 0.5f).normalized * angleTextOfs;
                    }
                }
            }

            //show length displays
            if((mMeasureDisplayFlags & MeasureDisplayFlag.Length) != MeasureDisplayFlag.None) {
                var lens = mShapeProfile.sideLengths;
                var dirs = mShapeProfile.sideDirs;

                for(int i = 0; i < lens.Length; i++) {
                    Vector2 pt1 = vtxPts[i];
                    Vector2 pt2 = i < vtxPts.Length - 1 ? vtxPts[i + 1] : vtxPts[0];

                    var dir = dirs[i];

                    var up = new Vector2(dir.y, -dir.x);

                    var midPt = Vector2.Lerp(pt1, pt2, 0.5f);

                    //length
                    if(mLengthWidgetCache.Count > 0) {
                        var lenRect = mLengthWidgetCache.RemoveLast();
                        lenRect.SetParent(shapeMeasureRoot, false);
                        mLengthWidgetActives.Add(lenRect);

                        lenRect.up = up;

                        var a = lenRect.eulerAngles; //prevent gimbal lock
                        a.x = a.y = 0f;
                        lenRect.eulerAngles = a;

                        lenRect.position = midPt + up * measureLengthDisplayOfs;

                        var size = lenRect.sizeDelta;
                        size.x = (pt2 - pt1).magnitude;
                        lenRect.sizeDelta = size;
                    }

                    if(mTextCache.Count > 0) {
                        var textWidget = mTextCache.RemoveLast();
                        textWidget.transform.SetParent(shapeMeasureDetailRoot, false);
                        mTextActives.Add(textWidget);

                        textWidget.text = Mathf.RoundToInt(lens[i]).ToString() + GameData.instance.measureLengthType;
                        textWidget.transform.position = midPt + up * lengthTextOfs;
                    }
                }
            }
        }
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

    private void ClearMeasureDisplays() {
        //clear angles
        for(int i = 0; i < mAngleWidgetActives.Count; i++) {
            var widget = mAngleWidgetActives[i];
            widget.transform.SetParent(cacheRoot);
            mAngleWidgetCache.Add(widget);
        }

        mAngleWidgetActives.Clear();

        //clear lengths
        for(int i = 0; i < mLengthWidgetActives.Count; i++) {
            var itm = mLengthWidgetActives[i];
            itm.transform.SetParent(cacheRoot);
            mLengthWidgetCache.Add(itm);
        }

        mLengthWidgetActives.Clear();

        //clear texts
        for(int i = 0; i < mTextActives.Count; i++) {
            var itm = mTextActives[i];
            itm.transform.SetParent(cacheRoot);
            mTextCache.Add(itm);
        }

        mTextActives.Clear();
    }

    private void DefaultActiveDisplay() {
        categoryHighlightGO.SetActive(false);
        categoryPickBase.gameObject.SetActive(false);
        scoreBase.gameObject.SetActive(false);
        nextBase.gameObject.SetActive(false);
    }

    private void ApplyDragInstruct() {
        if(mIsDragInstruct) {
            if(mDragInstructRout != null) {
                StopCoroutine(mDragInstructRout);
                mDragInstructRout = null;
            }

            if(mCurMode == Mode.PickCategories) {
                mDragInstructRout = StartCoroutine(DoDragInstruct());
            }
            else {
                dragGuide.Hide();
                dragInstructGO.SetActive(false);
            }
        }
    }

    IEnumerator DoDragInstruct() {
        if(mShapeCategoryWidgetActivePicks.Exists(mDragInstructTarget)) {
            yield return new WaitForSeconds(0.1f); //slight delay to allow UIs to position properly

            if(dragGuide.isActive)
                dragGuide.UpdatePositions(mDragInstructTarget.transform.position, categoryContainer.position);
            else
                dragGuide.Show(false, mDragInstructTarget.transform.position, categoryContainer.position);

            dragInstructGO.SetActive(true);
        }
        else if(mShapeCategoryWidgetActivePlaced.Exists(mDragInstructTarget)) {
            dragGuide.Hide();

            dragInstructGO.SetActive(false);
        }

        mDragInstructRout = null;
    }
}
