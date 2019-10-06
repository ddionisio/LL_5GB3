using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalScoreWidget : MonoBehaviour {
    [Header("Display")]
    public M8.UI.Texts.TextCounter scoreCounterText;
    public Text rankText;

    void OnEnable() {
        var totalScore = LoLManager.instance.curScore;

        //get max total score
        int maxScore = 0;
        for(int i = 0; i < GameData.instance.levels.Length; i++) {
            maxScore += GameData.instance.GetLevelScoreMax(i);
        }

        float rankScale;
        if(maxScore > 0)
            rankScale = Mathf.Clamp01((float)totalScore / maxScore);
        else
            rankScale = 1f;

        var rank = GameData.instance.GetRank(rankScale);

        scoreCounterText.SetCountImmediate(0);
        scoreCounterText.count = totalScore;

        rankText.text = rank.text;
        rankText.color = rank.color;
    }
}
