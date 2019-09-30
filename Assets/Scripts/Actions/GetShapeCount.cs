using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class GetShapeCount : FsmStateAction {
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmInt output;

        public override void Reset() {
            output = null;
        }

        public override void OnEnter() {
            output.Value = LevelController.instance.shapes.Length;
            Finish();
        }
    }
}