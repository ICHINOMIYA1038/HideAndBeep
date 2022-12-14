using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タイトル画面で音量を調整するためのクラス
/// </summary>
public class volumeController : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider seSlider;
    // Start is called before the first frame update
    void Start()
    {
        bgmSlider.onValueChanged.AddListener(SetVolumeBGM);
        seSlider.onValueChanged.AddListener(SetVolumeSE);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //BGM
    public void SetVolumeBGM(float value)
    {
        //5段階補正
        value /= 5;
        //-80~0に変換
        var volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);
        //audioMixerに代入
        audioMixer.SetFloat("BGM", volume);
        Debug.Log($"BGM:{volume}");
    }

    //SE
    public void SetVolumeSE(float value)
    {
        //5段階補正
        value /= 5;
        //-80~0に変換
        var volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);
        //audioMixerに代入
        audioMixer.SetFloat("SE", volume);
        Debug.Log(audioMixer.SetFloat("SE", volume));
        Debug.Log($"SE:{volume}");
    }
}
