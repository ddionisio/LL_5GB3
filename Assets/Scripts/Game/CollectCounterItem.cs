using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCounterItem : MonoBehaviour {
    public GameObject activeGO;

    void Awake() {
        activeGO.SetActive(false);
    }
}
