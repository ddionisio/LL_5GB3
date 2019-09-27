using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M8;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class AnalyzeLastCollectedShape : FsmStateAction {
        public FsmBool waitClose;

        public override void Reset() {
            waitClose = true;
        }

        public override void OnEnter() {
            LevelController.instance.AnalyzeLastCollectedShape();

            if(!waitClose.Value)
                Finish();
        }

        public override void OnUpdate() {
            if(!(ModalManager.main.isBusy || ModalManager.main.IsInStack(GameData.instance.modalShapeAnalyze)))
                Finish();
        }
    }
}