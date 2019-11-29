using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    private float m_FadeInDuration = 10.0f;
    [SerializeField]
    private float m_FadeOutDuration = 10.0f;
    [SerializeField]
    private float m_TargetVolume = 1.0f;

    private AudioSource m_Audio;
    private Coroutine m_FadeInCoroutine;
    private Coroutine m_FadeOutCoroutine;

    private IEnumerator FadeIn()
    {
        m_Audio.Play();
        m_Audio.volume = 0f;

        while (m_Audio.volume < m_TargetVolume)
        {
            m_Audio.volume += Time.deltaTime / m_FadeInDuration;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        while (m_Audio.volume > 0.001f)
        {
            m_Audio.volume -= Time.deltaTime / m_FadeOutDuration;
            yield return null;
        }

        m_Audio.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_FadeOutCoroutine != null)
        {
            StopCoroutine(m_FadeOutCoroutine);
        }
        m_FadeInCoroutine = StartCoroutine(this.FadeIn());
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_FadeInCoroutine != null)
        {
            StopCoroutine(m_FadeInCoroutine);
        }
        m_FadeOutCoroutine = StartCoroutine(this.FadeOut());
    }

    void Start()
    {
        m_Audio = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Player: " + CCPlayer.main.transform.position.y);
    }
}
