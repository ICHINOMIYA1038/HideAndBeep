using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LockerIncheck : MonoBehaviour
{
    [SerializeField] LockerScript lockerScript;
    public bool inLocker;
    [SerializeField]PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        inLocker = true;
    }

    private void OnTriggerExit(Collider other)
    {
        inLocker = false;
    }
}
