using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum AudioClipType
{
    Music,
    SoundEffect,
    UserInterface
}

[System.Serializable]
public class AudioProfileData
{
    public string name;
    public AudioClip clip;
    public AudioClipType clipType;
    [Range (0f,1f)] public float volume;
    public bool playOnAwake, loop, randomizePitch;
}

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private List<AudioProfileData> m_audioProfileDataList;
    private List<AudioProfile> m_audioProfiles;
    [SerializeField] private AudioMixer m_mixer;
    [SerializeField] private AudioMixerGroup m_musicMixer, m_sfxMixer, m_uiMixer, m_masterMixer;
    [SerializeField] private Slider m_masterSlider, m_musicSlider, m_sfxSlider, m_uiSlider;
    //private List<GameObject> m_audioSourceObjects;

    protected override void Awake()
    {
        base.Awake();
        InitializeAudioSources();

        if (!PlayerPrefs.HasKey("Master"))
        {
            ResetSettings();
        }
    }

    private void InitializeAudioSources()
    {
        if (m_audioProfileDataList.Count > 0)
        {
            m_audioProfiles = new List<AudioProfile>();

            foreach (AudioProfileData _data in m_audioProfileDataList)
            {
                GameObject sfxObject = new GameObject(_data.name, typeof(AudioProfile));
                sfxObject.transform.SetParent(this.transform);

                AudioProfile profile = sfxObject.GetComponent<AudioProfile>();

                profile.InitializeProfile(_data.name, _data.clip, _data.playOnAwake, _data.loop,
                                          _data.randomizePitch, _data.volume, _data.clipType, GetAudioMixerGroup(_data.clipType));

                m_audioProfiles.Add(profile);
            }
        }
    }
    private AudioMixerGroup GetAudioMixerGroup(AudioClipType _clipType)
    {
        if(_clipType == AudioClipType.Music)
        {
            return m_musicMixer;
        }
        else if(_clipType == AudioClipType.SoundEffect)
        {
            return m_sfxMixer;
        }
        else
        {
            return m_uiMixer;
        }
    }

    public bool IsValidName(string _name, out AudioProfile _profile)
    {
        foreach (AudioProfile profile in m_audioProfiles)
        {
            if(profile.GetName() == _name)
            {
                _profile = profile;
                return true;
            }
        }
        _profile = null;
        return false;
    }

    public void PlaySound(string _name)
    {
        if(IsValidName(_name,out AudioProfile _profile))
        {
            _profile.PlayAudio();
        }
    }

    public void PauseSound(string _name)
    {
        if (IsValidName(_name, out AudioProfile _profile))
        {
            _profile.Pause();
        }
    }

    public void StopSound(string _name)
    {
        if (IsValidName(_name, out AudioProfile _profile))
        {
            _profile.Stop();
        }
    }

    public void PauseAllSound()
    {
        foreach (AudioProfile profile in m_audioProfiles)
        {
            profile.Pause();
        }
    }

    public void StopAllSound()
    {
        foreach (AudioProfile profile in m_audioProfiles)
        {
            profile.Stop();
        }
    }

    public void ResetSettings()
    {
        SetMasterVol(0);
        SetMusicVol(0);
        SetSfxVol(0);
        SetUIVol(0);
    }

    public void SetMasterVol()
    {
        m_mixer.SetFloat("Master", m_masterSlider.value);
        PlayerPrefs.SetFloat("Master", m_masterSlider.value);
    }
    private void SetMasterVol(float _volume)
    {
        m_mixer.SetFloat("Master", _volume);
        PlayerPrefs.SetFloat("Master", _volume);
        m_masterSlider.value = PlayerPrefs.GetFloat("Master");
    }

    public void SetMusicVol()
    {
        m_mixer.SetFloat("Music", m_musicSlider.value);
        PlayerPrefs.SetFloat("Music", m_musicSlider.value);
    }
    private void SetMusicVol(float _volume)
    {
        m_mixer.SetFloat("Music", _volume);
        PlayerPrefs.SetFloat("Music", _volume);
        m_musicSlider.value = PlayerPrefs.GetFloat("Music");
    }

    public void SetSfxVol()
    {
        m_mixer.SetFloat("SFX", m_sfxSlider.value);
        PlayerPrefs.SetFloat("SFX", m_sfxSlider.value);
    }
    private void SetSfxVol(float volume)
    {
        m_mixer.SetFloat("SFX", volume);
        PlayerPrefs.SetFloat("SFX", volume);
        m_sfxSlider.value = PlayerPrefs.GetFloat("SFX");
    }

    public void SetUIVol()
    {
        m_mixer.SetFloat("UI", m_uiSlider.value);
        PlayerPrefs.SetFloat("UI", m_uiSlider.value);
    }
    private void SetUIVol(float volume)
    {
        m_mixer.SetFloat("UI", volume);
        PlayerPrefs.SetFloat("UI", volume);
        m_uiSlider.value = PlayerPrefs.GetFloat("UI");
    }


}
