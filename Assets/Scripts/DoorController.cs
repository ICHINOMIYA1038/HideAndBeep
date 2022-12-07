using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;


public class DoorController : InteractiveObject
{
    GameObject door;
    protected override void OnInteract()
    {
        door.transform.RotateAround(transform.position, transform.up, 90);
        playerController.animator.SetTrigger("Open");
    }

    protected override void ReInteract()
    {
        door.transform.RotateAround(transform.position, transform.up, -90);
        playerController.animator.SetTrigger("Open");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
