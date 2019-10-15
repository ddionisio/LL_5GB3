using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalScoreItemWidget : MonoBehaviour {
    public Text titleText;
    public Text rankText;
    public Button detailButton;

    public int index { get; set; }

    void Awake() {
        detailButton.onClick.AddListener(OnClick);
    }

    void OnClick() {

    }
}
