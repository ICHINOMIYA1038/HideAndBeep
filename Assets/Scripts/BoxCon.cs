using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;

public class BoxCon : MonoBehaviourPunCallbacks, IPunObservable, IPunOwnershipCallbacks
{
    bool isOpen = false;
    [SerializeField] GameObject progressbarInstance;
    [SerializeField] ProgressBarCon progressBar;
    [SerializeField] GameObject lid;
    PlayerController playerController;
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
        if(isOpen==true)
        {
            return;
        }
        
    }

    public void CompleteTask()
    {
        lid.transform.eulerAngles = new Vector3(90f, 0f, 0f);
        playerController.stopAction();
        int charaItem = playerController.itemState;
        playerController.OnItemChange(ItemState);
        ItemState = charaItem;
        playerController = null;
        isOpen = true;
    }


    private void OnTriggerStay(Collider other)
    {
        if(isOpen)
        {
            if (other.gameObject.tag == "Player"&& playerController == null)
            {
                if (Input.GetKey(KeyCode.E))
                {

                    photonview.RequestOwnership();
                    playerController = other.gameObject.GetComponent<PlayerController>();
                    int charaItem = playerController.itemState;
                    playerController.OnItemChange(ItemState);
                    ItemState = charaItem;
                    playerController = null;
                }
            }
                return;
        }



        if (other.gameObject.tag == "Player")
        {

            if (progressBar.isActive && playerController != null)
            {

                if(Input.GetKey(KeyCode.Q))
                {
                    playerController.stopAction();
                    progressBar.isActive = false;
                    progressbarInstance.SetActive(false);
                    playerController = null;

                }

            }

            if (Input.GetKey(KeyCode.E) && progressBar.isActive == false&&playerController==null)
                {
                    photonview.RequestOwnership();

                if (other.gameObject.GetComponent<PlayerController>() != null)
                {
                    if (!other.gameObject.GetComponent<PlayerController>().animator.GetCurrentAnimatorStateInfo(0).IsName("stand"))
                    {
                        return;
                    }
                }
               
                    playerController = other.gameObject.GetComponent<PlayerController>();
                    
                    playerController.searchBox(transform.position - new Vector3(0f, 0f, 3f), transform.position);
                    playerController.canMove = false;
                    progressBar.isActive = true;
                    progressbarInstance.SetActive(true);
               
                }
                
            }
     
    }

    void InputCheck()
    {

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
