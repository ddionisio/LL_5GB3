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
                    EndDrag();

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

    private Vector2 mDragMoveTo;
    private Vector2 mMoveVel;

    public void Setup(ShapeCategoryData dat) {
        data = dat;

        mIsDragging = false;
        mIsInputEnabled = false;

        dragRoot.SetParent(transform, false);
        dragRoot.localPosition = Vector3.zero;

        titleText.text = M8.Localize.Get(dat.textRef);

        errorGO.SetActive(false);
        missGO.SetActive(false);
        correctGO.SetActive(false);
    }

    void OnApplicationFocus(bool focus) {
        if(!focus) {
            if(mIsDragging) {
                EndDrag();

                dragCancelCallback?.Invoke(this);
            }
        }
    }

    void Update() {
        if(mIsDragging) {
            Vector2 curPos = dragRoot.position;
            dragRoot.position = Vector2.SmoothDamp(curPos, mDragMoveTo, ref mMoveVel, moveDelay);
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
                
        dragEndCallback?.Invoke(this, eventData);

        EndDrag();
    }

    private void EndDrag() {
        mIsDragging = false;

        dragRoot.SetParent(transform, false);
        dragRoot.localPosition = Vector3.zero;
    }
}
