using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;

/// <summary>
/// ロッカーのスクリプト
/// </summary>
public class LockerScript : InteractiveObject
{
    [SerializeField] GameObject door;
    [SerializeField] float openSpeed =1f;
    [SerializeField] LockerIncheck lockerIncheck;
    [SerializeField] GameObject cameraPosition;
    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip clip1;
    [SerializeField] AudioClip clip2;
    public bool playerExistsInLocker = false;
   
    SoundManager soundManager;
    public void Start()
    {
        interacted = false;
        soundManager = gameManager.GetComponent<SoundManager>();
    }
    protected override void OnInteract()
    {
        
        soundManager.soundDetect(transform.position, 12f, 0.8f);
        playerController.Freeze();
        StartCoroutine("open");
        openSound();
        if (lockerIncheck.inLocker == true)
        {
            
            playerController.ExitLocker();
            playerExistsInLocker = false;
        }
    }

    protected override void ReInteract()
    {
        soundManager.soundDetect(transform.position, 12f, 0.8f);
        playerController.Freeze();
        
        if (lockerIncheck.inLocker == true)
        {
            if (!playerController.getPhotonviewIsMine()) { return; }
            playerController.EnterLocker(cameraPosition.transform.position);
            playerExistsInLocker = true;
        }
        StartCoroutine("close");
        closeSound();

    }

    public void Opend()
    {
        StartCoroutine("EnemyOpen");
        openSound();
    }

    public void Closed()
    {
        StartCoroutine("Enemyclose");
        closeSound();
    }

    public IEnumerator EnemyOpen()
    {
        if (interacted == true)
        {
            yield break;
        }
        if (interacted == false)
        {
            interacted = true;
            while (Mathf.Cos(door.transform.localEulerAngles.y * Mathf.PI / 180) > 0 )
            {
                door.transform.Rotate(0, -Time.deltaTime * openSpeed, 0);
                yield return null;
            }
            
            yield break;
        }
    }

    public IEnumerator Enemyclose()
    {
        if (interacted == false)
        {
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
            
                yield break;
            

        }
    }

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
            if (lockerIncheck.inLocker == true)
            {
                yield break;
            }
            else
            {
                playerController.Free();
                yield break;
            }
            
        }
      
    }

    public void  openSound()
    {
        audio.PlayOneShot(clip1);
    }

    public void closeSound()
    {
        audio.PlayOneShot(clip2);
    }

    public PlayerController getPlayerController()
    {
        return playerController;
    }
}
