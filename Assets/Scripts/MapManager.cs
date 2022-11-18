using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MapManager : MonoBehaviour
{
    float[,] soundMap;
    Vector3[] soundPosition;
    [SerializeField]GameObject[] items;
    Renderer[,] renderers;
    int widthNum = 3;
    int heightNum = 3;

    // Start is called before the first frame update
    void Start()
    {
        soundMap = new float[heightNum, widthNum];
        soundPosition = new Vector3[9];

        for(int i=0;i<9;i++)
        Debug.Log(items[i].name);

        for(int i =0; i<3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                soundPosition[i * 3 + j] = items[i * 3 + j].transform.position;
                renderers[i,j] = items[i * 3 + j].GetComponent<MeshRenderer>();
            }
        }

        CatchSound(5, new Vector3(0, 0, 0));




    }

    // Update is called once per frame
    void Update()
    {

    }

    void CatchSound(int soundLevel, Vector3 position)
    {
        int row = 0;
        int column = 0;
        int length = alocationSound(position);
        showIndex(ref row, ref column, length);
        upDateParameter(row, column, length);


    }

    int alocationSound(Vector3 position)
    {
        int minimumIndex = -1;
        float minimumDistance = 100000;
        foreach (var posi in soundPosition.Select((v, i) => new { Value = v, Index = i }))
        {
            if (minimumDistance > (posi.Value - position).magnitude)
            {
                minimumDistance = (posi.Value - position).magnitude;
                minimumIndex = posi.Index;
            }
        }

        return minimumIndex;
    }

    void showIndex(ref int row, ref int column, int length)
    {
        row = length / heightNum;
        column = (length % heightNum) % heightNum;


    }

    void upDateParameter(int row, int column, int level)
    {

        soundMap[row, column] += level;

        float scaleFacter = 0.8f;
        soundMap[row - 1, column - 1] += level * scaleFacter;
        soundMap[row + 1, column - 1] += level * scaleFacter;
        soundMap[row - 1, column + 1] += level * scaleFacter;
        soundMap[row + 1, column + 1] += level * scaleFacter;

        soundMap[row, column + 1] += level * scaleFacter;
        soundMap[row, column - 1] += level * scaleFacter;
        soundMap[row - 1, column] += level * scaleFacter;
        soundMap[row + 1, column] += level * scaleFacter;

        Normarize();

        upDateColor(row, column);
        upDateColor(row + 1, column + 1);
        upDateColor(row + 1, column - 1);
        upDateColor(row, column - 1);
        upDateColor(row, column + 1);
        upDateColor(row - 1 , column + 1);
        upDateColor(row - 1, column - 1);
        upDateColor(row + 1 , column);
        upDateColor(row - 1, column);

    }

    void upDateColor(int row, int column)
    {
        renderers[row,column].material.color = new Color(soundMap[row, column], 0f, 0f, 1f);
    }

    void Normarize()
    {
        float minimumLevel = 10f;
        float maxLevel = 0f;
        foreach(var level in soundMap)
        {
            if(minimumLevel>level)
            {
                minimumLevel = level;
            }
            if (maxLevel < level)
            {
                maxLevel = level;
            }
        }
        float[,] dst = new float[3,3];
        for(int i=0;i<3; i++)
        {
            for(int j=0;j<3;j++)
            {
                dst[i,j] = (soundMap[i,j] - minimumLevel ) / (maxLevel - minimumLevel);
            }
        }
        Array.Copy(dst,soundMap, dst.Length);



    }
}