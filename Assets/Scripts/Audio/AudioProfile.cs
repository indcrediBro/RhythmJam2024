using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioProfile : MonoBehaviour
{
    private AudioSource m_source;

    [SerializeField] private string m_name;
    public string GetName() { return m_name; }

    [SerializeField] private AudioClipType m_clipType;

    [Range(0, 1)]
    [SerializeField] private float volume = 0.8f;

    private bool m_randomizePitch;
    private float m_minRandPitch = 0.8f;
    private float m_maxRandPitch = 1.2f;

    private void Awake()
    {
        m_source = GetComponent<AudioSource>();
        m_source.enabled = false;
    }

    private void Update()
    {
        if (m_clipType == AudioClipType.Music)
        {
            if (m_name != "BGM_0" && m_source.isPlaying)
            {
                BeatManager.I.GetMainMusicAudioSource().time = m_source.time;
            }
        }
    }

    public void InitializeProfile(string _name, AudioClip _clip, bool _playOnAwake, bool _loop, bool _randomize, float _volume,  AudioClipType _type, AudioMixerGroup _mixerGroup)
    {
        m_name = _name;

        m_source.clip = _clip;
        m_source.playOnAwake = _playOnAwake;
        m_source.loop = _loop;
        m_source.volume = _volume;
        m_source.outputAudioMixerGroup = _mixerGroup;

        m_randomizePitch = _randomize;

        m_source.enabled = true;
        InitializeClipType(_type);
    }

    private void InitializeClipType(AudioClipType _type)
    {
        m_clipType = _type;

        //Todo: Implement Multi-Channel Output System
        switch (m_clipType)
        {
            case AudioClipType.Music:
                break;
            case AudioClipType.SoundEffect:
                break;
            case AudioClipType.UserInterface:
                break;
            default:
                break;
        }

    }

    public void PlayAudio()
    {
        switch (m_clipType)
        {
            case AudioClipType.SoundEffect:
                PlayOneShot();
                break;
            case AudioClipType.UserInterface:
                PlayOneShot();
                break;
            case AudioClipType.Music:
                //PlayMusic();
                break;
        }
    }

    public void Stop()
    {
        m_source.Stop();
    }
    public void Pause()
    {
        m_source.Pause();
    }

    private void PlayOneShot()
    {
        if (m_randomizePitch)
        {
            m_source.pitch = Random.Range(m_minRandPitch, m_maxRandPitch);
            m_source.PlayOneShot(m_source.clip);
        }
        else
        {
            m_source.PlayOneShot(m_source.clip);
        }
    }

    private void PlayMusic()
    {
        m_source.Play();
    }

    public void PlayScheduled()
    {
        //float timeToPlay = BeatManager.I.GetSourceTime() - m_source.clip.length;
        //m_source.PlayScheduled((float)timeToPlay);

        // Calculate the time to play based on the current beat time
        float timeToPlay = BeatManager.I.GetSourceTime();

        // Uncomment the line below if you want to adjust for the audio clip length
        timeToPlay -= m_source.clip.length;

        // Schedule the playback
        m_source.PlayScheduled(AudioSettings.dspTime + timeToPlay);
    }
}
