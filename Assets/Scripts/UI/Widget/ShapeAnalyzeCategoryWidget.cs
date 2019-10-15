using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShapeAnalyzeCategoryWidget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [Header("Config")]
    public float moveDelay = 0.15f;

    [Header("Display")]
    public Text titleText;
    public GameObject errorGO;
    public GameObject missGO;
    public GameObject correctGO;
    public GameObject highlightGO; //enabled if click is active

    public RectTransform dragRoot;
    public Transform dragAreaRoot;

    [Header("SFX")]
    [M8.SoundPlaylist]
    public string sfxDragBegin;
    [M8.SoundPlaylist]
    public string sfxDragEnd;

    public bool isDragEnabled {
        get { return mIsDragEnabled; }
        set {
            if(mIsDragEnabled != value) {
                mIsDragEnabled = value;

                if(!mIsDragEnabled && mIsDragging) {
                    EndDrag();

                    dragCancelCallback?.Invoke(this);
                }
            }
        }
    }

    public bool isClickEnabled {
        get { return mIsClickEnabled; }
        set {
            if(mIsClickEnabled != value) {
                mIsClickEnabled = value;
                RefreshHighlight();
            }
        }
    }

    public ShapeCategoryData data { get; private set; }

    public event System.Action<ShapeAnalyzeCategoryWidget, PointerEventData> dragCallback;
    public event System.Action<ShapeAnalyzeCategoryWidget, PointerEventData> dragEndCallback;
    public event System.Action<ShapeAnalyzeCategoryWidget> dragCancelCallback;

    private bool mIsDragEnabled;
    private bool mIsClickEnabled;
    private bool mIsDragging;
    private bool mIsHover;

    private Vector2 mDragMoveTo;
    private Vector2 mMoveVel;

    private M8.GenericParams mDetailParms = new M8.GenericParams();
        
    public void Setup(ShapeCategoryData dat) {
        data = dat;

        mDetailParms[ShapeCategoryDetailModal.parmShapeCategoryData] = dat;

        mIsDragging = false;
        mIsDragEnabled = false;
        mIsClickEnabled = false;
        mIsHover = false;

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

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
        mIsHover = true;
        RefreshHighlight();
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
        mIsHover = false;
        RefreshHighlight();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
        if(mIsClickEnabled) {
            M8.ModalManager.main.Open(GameData.instance.modalShapeCategoryDetail, mDetailParms);
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        if(!mIsDragEnabled)
            return;

        mIsDragging = true;
        mMoveVel = Vector2.zero;

        mDragMoveTo = eventData.position;

        RefreshHighlight();

        dragRoot.SetParent(dragAreaRoot, true);

        M8.SoundPlaylist.instance.Play(sfxDragBegin, false);
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

        M8.SoundPlaylist.instance.Play(sfxDragEnd, false);

        dragEndCallback?.Invoke(this, eventData);

        EndDrag();
    }

    private void EndDrag() {
        mIsDragging = false;

        dragRoot.SetParent(transform, false);
        dragRoot.localPosition = Vector3.zero;

        RefreshHighlight();
    }

    private void RefreshHighlight() {
        if(!highlightGO) return;

        highlightGO.SetActive(mIsClickEnabled && !mIsDragging && mIsHover);
    }
}
