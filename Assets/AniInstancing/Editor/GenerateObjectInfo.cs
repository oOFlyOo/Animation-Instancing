using UnityEngine;
using UnityEditor;
using System;

namespace AnimationInstancing
{
    // 每个State每一帧的信息记录
    public class GenerateOjbectInfo
    {
        #region 用不上的
        // Matrix4x4.identity
        public Matrix4x4 worldMatrix;
        public int nameCode;
        public int frameIndex;
        #endregion

        // 记录在哪一帧
        public float animationTime;
        public int stateName;
        public int boneListIndex = -1;
        // 骨骼转换矩阵
        public Matrix4x4[] boneMatrix;
    }
}
