using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalScoreListWidget : MonoBehaviour {

    public FinalScoreItemWidget template;
    public Transform root;

    void Awake() {
        for(int i = 0; i < GameData.instance.levels.Length; i++) {
            string titleTextRef = "level_title_" + i.ToString();

            var itm = GameObject.Instantiate(template, root);

            itm.titleText.text = M8.Localize.Get(titleTextRef);

            int score = GameData.instance.GetLevelScore(i);
            int scoreMax = GameData.instance.GetLevelScoreMax(i);

            float scoreScale = (float)score / scoreMax;

            var rank = GameData.instance.GetRank(scoreScale);

            itm.rankText.text = rank.text;
            itm.rankText.color = rank.color;

            itm.index = i;
        }

        template.gameObject.SetActive(false);
    }
}
