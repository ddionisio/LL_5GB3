using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryModal : M8.ModalController, M8.IModalPush {

    [Header("Display")]
    public M8.UI.Texts.TextCounter scoreCounter;
    public Text rankText;

    private int mScore;
    private int mScoreMax;

    public void Next() {
        Close();

        GameData.instance.SaveCurLevelScore(mScore, mScoreMax);

        LoLManager.instance.curScore += mScore;

        GameData.instance.ProceedToNextLevel();
    }

    void M8.IModalPush.Push(M8.GenericParams parms) {
        //compute max score
        mScoreMax = 0;

        for(int i = 0; i < LevelController.instance.shapes.Length; i++)
            mScoreMax += GameData.instance.scoreShape;

        //score
        mScore = 0;

        for(int i = 0; i < LevelController.instance.shapesCollected.Count; i++)
            mScore += LevelController.instance.shapesCollected[i].score;

        scoreCounter.count = mScore;

        var rank = GameData.instance.GetRank(Mathf.Clamp01((float)mScore / mScoreMax));

        rankText.text = rank.text;
        rankText.color = rank.color;
    }
}
