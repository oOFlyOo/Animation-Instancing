using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;


namespace AnimationInstancing
{
    /// <summary>
    /// 扩展AnimationGenerator，尽量不改动原来的代码
    /// </summary>
    public partial class AnimationGenerator
    {
        private const string INSTANCE_FOLDER = "Instancing";
        private const string INSTANCE_PREFAB_SUFFIX = "_inst.prefab";
        private const string INSTANCE_AnimationInfo_SUFFIX = "_inst.bytes";
        private const string INSTANCE_MATERIAL_SUFFIX = "_inst.mat";

        private bool _isMenuMode;
        private string _originPrefabPath;
        private string _instPrefabPath;

        private static void InitWindow()
        {
            MakeWindow();
            s_window._isMenuMode = true;
        }


        private static void ReleaseWindow()
        {
            if (s_window != null)
            {
                s_window.Close();
                s_window = null;
            }
        }

        private static void GenerateAnimationFinish()
        {
            if (s_window._isMenuMode)
            {
                GenerateAnimationOnFinish();

                ReleaseWindow();
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Assets/Animator/GenerateInstancing", true)]
        private static bool CheckIsAnimatorPrefab()
        {
            var go = Selection.activeGameObject;

            return go && PrefabUtility.GetPrefabAssetType(Selection.activeGameObject) == PrefabAssetType.Regular &&
                   go.GetComponent<Animator>();
        }

        [MenuItem("Assets/Animator/GenerateInstancing")]
        private static void GenerateInstancingMenu()
        {
            InitWindow();

            GenerateInstancing(Selection.activeGameObject);
        }


        private static void CreateDirectoryIfNotExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        private static T AddComponetIfNotExitsts<T>(GameObject go) where T : Component
        {
            var com = go.GetComponent<T>();
            if (!com)
            {
                com = go.AddComponent<T>();
            }

            return com;
        }

        private static void CopyAsset(string originPath, string newPath)
        {
            // AssetDatabase.CopyAsset(originPath, newPath);
            File.Copy(originPath, newPath, true);
            AssetDatabase.ImportAsset(newPath);
        }


        private static string GetInstancingFolder(string prefabPath)
        {
            var folderRoot = Path.GetDirectoryName(prefabPath);
            return $"{folderRoot}/{INSTANCE_FOLDER}";
        }

        private static string GetInstancingPrefabPath(string prefabPath)
        {
            var folderRoot = GetInstancingFolder(prefabPath);
            var prefabName = Path.GetFileNameWithoutExtension(prefabPath);

            return $"{folderRoot}/{prefabName}{INSTANCE_PREFAB_SUFFIX}";
        }

        private static string GetInstancingAnimInfoPath(string prefabPath)
        {
            var folderRoot = GetInstancingFolder(prefabPath);
            var prefabName = Path.GetFileNameWithoutExtension(prefabPath);

            return $"{folderRoot}/{prefabName}{INSTANCE_AnimationInfo_SUFFIX}";
        }

        private static string GetInstancingAnimInfoPath()
        {
            if (!s_window._isMenuMode)
            {
                return null;
            }

            return GetInstancingAnimInfoPath(s_window._originPrefabPath);
        }

        private static string GetInstancingMaterialPath(string oldPath, string instancingFolder)
        {
            return $"{instancingFolder}/{Path.GetFileNameWithoutExtension(oldPath)}{INSTANCE_MATERIAL_SUFFIX}";
        }

        private static void GenerateInstancing(GameObject originPrefab)
        {
            var prefabPath = AssetDatabase.GetAssetPath(originPrefab);
            s_window._originPrefabPath = prefabPath;

            var instancingFolder = GetInstancingFolder(prefabPath);
            CreateDirectoryIfNotExists(instancingFolder);

            var instPrefabPath = GetInstancingPrefabPath(prefabPath);
            s_window._instPrefabPath = instPrefabPath;
            CopyAsset(prefabPath, instPrefabPath);
            var instGo = (GameObject)AssetDatabase.LoadMainAssetAtPath(instPrefabPath);
            AddComponetIfNotExitsts<AnimationInstancing>(instGo);

            s_window.ChangeGeneratedPrefab(instGo);
            EditorApplication.delayCall += () => s_window.BakeWithAnimator();
        }

        private static void HandleGameObjectRecursive(Transform trans, bool isRoot = true)
        {
            foreach (Transform child in trans)
            {
                HandleGameObjectRecursive(child, false);
            }

            if (!isRoot)
            {
                var com = trans.GetComponent<Renderer>();
                if (com)
                {
                    com.enabled = false;
                    GenerateMaterial(com);
                }
                else
                {
                    // DestroyImmediate(trans.gameObject, true);
                }
            }
        }

        private static void GenerateMaterial(Renderer renderer)
        {
            var mat = renderer.sharedMaterial;
            var matPath = AssetDatabase.GetAssetPath(mat);
            var newMatPath = GetInstancingMaterialPath(matPath, GetInstancingFolder(s_window._originPrefabPath));

            CopyAsset(matPath, newMatPath);
            var newMat = (Material) AssetDatabase.LoadMainAssetAtPath(newMatPath);
            newMat.shader = Shader.Find("AnimationInstancing/DiffuseInstancing");
            newMat.enableInstancing = true;
            renderer.sharedMaterial = newMat;
        }

        /// <summary>
        /// 大部分处理都要等待生成完之后再做处理
        /// </summary>
        private static void GenerateAnimationOnFinish()
        {
            var go = (GameObject)AssetDatabase.LoadMainAssetAtPath(s_window._instPrefabPath);

            HandleGameObjectRecursive(go.transform);
            var instAni = go.GetComponent<Animator>();
            DestroyImmediate(instAni, true);
            var aniInst = go.GetComponent<AnimationInstancing>();
            aniInst.prototype = go;

            PrefabUtility.SavePrefabAsset(go);
        }
    }
}