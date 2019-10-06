using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectActiveOnActionMode : MonoBehaviour {

    public GameObject displayGO;
    public ActionMode mode;
    [M8.SoundPlaylist]
    public string sfxActive;

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

        if(displayGO.activeSelf != active) {
            displayGO.SetActive(active);

            if(active) {
                if(!string.IsNullOrEmpty(sfxActive))
                    M8.SoundPlaylist.instance.Play(sfxActive, false);
            }
        }
    }
}
