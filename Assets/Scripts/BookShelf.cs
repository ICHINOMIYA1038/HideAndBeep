using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// 倒れている本棚のスクリプト
/// プログレスゲージが完了すると、Rotationを変更し、本棚を立たせる。
/// /// </summary>
public class BookShelf : MonoBehaviourPunCallbacks, IPunObservable, IPunOwnershipCallbacks, IcanInteract
{
    ///タスクが完了したかどうか
    bool isCompleted = false;
    /// <summary>
    /// プレイヤーがトリガーの中に入っているかどうか。
    /// </summary>
    bool playerEnterTrigger = false;
    /// <summary>
    /// プログレスバーのプレハブ
    /// </summary>
    [SerializeField] GameObject progressbarInstance;
    /// <summary>
    /// プログレスバーのスクリプト
    /// </summary>
    [SerializeField] ProgressBarCon progressBar;
    [SerializeField] PlayerController playerController;
    [SerializeField] PhotonView photonview;
    [SerializeField] GameObject target;
    [SerializeField] GameManager gameManager;
    /// <summary>
    /// 本を起こすときに回転させる角度
    /// </summary>
    float dstRotation = -35.38f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ///プレイヤーがトリガー内にいるときにだけ、入力をチェックする。
        if (playerEnterTrigger)
        {
            InputCheck();
        }
    }

    /// <summary>
    /// タスクが完了したときの処理
    /// プレイヤーに行動を辞めさせ、非同期処理を開始する。
    /// </summary>
    public void CompleteTask()
    {

        playerController.stopAction();
        isCompleted = true;
        StartCoroutine("BookStand");
        if (playerController.getPhotonviewIsMine())
        {
            gameManager.DeactiveInputAssist();
        }
    }

    /// <summary>
    /// 本を起こさせる非同期処理
    /// </summary>
    IEnumerator BookStand()
    {
        yield return new WaitForSeconds(0.2f);
        target.transform.Rotate(-35.38f, 0f, 0f);

    }


    private void OnTriggerEnter(Collider other)
    {
        ///プレイヤーがトリガー内に入ると、トリガーをtrueにし、プレイヤーコントローラーを格納する。
        ///また、ゲームマネージャー経由で、インプットアシストを表示させる。
        if (isCompleted)
        {
            return;
        }
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

    ///プレイヤーがトリガー外にでると、トリガーをfalseにし、プレイヤーコントローラーをnullにする。
    ///また、ゲームマネージャー経由で、インプットアシストを消去させる。
    private void OnTriggerExit(Collider other)
    {
        if (isCompleted)
        {
            return;
        }
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
    /// 入力の受付
    /// </summary>
    private void InputCheck()
    {
        
        ///アクションが行われているときには、アクションの中止を受付
        if (progressBar.isActive)
        {

            if (Input.GetKeyDown(KeyCode.Q))
            {
                playerController.stopAction();
                progressBar.isActive = false;
                progressbarInstance.SetActive(false);
                gameManager.ActiveInputAssist("E");
            }

        }

        ///アクションが行われていないときには、アクションの入力を受付
        else if (Input.GetKeyDown(KeyCode.E) && progressBar.isActive == false && playerController != null)
        {
            photonview.RequestOwnership();


            if (!playerController.animator.GetCurrentAnimatorStateInfo(0).IsName("stand"))
            {
                return;
            }
            gameManager.ActiveInputAssist("Q");
            playerController.LiftUpBookShelf(this.transform.position);
            progressbarInstance.SetActive(true);
            progressBar.isActive = true;

        }
    }



    /// <summary>
    /// 通信部分
    /// タスクが完了しているかどうかを同期する。
    /// </summary>
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            stream.SendNext(isCompleted);

        }
        else
        {
            this.isCompleted = (bool)stream.ReceiveNext();
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

    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
}