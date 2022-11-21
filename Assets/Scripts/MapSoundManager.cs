using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class MapSoundManager : MonoBehaviour
{
    Text[] text;
    float[,] soundMap;
    [SerializeField] Vector3[] soundPosition;
    [SerializeField] GameObject flagPrefab;
    [SerializeField]GameObject[] items;
    [SerializeField] Renderer[,] renderers;
    int widthNum = 15;
    int heightNum = 15;
    [SerializeField] Vector3 testPosition;
    float deltaTimeSound = 3f;
    [SerializeField] float testSoundLevel = 30f;
    [SerializeField] float testRange = 50f;

    // Start is called before the first frame update
    void Start()
    {
        soundMap = new float[heightNum, widthNum];
        soundPosition = new Vector3[heightNum*widthNum];
        items = new GameObject[heightNum*widthNum];
        text = new Text[heightNum * widthNum];

        renderers = new Renderer[widthNum, heightNum];

        for (int i = 0; i< heightNum; i++)
        {
            for(int j = 0; j< widthNum; j++)
            {
                soundPosition[i*heightNum+j] = new Vector3(i*10f,0f,j*10f);
                items[i * heightNum + j] = Instantiate(flagPrefab, soundPosition[i * heightNum + j], Quaternion.identity);
                renderers[i,j] = items[i * heightNum + j].GetComponent<Renderer>();
                text[i * heightNum + j] = items[i * heightNum + j].GetComponentInChildren<Text>();
                soundMap[i, j] = 0;
            }
        }

        


    }

    // Update is called once per frame
    void Update()
    {
        attenuationSound(0.1f);
        deltaTimeSound -= 1f*Time.deltaTime;
        if(deltaTimeSound < 0)
        {
            deltaTimeSound = 1f;
            testPosition = new Vector3(UnityEngine.Random.Range(0, heightNum * 10f), 0f, UnityEngine.Random.Range(-0, heightNum * 10f));
            soundDetect(testPosition, testRange, testSoundLevel);
        }
        upDateAllColor();
    }

    void CatchSound(float soundLevel, Vector3 position)
    {
        int row = 0;
        int column = 0;
        int length = alocationSound(position);
        showIndex(ref row, ref column, length);


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

    void soundDetect(Vector3 position, float range,float soundLevel)
    {
        foreach (var posi in soundPosition.Select((v, i) => new { Value = v, Index = i }))
        {
            float distance = (posi.Value - position).magnitude;
            if (range >= distance)
            {
                double scaleFactor = scaleFunction(distance);
                updateParam(posi.Index/heightNum,(posi.Index % heightNum)%heightNum, (float)(scaleFactor * soundLevel));
            }
        }
    }

    void showIndex(ref int row, ref int column, int length)
    {
        row = length / heightNum;
        column = (length % heightNum) % heightNum;


    }

    private void attenuationSound(float attenuationRate)
    {
        for(int i = 0; i<widthNum*heightNum; i++)
        {
            soundMap[i/ heightNum, (i % heightNum) % heightNum] -= attenuationRate;
            if (soundMap[i / heightNum, (i % heightNum) % heightNum]<0)
            {
                soundMap[i / heightNum, (i % heightNum) % heightNum] = 0;
            }
            text[i].text = String.Format("{0:#.##}",soundMap[i / heightNum, (i % heightNum) % heightNum]);
        }
    }

    private void updateParam(int row, int column, float level)
    {
        if (row<0||column<0||row>heightNum||column>widthNum)
        {
            return;
        }
        else
        {
            soundMap[row,column] += level;
            
        }
    }

    /// <summary>
    /// ?????????????????????????????????`????
    /// ?????????I??????????2??????????????
    /// ?????????A?w???????????p?????B
    /// 
    /// </summary>
    /// <param name="x"></param>
    private double scaleFunction(float x)
    {
        double exponentialDecay = Math.Exp(-(x/80));
       
        return exponentialDecay;
    }

    void upDateParameter(int row, int column, float level,int distance)
    {
        for(int i = -distance; i < distance; i++)
        {
            updateParam(row, column + i, level);
            updateParam(row + i, column,level);
        }


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

    void upDateAllColor()
    {
        for (int i = 0; i < widthNum * heightNum; i++)
        {
            int row = i / heightNum;
            int column = (i % heightNum) % heightNum;

            upDateColor(row, column);
        }
    }

    void upDateColor(int row, int column)
    {
        if(row>=0&&column>=0)
        renderers[row,column].material.color = new Color((soundMap[row, column]/testSoundLevel)*1f, 0f, 0f, 1f);
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
        float[,] dst = new float[widthNum,heightNum];
        for(int i=0;i< heightNum; i++)
        {
            for(int j=0;j< widthNum; j++)
            {
                dst[i,j] = (soundMap[i,j] - minimumLevel ) / (maxLevel - minimumLevel);
            }
        }
        Array.Copy(dst,soundMap, dst.Length);



    }
}