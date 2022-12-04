using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
public class caligrahyGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject caligraphy;
    [SerializeField]GameObject[] papers;
    TextMeshProUGUI[] mainTexts;
    TextMeshProUGUI[] subTexts;
    [SerializeField]
    TextAsset textAsset;
    
    // Start is called before the first frame update
    void Start()
    {
        papers = new GameObject[22];
        mainTexts = new TextMeshProUGUI[22];
        subTexts = new TextMeshProUGUI[22];
        for (int i = 0; i < 22; i++)
        {
            papers[i] = caligraphy.transform.GetChild(i).gameObject;
            var texts = papers[i].GetComponentsInChildren<TextMeshProUGUI>();
            mainTexts[i] = texts[0];
            subTexts[i] = texts[1];

        }

        string TextLines = textAsset.text;
        string[] textMessage = TextLines.Split('\n'); 
        string[] randomTextMessage = textMessage.OrderBy(i => Guid.NewGuid()).ToArray();
        for (int i=0; i< randomTextMessage.Length;i++)
        {
            string[] texts = randomTextMessage[i].Split(',');
            setTexts(texts[0],texts[1], i);   
        }

    }

    void setTexts(string main, string name,int index)
    {
        mainTexts[index].text = main;
        subTexts[index].text = name;
      
    }

    void setTexts(string main,int index)
    {
        mainTexts[index].text = main;
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
