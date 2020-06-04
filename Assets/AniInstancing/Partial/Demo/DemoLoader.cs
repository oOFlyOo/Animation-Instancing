using System.Collections;
using System.Collections.Generic;
using AnimationInstancing;
using UnityEngine;

public class DemoLoader : MonoBehaviour
{
    [SerializeField]
    private string _stateName;
    [SerializeField]
    private GameObject _prefab;
    [SerializeField]
    private TextAsset _textAsset;

    private void Start()
    {
        AnimationManager.Instance.LoadAnimationInfo(_textAsset.bytes, _prefab);
        var go = AnimationInstancingMgr.Instance.CreateInstance(_prefab);

        StartCoroutine(Play(go));
    }

    private IEnumerator Play(GameObject go)
    {
        yield return new WaitForSeconds(1);

        if (!string.IsNullOrEmpty(_stateName))
        {
            go.GetComponent<AnimationInstancing.AnimationInstancing>().PlayAnimation(_stateName);
        }
    }
}
