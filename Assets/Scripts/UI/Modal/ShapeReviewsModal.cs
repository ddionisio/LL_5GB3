using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeReviewsModal : M8.ModalController, M8.IModalPush {
    public const string parmTitle = "title";
    public const string parmIndex = "index";

    [System.Serializable]
    public struct CategoryGroup {
        public string[] categories;
    }

    public CategoryGroup[] categoryGroups;

    public Text titleText;
    public Transform root;
    public GameObject hierarchyGO;

    private Dictionary<string, GameObject> mCategories;

    void M8.IModalPush.Push(M8.GenericParams parms) {
        ResetDisplay();

        int ind = -1;

        if(parms != null) {
            if(parms.ContainsKey(parmTitle))
                titleText.text = parms.GetValue<string>(parmTitle);

            if(parms.ContainsKey(parmIndex))
                ind = parms.GetValue<int>(parmIndex);
        }

        if(ind != -1 && ind < categoryGroups.Length) {
            var cat = categoryGroups[ind];

            for(int i = 0; i < cat.categories.Length; i++) {
                var key = cat.categories[i];
                if(mCategories.ContainsKey(key))
                    mCategories[key].SetActive(true);
            }
        }
    }

    void Awake() {
        mCategories = new Dictionary<string, GameObject>(root.childCount + 1);

        for(int i = 0; i < root.childCount; i++) {
            var child = root.GetChild(i);
            child.gameObject.SetActive(false);

            mCategories.Add(child.name, child.gameObject);
        }

        mCategories.Add(hierarchyGO.name, hierarchyGO);
        hierarchyGO.SetActive(false);
    }

    void ResetDisplay() {
        foreach(var pair in mCategories)
            pair.Value.SetActive(false);
    }
}
