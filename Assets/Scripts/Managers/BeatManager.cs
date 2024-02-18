using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum BeatType
{
    None,
    Beat,
    HalfBeat,
    QuarterBeat,
    EighthBeat,
    SixteenthBeat
}

[System.Serializable]
public class Intervals
{
    [SerializeField] private BeatType m_beatType;

    private int m_lastInterval;

    public BeatType GetBeatType() { return m_beatType; }
}

public class BeatManager : Singleton<BeatManager>
{
    private AudioSource m_audioSource;
    public AudioSource GetMainMusicAudioSource()
    {
        return m_audioSource;
    }

    public void InitializeAudioSource(AudioSource _source) { m_audioSource = _source; }

    [SerializeField] private float m_bpm = 120f;
    public float GetBPM() { return m_bpm; }
    [SerializeField] private Intervals[] m_intervals;

    private float sourceTime;
    public float GetSourceTime() { return sourceTime; }

    private float m_secondsPerBeat;
    private float m_nextBeatTime;
    public float GetNextBeatTime() { return m_nextBeatTime; }
    private int m_lastInterval;
    public int GetLastIntervalCount() { return m_lastInterval; }

    public Action OnBeat;
    public Action OnHalfBeat;
    public Action OnQuarterBeat;
    public Action OnEighthBeat;
    public Action OnSixteenthBeat;

    private void Start()
    {
        m_secondsPerBeat = GetIntervalLength(BeatType.HalfBeat);
        m_lastInterval = 0;
    }

    private void Update()
    {
        if (m_audioSource)
        {
            NotifyBeats();
            CalculateNextBeat();
        }
    }

    public float GetIntervalLength(BeatType _beatType)
    {
        switch (_beatType)
        {
            case BeatType.Beat:
                return 60 / (m_bpm * 1f);
            case BeatType.HalfBeat:
                return 60 / (m_bpm * 0.5f);
            case BeatType.QuarterBeat:
                return 60 / (m_bpm * 0.25f);
            case BeatType.EighthBeat:
                return 60 / (m_bpm * 0.125f);
            case BeatType.SixteenthBeat:
                return 60 / (m_bpm * 0.0625f);
            default:
                return 0f;
        }
    }

    public void CheckForNewInterval(float _interval, Action _beatHandler)
    {
        if (Mathf.FloorToInt(_interval) != m_lastInterval)
        {
            m_lastInterval = Mathf.FloorToInt(_interval);
            _beatHandler?.Invoke();
        }
    }

    private void NotifyBeats()
    {
        foreach (Intervals interval in m_intervals)
        {
            sourceTime = (m_audioSource.timeSamples / (m_audioSource.clip.frequency * GetIntervalLength(interval.GetBeatType())));

            switch (interval.GetBeatType())
            {
                case BeatType.Beat:
                    CheckForNewInterval(sourceTime, OnBeat);
                    break;
                case BeatType.HalfBeat:
                    CheckForNewInterval(sourceTime, OnHalfBeat);
                    break;
                case BeatType.QuarterBeat:
                    CheckForNewInterval(sourceTime, OnQuarterBeat);
                    break;
                case BeatType.EighthBeat:
                    CheckForNewInterval(sourceTime, OnEighthBeat);
                    break;
                case BeatType.SixteenthBeat:
                    CheckForNewInterval(sourceTime, OnSixteenthBeat);
                    break;
                default:
                    break;
            }

        }
    }

    private void CalculateNextBeat()
    {
        //double currentTime = AudioSettings.dspTime;

        if (sourceTime >= m_nextBeatTime)
        {
            m_nextBeatTime += m_secondsPerBeat;
        }

        if (m_nextBeatTime >= (m_audioSource.clip.length / m_audioSource.clip.frequency))
        {
            m_nextBeatTime = 0;
        }
    }

    public static void SubscribeToBeatEvent(Action beatHandler)
    {
        BeatManager.I.OnBeat += beatHandler;

    }
    public static void UnsubscribeFromBeatEvent(Action beatHandler)
    {
        BeatManager.I.OnBeat -= beatHandler;
    }

    public static void SubscribeToHalfBeatEvent(Action beatHandler)
    {
        BeatManager.I.OnHalfBeat += beatHandler;

    }
    public static void UnsubscribeFromHalfBeatEvent(Action beatHandler)
    {
        BeatManager.I.OnHalfBeat -= beatHandler;
    }

    public static void SubscribeToQuarterBeatEvent(Action beatHandler)
    {
        BeatManager.I.OnQuarterBeat += beatHandler;

    }
    public static void UnsubscribeFromQuarterBeatEvent(Action beatHandler)
    {
        BeatManager.I.OnQuarterBeat -= beatHandler;
    }

    public static void SubscribeToEighthBeatEvent(Action beatHandler)
    {
        BeatManager.I.OnEighthBeat += beatHandler;

    }
    public static void UnsubscribeFromEighthBeatEvent(Action beatHandler)
    {
        BeatManager.I.OnEighthBeat -= beatHandler;
    }

    public static void SubscribeToSixteenthBeatEvent(Action beatHandler)
    {
        BeatManager.I.OnSixteenthBeat += beatHandler;

    }
    public static void UnsubscribeFromSixteenthBeatEvent(Action beatHandler)
    {
        BeatManager.I.OnSixteenthBeat -= beatHandler;
    }

}
