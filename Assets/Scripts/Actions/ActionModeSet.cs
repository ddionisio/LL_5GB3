using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class ActionModeSet : FsmStateAction {
        public ActionMode mode;

        public override void Reset() {
            mode = ActionMode.None;
        }

        public override void OnEnter() {
            LevelController.instance.actionMode = mode;
            Finish();
        }
    }
}