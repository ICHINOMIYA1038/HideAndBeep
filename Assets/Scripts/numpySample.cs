using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using System.Drawing;   // �ǉ��F�u�Q�Ɓv��System.Drawing��ǉ����邱��
using ImagingSolution;  // ImagingSolution.Mat�N���X�p

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
