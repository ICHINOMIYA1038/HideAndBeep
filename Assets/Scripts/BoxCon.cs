using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using util;
public class BoxCon : MonoBehaviourPunCallbacks, IPunObservable, IPunOwnershipCallbacks,IcanInteract
{
    [SerializeField] GameManager gameManager;
    bool isOpen = false;
    [SerializeField]bool playerEnterTrigger = false;
    [SerializeField] GameObject progressbarInstance;
    [SerializeField] ProgressBarCon progressBar;
    [SerializeField] GameObject lid;
    [SerializeField] PlayerController playerController;
    [SerializeField] float waitTime = 3.0f;
    [SerializeField] PhotonView photonview;
    [SerializeField] int ItemState;
     
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
        lid.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        playerController.stopAction();
        int charaItem = playerController.itemState;
        playerController.OnItemChange(ItemState);
        ItemState = charaItem;
        isOpen = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            if (!photonview.IsMine)
            {
                return;
            }
            Debug.Log("PlayerEnter");
            playerEnterTrigger = true;
            playerController = other.gameObject.GetComponent<PlayerController>();
            gameManager.ActiveInputAssist();
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (!photonview.IsMine)
        {
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {   

            Debug.Log("PlayerExit");
            playerEnterTrigger = false;
            playerController = null;
            gameManager.DeactiveInputAssist();
        }
    }


    




    private void InputCheck()
    {
        if (!photonview.IsMine)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("aa");
        }

            if (isOpen&&playerController!=null)
        {
            if (!playerController.animator.GetCurrentAnimatorStateInfo(0).IsName("stand"))
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.E))
                {
                    int charaItem = playerController.itemState;
                    playerController.OnItemChange(ItemState);
                    photonview.RequestOwnership();
                    ItemState = charaItem;
                }
        }

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

            Debug.Log("BOXOPEN");

            if (!playerController.animator.GetCurrentAnimatorStateInfo(0).IsName("stand"))
            {
                return;
            }


            playerController.searchBox(transform.position - transform.forward*3f, transform.position);
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
        //targetView.TransferOwnership(requestingPlayer);
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
