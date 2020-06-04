using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnimationInstancing
{
    public class RuntimeHelper
    {
        /// <summary>
        /// Merge all bones to a single array and merge all bind pose
        /// 将多个蒙皮模型的网格合并到一起
        /// todo 单个Render的话，感觉没太必要，不需要bindPose的话，直接返回bones应该就行了
        /// </summary>
        /// <param name="meshRender"></param>
        /// <param name="bindPose">外部传入的空数组 todo 改成内部或者外部缓存</param>
        /// <returns></returns>
        public static void MergeBone(SkinnedMeshRenderer[] meshRender, List<Matrix4x4> bindPose, List<Transform> listTransform)
        {
            UnityEngine.Profiling.Profiler.BeginSample("MergeBone()");
            for (int i = 0; i != meshRender.Length; ++i)
            {
                Transform[] bones = meshRender[i].bones;
                Matrix4x4[] checkBindPose = meshRender[i].sharedMesh.bindposes;
                for (int j = 0; j != bones.Length; ++j)
                {
#if UNITY_EDITOR
                    Debug.Assert(checkBindPose[j].determinant != 0, "The bind pose can't be 0 matrix.");
#endif
                    // the bind pose is correct base on the skinnedMeshRenderer, so we need to replace it
                    int index = listTransform.FindIndex(q => q == bones[j]);
                    if (index < 0)
                    {
                        listTransform.Add(bones[j]);
                        if (bindPose != null)
                        {
                            bindPose.Add(checkBindPose[j]);
                        }
                    }
                    else
                    {
                        // todo 这里的操作好像没太必要，而且判空不见了
                        bindPose[index] = checkBindPose[j];
                    }
                }
                // todo 这里好像不应该这么做
                meshRender[i].enabled = false;
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }
        
        private static readonly List<Matrix4x4> TempBindPoses = new List<Matrix4x4>(AnimationInstancingMgr.MAX_BIND_POSE);
        private static readonly List<Transform> TempBones = new List<Transform>(AnimationInstancingMgr.MAX_BIND_POSE);

        public static Transform[] MergeBone(SkinnedMeshRenderer[] meshRender)
        {
            MergeBone(meshRender, TempBindPoses, TempBones);
            TempBindPoses.Clear();
            var bones = TempBones.ToArray();
            TempBones.Clear();

            return bones;
        }
        
        public static Transform[] MergeBoneEditorOnly(SkinnedMeshRenderer[] meshRender, List<Matrix4x4> bindPoses)
        {
            MergeBone(meshRender, bindPoses, TempBones);
            var bones = TempBones.ToArray();
            TempBones.Clear();

            return bones;
        }

        public static Transform[] MergeBoneRunTime(SkinnedMeshRenderer[] meshRender, out Matrix4x4[] bindPose,
            Transform[] trans = null, string[] extraBones = null, Matrix4x4[] extraBindPoses = null)
        {
            List<Matrix4x4> tempBindPoses;
            var tempBones = MergeBoneRunTime(meshRender, out tempBindPoses, trans, extraBones, extraBindPoses);

            bindPose = tempBindPoses.ToArray();
            tempBindPoses.Clear();
            var bones = tempBones.ToArray();
            tempBones.Clear();

            return bones;
        }
        
        
        private static List<Transform> MergeBoneRunTime(SkinnedMeshRenderer[] meshRender, out List<Matrix4x4> bindPose, Transform[] trans = null, string[] extraBones = null, Matrix4x4[] extraBindPoses = null)
        {
            // 这里得注意非线程安全
            TempBindPoses.Clear();
            TempBones.Clear();

            MergeBone(meshRender, TempBindPoses, TempBones);

            if (extraBones?.Length > 0)
            {
                for (int i = 0; i < extraBones.Length; i++)
                {
                    var extraBone = extraBones[i];
                    {
                        for (int j = 0; j < trans.Length; j++)
                        {
                            var tran = trans[j];
                            if (tran.name == extraBone)
                            {
                                TempBones.Add(tran);
                            }
                        }

                        TempBindPoses.Add(extraBindPoses[i]);
                    }
                }
            }

            bindPose = TempBindPoses;
            
            return TempBones;
        }

        public static Quaternion QuaternionFromMatrix(Matrix4x4 mat)
        {
            Vector3 forward;
            forward.x = mat.m02;
            forward.y = mat.m12;
            forward.z = mat.m22;

            Vector3 upwards;
            upwards.x = mat.m01;
            upwards.y = mat.m11;
            upwards.z = mat.m21;

            return Quaternion.LookRotation(forward, upwards);
        }
    }
}
