using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Add this next to the ShapeProfile
/// </summary>
public class CursorInteractiveCollect : CursorInteractive {

    private ShapeProfile mShapeProfile;
    private Collider2D mColl;

    protected override bool CanInteract(PointerEventData eventData) {
        if(mShapeProfile.isCollected) //fail-safe
            return false;

        return true;
    }

    protected override void OnClick() {
        LevelController.instance.cursor.Collect();

        mColl.enabled = false;
        LevelController.instance.CollectShape(mShapeProfile);
    }

    void Awake() {
        mShapeProfile = GetComponent<ShapeProfile>();
        mColl = GetComponent<Collider2D>();
    }
}
