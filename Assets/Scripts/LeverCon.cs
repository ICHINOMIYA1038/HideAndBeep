using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using util;

/// <summary>
/// ���o�[�̐���̃X�N���v�g
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
    /// �^�X�N�����������Ƃ��̃X�N���v�g
    /// </summary>
    public void CompleteTask()
    {
        playerController.stopAction();
        isOpen = true;
        StartCoroutine("GateOpen");
    }

    /// <summary>
    /// �Q�[�g���J����񓯊�����
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
    /// �g���K�[�ɓ������Ƃ��̏���
    /// �C���v�b�g�A�V�X�g���I���ɂ��Ă���B
    /// </summary>
    /// <param name="other">�g���K�[�ɓ������I�u�W�F�N�g</param>

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
    /// �g���K�[����ł��Ƃ��̏���
    /// �C���v�b�g�A�V�X�g���I�t�ɂ��Ă���B
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
    /// ���͂̎�t����
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
