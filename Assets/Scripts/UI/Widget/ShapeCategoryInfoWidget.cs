using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeCategoryInfoWidget : MonoBehaviour {

    [Header("Display")]
    public Text titleText;
    public Text descText;
    public Transform categoryDisplayRoot;

    private Dictionary<string, GameObject> mCategoryDisplayGOs;

    private GameObject mCurCategoryDisplayGO;

    public void Setup(ShapeCategoryData dat) {
        if(mCategoryDisplayGOs == null)
            InitCategories();

        if(mCurCategoryDisplayGO)
            mCurCategoryDisplayGO.SetActive(false);

        //setup display
        GameObject go;
        if(mCategoryDisplayGOs.TryGetValue(dat.name, out go)) {
            mCurCategoryDisplayGO = go;
            mCurCategoryDisplayGO.SetActive(true);
        }
        else
            mCurCategoryDisplayGO = null;

        //title
        titleText.text = M8.Localize.Get(dat.textRef);

        //desc
        var sb = new System.Text.StringBuilder();

        for(int i = 0; i < dat.attributes.Length; i++) {
            var attr = dat.attributes[i];

            sb.Append("· ");
            sb.Append(M8.Localize.Get(attr.textRef));

            if(i < dat.attributes.Length - 1)
                sb.Append('\n');
        }

        descText.text = sb.ToString();
    }

    private void InitCategories() {
        mCategoryDisplayGOs = new Dictionary<string, GameObject>();

        for(int i = 0; i < categoryDisplayRoot.childCount; i++) {
            var child = categoryDisplayRoot.GetChild(i);
            mCategoryDisplayGOs.Add(child.name, child.gameObject);
        }
    }
}
