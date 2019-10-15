using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalScoreItemWidget : MonoBehaviour {
    public Text titleText;
    public Text rankText;
    public Button detailButton;

    public int index { get; set; }
    public string title { get; set; }

    private M8.GenericParams mParms = new M8.GenericParams();

    void Awake() {
        detailButton.onClick.AddListener(OnClick);
    }

    void OnClick() {
        mParms[ShapeReviewsModal.parmIndex] = index;
        mParms[ShapeReviewsModal.parmTitle] = title;

        M8.ModalManager.main.Open(GameData.instance.modalShapeReview, mParms);
    }
}
