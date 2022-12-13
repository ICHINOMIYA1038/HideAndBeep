using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;
/// <summary>
/// �h�A�𐧌䂷��X�N���v�g
/// </summary>
public class DoorCon : InteractiveObject
{
    [SerializeField] GameObject door;
    [SerializeField] float openSpeed = 20f;
    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip clip1;
    [SerializeField] AudioClip clip2;
    SoundManager soundManager;

    private void Start()
    {
        interacted = false;
        soundManager = gameManager.GetComponent<SoundManager>();
    }

    /// <summary>
    /// �C���^���N�g�����Ƃ��̏���
    /// </summary>
    protected override void OnInteract()
    {
        soundManager.soundDetect(transform.position, 12f, 0.8f);
        playerController.Freeze();
        playerController.animator.SetTrigger("Open");
        StartCoroutine("open");
        openSound();
    }

    /// <summary>
    /// 
    /// ��x�C���^���N�g�����I�u�W�F�N�g�ɂ�����x�C���^���N�g����Ƃ��̏���
    /// </summary>
    protected override void ReInteract()
    {
        soundManager.soundDetect(transform.position, 12f, 0.8f);
        playerController.Freeze();
        playerController.animator.SetTrigger("Open");
        StartCoroutine("close");
        closeSound();

    }
    /// <summary>
    /// �G�����b�J�[���J����Ƃ��̏���
    /// </summary>
    public void Opend()
    {
        
        StartCoroutine("EnemyOpen");
        openSound();
    }

    /// <summary>
    /// �G�����b�J�[�����߂�Ƃ��̏���
    /// </summary>
    public void Closed()
    {

        StartCoroutine("EnemyClose");
        closeSound();
    }

    /// <summary>
    /// �G�����b�J�[���J����Ƃ��̔񓯊�����
    /// </summary>
    public IEnumerator EnemyOpen()
    {
        if (interacted == true)
        {
            yield break;
        }
        if (interacted == false)
        {

            while (Mathf.Cos(door.transform.localEulerAngles.y * Mathf.PI / 180) > 0 && interacted == false)
            {
                door.transform.Rotate(0, -Time.deltaTime * openSpeed, 0);
                yield return null;
            }
            playerController.Free();
            yield break;
        }
    }
    /// <summary>
    /// �G�����b�J�[��߂�Ƃ��̔񓯊�����
    /// </summary>
    public IEnumerator Enemyclose()
    {
        if (interacted == false)
        {
            yield break;
        }
        if (interacted == true)
        {

            while (Mathf.Sin(door.transform.localEulerAngles.y * Mathf.PI / 180) < 0)
            {
                door.transform.Rotate(0, +Time.deltaTime * openSpeed, 0);
                yield return null;
            }

        }
    }

    /// <summary>
    /// ���b�J�[���J����Ƃ��̔񓯊�����
    /// �O�p�֐��Ŋp�x�𐧌�
    /// </summary>
    /// <returns></returns>
    public IEnumerator open()
    {
        if (interacted == true)
        {
            playerController.Free();
            yield break;
        }
        if (interacted == false)
        {
            interacted = true;

            while (Mathf.Cos(door.transform.localEulerAngles.y * Mathf.PI / 180) > 0)
            {
                door.transform.Rotate(0, -Time.deltaTime * openSpeed, 0);
                yield return null;
            }
            playerController.Free();
            yield break;
        }
        
    }


    /// <summary>
    /// ���b�J�[��߂�Ƃ��̔񓯊�����
    /// �O�p�֐��Ŋp�x�𐧌�
    /// </summary>
    /// <returns></returns>
    public IEnumerator close()
    {
        if (interacted == false)
        {
            playerController.Free();
            yield break;
        }
        if (interacted == true)
        {
            interacted = false;
            while (Mathf.Sin(door.transform.localEulerAngles.y * Mathf.PI / 180) < 0)
            {
                door.transform.Rotate(0, +Time.deltaTime * openSpeed, 0);
                yield return null;
            }
            playerController.Free();
            yield break;
        }
    }

    /// <summary>
    /// ���b�J�[���J����Ƃ��̉�
    /// </summary>
    public void openSound()
    {
        audio.PlayOneShot(clip1);
    }

    /// <summary>
    /// ���b�J�[��߂�Ƃ��̉�
    /// </summary>
    public void closeSound()
    {
        audio.PlayOneShot(clip2);
    }
}
