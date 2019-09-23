using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("")]
public abstract class CursorInteractive : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    //[Header("Config")]
    

    private float mCurVal = 0f;
    private float mLastTime;
    private bool mIsHold = false;

    private bool mIsPointerDown = false;

    protected virtual bool CanInteract(PointerEventData eventData) {
        return true;
    }

    protected abstract void OnClick();

    void OnDisable() {
        if(LevelController.isInstantiated)
            LevelController.instance.actionChangedCallback -= OnActionModeChanged;
    }

    void OnEnable() {
        mCurVal = 0f;
        mIsHold = false;
        mIsPointerDown = false;

        LevelController.instance.actionChangedCallback += OnActionModeChanged;
    }

    void Update() {
        if(mIsHold) {
            var curTime = Time.time;
            var dt = curTime - mLastTime;
            var val = Mathf.Clamp01(dt / GameData.instance.cursorHoldDelay);
            if(mCurVal != val) {
                mCurVal = val;                

                if(mCurVal == 1.0f) {
                    Release();
                    OnClick();
                }
                else
                    ApplyCurValue();
            }
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
        //only valid when action is ready
        if(LevelController.instance.actionMode != ActionMode.Ready)
            return;

        //don't interact when cursor is busy
        if(LevelController.instance.cursor.isBusy)
            return;

        if(!CanInteract(eventData))
            return;

        mIsPointerDown = true;
        UpdateHold();
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
        mIsPointerDown = false;
        UpdateHold();
    }

    void OnActionModeChanged() {
        if(LevelController.instance.actionMode != ActionMode.Ready)
            Release();
    }

    private void UpdateHold() {
        bool hold = mIsPointerDown;
        if(mIsHold != hold) {
            mIsHold = hold;
            if(mIsHold) {
                mLastTime = Time.time;
            }
            else
                Release();
        }
    }

    private void Release() {
        if(mCurVal != 0f) {
            mCurVal = 0f;
            ApplyCurValue();
        }

        mIsHold = false;
    }

    private void ApplyCurValue() {
        LevelController.instance.cursor.fillValue = mCurVal;
    }
}
