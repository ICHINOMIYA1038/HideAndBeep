using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;
using Photon.Pun;

public class ProgressBarCon : ProgressBar, IPunObservable
{
    [SerializeField] PhotonView photonview;
    [SerializeField] IcanInteract interactController;
    [SerializeField] BoxCon boxCon;
    [SerializeField] LeverCon levercon;
    [SerializeField] BookShelf shelfcon;
    private void Update()
    {
        if(isActive)
        {
            Progress();
        }
        
    }

    private void Start()
    {
        //SerializeFieldÇ≈ÇÕÅAInterfaceÇÕìnÇπÇ»Ç¢ÇΩÇﬂÅAÇ±ÇÃÇÊÇ§Ç»é¿ëïÇ…ÇµÇΩÅB
        if (boxCon != null)
        {
            interactController = boxCon;
        }
        if (levercon != null)
        {
            interactController = levercon;
        }
        if (shelfcon != null)
        {
            interactController = shelfcon;
        }

    }

    new void Progress()
    {
        progressTime += Time.deltaTime;
        progressRatio = progressTime / needTime;
        float changedSize = progressRatio * maxWidth;
        if (0 < progressRatio && progressRatio < 1.0)
        {
            changePanelSize(ref panelTransform, changedSize);
        }
        if (progressRatio >= 1)
        {
            changePanelSize(ref panelTransform, maxWidth);
            CompleteTask();
        }
    }

    override public void CompleteTask()
    {
        base.CompleteTask();
        interactController.CompleteTask();
        this.gameObject.SetActive(false);   
        
    }

    public void stopProgress()
    {
        isActive = false;
    }

    public bool taskCompleteCheck()
    {
        return istaskCompleted;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            if (!photonView.IsMine)
            {
                return;
            }
            stream.SendNext(progressTime);

        }
        else
        {
            this.progressTime = (float)stream.ReceiveNext();
        }
    }
}
