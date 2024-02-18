using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SongManager : Singleton<SongManager>
{
    [System.Serializable]
    private class Song
    {
        public string name;
        public int id;
        public SongData[] songLayers;
    }

    [System.Serializable]
    private class SongData
    {
        public AudioClip clip;
        [Range(0,1)] public float volume;
    }

    [SerializeField] private AudioMixerGroup m_musicMixer;
    [SerializeField] private List<Song> m_allSongs;
    //public void AddAudioSourceToMusicSource(AudioSource _audioSource) { m_allMusicSources.Add(_audioSource); }
    private Song m_currentSong;
    private List<AudioSource> m_currentSongLayerSources;
    private int m_currentActiveLayers = 0;

    protected override void Awake()
    {
        base.Awake();
        m_currentSongLayerSources = new List<AudioSource>();
        LoadSong(0);
    }
    private Song GetSong(string _name)
    {
        if(m_allSongs.Count>0)
        {
            for (int i = 0; i < m_allSongs.Count; i++)
            {
                if(m_allSongs[i].name == _name)
                {
                    return m_allSongs[i];
                }
            }
        }
        return null;
    }
    private Song GetSong(int _id)
    {
        if (m_allSongs.Count > 0)
        {
            for (int i = 0; i < m_allSongs.Count; i++)
            {
                if (m_allSongs[i].id == _id)
                {
                    return m_allSongs[i];
                }
            }
        }
        return null;
    }

    private void LoadSong(string _name)
    {
        if (m_currentSongLayerSources.Count > 0)
        {
            foreach (var source in m_currentSongLayerSources)
            {
                Destroy(source.gameObject);
            }
        }
        m_currentSongLayerSources = new List<AudioSource>();

        if(m_currentSong!=null)
        {
            Destroy(GameObject.Find(m_currentSong.name));
        }
        m_currentSong = GetSong(_name);
        GameObject currentSongObject = new GameObject(m_currentSong.name);

        for (int i = 0; i < m_currentSong.songLayers.Length; i++)
        {
            SongData layerData = m_currentSong.songLayers[i];

            GameObject newObject = new GameObject(layerData.clip.name,typeof(AudioSource));

            AudioSource newSource = newObject.GetComponent<AudioSource>();
            newSource.playOnAwake = false;
            newSource.loop = true;
            newSource.clip = layerData.clip;
            newSource.volume = layerData.volume;
            newSource.outputAudioMixerGroup = m_musicMixer;

            m_currentSongLayerSources.Add(newSource);
        }

        StartInitialPlayback();
    }
    private void LoadSong(int _id)
    {
        if (m_currentSongLayerSources.Count > 1)
        {
            foreach (var source in m_currentSongLayerSources)
            {
                Destroy(source.gameObject);
            }
        }
        m_currentSongLayerSources = new List<AudioSource>();

        if (m_currentSong != null)
        {
            Destroy(GameObject.Find(m_currentSong.name));
        }
        m_currentSong = GetSong(_id);
        GameObject currentSongObject = new GameObject(m_currentSong.name);

        for (int i = 0; i < m_currentSong.songLayers.Length; i++)
        {
            SongData layerData = m_currentSong.songLayers[i];

            GameObject newObject = new GameObject(layerData.clip.name, typeof(AudioSource));
            newObject.transform.SetParent(currentSongObject.transform);
            AudioSource newSource = newObject.GetComponent<AudioSource>();
            newSource.playOnAwake = false;
            newSource.loop = true;
            newSource.clip = layerData.clip;
            newSource.volume = layerData.volume;
            newSource.outputAudioMixerGroup = m_musicMixer;

            m_currentSongLayerSources.Add(newSource);
        }

        StartInitialPlayback();
    }

    private void StartInitialPlayback()
    {
        foreach (var aSource in m_currentSongLayerSources)
        {
            if (aSource.transform.GetSiblingIndex() != 0)
            {
                BeatManager.I.InitializeAudioSource(aSource);
                aSource.volume = 0f;
            }
            aSource.Play();
        }
        m_currentActiveLayers = 1;
    }

    public void AddMoreMusicLayer()
    {
        if (m_currentActiveLayers < m_currentSongLayerSources.Count)
        {
            m_currentSongLayerSources[m_currentActiveLayers].volume = m_currentSong.songLayers[m_currentActiveLayers].volume;
            m_currentActiveLayers++;
        }
    }

    public void RemoveMusicLayer()
    {
        if (m_currentActiveLayers > 1)
        {
            m_currentSongLayerSources[m_currentActiveLayers].volume = 0;
            m_currentActiveLayers--;
        }
    }
}
