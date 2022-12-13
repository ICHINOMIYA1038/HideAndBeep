using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����̃N���X
/// </summary>
public class FootSound : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip soundClip;
    [SerializeField] AudioClip soundClip2;
    // Start is called before the first frame update

    /// <summary>
    /// ���鑫�����o���֐�
    /// ����͉E���ƍ����œ����������g���Ă��邪�A�ς���ꍇ�́AsoundClip��ύX����
    /// </summary>
    public void runSound()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(soundClip,1);
        }
    }
    /// <summary>
    /// ���鑫�����o���֐�2
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
    /// �����������o���֐�
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
    /// �����������o���֐�2
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
