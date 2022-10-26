using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    // 사운드 재생 객체
    public AudioSource[] efxSource;
    
    public AudioSource musicSource;
    
    public float lowPitRange = .55f;
    public float highPitRange = .75f;
    //public float volume = 0.75f;

    // 싱글톤
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // 재생
    public void PlaySingle(AudioClip clip)
    {
        efxSource[0].clip = clip;
        efxSource[0].Play();
    }

    // 클립
    // params
    public void RandomizeSfx(float volume, params AudioClip [] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitRange, highPitRange);

        for (int i = 0; i < efxSource.Length; i++)
        {
            if (efxSource[i].isPlaying)
                continue;

            efxSource[i].pitch = randomPitch;
            efxSource[i].clip = clips[randomIndex];
            efxSource[i].volume = volume;
            efxSource[i].Play();
            break;
        }
    }
}
