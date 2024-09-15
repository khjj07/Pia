using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Default.Scripts.Extension
{
#if UNITY_EDITOR
    public static class UnityEditorAssetExtensions
    {
        /// <summary> 폴더 애셋으로부터 Assets로 시작하는 로컬 경로 얻기 </summary>
        public static string GetLocalPath(this DefaultAsset @this)
        {
            var success =
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(@this, out var guid, out long _);

            if (success)
                return AssetDatabase.GUIDToAssetPath(guid);
            return null;
        }

        /// <summary> 폴더 애셋으로부터 절대 경로 얻기 </summary>
        public static string GetAbsolutePath(this DefaultAsset @this)
        {
            var path = GetLocalPath(@this);
            if (path == null)
                return null;

            path = path.Substring(path.IndexOf('/') + 1);
            return Application.dataPath + "/" + path;
        }

        /// <summary> 폴더 애셋으로부터 DirectoryInfo 객체 얻기 </summary>
        public static DirectoryInfo GetDirectoryInfo(this DefaultAsset @this)
        {
            var absPath = GetAbsolutePath(@this);
            return absPath != null ? new DirectoryInfo(absPath) : null;
        }

        public static List<T> LoadAllObjectsInFolder<T>(this DefaultAsset @this) where T : class
        {
            var assets = new List<T>();
            // 폴더 내 모든 자산의 GUID 배열을 가져옴
            var assetGUIDs = AssetDatabase.FindAssets("", new[] { GetLocalPath(@this) });

            foreach (var guid in assetGUIDs)
            {
                // GUID를 경로로 변환
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                // 자산을 Object 타입으로 로드
                if (AssetDatabase.LoadAssetAtPath<Object>(assetPath) is T tmp) assets.Add(tmp);
            }

            return assets;
        }
    }
#endif
}