using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Utility 
{
    public static float GetPercentage(float min, float max, float current) => (current - min) / (max - min);
    public static bool HeadsOrTails(float winProba) => UnityEngine.Random.Range(0f, 100f) < winProba;

    public static IEnumerable<Scene> ActiveScenes
    {
        get
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                yield return SceneManager.GetSceneAt(i);
            }
            yield break;
        }
    }
}
