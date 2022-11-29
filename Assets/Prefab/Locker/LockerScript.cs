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

    protected override void OnInteract()
    {
        if(!photonview.IsMine)
        {
            return;
        }
        playerController.Freeze();
        StartCoroutine("open");
        if (lockerIncheck.inLocker == true)
        {
            playerController.ExitLocker();
            Debug.Log("Exit");
        }
    }

    protected override void ReInteract()
    {
        if (!photonview.IsMine)
        {
            return;
        }
        playerController.Freeze();
        
        if (lockerIncheck.inLocker == true)
        {
            playerController.EnterLocker(cameraPosition.transform.position);
            Debug.Log("IntoTheLocker");
        }
        StartCoroutine("close");

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

            while (Mathf.Cos(door.transform.localEulerAngles.y * Mathf.PI / 180) > 0 && interacted == false)
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
                playerController.Free();
                yield break;
            }
            
        }
    }
}
