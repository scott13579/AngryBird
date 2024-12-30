using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip clip;

    public GameObject WinUI;
    public GameObject LoseUI;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = this.clip;
        audioSource.Play(); // 반복 재생, 지속적으로 재생
    }

    void Update()
    {
        if (WinUI.activeSelf == true || LoseUI.activeSelf == true)
        {
            audioSource.Stop();
        }
    }
}
