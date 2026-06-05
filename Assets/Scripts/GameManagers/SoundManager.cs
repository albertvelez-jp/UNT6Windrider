using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] audios;
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioClip[] musics;


    public void SelectAudio(int index,float volume)
    {
        sfxSource.PlayOneShot(audios[index], volume);
    }

    public void SelectMusic(int index, float volume)
    {
        musicSource.PlayOneShot(audios[index], volume);
    }
}
