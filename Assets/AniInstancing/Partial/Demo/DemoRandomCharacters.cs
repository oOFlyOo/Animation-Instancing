/// <summary>
/// 
/// </summary>

using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

//[RequireComponent(typeof(Animator))]  

//Name of class must be name of file as well

public class DemoRandomCharacters : MonoBehaviour
{

    public float AvatarRange = 20;
    private float _moveInterval = 0.1f;
    private float _lastMoveTime = 0;

    private float _playInterval = 2;
    private float _lastPlayTime = 0;

    protected Animator avatar;
    private AnimationInstancing.AnimationInstancing instancing;

    /// <summary>
    /// 先写死States，暂时没有比较好的获取方式
    /// </summary>
    private enum States
    {
        Attack,
        Dire,
        Hit,
        Idle,
        Run,
    }

    // Use this for initialization
    void Start()
    {
        if (!AnimationInstancing.AnimationInstancingMgr.Instance.UseInstancing)
        {
            avatar = GetComponent<Animator>();
        }
        else
        {
            instancing = GetComponent<AnimationInstancing.AnimationInstancing>();
            Debug.Assert(instancing);
            if (instancing == null)
            {
                gameObject.SetActive(false);
            }
        }

        transform.position = GetRandomPos();
    }


    void Update()
    {
        if (Time.time - _lastMoveTime > _moveInterval)
        {
            var targetPos = GetRandomPos();
            transform.Translate(targetPos * Time.deltaTime);
            transform.LookAt(targetPos);

            _lastMoveTime = Time.time;
        }

        if (Time.time - _lastPlayTime > _playInterval)
        {
            if (avatar)
            {
                var names = Enum.GetNames(typeof(States));
                avatar.Play(names[Random.Range(0, names.Length)]);
            }
            else
            {
                var infos = instancing.aniInfo;
                instancing.PlayAnimation(Random.Range(0, infos.Count));
            }

            _lastPlayTime = Time.time;
        }

    }

    private Vector3 GetRandomPos()
    {
        var targetPos = Random.insideUnitCircle * AvatarRange;

        return new Vector3(targetPos.x, 0, targetPos.y);
    }
}
