using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using util;

public class LeverCon: MonoBehaviourPunCallbacks, IPunObservable, IPunOwnershipCallbacks, IcanInteract
{
    bool isOpen = false;
    bool playerEnterTrigger = false;
    [SerializeField] GameObject progressbarInstance;
    [SerializeField] ProgressBarCon progressBar;
    [SerializeField] GameObject lid;
    [SerializeField]PlayerController playerController;
    [SerializeField] float waitTime = 3.0f;
    [SerializeField] PhotonView photonview;
    [SerializeField] int ItemState;
    [SerializeField] GameObject gate;
     
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (playerEnterTrigger)
        {
            InputCheck();
        }
    }

    public void CompleteTask()
    {
        lid.transform.eulerAngles = new Vector3(90f, 0f, 0f);
        playerController.stopAction();
        isOpen = true;
        StartCoroutine("GateOpen");
    }

    IEnumerator GateOpen()
    {
        yield return new WaitForSeconds(1);
        for(int i = 0; i < 300; i++)
        {
            gate.transform.Translate(new Vector3(0f, 0.1f, 0f));
            yield return null;
        }
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerEnterTrigger = true;
            playerController = other.gameObject.GetComponent<PlayerController>();
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerEnterTrigger = false;
            playerController = null;
        }
    }


    




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



            playerController.pullLever(transform.position - new Vector3(0f, 0f, 3f), transform.position);
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
            stream.SendNext(ItemState);

        }
        else
        {
            this.isOpen = (bool)stream.ReceiveNext();
            this.ItemState = (int)stream.ReceiveNext();
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
