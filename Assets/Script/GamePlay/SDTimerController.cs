using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LuaFramework;
using UnityEngine;

public class SDTimerController : MonoBehaviour
{
    private static SDTimerController _ins;

    public static SDTimerController Ins
    {
        get
        {
            if (!_ins)
            {
                _ins = FindObjectOfType<SDTimerController>();
                if (!_ins)
                {
                    _ins = new GameObject("SDTimer").AddComponent<SDTimerController>();
                }

                return _ins;
            }

            return _ins;
        }
    }

    public static long IncrementId;
    private const float StepCounter = 0.2f;
    private float timeOnPause = 0;
    private static readonly WaitForSeconds StepWait = new WaitForSeconds(StepCounter);

    private readonly List<SDTimer> timers = new List<SDTimer>();

    public void AddTimer(SDTimer timer)
    {
        if (timers.All(s => s.id != timer.id))
        {
            timers.Add(timer);
            timer.timerCt = Executors.RunOnCoroutineReturn(ISDTimer(timer));
        }
        else
        {
            timer.timeLeft = timer.countTime;
            if (timer.timerCt != null)
            {
                Executors.StopCoroutine(timer.timerCt);
            }

            timer.timerCt = Executors.RunOnCoroutineReturn(ISDTimer(timer));
        }
    }

    public void RemoveTimer(SDTimer t)
    {
        timers.Remove(t);
    }

    private IEnumerator ISDTimer(SDTimer t)
    {
        while (t.timeLeft > 0)
        {
            t.running = true;
            yield return StepWait;
            t.timeLeft -= StepCounter;
        }

        t.running = false;
        t.callBack?.Invoke();
        Ins.RemoveTimer(t);
    }

#if !UNITY_EDITOR
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            timeOnPause = Time.realtimeSinceStartup;
        }
        else
        {
            var currentTime = Time.realtimeSinceStartup;
            var pauseTime = currentTime - timeOnPause;
            foreach (var t in timers)
            {
                if (!t.isIgnorePauseTime)
                {
                    t.timeLeft -= pauseTime;
                }
            }
        }
    }
#endif
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            timeOnPause = Time.realtimeSinceStartup;
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            var currentTime = Time.realtimeSinceStartup;
            var pauseTime = currentTime - timeOnPause;
            Debug.LogError("UnPause: " + pauseTime);
            foreach (var t in timers)
            {
                if (!t.isIgnorePauseTime)
                {
                    t.timeLeft -= pauseTime;
                }
            }
        }
    }
#endif
}

public class SDTimer
{
    public long id;
    public bool isIgnorePauseTime;
    public bool running;
    public Coroutine timerCt;
    public float countTime;
    public float timeLeft;
    public Action callBack;

    public SDTimer(float timeDelay, bool _isIgnorePauseTime = false)
    {
        id = SDTimerController.IncrementId++;
        countTime = timeDelay;
        timeLeft = timeDelay;
        running = false;
        isIgnorePauseTime = _isIgnorePauseTime;
    }

    public void AddEvent(Action _event)
    {
        callBack = _event;
    }

    public void StartTimer()
    {
        SDTimerController.Ins.AddTimer(this);
    }

    public void StopTimer()
    {
        if (timerCt != null)
        {
            Executors.StopCoroutine(timerCt);
        }

        running = false;
        SDTimerController.Ins.RemoveTimer(this);
    }

    public void ResetTimer()
    {
        timeLeft = countTime;
    }
}