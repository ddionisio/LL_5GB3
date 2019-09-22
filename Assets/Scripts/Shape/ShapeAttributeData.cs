using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShapeAttributeData : ScriptableObject {
    [M8.Localize]
    public string textRef;

    public abstract bool Evaluate(ShapeProfile shapeProfile);
}
