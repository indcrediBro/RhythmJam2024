using System;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public event Action OnGamePaused;
    public event Action OnGameUnPaused;

    private void Update()
    {
        Application.targetFrameRate = 60;
    }
    public void OnGamePaused_Invoke()
    {
        OnGamePaused?.Invoke();
    }

    public void OnGameUnPaused_Invoke()
    {
        OnGameUnPaused?.Invoke();
    }
}
