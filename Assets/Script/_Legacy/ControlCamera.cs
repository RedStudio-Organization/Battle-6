using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCamera : MonoBehaviour
{
    #region InternalTypes
    [Serializable] class ImpulseType
    {
        public AnimationCurve Amplitude;
    }
    #endregion


    [SerializeField] CameraReference _cam;

    [Header("Impulses")]
    [SerializeField] ImpulseType[] _impulses;

    public void LaunchImpulse(int impulseIndex) => Impulse(_impulses[impulseIndex]);
    
    void Impulse(ImpulseType it)
    {
        var noise = _cam.Acquire().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        StartCoroutine(NoiseImpulse());

        IEnumerator NoiseImpulse()
        {
            var startTime = Time.time;
            var length = it.Amplitude.keys.Last().time;
            var currentTimer = Time.time - startTime;

            while(currentTimer < length)
            {
                yield return null;
                currentTimer = Time.time - startTime;
                noise.m_AmplitudeGain = it.Amplitude.Evaluate(Time.time - startTime);
            }
            noise.m_AmplitudeGain = 0f;
        }
    }

}
