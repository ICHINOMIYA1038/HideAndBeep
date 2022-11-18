using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Numpy;
using System;


public class numpySample : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        var a = np.arange(1000);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
