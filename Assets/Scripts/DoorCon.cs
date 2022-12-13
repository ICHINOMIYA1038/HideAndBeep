using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;
/// <summary>
/// ドアを制御するスクリプト
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
    /// インタラクトしたときの処理
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
    /// 一度インタラクトしたオブジェクトにもう一度インタラクトするときの処理
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
    /// 敵がロッカーを開けるときの処理
    /// </summary>
    public void Opend()
    {
        
        StartCoroutine("EnemyOpen");
        openSound();
    }

    /// <summary>
    /// 敵がロッカーをしめるときの処理
    /// </summary>
    public void Closed()
    {

        StartCoroutine("EnemyClose");
        closeSound();
    }

    /// <summary>
    /// 敵がロッカーを開けるときの非同期処理
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
    /// 敵がロッカーを閉めるときの非同期処理
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
    /// ロッカーを開けるときの非同期処理
    /// 三角関数で角度を制限
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
    /// ロッカーを閉めるときの非同期処理
    /// 三角関数で角度を制限
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
    /// ロッカーを開けるときの音
    /// </summary>
    public void openSound()
    {
        audio.PlayOneShot(clip1);
    }

    /// <summary>
    /// ロッカーを閉めるときの音
    /// </summary>
    public void closeSound()
    {
        audio.PlayOneShot(clip2);
    }
}
