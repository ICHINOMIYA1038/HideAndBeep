using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using util;

/// <summary>
/// レバーの制御のスクリプト
/// </summary>
public class LeverCon: MonoBehaviourPunCallbacks, IPunObservable, IPunOwnershipCallbacks, IcanInteract
{
    bool isOpen = false;
    bool playerEnterTrigger = false;
    [SerializeField] GameObject progressbarInstance;
    [SerializeField] ProgressBarCon progressBar;
    [SerializeField] GameObject lid;
    [SerializeField]PlayerController playerController;
    [SerializeField] PhotonView photonview;
    [SerializeField] GameObject gate;
    [SerializeField] GameManager gameManager;

    // Update is called once per frame
    void Update()
    {
        if (playerEnterTrigger)
        {
            InputCheck();
        }
    }

    /// <summary>
    /// タスクを完了したときのスクリプト
    /// </summary>
    public void CompleteTask()
    {
        playerController.stopAction();
        isOpen = true;
        StartCoroutine("GateOpen");
    }

    /// <summary>
    /// ゲートを開ける非同期処理
    /// </summary>
    /// <returns></returns>
    IEnumerator GateOpen()
    {
        yield return new WaitForSeconds(1);
        for(int i = 0; i < 300; i++)
        {
            photonview.RequestOwnership();

            gate.transform.Translate(new Vector3(0f, 0.1f, 0f));
            yield return null;
        }
        
    }
    /// <summary>
    /// トリガーに入ったときの処理
    /// インプットアシストをオンにしている。
    /// </summary>
    /// <param name="other">トリガーに入ったオブジェクト</param>

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerEnterTrigger = true;
            playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController.getPhotonviewIsMine())
            {
                gameManager.ActiveInputAssist("E");
            }
        }
        
    }
    /// <summary>
    /// トリガーからでたときの処理
    /// インプットアシストをオフにしている。
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (playerController.getPhotonviewIsMine())
            {
                gameManager.DeactiveInputAssist();
            }
            playerEnterTrigger = false;
            playerController = null;
            
        }
    }
    /// <summary>
    /// 入力の受付処理
    /// </summary>
    private void InputCheck()
    {

        if (progressBar.isActive)
            {

                if(Input.GetKey(KeyCode.Q))
                {
                    playerController.stopAction();
                    progressBar.isActive = false;
                    progressbarInstance.SetActive(false);

                }

         }

        if (Input.GetKey(KeyCode.E) && progressBar.isActive == false&&playerController!=null)
        {
            photonview.RequestOwnership();


            if (!playerController.animator.GetCurrentAnimatorStateInfo(0).IsName("stand"))
            {
                return;
            }



            playerController.pullLever(transform.position - new Vector3(0f, 10f, 3f), transform.position - new Vector3(0f,10f,0f));
            playerController.canMove = false;
            progressBar.isActive = true;
            progressbarInstance.SetActive(true);


        }
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
            stream.SendNext(isOpen);

        }
        else
        {
            this.isOpen = (bool)stream.ReceiveNext();
        }
    }

    void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        targetView.TransferOwnership(requestingPlayer);
    }

    void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        
    }

    void IPunOwnershipCallbacks.OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        //StartCoroutine("RetryRequest");
    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
}
