using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using util;
/// <summary>
/// 宝箱の処理を決めるクラス
/// </summary>
public class BoxCon : MonoBehaviourPunCallbacks, IPunObservable, IPunOwnershipCallbacks,IcanInteract
{
    [SerializeField] GameManager gameManager;
    bool isOpen = false;
    [SerializeField]bool playerEnterTrigger = false;
    [SerializeField] GameObject progressbarInstance;
    [SerializeField] ProgressBarCon progressBar;
    /// <summary>
    /// lidは宝箱の蓋のインスタンス
    /// </summary>
    [SerializeField] GameObject lid;
    [SerializeField] PlayerController playerController;
    [SerializeField] PhotonView photonview;
    /// <summary>
    /// 宝箱に入っているアイテムのインデックス
    /// </summary>
    [SerializeField] int ItemState;
     
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ///プレイヤーがトリガー内に入っている時に入力の処理をする。
        if (playerEnterTrigger)
        {
            InputCheck();
        }
    }

    /// <summary>
    /// プログレスバーのタスクが完了した時のスクリプト
    /// </summary>
    public void CompleteTask()
    {
        lid.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        playerController.stopAction();
        int charaItem = playerController.itemState;
        playerController.OnItemChange(ItemState);
        ItemState = charaItem;
        isOpen = true;
        if (playerController.getPhotonviewIsMine())
        {
            gameManager.DeactiveInputAssist();
        }
    }

    /// <summary>
    /// トリガーに入った時のスクリプト
    /// playerEnterTriggerをTrueにしてPlayerControllerを取得する。
    /// 対象のプレイヤーがプレイヤー自身ならばインプットアシストをOnにする。
    /// </summary>
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
    /// トリガーから出た時のスクリプト
    /// playerEnterTriggerをTrueにしてPlayerControllerを取得する。
    /// 対象のプレイヤーがプレイヤー自身ならばインプットアシストをOnにする。
    /// </summary>
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
    /// 入力処理の受付
    /// 
    /// </summary>
    private void InputCheck()
    {

        ///箱が開いている時の処理
        if(isOpen&&playerController!=null)
        {
            ///プレイヤーが静止している時以外の処理は拒否する。
            if (!playerController.animator.GetCurrentAnimatorStateInfo(0).IsName("stand"))
            {
                return;
            }
            ///Eを押した時の処理
            ///箱が空いている時には、自分の持っているアイテムと箱の中のアイテムを入れ替える。
            if (Input.GetKeyDown(KeyCode.E))
                {
                    int charaItem = playerController.itemState;
                    playerController.OnItemChange(ItemState);
                    photonview.RequestOwnership();
                    ItemState = charaItem;
                }
        }

        ///プログレスバ-が起動中の時(アクション中)の入力受付
        if (progressBar.isActive)
            {
                ///Qを押すと、行動を辞める。
                if(Input.GetKeyDown(KeyCode.Q))
                {
                    playerController.stopAction();
                    progressBar.isActive = false;
                    progressbarInstance.SetActive(false);
                    gameManager.ActiveInputAssist("E");
            }

         }

        ///Eを押した時の処理。箱を開け始める。
        ///この時、インプットのアシストをQに変える。
        ///また、プレイヤーに箱を開けるアニメーションをさせる。
        if (Input.GetKeyDown(KeyCode.E) && progressBar.isActive == false&&playerController!=null)
        {
            if (!playerController.animator.GetCurrentAnimatorStateInfo(0).IsName("stand"))
            {
                return;
            }
            gameManager.ActiveInputAssist("Q");
            photonview.RequestOwnership();

            playerController.searchBox(transform.position - transform.forward*3f, transform.position);
            playerController.canMove = false;
            progressBar.isActive = true;
            progressbarInstance.SetActive(true);


        }
            }

     

    /// <summary>
    ///  通信処理
    /// </summary>
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        


        if (stream.IsWriting)
        {
            if (!photonView.IsMine)
            {
                return;
            }
            //箱が開いているかどうかと箱の中身を同期する。
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
