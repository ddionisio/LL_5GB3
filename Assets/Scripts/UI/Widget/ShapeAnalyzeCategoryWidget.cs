using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShapeAnalyzeCategoryWidget : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [Header("Config")]
    public float moveDelay = 0.15f;

    [Header("Display")]
    public Text titleText;
    public GameObject errorGO;
    public GameObject missGO;
    public GameObject correctGO;

    public RectTransform dragRoot;
    public Transform dragAreaRoot;

    public bool isInputEnabled {
        get { return mIsInputEnabled; }
        set {
            if(mIsInputEnabled != value) {
                mIsInputEnabled = value;

                if(!mIsInputEnabled && mIsDragging) {
                    EndDrag(true);

                    dragCancelCallback?.Invoke(this);
                }
            }
        }
    }

    public ShapeCategoryData data { get; private set; }

    public event System.Action<ShapeAnalyzeCategoryWidget, PointerEventData> dragCallback;
    public event System.Action<ShapeAnalyzeCategoryWidget, PointerEventData> dragEndCallback;
    public event System.Action<ShapeAnalyzeCategoryWidget> dragCancelCallback;

    private bool mIsInputEnabled;

    private bool mIsDragging;
    private bool mIsMoving;

    private Vector2 mDragMoveTo;
    private Vector2 mMoveVel;

    public void Setup(ShapeCategoryData dat) {
        data = dat;

        mIsDragging = false;
        mIsInputEnabled = false;

        titleText.text = M8.Localize.Get(dat.textRef);

        errorGO.SetActive(false);
        missGO.SetActive(false);
        correctGO.SetActive(false);
    }

    void OnApplicationFocus(bool focus) {
        if(!focus) {
            if(mIsDragging) {
                EndDrag(false);

                dragCancelCallback?.Invoke(this);
            }
        }
    }

    void OnDisable() {
        EndDrag(false);
    }

    void Update() {
        if(mIsDragging) {
            Vector2 curPos = dragRoot.position;
            dragRoot.position = Vector2.SmoothDamp(curPos, mDragMoveTo, ref mMoveVel, moveDelay);
        }
        else if(mIsMoving) {
            Vector2 curPos = dragRoot.position;
            Vector2 toPos = transform.position;

            if(curPos == toPos) {
                mIsMoving = false;

                dragRoot.SetParent(transform, false);
                dragRoot.localPosition = Vector3.zero;
            }
            else
                dragRoot.position = Vector2.SmoothDamp(curPos, toPos, ref mMoveVel, moveDelay);
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        if(!mIsInputEnabled)
            return;

        mIsDragging = true;
        mMoveVel = Vector2.zero;

        mDragMoveTo = eventData.position;

        dragRoot.SetParent(dragAreaRoot, true);
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

        mDragMoveTo = eventData.position;

        dragCallback?.Invoke(this, eventData);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

        EndDrag(true);

        dragEndCallback?.Invoke(this, eventData);
    }

    private void EndDrag(bool moveBack) {
        mIsDragging = false;

        if(moveBack) {
            mIsMoving = true; //put back to origin, then revert parent afterwards
            mMoveVel = Vector2.zero;
        }
        else {
            mIsMoving = false;

            dragRoot.SetParent(transform, false);
            dragRoot.localPosition = Vector3.zero;
        }
    }
}
