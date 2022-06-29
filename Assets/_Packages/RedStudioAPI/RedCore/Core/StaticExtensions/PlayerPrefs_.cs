using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefs_
{
    public static bool GetBool(string name, bool defaultValue = default)
            => PlayerPrefs.GetInt(name, defaultValue ? 1 : 0) == 1 ? true : false;
    public static void SetBool(string name, bool value)
        => PlayerPrefs.SetInt(name, value ? 1 : 0);

}
