using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MapManager : MonoBehaviour
{
    float[,] soundMap;
    [SerializeField] Vector3[] soundPosition;
    [SerializeField] GameObject flag;
    [SerializeField] GameObject[] items;
    [SerializeField] Renderer[,] renderers;
    [SerializeField] int widthNum = 3;
    [SerializeField] int heightNum = 3;
    [SerializeField] Vector3 testPosition;
      
    // Start is called before the first frame update
    void Start()
    {
        soundMap = new float[heightNum, widthNum];
        soundPosition = new Vector3[9];

        renderers = new Renderer[widthNum, heightNum];

        for (int i = 0; i<3; i++)
        {
            for(int j = 0; j<3; j++)
            {
               
                soundPosition[i * 3 + j] = items[i * 3 + j].transform.position;
                renderers[i,j] = items[i * 3 + j].GetComponent<Renderer>();
            }
        }

        CatchSound(1f, testPosition);


    }

    // Update is called once per frame
    void Update()
    {

    }

    void CatchSound(float soundLevel, Vector3 position)
    {
        int row = 0;
        int column = 0;
        int length = alocationSound(position);
        showIndex(ref row, ref column, length);
        upDateParameter(row, column, soundLevel);


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

    void upDateParameter(int row, int column, float level)
    {

        soundMap[row, column] += level;

        float scaleFacter = 0.95f;

            if(row - 1 >= 0)
        {
            soundMap[row - 1, column] += level * scaleFacter;
            soundMap[row - 1, column + 1] += level * scaleFacter;
        }
            if(column - 1 >= 0)
        {
            soundMap[row, column - 1] += level * scaleFacter;
            soundMap[row + 1, column - 1] += level * scaleFacter;
        }
            if(row-1 >= 0 && column - 1 >= 0)
        {
            soundMap[row - 1, column - 1] += level * scaleFacter;
        }
            
        
            soundMap[row + 1, column + 1] += level * scaleFacter;
            soundMap[row, column + 1] += level * scaleFacter;
            soundMap[row + 1, column] += level * scaleFacter;

        Normarize();

        upDateColor(row + 1, column - 1);
        upDateColor(row, column - 1);
        upDateColor(row - 1, column + 1);
        upDateColor(row - 1, column);
        upDateColor(row, column);
        upDateColor(row + 1, column + 1);
        upDateColor(row, column + 1);
        upDateColor(row + 1 , column);
        upDateColor(row -1, column -1);


    }

    void upDateColor(int row, int column)
    {
        if(row>=0&&column>=0)
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