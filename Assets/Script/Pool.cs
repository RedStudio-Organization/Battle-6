using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public static Pool Singleton { get; private set; } // Bad dirty singleton

    [SerializeField] Transform _root;

    void Awake()
    {
        Singleton = Singleton ?? this;
    }

    public T Create<T>(T go, Vector3 pos, Quaternion rot) where T : MonoBehaviour
    {
        return Instantiate(go.gameObject, pos, rot, _root).GetComponent<T>();
    }


}
