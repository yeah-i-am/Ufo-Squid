using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    public bool loop = false;

    [HideInInspector]
    public AudioSource source;
}

public class SoundController : MonoBehaviour
{
    public Sound[] sounds;
    public Sound[] music;

    private Queue<Sound> musicQueue;

    public int musicFadeSteps = 30;

    [Range(0f, 1f)]
    public float globalVolume = 0.05f;

    private bool musicMute = false;

    private float GlobalMusicVolume => (musicMute ? 0f : globalVolume);

    private Sound currentMusic;

    bool fading = false;

    void Awake()
    {
        musicQueue = new Queue<Sound>();

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
        }

        foreach (Sound s in music)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
        }

        if (music.Length == 0)
        {
            Debug.LogWarning("No music available!");
            return;
        }

        currentMusic = music[0];

        currentMusic.source.volume = currentMusic.volume * GlobalMusicVolume;
        currentMusic.source.Play();

        StartCoroutine("MusicQueueHandler");
    }

    public void PlaySound(string soundName)
    {
        Sound s = Array.Find(sounds, item => item.name == soundName);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.volume = s.volume * globalVolume;

        s.source.Play();
    }

    public void SwitchMusic(string musicName, float repeatTime)
    {
        Sound s = Array.Find(music, item => item.name == musicName);

        if (s == null)
        {
            Debug.LogWarning("Music: " + name + " not found!");
            return;
        }

        musicQueue.Clear();

        StartCoroutine("StartFadeBetwenMusic", new Tuple<Sound, float>(s, repeatTime));
    }

    private IEnumerator StartFadeBetwenMusic(Tuple<Sound, float> param)
    {
        yield return new WaitUntil(() => !fading);

        StartCoroutine("FadeBetwenMusic", param);
    }

    private IEnumerator FadeBetwenMusic( Tuple<Sound, float> param )
    {
        fading = true;
        Sound newMusic = param.Item1;
        float repeatTime = param.Item2;

        if (newMusic == currentMusic)
        {
            fading = false;
            yield break;
        }

        newMusic.source.Play();

        newMusic.source.time = currentMusic.source.time % repeatTime;

        for (int i = 0; i < musicFadeSteps; i++)
        {
            float volume = (float)i / musicFadeSteps;

            newMusic.source.volume = volume * newMusic.volume * GlobalMusicVolume;

            currentMusic.source.volume = (1f - volume) * currentMusic.volume * GlobalMusicVolume;

            yield return null;
        }

        currentMusic.source.Stop();
        currentMusic = newMusic;
        fading = false;
    }

    public void AddTrackToQueue( string musicName )
    {
        Sound s = Array.Find(music, item => item.name == musicName);

        if (s == null)
        {
            Debug.LogWarning("Music: " + name + " not found!");
            return;
        }

        musicQueue.Enqueue(s);
    }

    private IEnumerator MusicQueueHandler()
    {
        Sound oldMusic = null;
        float delay = 0f;

        while (true)
        {
            if (oldMusic != null)
            {
                oldMusic.source.loop = oldMusic.loop;
                oldMusic = null;
            }

            if (musicQueue.Count != 0)
            {
                delay = currentMusic.clip.length - currentMusic.source.time;
                Sound newMusic = musicQueue.Dequeue();

                newMusic.source.PlayDelayed(delay);
                
                oldMusic = currentMusic;
                currentMusic = newMusic;

                currentMusic.source.volume = currentMusic.volume * GlobalMusicVolume;

                oldMusic.source.loop = false;
            }

            yield return new WaitForSeconds(delay);
        }
    }

    public void SetMusicMute(bool mute)
    {
        musicMute = mute;
        currentMusic.source.volume = currentMusic.volume * GlobalMusicVolume;
    }

    void Update()
    {
        
    }
}
