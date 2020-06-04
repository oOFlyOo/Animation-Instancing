using System.Collections;
using System.Collections.Generic;
using AnimationInstancing;
using UnityEngine;

public class DemoSpawner : MonoBehaviour
{
    [SerializeField]
    private int _showCount;
    [SerializeField] 
    private float _increaseInterval;

    [SerializeField]
    private List<GameObject> _prefabs;
    [SerializeField]
    private List<GameObject> _instPrefabs;
    [SerializeField]
    private List<TextAsset> _instInfos;

    private int _curCount;
    private float _lastTime = 0;
    private List<GameObject> _tempGos;

    private void OnGUI()
    {
        GUILayout.Label(string.Format("Spawns up to {0} characters, current {1}", _showCount, _curCount));

        if (GUI.Button(new Rect(10, 100, 100, 40), "Decrease"))
        {
            _showCount -= 50;
        }
        if (GUI.Button(new Rect(130, 100, 100, 40), "Increase"))
        {
            _showCount += 50;
        }

        string text = AnimationInstancing.AnimationInstancingMgr.Instance.UseInstancing ? "EnableInstancing" : "DisableInstancing";
        if (GUI.Button(new Rect(10, 150, 140, 40), text))
        {
            AnimationInstancing.AnimationInstancingMgr.Instance.UseInstancing = !AnimationInstancing.AnimationInstancingMgr.Instance.UseInstancing;
            ClearAndLoad();
        }
    }

    private void Start()
    {
        _lastTime = Time.time;
        _tempGos = new List<GameObject>();
        ClearAndLoad();
        // LoadAB();
        AnimationInstancing.AnimationInstancingMgr.Instance.UseInstancing = true;
    }

    private void Load()
    {
        for (int i = 0; i < _instPrefabs.Count; i++)
        {
            var inst = _instPrefabs[i];
            var info = _instInfos[i];

            AnimationManager.Instance.LoadAnimationInfo(info.bytes, inst);
        }
    }

    private void LoadAB()
    {
        StartCoroutine(AnimationInstancing.AnimationManager.Instance.LoadAnimationAssetBundle(Application.streamingAssetsPath + "/AssetBundle/animationtexture"));
    }



    private void ClearAndLoad()
    {
        foreach (var go in _tempGos)
        {
            Destroy(go);
        }
        AnimationInstancing.AnimationInstancingMgr.Instance.Clear();

        _tempGos.Clear();
        _curCount = 0;

        Resources.UnloadUnusedAssets();
        if (AnimationInstancing.AnimationInstancingMgr.Instance.UseInstancing)
        {
            Load();
        }
    }

    private void Update()
    {
        if (_curCount < _showCount)
        {
            bool alt = Input.GetButton("Fire1");

            if (Time.time - _lastTime > _increaseInterval)
            {
                GameObject go;
                if (AnimationInstancingMgr.Instance.UseInstancing)
                {
                    var inst = _instPrefabs[Random.Range(0, _instPrefabs.Count)];
                    go = AnimationInstancingMgr.Instance.CreateInstance(inst);
                    _tempGos.Add(go);
                }
                else
                {
                    var prefab = _prefabs[Random.Range(0, _prefabs.Count)];
                    go = Instantiate(prefab);
                }

                go.AddComponent<DemoRandomCharacters>();
                go.transform.localScale = Vector3.one * 6;
                _tempGos.Add(go);

                _lastTime = Time.time;
                _curCount++;
            }
        }
    }

}
