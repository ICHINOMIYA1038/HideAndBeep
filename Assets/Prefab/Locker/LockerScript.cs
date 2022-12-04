using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;

public class LockerScript : InteractiveObject
{
    [SerializeField] GameObject door;
    [SerializeField] float openSpeed =1f;
    [SerializeField] LockerIncheck lockerIncheck;
    [SerializeField] GameObject cameraPosition;
    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip clip1;
    [SerializeField] AudioClip clip2;
    SoundManager soundManager;
    public void Start()
    {
        interacted = false;
        soundManager = gameManager.GetComponent<SoundManager>();
    }
    protected override void OnInteract()
    {
        if(!photonview.IsMine)
        {
            return;
        }
        soundManager.soundDetect(transform.position, 12f, 0.8f);
        playerController.Freeze();
        StartCoroutine("open");
        openSound();
        if (lockerIncheck.inLocker == true)
        {
            playerController.ExitLocker();
        }
    }

    protected override void ReInteract()
    {
        if (!photonview.IsMine)
        {
            return;
        }
        soundManager.soundDetect(transform.position, 12f, 0.8f);
        playerController.Freeze();
        
        if (lockerIncheck.inLocker == true)
        {
            playerController.EnterLocker(cameraPosition.transform.position);
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
        StartCoroutine("EnemyClose");
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

            while (Mathf.Cos(door.transform.localEulerAngles.y * Mathf.PI / 180) > 0 && interacted == false)
            {
                door.transform.Rotate(0, -Time.deltaTime * openSpeed, 0);
                yield return null;
            }
            playerController.Free();
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

            while (Mathf.Sin(door.transform.localEulerAngles.y * Mathf.PI / 180) < 0 && interacted == true)
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
                yield break;
            }

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
}
