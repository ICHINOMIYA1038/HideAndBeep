using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using System.Drawing;   // 追加：「参照」でSystem.Drawingを追加すること
using ImagingSolution;  // ImagingSolution.Matクラス用

public class numpySample : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        var matA = new double[,] {
    { 1, 2, 3 },
    { 4, 5, 6 },
    { 7, 8, 9 }
};

        matA.Print("matA =");


        var matB = new double[,] {
    { 1, 4, 8 },
    { 6, 2, 5 },
    { 9, 7, 3 }
};
        var matMult = matA.Mult(matB);
        
        foreach(var mat in matMult)
        {

        }

    }

    void Update()
    {
        
    }

    // Update is called once per frame
}
