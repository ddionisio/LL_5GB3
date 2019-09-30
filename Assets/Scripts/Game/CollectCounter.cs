using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCounter : MonoBehaviour {
    [Header("Template")]
    public CollectCounterItem itemTemplate;
    
    [Header("Config")]
    public float space = 1f;

    [Header("Display")]
    public Transform itemRoot;
    public SpriteRenderer baseSpriteRender;

    private M8.CacheList<CollectCounterItem> mCollectItems;

    void OnDestroy() {
        if(LevelController.isInstantiated) {
            LevelController.instance.shapeCollectedCallback -= OnShapeCollected;
        }
    }

    void Awake() {
        var levelCtrl = LevelController.instance;

        levelCtrl.shapeCollectedCallback += OnShapeCollected;

        var count = levelCtrl.shapes.Length;

        mCollectItems = new M8.CacheList<CollectCounterItem>(count);

        var baseLen = (count - 1) * space;

        if(count > 1) {
            var s = baseSpriteRender.size;
            s.x = baseLen;
            baseSpriteRender.size = s;
        }
        else
            baseSpriteRender.gameObject.SetActive(false);

        var x = -baseLen * 0.5f;

        for(int i = 0; i < count; i++) {
            var itm = Instantiate(itemTemplate, itemRoot);
            itm.transform.localPosition = new Vector3(x, 0f, 0f);

            mCollectItems.Add(itm);

            x += space;
        }

        itemTemplate.gameObject.SetActive(false);
    }

    void OnShapeCollected(ShapeProfile shape) {
        var ind = LevelController.instance.shapesCollected.Count - 1;

        if(ind < mCollectItems.Count) {
            mCollectItems[ind].activeGO.SetActive(true);
        }
    }
}
