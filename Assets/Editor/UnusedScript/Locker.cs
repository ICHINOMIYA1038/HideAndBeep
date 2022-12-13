using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour
{
    [SerializeField] GameObject lockerDoor;
    [SerializeField] float openSpeed = 30f;
    [SerializeField] bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        if (isOpen == true)
        {
            StartCoroutine("close");
        }
        if (isOpen == false)
        {
            StartCoroutine("open");
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public IEnumerator open()
    {
        if (isOpen == true)
        {
            yield break;
        }
        if(isOpen == false)
        {
            
            while (Mathf.Cos(lockerDoor.transform.localEulerAngles.y * Mathf.PI / 180) > 0 && isOpen==false)
            {
                lockerDoor.transform.Rotate(0, -Time.deltaTime * openSpeed, 0);
                yield return null;
            }
            yield break;
        }
    }

    public IEnumerator close()
    {
        if (isOpen == false)
        {
            yield break;
        }
        if (isOpen == true)
        {

            while (Mathf.Sin(lockerDoor.transform.localEulerAngles.y * Mathf.PI / 180) < 0 && isOpen == true)
            {
                lockerDoor.transform.Rotate(0, + Time.deltaTime * openSpeed, 0);
                yield return null;
            }
            yield break;
        }
    }


}
