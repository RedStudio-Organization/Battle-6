using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShaderAnimation : MonoBehaviour
{
    #region InternalType
    enum ShaderParamType { Null=-1, Float=0 }
    [Serializable] class ShaderAnimInfo
    {
        public string Label;

        public Renderer Target;
        public string ShaderName;
        public string ParamName;
        public ShaderParamType ParamType;
        public AnimationCurve ParamProgression;

        public float ResetParam;
    }
    #endregion

    [SerializeField, BoxGroup("Conf")] ShaderAnimInfo[] _animations;

    List<(string, Coroutine)> _launchedEffects;

    private void Awake()
    {
        _launchedEffects = new List<(string, Coroutine)>();
    }

    public void LaunchEffect(string label)
    {
        if (_launchedEffects.Count(i => i.Item1 == label) > 0) return;
        var fxToLaunch = _animations.FirstOrDefault(i => i.Label == label);
        if(fxToLaunch == null)
        {
            Debug.LogWarning($"[ShaderAnimation] Can't find effect named {label}", this);
            return;
        }

        var c = StartCoroutine(ShaderFX(fxToLaunch));
        _launchedEffects.Add((label, c));

        IEnumerator ShaderFX(ShaderAnimInfo shaderAnim)
        {
            var mat = shaderAnim.Target.materials.FirstOrDefault(i => i.shader.name == shaderAnim.ShaderName);
            if(mat == null) 
            { 
                Debug.LogWarning($"[ShaderAnimation] Can't find mat named {shaderAnim.ShaderName}", this);
                StopEffect(label);
                yield break; 
            }
            var startTime = Time.time;
            var animationLastKeyTime = shaderAnim.ParamProgression.keys.Last().time;

            do
            {
                var currentTime = (Time.time - startTime);
                var step = (int)(currentTime / animationLastKeyTime);
                currentTime -= step * animationLastKeyTime;

                switch (shaderAnim.ParamType)
                {
                    case ShaderParamType.Float:
                        mat.SetFloat(shaderAnim.ParamName, shaderAnim.ParamProgression.Evaluate(currentTime));
                        break;
                    case ShaderParamType.Null:
                    default:
                        yield break;
                }
                yield return null;

            } while (_launchedEffects.Count(i => i.Item1 == shaderAnim.Label) > 0);

            // Reset initiale parameter
            switch (shaderAnim.ParamType)
            {
                case ShaderParamType.Float:
                    mat.SetFloat(shaderAnim.ParamName, shaderAnim.ResetParam);
                    break;
                case ShaderParamType.Null:
                default:
                    yield break;
            }
            yield break;
        }
    }

    public void StopEffect(string label)
    {
        foreach(var el in _launchedEffects.Where(i => i.Item1 == label).ToArray())
        {
            _launchedEffects.Remove(el);
        }

    }


}
