using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeCategoryModal : M8.ModalController, M8.IModalPush, M8.IModalActive {
    public const string parmShapes = "shapes"; //ShapeCategoryData[]

    [Header("Data")]
    public string speakGroup = "category";

    [Header("Display")]
    public ShapeCategoryInfoWidget categoryCurrent;
    public ShapeCategoryInfoWidget categoryNext;

    public Transform categoryPrevAnchor;
    public Transform categoryNextAnchor;

    public Button prevButton;
    public Button nextButton;
    public Button exitButton;

    public GameObject exitGO;

    [Header("Animation")]
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeCategoryNext;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeCategoryPrev;

    private ShapeCategoryData[] mShapeCategories;
    private int mCurShapeCategoryIndex;

    void M8.IModalActive.SetActive(bool aActive) {
        if(aActive) {
            //speak current category
            SpeakCurrent();
        }
    }

    void M8.IModalPush.Push(M8.GenericParams parms) {
        if(parms != null) {
            if(parms.ContainsKey(parmShapes))
                mShapeCategories = parms.GetValue<ShapeCategoryData[]>(parmShapes);
        }

        mCurShapeCategoryIndex = 0;

        prevButton.interactable = false;
        nextButton.interactable = false;

        exitGO.SetActive(false);

        ApplyCurrent();
    }

    void Awake() {
        prevButton.onClick.AddListener(OnPrev);
        nextButton.onClick.AddListener(OnNext);
        exitButton.onClick.AddListener(OnExit);
    }

    void OnPrev() {
        prevButton.interactable = false;
        nextButton.interactable = false;

        StartCoroutine(DoPrev());
    }

    void OnNext() {
        prevButton.interactable = false;
        nextButton.interactable = false;

        StartCoroutine(DoNext());
    }

    void OnExit() {
        prevButton.interactable = false;
        nextButton.interactable = false;

        LoLManager.instance.StopSpeakQueue();

        Close();
    }

    private void ApplyCurrent() {
        categoryCurrent.gameObject.SetActive(true);
        categoryCurrent.Setup(mShapeCategories[mCurShapeCategoryIndex]);

        categoryNext.gameObject.SetActive(false);

        prevButton.interactable = mCurShapeCategoryIndex > 0 && mShapeCategories.Length > 1;
        nextButton.interactable = mCurShapeCategoryIndex < mShapeCategories.Length - 1;
    }

    private void SpeakCurrent() {
        LoLManager.instance.StopSpeakQueue();

        var shapeCategory = mShapeCategories[mCurShapeCategoryIndex];

        var ind = 0;

        LoLManager.instance.SpeakTextQueue(shapeCategory.textRef, speakGroup, ind);

        for(int i = 0; i < shapeCategory.attributes.Length; i++) {
            ind++;

            var attr = shapeCategory.attributes[i];

            LoLManager.instance.SpeakTextQueue(attr.textRef, speakGroup, ind);
        }
    }

    IEnumerator DoPrev() {
        var prevShapeCat = mShapeCategories[mCurShapeCategoryIndex - 1];
        categoryNext.Setup(prevShapeCat);
        categoryNext.transform.position = categoryPrevAnchor.position;
        categoryNext.gameObject.SetActive(true);

        if(animator && !string.IsNullOrEmpty(takeCategoryPrev)) {
            yield return animator.PlayWait(takeCategoryPrev);

            animator.ResetTake(takeCategoryPrev);
        }

        mCurShapeCategoryIndex--;

        ApplyCurrent();
        SpeakCurrent();
    }

    IEnumerator DoNext() {
        var nextShapeCat = mShapeCategories[mCurShapeCategoryIndex + 1];
        categoryNext.Setup(nextShapeCat);
        categoryNext.transform.position = categoryNextAnchor.position;
        categoryNext.gameObject.SetActive(true);

        if(animator && !string.IsNullOrEmpty(takeCategoryNext)) {
            yield return animator.PlayWait(takeCategoryNext);

            animator.ResetTake(takeCategoryNext);
        }

        mCurShapeCategoryIndex++;

        ApplyCurrent();
        SpeakCurrent();

        //show exit?
        if(mCurShapeCategoryIndex == mShapeCategories.Length - 1) {
            exitGO.SetActive(true);
        }
    }
}
