using UnityEngine.Audio;
using System;
using UnityEngine;
//This script is used to access sounds while in other scripts and provides paramaters that can be edited in the AudioManager 
//gameObject which has this script attached to it.
public class AudioManagerScript : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManagerScript instance;
    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip=s.clip;
            s.source.volume=s.volume;
            s.source.pitch=s.pitch;
            s.source.loop=s.loop;
        }
    }
    void OnEnable()
    {
        instance=this;
    }
    public void Play(string name)
    {
        Sound s =  Array.Find(sounds, sound=>sound.name==name);
        if(s==null)
        {
            return;
        }
        s.source.Play();
    }
    public void Stop(string name)
    {
        Sound s =  Array.Find(sounds, sound=>sound.name==name);
        if(s==null)
        {
            return;
        }
        s.source.Stop();
    }
}
