using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundArea : MonoBehaviour
{
    [SerializeField]
    private float fadeInDuration = 10.0f;
    [SerializeField]
    private float fadeOutDuration = 10.0f;
    [SerializeField]
    private float targetVolume = 1.0f;
    [SerializeField]
    public bool shouldFadeOut = true;

    new private AudioSource audio;
    private Coroutine fadeInCoroutine;
    private Coroutine fadeOutCoroutine;

    void Start()
    {
        audio = gameObject.GetComponent<AudioSource>();
    }

    public void StartFadeOut()
    {
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
        }
        fadeOutCoroutine = StartCoroutine(this.FadeOut());
    }

    private IEnumerator FadeIn()
    {
        audio.Play();   
        audio.volume = 0f;

        while (audio.volume < targetVolume)
        {
            audio.volume += Time.deltaTime / fadeInDuration;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        while (audio.volume > 0.001f)
        {
            audio.volume -= Time.deltaTime / fadeOutDuration;
            yield return null;
        }

        audio.Stop();
    }

    private void OnTriggerEnter()
    {
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
        }
        fadeInCoroutine = StartCoroutine(this.FadeIn());
    }

    private void OnTriggerExit()
    {
        if(!shouldFadeOut) return;
        StartFadeOut();
    }
}
