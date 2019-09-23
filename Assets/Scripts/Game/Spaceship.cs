using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour {

    [Header("Display")]
    public GameObject rootGO;

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

    public bool isBusy { get { return mRout != null; } }

    private Coroutine mRout;
    private ShapeProfile mShapeCollect;

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
        
    }

    void OnActionModeChanged() {

        switch(LevelController.instance.actionMode) {
            case ActionMode.Enter:
            case ActionMode.Ready:
                break;

            case ActionMode.Collect:
                break;

            default: //leave
                break;
        }
    }

    void OnCollect(ShapeProfile shapeProfile) {

    }
}
