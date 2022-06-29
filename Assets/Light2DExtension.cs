using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class Light2DExtension : MonoBehaviour
    {

        [SerializeField] UnityEngine.Rendering.Universal.Light2D _light;

        public void ColorLerpToWhite(float duration, float intensityDestination) // Just some manual tweening
        {
            StartCoroutine(LerpRoutine());
            IEnumerator LerpRoutine()
            {
                var startIntensity = _light.intensity;
                var startColor = _light.color;
                var endIntensity = intensityDestination;
                var endColor = Color.white;

                SpecialCountDown scd = new SpecialCountDown(duration);
                while(!scd.isDone)
                {
                    yield return null;
                    _light.color = Color.Lerp(startColor, endColor, scd.Progress);
                    _light.intensity = Mathf.Lerp(startIntensity, endIntensity, scd.Progress);
                }
                _light.color = endColor;
                _light.intensity = endIntensity;
                yield break;
            }
        }


    }
}
