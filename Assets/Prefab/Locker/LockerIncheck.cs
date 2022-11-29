using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerIncheck : MonoBehaviour
{
    [SerializeField] LockerScript lockerScript;
    public bool inLocker; 
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
