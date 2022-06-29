using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTimeCountDown
{
    enum InternalState { NULL=-1, Counting = 0, Paused=1, Finished=2 }

    float _savedTime;
    float _duration;

    InternalState _state;
    Coroutine _countingRoutine;
    Trigger _pause;
    Trigger _resume;
    MonoBehaviour _host;

    public float CumulatedTime => _savedTime;
    public float RemainingTime => _state == InternalState.Finished ? 0 : _duration - _savedTime;
    public bool IsRunning => _state == InternalState.Counting;
    public bool IsPaused => _state == InternalState.Paused;
    public bool IsFinished => _state == InternalState.Finished;
    public float Progress => _state == InternalState.Finished ? 1 : _savedTime / _duration;

    public void Pause() => _pause.Activate();
    public void Resume() => _resume.Activate();

    public RealTimeCountDown(MonoBehaviour host, float duration)
    {
        _host = host;
        _duration = duration;
        _pause = new Trigger();
        _resume = new Trigger();
        StartProcess();
    }
    public void ResetValues() => StartProcess();

    void StartProcess()
    {
        // Init Struct
        _savedTime = 0.0001f;
        _state = InternalState.Counting;
        
        if(_countingRoutine != null) _host.StopCoroutine(_countingRoutine);
        _countingRoutine = _host.StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        float lastTick = 0f;
        while (true)
        {
            lastTick = Time.realtimeSinceStartup;
            yield return null;

            if (_savedTime >= _duration)
            {
                _state = InternalState.Finished;
                break;
            }
            else if ((_pause.IsActivated() && _state == InternalState.Counting))
            {
                _state = InternalState.Paused;
                continue;
            }
            else if (_resume.IsActivated() && _state == InternalState.Paused)
            {
                _state = InternalState.Counting;
                continue;
            }
            else if (_state == InternalState.Counting)
            {
                _savedTime += Time.realtimeSinceStartup - lastTick;
            }
        }

        _countingRoutine = null;
        yield break;
    }

}
