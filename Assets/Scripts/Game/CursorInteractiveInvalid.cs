using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorInteractiveInvalid : CursorInteractive {
    protected override void OnClick() {
        LevelController.instance.cursor.Error();
    }
}
