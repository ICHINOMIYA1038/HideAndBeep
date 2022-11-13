using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Photon.Pun;

public class FlashlightToggle : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject lightGO; //light gameObject to work with
    public bool isOn = false; //is flashlight on or off?
    [SerializeField] PhotonView photonview;

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (!photonView.IsMine)
            {
                return;
            }
            //データの送信
            stream.SendNext(isOn);

        }
        else
        {
            this.isOn = (bool)stream.ReceiveNext();
        }

    }

    // Use this for initialization
    void Start()
    {
        if (!photonview.IsMine)
        {
            return;

        }
        //set default off
        lightGO.SetActive(isOn);
    }

    // Update is called once per frame
    void Update()
    {

        if (isOn)
        {
            lightGO.SetActive(true);
        }
        //turn light off
        else
        {
            lightGO.SetActive(false);

        }
        if (!photonview.IsMine)
        {
            return;
        }
        //toggle flashlight on key down
        if (Input.GetKeyDown(KeyCode.X))
        {
            //toggle light
            isOn = !isOn;
            //turn light on
        }
       




    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }


}
