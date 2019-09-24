using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class SpaceshipBusyWait : FsmStateAction {
        public override void OnUpdate() {
            if(!LevelController.instance.spaceship.isBusy)
                Finish();
        }
    }
}