using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

/// <summary>
/// サウンドマネージャークラス
/// プレイヤーやオブジェクトから発生する音を検知して、エネミーに伝える役割をする。
/// </summary>
public class SoundManager : MonoBehaviour
{
    Text[] text;
    float[] soundMap;
    [SerializeField] Vector3[] soundPosition;
    [SerializeField] GameObject flagPrefab;
    [SerializeField]GameObject[] items;
    [SerializeField] Renderer[] renderers;
     int pointNum=800;
    [SerializeField] GameManager gameManager;
    [SerializeField] float AttenuationLevel=30f;
    [SerializeField] Vector3 testPosition;
    float deltaTimeSound = 1f;
    [SerializeField] float testSoundLevel = 30f;
    [SerializeField] float testRange = 30f;
    
    [SerializeField] Vector3 offset = new Vector3(0f,0f,0f);
    [SerializeField] float interval = 15f;
    /// <summary>
    /// マップを可視化するためのスクリプト
    /// </summary>
    [SerializeField] bool visibleMap;


    void Awake()
    {
        soundMap = new float[pointNum];
        soundPosition = new Vector3[pointNum];
        items = new GameObject[pointNum];
        text = new Text[pointNum];
        renderers = new Renderer[pointNum];

        for (int i = 0; i < 20; i++)
        {
            for(int j=0; j< 20; j++)
            {
                soundPosition[i * 20 + j] = new Vector3(i * interval, 0f, j * interval) + offset;
                soundPosition[i * 20 + j + 400] = new Vector3(i * interval, 22.5f, j * interval) + offset;
            }
            
        }

    }
    // Start is called before the first frame update

    void Start()
    {
        for (int i = 0; i < pointNum; i++)
        {
            items[i] = Instantiate(flagPrefab, soundPosition[i], Quaternion.identity);
            renderers[i] = items[i].GetComponent<Renderer>();
            text[i] = items[i].GetComponentInChildren<Text>();
            soundMap[i] = 0;
        }
        if (!visibleMap)
        {
            for (int i = 0; i < pointNum; i++)
            {
                items[i].SetActive(false);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.sceneState != 1)
        {
            return;
        }
        
        deltaTimeSound -= 1f*Time.deltaTime;
        if(deltaTimeSound < 0)
        {
            deltaTimeSound = 1f;
            attenuationSound(AttenuationLevel);
        }
        upDateAllColor();
    }

    void CatchSound(float soundLevel, Vector3 position)
    {
        int row = 0;
        int column = 0;
        int length = alocationSound(position);
        


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

    public void soundDetect(Vector3 position, float range,float soundLevel)
    {
        foreach (var posi in soundPosition.Select((v, i) => new { Value = v, Index = i }))
        {
            float distance = (posi.Value - position).magnitude;
            if (range >= distance)
            {
                double scaleFactor = scaleFunction(distance);
                updateParam(posi.Index, (float)(scaleFactor * soundLevel));
            }
        }
    }



    private void attenuationSound(float attenuationRate)
    {
        for(int i = 0; i<pointNum; i++)
        {
            soundMap[i] -= attenuationRate;
            if (soundMap[i]<0)
            {
                soundMap[i] = 0;
            }
            //text[i].text = String.Format("{0:#.##}",soundMap[i / heightNum, (i % heightNum) % heightNum]);
        }
    }

    private void updateParam(int index, float level)
    {
        if (index<0&&index>=pointNum)
        {
            return;
        }
        else
        {
            soundMap[index] += level;
            
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    private double scaleFunction(float x)
    {
        double exponentialDecay = Math.Exp(-(x/80));
       
        return exponentialDecay;
    }

    

    void upDateAllColor()
    {
        for (int i = 0; i < pointNum; i++)
        {

            upDateColor(i);
        }
    }

    void upDateColor(int index)
    {
        if(index >= 0 && index < pointNum)
        renderers[index].material.color = new Color((soundMap[index]/testSoundLevel)*1f, 0f, 0f, 1f);
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
        float[] dst = new float[pointNum];
        for(int i=0;i< pointNum; i++)
        {
            
                dst[i] = (soundMap[i] - minimumLevel ) / (maxLevel - minimumLevel);
            
        }
        Array.Copy(dst,soundMap, dst.Length);



    }

    public soundData MaxPosition()
    {
        float maxLevel = 0f;
        int maxIndex = 0;
        foreach (var level in soundMap.Select((v, i) => new { Value = v, Index = i }))
        {
            if (maxLevel < level.Value)
            {
                maxLevel = level.Value;
                maxIndex = level.Index;
            }
        }
        soundData sound = new soundData();
        sound.Value = maxLevel;
        sound.Position = soundPosition[maxIndex];

        return sound;
    }

    public struct soundData
    {
        public float Value;
        public Vector3 Position;
    }
}