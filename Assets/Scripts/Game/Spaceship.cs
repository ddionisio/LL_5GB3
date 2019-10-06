using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour {
    public enum Mode {
        None,
        Show,
        Hide,
        Collect
    }

    [Header("Data")]
    public DG.Tweening.Ease moveEase = DG.Tweening.Ease.InOutSine;
    public float moveDelay = 0.3f;

    [Header("Display")]
    public GameObject displayGO;
    public Transform root;

    public SpriteRenderer beamRender; //ensure pivot is top
    public float beamLengthOfs;

    [M8.SortingLayer]
    public string collectSortLayer; //set collecting shape to this render sort layer
    public Transform collectRoot;
    public Transform collectShapeAnchor; //ensure this is a child of collectFXRoot, set collect's parent to this during collection

    [Header("Animation")]
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeEnter;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeIdle;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeExit;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeCollect;

    [Header("SFX")]
    [M8.SoundPlaylist]
    public string sfxCollect;

    public bool isBusy { get { return mRout != null; } }

    private Coroutine mRout;
    private ShapeProfile mShapeCollect;
    private Mode mMode;

    void OnDisable() {
        mRout = null;

        if(LevelController.isInstantiated) {
            LevelController.instance.actionChangedCallback -= OnActionModeChanged;
            LevelController.instance.shapeCollectedCallback -= OnCollect;
        }
    }

    void OnEnable() {
        OnActionModeChanged();

        LevelController.instance.actionChangedCallback += OnActionModeChanged;
        LevelController.instance.shapeCollectedCallback += OnCollect;
    }

    void Awake() {
        displayGO.SetActive(false);
    }

    void OnActionModeChanged() {

        Mode toMode = mMode;

        switch(LevelController.instance.actionMode) {
            case ActionMode.Enter:
            case ActionMode.Ready:
                toMode = Mode.Show;
                break;

            case ActionMode.Collect:
                toMode = Mode.Collect;
                break;

            default: //leave
                toMode = Mode.Hide;
                break;
        }

        if(mMode != toMode) {
            mMode = toMode;

            if(mRout != null)
                StopCoroutine(mRout);

            collectRoot.gameObject.SetActive(false);

            switch(mMode) {
                case Mode.Show:
                    mRout = StartCoroutine(DoEnter());
                    break;
                case Mode.Hide:
                    if(displayGO.activeSelf)
                        mRout = StartCoroutine(DoExit());
                    break;
                case Mode.Collect:
                    mRout = StartCoroutine(DoCollect());
                    break;
            }
        }
    }

    void OnCollect(ShapeProfile shapeProfile) {
        mShapeCollect = shapeProfile;
    }

    IEnumerator DoEnter() {
        displayGO.SetActive(true);

        if(animator) {
            if(!string.IsNullOrEmpty(takeEnter))
                yield return animator.PlayWait(takeEnter);

            if(!string.IsNullOrEmpty(takeIdle))
                animator.Play(takeIdle);
        }

        mRout = null;
    }

    IEnumerator DoExit() {

        if(animator && !string.IsNullOrEmpty(takeExit))
            yield return animator.PlayWait(takeExit);

        mRout = null;

        displayGO.SetActive(false);
    }

    IEnumerator DoCollect() {
        if(animator && !string.IsNullOrEmpty(takeIdle))
            animator.Play(takeIdle);
                
        if(mShapeCollect) {
            //move towards collection
            var easeFunc = DG.Tweening.Core.Easing.EaseManager.ToEaseFunction(moveEase);

            var startPos = root.position;

            Vector2 collectPos = startPos;
            Vector2 destPos = startPos;
                        
            var curTime = 0f;
            while(curTime < moveDelay) {
                yield return null;

                collectPos = mShapeCollect.transform.position;
                destPos = new Vector2(collectPos.x, startPos.y);

                curTime += Time.deltaTime;

                var t = easeFunc(curTime, moveDelay, 0f, 0f);

                root.position = Vector2.Lerp(startPos, destPos, t);
            }

            //setup beam size, assume pivot is top
            var beamPos = beamRender.transform.position;
            var beamLen = Mathf.Abs(collectPos.y - beamPos.y) + beamLengthOfs;
            var beamSize = beamRender.size;
            beamSize.y = beamLen;
            beamRender.size = beamSize;

            //setup collect
            if(animator && !string.IsNullOrEmpty(takeCollect))
                animator.ResetTake(takeCollect);

            var collectRender = mShapeCollect.shape.GetComponent<SpriteRenderer>();
            collectRender.sortingLayerName = collectSortLayer;

            collectRoot.position = collectPos;
            collectRoot.gameObject.SetActive(true);

            mShapeCollect.transform.SetParent(collectShapeAnchor, true);

            M8.SoundPlaylist.instance.Play(sfxCollect, false);

            //play collect
            if(animator && !string.IsNullOrEmpty(takeCollect))
                yield return animator.PlayWait(takeCollect);

            //hide collect
            collectRoot.gameObject.SetActive(false);

            mShapeCollect.transform.SetParent(null, false);
            mShapeCollect.gameObject.SetActive(false);
            mShapeCollect = null;

            if(animator && !string.IsNullOrEmpty(takeExit))
                yield return animator.PlayWait(takeExit);

            displayGO.SetActive(false);
        }

        mRout = null;
    }
}
