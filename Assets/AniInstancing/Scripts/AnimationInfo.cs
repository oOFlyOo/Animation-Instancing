/*
THIS FILE IS PART OF Animation Instancing PROJECT
AnimationInstancing.cs - The core part of the Animation Instancing library

©2017 Jin Xiaoyu. All Rights Reserved.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimationInstancing
{
    public class AnimationEvent
    {
        public string function;
        public int intParameter;
        public float floatParameter;
        public string stringParameter;
        public string objectParameter;
        public float time;
    }

    public class AnimationInfo
    {
        public string animationName;
        // 本质是StateNameHash
        public int animationNameHash;
        // 帧长，多记录一帧
        public int totalFrame;
        public int fps;
        // 每个动画帧开始帧Index
        public int animationIndex;
        public int textureIndex;
        public bool rootMotion;
        public WrapMode wrapMode;
        // todo 以下两个数据用于rootMotion
        public Vector3[] velocity;
        public Vector3[] angularVelocity;
        // todo 不使用帧事件系统可以去除
        public List<AnimationEvent> eventList; 
    }

    public class ExtraBoneInfo
    {
        public string[] extraBone;
        public Matrix4x4[] extraBindPose;
    }

    /// <summary>
    /// 用于Sort，则会打断排序
    /// </summary>
    public class ComparerHash : IComparer<AnimationInfo>
    {
        public int Compare(AnimationInfo x, AnimationInfo y)
        {
            return x.animationNameHash.CompareTo(y.animationNameHash);
        }
    }
}
