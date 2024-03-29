﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Put this in UI screen space canvas
/// </summary>
public class CursorCollector : MonoBehaviour {
    [Header("Data")]
    public float followDelay = 0.1f;

    [Header("Display")]
    public Transform root;
    public Image fillImage;

    [Header("Animation")]
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeEnter;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeExit;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeError;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeCollect;

    [Header("SFX")]
    [M8.SoundPlaylist]
    public string sfxCollect;
    [M8.SoundPlaylist]
    public string sfxError;

    public float fillValue {
        get { return mFillValue; }
        set {
            var v = Mathf.Clamp01(value);
            if(mFillValue != v) {
                mFillValue = v;
                fillImage.fillAmount = mFillValue;
            }
        }
    }

    public bool isBusy { get { return animator ? animator.isPlaying : false; } }

    private float mFillValue;
    private Vector2 mCurVel;

    public void Collect() {
        M8.SoundPlaylist.instance.Play(sfxCollect, false);
    }

    public void Error() {
        M8.SoundPlaylist.instance.Play(sfxError, false);

        if(animator && !string.IsNullOrEmpty(takeError))
            animator.Play(takeError);
    }

    void OnDisable() {
        if(LevelController.isInstantiated)
            LevelController.instance.actionChangedCallback -= OnActionModeChanged;
    }

    void OnEnable() {
        ApplyCurActionState();

        mCurVel = Vector2.zero;

        LevelController.instance.actionChangedCallback += OnActionModeChanged;
    }

    void Awake() {
        if(animator) {
            animator.takeCompleteCallback += OnAnimationEnd;

            fillImage.fillAmount = 0f;
        }
    }

    void Update() {
        //follow cursor position
        var curAction = LevelController.instance.actionMode;
        switch(curAction) {
            case ActionMode.Ready:
                var pos = Input.mousePosition;
                root.position = Vector2.SmoothDamp(root.position, pos, ref mCurVel, followDelay);
                break;
        }
    }

    void OnActionModeChanged() {
        ApplyCurActionState();
    }

    private void ApplyCurActionState() {
        bool isActive = false;
        string take = null;

        var curAction = LevelController.instance.actionMode;
        switch(curAction) {
            case ActionMode.Ready:
                isActive = true;

                if(!root.gameObject.activeSelf)
                    take = takeEnter;
                break;

            case ActionMode.Collect:
                isActive = true;
                //take = takeCollect;
                take = takeExit;
                break;

            default:
                if(root.gameObject.activeSelf) {
                    isActive = true;
                    take = takeExit;
                }
                break;
        }
                
        root.gameObject.SetActive(isActive);

        if(animator && !string.IsNullOrEmpty(take))
            animator.Play(take);
    }

    void OnAnimationEnd(M8.Animator.Animate anim, M8.Animator.Take take) {
        if(take.name == takeExit || take.name == takeCollect)
            root.gameObject.SetActive(false);
    }
}
