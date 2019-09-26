using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using M8;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class ShowLevelShapeCategories : FsmStateAction {
        public FsmBool waitClose;

        public override void Reset() {
            waitClose = true;
        }

        public override void OnEnter() {
            var parms = new GenericParams();
            parms[ShapeCategoryModal.parmShapes] = LevelController.instance.shapeCategories;

            ModalManager.main.Open(GameData.instance.modalShapeCategory, parms);

            if(!waitClose.Value)
                Finish();
        }

        public override void OnUpdate() {
            if(!(ModalManager.main.isBusy || ModalManager.main.IsInStack(GameData.instance.modalShapeCategory)))
                Finish();
        }
    }
}