using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioData> sfxList;

    [SerializeField] AudioSource musicManager;
    [SerializeField] AudioSource sfxManager;

    [SerializeField] float durationOfFade = 0.5f;
    private float originalMusicVolume;

    private Dictionary<AudioId, AudioData> sfxBank;

    //Singleton 
    public static AudioManager i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    private void Start()
    {
        originalMusicVolume = musicManager.volume;
        //transform list into dictonary
        sfxBank = sfxList.ToDictionary(x => x.id);
    }

    //MUSIC
    public void PlayMusic(AudioClip clip, bool canLoop = true, bool fadeMusic = false) 
    {
        if (clip == null)
        {
            return;
        }
        StartCoroutine(PlayMusicWithFade(clip, canLoop, fadeMusic));
    }

    private IEnumerator PlayMusicWithFade(AudioClip clip, bool canLoop, bool fadeMusic)
    {
        if (fadeMusic)
        {
            yield return musicManager.DOFade(0, durationOfFade).WaitForCompletion();
        }
        musicManager.clip = clip;
        musicManager.loop = canLoop;
        musicManager.Play();


        if (fadeMusic)
        {
            yield return musicManager.DOFade(originalMusicVolume, durationOfFade).WaitForCompletion();
        }
    }

    //SFX
    public void PlaySFX(AudioClip clip, bool pauseMusic = false)
    {
        if (clip == null)
        {
            return;
        }
        if (pauseMusic)
        {
            musicManager.Pause();
            StartCoroutine(UnpauseMusic(clip.length));
        }

        //OneShot not to cancel the other sound
        sfxManager.PlayOneShot(clip);
    }
    public void PlaySFX(AudioId id, bool pauseMusic = false)
    {
        if (!sfxBank.ContainsKey(id))
        {
            return;
        }
        var audioData = sfxBank[id];

        PlaySFX(audioData.audioClip, pauseMusic);
    }

    private IEnumerator UnpauseMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        musicManager.volume = 0;
        musicManager.UnPause();
        musicManager.DOFade(originalMusicVolume, durationOfFade);
    }

}
//To indentify what type of audio
public enum AudioId {UIManage, GettingHit, Death, ItemGained,
    MoneyGained, ItemBattle
}

//Hold information about audios
[System.Serializable]
public class AudioData
{
    public AudioId id;
    public AudioClip audioClip;
}
