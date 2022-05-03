using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonBehavior : MonoBehaviour
{
    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
        //LevelManager.instance.StartLevel();
    }
    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void PlaySFX(AudioClip sound)
    {

        Sound audio = new Sound();
        audio.name = sound.name;
        audio.clip = sound;
        audio.channelType = AudioChannelType.monoSFX;
        audio.volume = 1;
        audio.loop = false;
        AudioManager.audioManager.PlaySound(audio);

    }
    public void PlayMusic(AudioClip song)
    {
        Sound audio = new Sound();
        audio.name = song.name;
        audio.clip = song;
        audio.channelType = AudioChannelType.music;
        audio.volume = 1;
        audio.loop = true;
        AudioManager.audioManager.PlayMusic(audio);
    }
    public void PauseMusic() {
        AudioManager.audioManager.PauseMusic();
    }
}
