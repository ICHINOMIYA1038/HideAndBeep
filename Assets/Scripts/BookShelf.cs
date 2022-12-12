using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// �|��Ă���{�I�̃X�N���v�g
/// �v���O���X�Q�[�W����������ƁARotation��ύX���A�{�I�𗧂�����B
/// /// </summary>
public class BookShelf : MonoBehaviourPunCallbacks, IPunObservable, IPunOwnershipCallbacks, IcanInteract
{
    ///�^�X�N�������������ǂ���
    bool isCompleted = false;
   /// <summary>
   /// �v���C���[���g���K�[�̒��ɓ����Ă��邩�ǂ����B
   /// </summary>
    bool playerEnterTrigger = false;
    /// <summary>
    /// �v���O���X�o�[�̃v���n�u
    /// </summary>
    [SerializeField] GameObject progressbarInstance;
    /// <summary>
    /// �v���O���X�o�[�̃X�N���v�g
    /// </summary>
    [SerializeField] ProgressBarCon progressBar;
    [SerializeField] PlayerController playerController;
    [SerializeField] PhotonView photonview;
    [SerializeField] GameObject target;
    [SerializeField] GameManager gameManager;
    /// <summary>
    /// �{���N�����Ƃ��ɉ�]������p�x
    /// </summary>
    float dstRotation = -35.38f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ///�v���C���[���g���K�[���ɂ���Ƃ��ɂ����A���͂��`�F�b�N����B
        if (playerEnterTrigger)
        {
            InputCheck();
        }
    }

    /// <summary>
    /// �^�X�N�����������Ƃ��̏���
    /// �v���C���[�ɍs�������߂����A�񓯊��������J�n����B
    /// </summary>
    public void CompleteTask()
    {
        
        playerController.stopAction();
        isCompleted = true;
        StartCoroutine("BookStand");
    }

    /// <summary>
    /// �{���N��������񓯊�����
    /// </summary>
    IEnumerator BookStand()
    {
        yield return new WaitForSeconds(0.2f);
        target.transform.Rotate(-35.38f, 0f, 0f);

    }


    private void OnTriggerEnter(Collider other)
    {
        ///�v���C���[���g���K�[���ɓ���ƁA�g���K�[��true�ɂ��A�v���C���[�R���g���[���[���i�[����B
        ///�܂��A�Q�[���}�l�[�W���[�o�R�ŁA�C���v�b�g�A�V�X�g��\��������B
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

    ///�v���C���[���g���K�[�O�ɂł�ƁA�g���K�[��false�ɂ��A�v���C���[�R���g���[���[��null�ɂ���B
    ///�܂��A�Q�[���}�l�[�W���[�o�R�ŁA�C���v�b�g�A�V�X�g������������B
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
    /// ���͂̎�t
    /// 
    /// </summary>
    private void InputCheck()
    {
        ///�A�N�V�������s���Ă���Ƃ��ɂ́A�A�N�V�����̒��~����t
        if (progressBar.isActive)
        {

            if (Input.GetKey(KeyCode.Q))
            {
                playerController.stopAction();
                progressBar.isActive = false;
                progressbarInstance.SetActive(false);
                gameManager.ActiveInputAssist("E");
            }

        }

        ///�A�N�V�������s���Ă��Ȃ��Ƃ��ɂ́A�A�N�V�����̓��͂���t
        if (Input.GetKey(KeyCode.E) && progressBar.isActive == false && playerController != null)
        {
            photonview.RequestOwnership();


            if (!playerController.animator.GetCurrentAnimatorStateInfo(0).IsName("stand"))
            {
                return;
            }
            gameManager.ActiveInputAssist("Q");

            playerController.canMove = false;
            progressbarInstance.SetActive(true);
            progressBar.isActive = true;

        }
    }



    /// <summary>
    /// �ʐM����
    /// �^�X�N���������Ă��邩�ǂ����𓯊�����B
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
