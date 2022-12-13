using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 足音のクラス
/// </summary>
public class FootSound : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip soundClip;
    [SerializeField] AudioClip soundClip2;
    // Start is called before the first frame update

    /// <summary>
    /// 走る足音を出す関数
    /// 今回は右足と左足で同じ音源を使っているが、変える場合は、soundClipを変更する
    /// </summary>
    public void runSound()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(soundClip,1);
        }
    }
    /// <summary>
    /// 走る足音を出す関数2
    /// </summary>
    public void runSound2()
    {
        if(audioSource == null)
        {
            Debug.Log("null:Audiosource");
        }
        if (audioSource != null)
        {
            audioSource.PlayOneShot(soundClip2,1);
        }
    }

    /// <summary>
    /// 歩く足音を出す関数
    /// </summary>
    public void walkSound()
    {
        if (audioSource == null)
        {
            Debug.Log("null:Audiosource");
        }
        if (audioSource != null)
        {
            audioSource.PlayOneShot(soundClip2, 0.3f);
        }
    }

    /// <summary>
    /// 歩く足音を出す関数2
    /// </summary>
    public void walkSound2()
    {
        if (audioSource == null)
        {
            Debug.Log("null:Audiosource");
        }
        if (audioSource != null)
        {
            audioSource.PlayOneShot(soundClip2, 0.3f);
        }
    }
}
