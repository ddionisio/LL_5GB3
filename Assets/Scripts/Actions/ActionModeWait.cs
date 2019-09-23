using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class ActionModeWait : FsmStateAction {
        public ActionMode mode;

        public override void Reset() {
            mode = ActionMode.None;
        }

        public override void OnUpdate() {
            if(LevelController.instance.actionMode == mode)
                Finish();
        }
    }
}