using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;


public class SceneChangeManager : MonoBehaviour
{
    private static SceneChangeManager instance;
    int GammeState;
    int playerHP;
    int playermaxHP;
    public const int TITLE = 0;
    public const int MAIN = 1;
    public const int GAMEOVER = 2;
    public const int GAMECLEAR = 3;
    [SerializeField]
    GameObject gameOverText;

    public static SceneChangeManager Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(SceneChangeManager);

                instance = (SceneChangeManager)FindObjectOfType(t);
                if (instance == null)
                {
                    Debug.LogError("???");
                }
            }

            return instance;
        }
    }

    protected bool CheckInstance()
    {
        if (instance == null)
        {
            instance = this;
            return true;
        }
        else if (Instance == this)
        {
            return true;
        }
        Destroy(this);
        return false;
    }

    void syncPlayerProperty() { }

   public static void checkFlag(int flag) {
        switch (flag)
        {
            case TITLE:
                SceneManager.LoadScene(0);
                break;
            case MAIN:
                SceneManager.LoadScene(MAIN);
                break;
            case GAMEOVER:
                
                break;
            case GAMECLEAR:
                SceneManager.LoadScene(GAMECLEAR);
                break;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        CheckInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
