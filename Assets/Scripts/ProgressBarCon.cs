using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;
using Photon.Pun;

public class ProgressBarCon : ProgressBar, IPunObservable
{
    [SerializeField] PhotonView photonview;
    [SerializeField] BoxCon boxController;
    private void Update()
    {
        if(isActive)
        {
            Progress();
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
        boxController.CompleteTask();
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
            //?f?[?^?????M
            stream.SendNext(progressTime);

        }
        else
        {
            this.progressTime = (float)stream.ReceiveNext();
        }
    }
}
