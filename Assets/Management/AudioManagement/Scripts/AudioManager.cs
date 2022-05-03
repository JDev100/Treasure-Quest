using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public List<AudioMixerGroup> outputGroups;


    [Range(0, 1)]
    [SerializeField] private float globalMusicVolume = 1;
    [Range(0, 1)]
    [SerializeField] private float globalSFXVolume = 1;

    [HideInInspector]
    public static AudioManager audioManager;

    //[SerializeField]
    // Sound[] sounds;

    // NOTE: SUGGESTED CHANNEL LAYOUT:
    // Channel 0: Music Primary (Peristent)
    // Channel 1: Music Secondary (Peristent)
    // Channel 2: monoSFX 0
    // Channel 3: monoSFX 1
    // Channel 4: monoSFX 2
    // Channel 5: auralSFX 0
    // Channel 6: auralSFX 1
    // Channel 7: auralSFX 2
    // Channel 8: directionalSFX 0
    // Channel 9: directionalSFX 1
    // Channel 10: directionalSFX 2
    // Channel 11: ambience 0


    [Header("AudioChannel")]
    [SerializeField] private List<AudioChannelType> audioChannelSetup = new List<AudioChannelType>();
    //private List<AudioChannel> inactiveAudioChannels = new List<AudioChannel>();
    private List<AudioChannel> audioChannels = new List<AudioChannel>();

    /***********************************************/
    /***********************************************/
    /*          AWAKE FUNCTION                     */
    /***********************************************/
    /***********************************************/
    void Awake()
    {

        //Singleton stuff
        if (audioManager == null)
        {
            audioManager = this;
        }
        else
        {
            Destroy(audioManager);
        }
    }

    /***********************************************/
    /***********************************************/
    /*          START FUNCION                      */
    /***********************************************/
    /***********************************************/
    private void Start()
    {
        // for (int i = 0; i < sounds.Length; i++)
        // {
        //     GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
        //     _go.transform.SetParent(this.transform);
        //     sounds[i].SetSource(_go.AddComponent<AudioSource>());

        // }

        //Initialize audio channels
        for (int i = 0; i < audioChannelSetup.Count; i++)
        {
            GameObject channelObject = new GameObject(string.Format("AudioChannel_" + audioChannelSetup[i]),
                                                        new System.Type[] { typeof(AudioSource), typeof(AudioChannel) });
            if (channelObject)
            {
                AudioSource audioSource = channelObject.GetComponent<AudioSource>();
                AudioChannel audioChannel = channelObject.GetComponent<AudioChannel>();

                audioSource.outputAudioMixerGroup = outputGroups[i];
                audioChannel.SetAudioChannel(audioSource, audioChannelSetup[i]);
                channelObject.transform.SetParent(this.transform);

                audioChannels.Add(audioChannel);
            }
        }
    }

    /***********************************************/
    /***********************************************/
    /*          CHANNEL MANAGEMENT                 */
    /***********************************************/
    /***********************************************/

    public AudioChannel FindNextAvailableChannel(AudioChannelType channelType)
    {
        //AudioChannel audioChannel = null;

        for (int i = 0; i < audioChannels.Count; i++)
        {
            if (!audioChannels[i].isChannelActive && audioChannels[i].ChannelType == channelType)
            {
                return audioChannels[i];
            }
        }

        return null;
    }

    public void FreeChannels()
    {
        for (int i = 0; i < this.audioChannels.Count; i++)
        {
            if (!audioChannels[i].AudioSource.isPlaying && audioChannels[i].isChannelActive)
            {
                audioChannels[i].isChannelActive = false;
            }
        }
    }

    /***********************************************/
    /***********************************************/
    /*          REQUEST FUNCTIONS                  */
    /***********************************************/
    /***********************************************/
    public void SetGlobalFromSaveData()
    {
        globalMusicVolume = GameplayData.instance.globalMusicVolume;
        globalSFXVolume = GameplayData.instance.globalSFXVolume;
        AdjustLevelsForNewGlobalVolume();
    }
    public void SetGlobalMusicVolume(float volume)
    {
        if (globalMusicVolume != volume)
        {
            globalMusicVolume = volume;
            AdjustLevelsForNewGlobalVolume();
        }
    }
    public void SetGlobalSFXVolume(float volume)
    {
        if (globalMusicVolume != volume)
        {
            globalMusicVolume = volume;

            AdjustLevelsForNewGlobalVolume();
        }
    }

    void AdjustLevelsForNewGlobalVolume()
    {
        foreach (AudioChannel channel in audioChannels)
        {
            if (channel.ChannelType == AudioChannelType.music)
                channel.AudioSource.volume = globalMusicVolume;
            else
                channel.AudioSource.volume = channel.AudioSource.volume * globalSFXVolume;
        }
    }

    public void SetChannelVolume(ref AudioChannel channel, float volume)
    {
        float newVolume = (channel.ChannelType == AudioChannelType.music) ? globalMusicVolume : globalSFXVolume;
        channel.AudioSource.volume = volume * newVolume;
    }

    public void PlaySound(Sound sound)
    {
        StopSound(sound.clip, sound.channelType);

        FreeChannels();

        AudioChannel audioChannel = FindNextAvailableChannel(sound.channelType);

        if (audioChannel != null)
        {
            audioChannel.AudioSource.clip = sound.clip;
            audioChannel.AudioSource.loop = sound.loop;
            audioChannel.AudioSource.pitch = sound.pitch;
            SetChannelVolume(ref audioChannel, sound.volume);
            audioChannel.isChannelActive = true;
            audioChannel.AudioSource.Play();

            //audioChannels.Add(audioChannel);
        }
    }

    public void PlayMusic(Sound sound)
    {
        StopSound(sound.clip, sound.channelType);

        FreeChannels();

        StopSound(sound.channelType);

        AudioChannel audioChannel = FindNextAvailableChannel(sound.channelType);

        if (audioChannel != null)
        {
            audioChannel.AudioSource.clip = sound.clip;
            audioChannel.AudioSource.loop = sound.loop;
            audioChannel.AudioSource.pitch = sound.pitch;
            SetChannelVolume(ref audioChannel, sound.volume);
            audioChannel.isChannelActive = true;
            audioChannel.AudioSource.Play();

            //audioChannels.Add(audioChannel);
        }
    }

    public void PauseMusic() {
        for (int i = 0; i < audioChannels.Count; i++)
        {
            if (audioChannels[i].ChannelType == AudioChannelType.music)
            {
                if (audioChannels[i].isChannelActive && audioChannels[i].AudioSource.isPlaying)
                    audioChannels[i].AudioSource.Pause();
            }
        }
    }

    public void UnPauseMusic() {
           for (int i = 0; i < audioChannels.Count; i++)
        {
            if (audioChannels[i].ChannelType == AudioChannelType.music)
            {
                if (audioChannels[i].isChannelActive && !audioChannels[i].AudioSource.isPlaying)
                    audioChannels[i].AudioSource.UnPause();
            }
        }
    }

    // public void PauseSound(AudioClip audio, AudioChannelType channelType) {

    // }

    public void StopSound(AudioClip audio, AudioChannelType channelType)
    {
        FreeChannels();

        for (int i = 0; i < audioChannels.Count; i++)
        {
            if (audioChannels[i].AudioSource.clip == audio)
            {
                if (audioChannels[i].isChannelActive && audioChannels[i].AudioSource.isPlaying)
                    audioChannels[i].AudioSource.Stop();
                audioChannels[i].ClearAudio();
            }
        }

        // //FindAndRemoveChannelByClip(audio);
        // if (audioChannel != null && audioChannel.ChannelType == channelType)
        // {
        //     Debug.Log("Stopped");
        //     audioChannel.AudioSource.Stop();
        //     //audioChannel.
        //     
        // inactiveAudioChannels.Add(audioChannel);
        // }
    }
    public void StopSound(AudioChannelType channelType)
    {
        FreeChannels();

        for (int i = 0; i < audioChannels.Count; i++)
        {
            if (audioChannels[i].ChannelType == channelType)
            {
                if (audioChannels[i].isChannelActive && audioChannels[i].AudioSource.isPlaying)
                    audioChannels[i].AudioSource.Stop();
                audioChannels[i].ClearAudio();
            }
        }

        // //FindAndRemoveChannelByClip(audio);
        // if (audioChannel != null && audioChannel.ChannelType == channelType)
        // {
        //     Debug.Log("Stopped");
        //     audioChannel.AudioSource.Stop();
        //     //audioChannel.
        //     
        // inactiveAudioChannels.Add(audioChannel);
        // }
    }

    // public void PlaySound(string _name)
    // {
    //     for (int i = 0; i < sounds.Length; i++)
    //     {
    //         if (sounds[i].name == _name)
    //         {
    //             sounds[i].Play();
    //             return;
    //         }
    //     }

    //     Debug.LogWarning("Audio Manager: Sound not found in library: " + name);
    // }
}

/***********************************************/
/***********************************************/
/*          ESSENTIAL CLASSES                  */
/***********************************************/
/***********************************************/

[System.Serializable]
public class Sound
{
    [Header("Demographics")]
    public string name;
    public AudioClip clip;
    public AudioChannelType channelType = AudioChannelType.monoSFX;

    [Header("Audio Settings")]
    [Range(0, 1)]
    public float volume = 1;
    [Range(-3, 3)]
    public float pitch = 1;
    public bool loop = false;

    private AudioSource source;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
    }

    // public void Play()
    // {
    //     source.volume = volume;
    //     source.pitch = 1f;
    //     source.Play();
    // }
}

[System.Serializable]
public class AudioChannel : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioChannelType channelType = AudioChannelType.monoSFX;

    public AudioSource AudioSource { get { return audioSource; } }
    public AudioChannelType ChannelType { get { return channelType; } }

    //[HideInInspector]
    public bool isChannelActive;

    public void SetAudioChannel(AudioSource audioSource, AudioChannelType channelType)
    {
        this.audioSource = audioSource;
        this.channelType = channelType;
    }

    public void ClearAudio()
    {
        //this.audioSource = null;
        audioSource.Stop();
        isChannelActive = false;
    }
}

public enum AudioChannelType
{
    music,
    monoSFX,
    auralSFX,
    directionalSFX,
    ambience,
    Size
}