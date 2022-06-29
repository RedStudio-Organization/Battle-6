using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension
{

    /// <summary>
    /// Checks if a GameObject has been destroyed.
    /// Source : http://answers.unity.com/answers/1001265/view.html
    /// </summary>
    /// <param name="gameObject">GameObject reference to check for destructedness</param>
    /// <returns>If the game object has been marked as destroyed by UnityEngine</returns>
    public static bool IsDestroyed(this GameObject gameObject)
        => gameObject == null && !ReferenceEquals(gameObject, null);
    public static bool IsDestroyed(this UnityEngine.Object @this)
        => @this == null && !ReferenceEquals(@this, null);

    public static void SelfSafeDestroy(this UnityEngine.GameObject @this)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) GameObject.DestroyImmediate(@this);
        else
#endif
        {
            GameObject.Destroy(@this);
        }
    }

    public static GameObject InstantiatePrefabOrGameObject(this GameObject @this, GameObject prefab, Transform root)
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            return UnityEditor.PrefabUtility.InstantiatePrefab(prefab, root) as GameObject;
        }
#endif
        return GameObject.Instantiate(prefab, root);
    }

}
