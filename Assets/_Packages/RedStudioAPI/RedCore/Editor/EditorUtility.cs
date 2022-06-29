using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class RedEditorUtility
{

    public static IEnumerable<T> GetAllAssetsOfType<T>() where T : UnityEngine.Object
    {
        return AssetDatabase.FindAssets($"t:{typeof(T).Name}")
            .Select(i => AssetDatabase.GUIDToAssetPath(i))
            .Select(i => AssetDatabase.LoadAssetAtPath<T>(i));
    }

}
