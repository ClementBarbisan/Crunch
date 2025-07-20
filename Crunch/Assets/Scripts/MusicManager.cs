using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] clips;
    private AudioSource _source;
    private float _volumeMax;

    
    private static MusicManager instance = null;
    public static MusicManager Instance => instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
            instance = this;

        DontDestroyOnLoad(gameObject);
        
        _source = GetComponent<AudioSource>();
        _volumeMax = _source.volume;
        ChangeMusic(0);
    }

    public void ChangeMusic(int index)
    {
        StartCoroutine((CoroutineChangeMusic(index)));
    }
    
    private IEnumerator CoroutineChangeMusic(int index)
    {
        float value = _volumeMax;

        while (value > 0f)
        {
            value -= Time.deltaTime * .8f;
            _source.volume = value;
            yield return null;
        }
        _source.volume = 0f;
        _source.Stop();
        _source.clip = clips[index];
        _source.Play();
        value = 0f;
        
        while (value < _volumeMax)
        {
            value += Time.deltaTime * .3f;
            _source.volume = value;
            yield return null;
        }

        _source.volume = _volumeMax;
    }
}
