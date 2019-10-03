using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectActiveOnActionMode : MonoBehaviour {

    public GameObject displayGO;
    public ActionMode mode;

    void OnDestroy() {
        if(LevelController.isInstantiated)
            LevelController.instance.actionChangedCallback -= OnModeChanged;
    }

    void Awake() {
        OnModeChanged();
        LevelController.instance.actionChangedCallback += OnModeChanged;
    }

    void OnModeChanged() {
        var active = LevelController.instance.actionMode == mode;

        displayGO.SetActive(active);
    }
}
