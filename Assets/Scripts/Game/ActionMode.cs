using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionMode {
    None,
    Enter, //ship is entering play area
    Ready, //ready to collect
    Collect, //shape confirmed, collect, then analyze
    Analyze, //show category/attribute selection, grading, return to ready
    Victory, //tally score
}
