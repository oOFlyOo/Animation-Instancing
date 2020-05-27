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
        public static Transform[] MergeBone(SkinnedMeshRenderer[] meshRender, List<Matrix4x4> bindPose)
        {
            UnityEngine.Profiling.Profiler.BeginSample("MergeBone()");
            // todo 改成缓存
            List<Transform> listTransform = new List<Transform>(150);
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
            return listTransform.ToArray();
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
