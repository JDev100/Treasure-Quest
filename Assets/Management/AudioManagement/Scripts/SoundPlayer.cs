using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public Sound[] sounds;
    public void PlaySound(string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (name == sounds[i].name)
            {
                AudioManager.audioManager.PlaySound(sounds[i]);
                return;
            }
        }
    }
    public void PlaySound(Sound sound) {
       // Debug.Log(sound.name);
        AudioManager.audioManager.PlaySound(sound);
    }

    public void StopSound(Sound sound) {
        AudioManager.audioManager.StopSound(sound.clip, sound.channelType);
    }
}
