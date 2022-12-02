using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSound : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip soundClip;
    [SerializeField] AudioClip soundClip2;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void runSound()
    {
        if (audioSource != null)
        {
            Debug.Log("sound");
            audioSource.PlayOneShot(soundClip,1);
        }
    }
    public void runSound2()
    {
        if(audioSource == null)
        {
            Debug.Log("null:Audiosource");
        }
        if (audioSource != null)
        {
            Debug.Log("sound");
            audioSource.PlayOneShot(soundClip2,1);
        }
    }

    public void walkSound()
    {
        if (audioSource == null)
        {
            Debug.Log("null:Audiosource");
        }
        if (audioSource != null)
        {
            Debug.Log("sound");
            audioSource.PlayOneShot(soundClip2, 0.3f);
        }
    }

    public void walkSound2()
    {
        if (audioSource == null)
        {
            Debug.Log("null:Audiosource");
        }
        if (audioSource != null)
        {
            Debug.Log("sound");
            audioSource.PlayOneShot(soundClip2, 0.3f);
        }
    }
}
