using UnityEngine;
using System.Collections;
using Photon.Pun;
using Cinemachine;
using UnityEngine.UI;

public class PlayerController: MonoBehaviourPun
{
    string playerName; 
    Quaternion targetRotation;
    public Animator animator;
    public float speed;
    public Vector3 velocity;
    public bool checkWait;
    public float walkSpeed = 1.0f;
    public float runSpeed = 2.0f;
    public bool canMove = true;
    Rigidbody rb;
    public int itemState;
    static readonly int HasNoItem = 0;
    static readonly int HasLight = 1;
    public static readonly int HasWhistle = 2;
    public static readonly int Hasamulet = 3;
    public static readonly int HasKey = 4;
    [SerializeField] GameObject CameraLookAtObject;
    Vector3 defaultLookAtPosition;
    [SerializeField] GameObject ItemLight;
    [SerializeField] GameObject ItemWhistle;
    [SerializeField] GameObject ItemAmulet;
    [SerializeField] GameObject blankImage;
    [SerializeField] SoundManager soundmanager;
    [SerializeField] GameObject[] enemys;
    ZombieController[] zombieControllers;
    [SerializeField] AudioSource audiosource;
    float audioMaxDistance = 80f;
    [SerializeField] AudioClip bgm;
    [SerializeField] AudioClip chaseBGM;
    [SerializeField] CinemachineFreeLook NormalCamera;
    [SerializeField] CinemachineVirtualCamera gameOvercamera;
    [SerializeField] AudioSource seAudioSource;
    [SerializeField] AudioClip seMagic;
    [SerializeField] AudioClip seWhistle;
    [SerializeField] AudioListener audioListener;
    bool isMovie = false;
    public bool inLocker = false;
    GameManager gameManager;
    GameObject ItemCoolTimePanel;
    float itemCooltime = 5f;
    float coolTimeTimer = 0.0f;
    float changeAnimationTime = 8.0f;



    // Use this for initialization
    void Start()
    {
        soundmanager = GameObject.FindWithTag("GameManager").GetComponent<SoundManager>();
        if (!photonView.IsMine)
        {
            audioListener.enabled = false;
            return;
        }
        OnItemChange(0);
        ItemCoolTimePanel = GameObject.FindGameObjectWithTag("cooltimePanel");
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        defaultLookAtPosition = CameraLookAtObject.transform.localPosition;
        enemys = GameObject.FindGameObjectsWithTag("Enemy");
        zombieControllers = new ZombieController[2];
        for (int i = 0; i < 2; i++)
        {
            zombieControllers[i] = enemys[i].GetComponent<ZombieController>();
        }
        soundmanager = GameObject.FindWithTag("GameManager").GetComponent<SoundManager>();
        
        TryGetComponent(out animator);
        targetRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        Application.targetFrameRate =60;
        ItemAmulet.SetActive(false);
        ItemWhistle.SetActive(false);
        ItemLight.SetActive(false);
        ItemCoolTimePanel.GetComponent<Image>().fillAmount = 0f;
        ///アニメーターを取得。
        if (animator == null)
        {
            TryGetComponent(out animator);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ///Sound処理
        ///走っている時に、サウンドマネージャーに音を拾わせる。
        ///この処理に関しては、プレイヤーの所有者に関わらず行う。
        ///
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("run"))
        {
            float range = 12f;
            float soundLevel = 0.8f;
            soundmanager.soundDetect(transform.position, range, soundLevel);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("searchChest"))
        {
            float range = 12f;
            float soundLevel = 2f;
            soundmanager.soundDetect(transform.position, range, soundLevel);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("LiftUp"))
        {
            float range = 12f;
            float soundLevel = 2f;
            soundmanager.soundDetect(transform.position, range, soundLevel);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("open"))
        {
            float range = 20f;
            float soundLevel = 10f;
            soundmanager.soundDetect(transform.position, range, soundLevel);
        }
        //このキャラクターの所有者が自分自身でないならば、処理を中止する。
        if (!photonView.IsMine)
        {
            return;
        }

        ///ゲームクリアやゲームエンドの状態ならば、処理をしない。
        if (isMovie == true)
        {
            return;
        }
        

        ///Escapeキーを押すと、処理を終了する。
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        


        //Soundの処理。
        ///Enemyが近づくに連れて、音が大きくなる。
        float distance = 100;

        if (zombieControllers[0].getEnemyState() == ZombieController.FIND_MODE || zombieControllers[1].getEnemyState() == ZombieController.FIND_MODE)
        {
            audiosource.clip = chaseBGM;
        }
        if (zombieControllers[0].getEnemyState() != ZombieController.FIND_MODE && zombieControllers[1].getEnemyState() != ZombieController.FIND_MODE)
        {
            audiosource.clip = bgm;
        }

        foreach (var enemy in enemys)
        {
            
            
            if ((enemy.transform.position - transform.position).magnitude < distance)
            {
                distance = (enemy.transform.position - transform.position).magnitude;
            }
                
        }

        if (distance < audioMaxDistance && audiosource.isPlaying == false)
        {
            audiosource.Play();
            audiosource.volume = 1f * (1 - Mathf.Pow((distance / audioMaxDistance), 2));
            Debug.Log(audiosource.volume);
        }
        else if (distance < audioMaxDistance && audiosource.isPlaying == true)
         {
            audiosource.volume = 1f * (1 - Mathf.Pow((distance / audioMaxDistance),2));
        }
        else if (distance > 80f && audiosource.isPlaying == true)
        {
            audiosource.Stop();
            
        }

        ///ライトを持っている時の処理
        if (itemState == HasLight)
        {
            if (Input.GetMouseButtonDown(1) && !animator.GetCurrentAnimatorStateInfo(0).IsName("light"))
            {
                animator.SetTrigger("light");
            }

            if (Input.GetMouseButtonDown(1) && animator.GetCurrentAnimatorStateInfo(0).IsName("light"))
            {
                animator.SetTrigger("Lightstop");
            }
        }

        ///ホイッスルを持っている時の処理
        if (itemState == HasWhistle)
        {
            ItemCoolTime();
            if (coolTimeTimer >= itemCooltime)
            {
                if (Input.GetMouseButtonDown(1) && !animator.GetCurrentAnimatorStateInfo(0).IsName("light"))
                {
                    animator.SetTrigger("Open");

                    seAudioSource.PlayOneShot(seWhistle);

                    soundmanager.soundDetect(this.transform.position, 30f, 30f);

                    gameManager.enemyDamaged(transform.position, 30f);

                    coolTimeTimer = 0.0f;
                }
            }
        }

        ///
        if (canMove ==true)
        {
            //カメラの向きで補正した入力ベクトルの取得
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            var horizontalRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
            var velocity = horizontalRotation * new Vector3(horizontal, 0, vertical).normalized;
            ///シフトを押しているときに、スピード走る速度を変える。
            speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            if (horizontal == 0 && vertical == 0)
            {
                speed = 0f;
            }
            var rotationSpeed = 600 * Time.deltaTime;
            transform.position += velocity * speed * 1f * Time.deltaTime;

            //移動方向を向く
            if (velocity.magnitude > 0.5f)
            {
                targetRotation = Quaternion.LookRotation(velocity, Vector3.up);
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);

            ///アニメーションの遷移
            if (speed == walkSpeed)
            { animator.SetFloat("speed", 3f); }
            else if (speed == runSpeed)
            { animator.SetFloat("speed", 5f); }
            else if (speed == 0)
            { animator.SetFloat("speed", 0); }

            /*
            ///アニメーションの二重チェック
            ///アニメーションに入っている時には、canMoveをfalseにしておく
           
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("searchChest"))
            {
                canMove = false;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("LiftUp"))
            {
                canMove = false;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("open"))
            {
                canMove = false;
            }
            */

            ///立って放置しているときに、待ちのモーションをチェックする。
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("stand") && checkWait == false)
            {
                StartCoroutine(waitMotion());
                checkWait = true;
            }

            

        }



    }

    /// <summary>
    /// このプレイヤーコントローラーが自分自身で操作しているものかをチェックする。
    /// </summary>
    /// <returns></returns>
    public bool getPhotonviewIsMine()
    {
        return photonView.IsMine;
    }

    /// <summary>
    /// このオブジェクトが持つPhotonViewを返す。
    /// </summary>
    public PhotonView getPhotonView()
    {
        return photonView;
    }

    /// <summary>
    /// アイテムを変更した時の処理
    /// </summary>
    /// <param name="index">アイテムを指定するインデックス</param>
    public void OnItemChange(int index)
    {
        if (!photonView.IsMine)
        {
            return; 
        }
        itemState = index;
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable["ItemState"] = index;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        if(index==HasNoItem)
        {
            ItemLight.SetActive(false);
            ItemAmulet.SetActive(false);
            ItemWhistle.SetActive(false);
            
        }
        if (index == HasLight)
        {
            ItemLight.SetActive(true);
            ItemAmulet.SetActive(false);
            ItemWhistle.SetActive(false);
            if (photonView.IsMine)
            {
                gameManager.AddText("ライトを手に入れた。使用:右クリック");
            }
        }
        if (index == Hasamulet)
        {
            ItemLight.SetActive(false);
            ItemAmulet.SetActive(true);
            ItemWhistle.SetActive(false);
            gameManager.AddText("お守りを手に入れた。敵の攻撃を一度防ぐ。");
        }
        if (index == HasWhistle)
        {
            ItemLight.SetActive(false);
            ItemAmulet.SetActive(false);
            ItemWhistle.SetActive(true);
            gameManager.AddText("ホイッスルを手に入れた。使用:右クリック");
        }
    }

    /// <summary>
    /// 指定秒数を経過すると、だらけたアニメーションになる。
    /// </summary>
    IEnumerator waitMotion()
    {
        float count = 0f;
        while (true)
        {
            yield return null;
            count += Time.deltaTime;
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("stand"))
            {
                checkWait = false;
                count = 0f;
                break;

            }
            if (count > changeAnimationTime)
            {
                if (speed == 0)
                {
                    animator.SetTrigger("waitting");
                    checkWait = false;
                    break;
                }
                //break;
            }

        }



    }

    /// <summary>
    /// 箱を開けるスクリプト
    /// 箱開けのアニメーションを開始する。
    /// </summary>
    /// <param name="playerPosi">プレイヤーが立つ位置</param>
    /// <param name="targetPosi">箱の位置</param>
    public void searchBox(Vector3 playerPosi, Vector3 targetPosi)
    {
        transform.position = playerPosi;
        transform.LookAt(targetPosi);
        animator.SetTrigger("searchChest");
        speed = 0f;
        canMove = false;
        rb.constraints = RigidbodyConstraints.FreezePosition
            | RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationY
            | RigidbodyConstraints.FreezeRotationZ;

    }

    /// <summary>
    /// 本棚を持ち上げる処理
    /// 持ち上げのアニメーションを再生する。
    /// </summary>
    /// <param name="playerPosi"></param>
    /// <param name="targetPosi"></param>
    public void LiftUpBookShelf(Vector3 targetPosi)
    {
        transform.LookAt(targetPosi);
        animator.SetTrigger("LiftUp");
        speed = 0f;
        canMove = false;
        rb.constraints = RigidbodyConstraints.FreezePosition
            | RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationY
            | RigidbodyConstraints.FreezeRotationZ;

    }

    /// <summary>
    /// レバーを引く処理
    /// プレイヤーを指定の位置に立たせて、レバーの方向を向かせる。
    /// Rigidbodyで動きの制限と角度の制限を加える。
    /// </summary>
    /// <param name="playerPosi"></param>
    /// <param name="targetPosi"></param>
    public void pullLever(Vector3 playerPosi, Vector3 targetPosi)
    {
        transform.position = playerPosi;
        transform.LookAt(targetPosi);
       // animator.SetTrigger("searchChest");
        speed = 0f;
        canMove = false;
        rb.constraints = RigidbodyConstraints.FreezePosition
            | RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationY
            | RigidbodyConstraints.FreezeRotationZ;

    }

    /// <summary>
    /// アクションを辞める時の処理。Rigidbodyで動きの制限と角度の制限を変える。
    /// </summary>
    public void stopAction()
    {
        canMove = true;
        animator.SetTrigger("stop");
        rb.constraints = RigidbodyConstraints.FreezePositionY
            | RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationZ;
    }

    /// <summary>
    /// アニメーションなどでキャラクターを固定するための関数
    /// </summary>
    public void Freeze()
    {
        speed = 0f;
        animator.SetFloat("speed", 0);
        canMove = false;
        rb.constraints = RigidbodyConstraints.FreezePosition
            | RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationY
            | RigidbodyConstraints.FreezeRotationZ;
    }
    /// <summary>
    /// 固定を解除するための関数
    /// </summary>
    public void Free()
    {
        canMove = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationZ;
    }

    /// <summary>
    /// 名前を格納する。
    /// </summary>
    /// <param name="name"></param>
    public void setName(string name)
    {
        playerName = name;
    }
    /// <summary>
    /// 名前を取得する
    /// </summary>
    /// <returns></returns>
    public string getName()
    {
        return playerName;
    }

    /// <summary>
    /// お守りのアイテム効果が発動した時のスクリプト
    /// </summary>
    public　void magic()
    {
        seAudioSource.PlayOneShot(seMagic);
    }

    /// <summary>
    /// エネミーと接触した時の処理
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Enemy") && collision.gameObject.GetComponent<ZombieController>().beingDamaged == false&&itemState!=Hasamulet)
        {
            
            gameOver(collision.gameObject);
        }
    }

    /// <summary>
    /// ゲームオーバーの処理
    /// </summary>
    /// <param name="enemy"></param>
    public void gameOver(GameObject enemy)
    {
        gameOvercamera.LookAt = enemy.transform;
        NormalCamera.Priority = -1;
        gameOvercamera.Priority = 10;
        isMovie = true;
        rb.constraints = RigidbodyConstraints.FreezePosition
        | RigidbodyConstraints.FreezeRotationX
        | RigidbodyConstraints.FreezeRotationY
        | RigidbodyConstraints.FreezeRotationZ;
    }

    /// <summary>
    /// ロッカーに入る時の処理
    /// </summary>
    /// <param name="cameraPosition">カメラの変更位置</param>
    public void EnterLocker(Vector3 cameraPosition)
    {

        canMove = false;
        speed = 0f;
        animator.SetFloat("speed", 0);
        CameraLookAtObject.transform.position = cameraPosition;
        inLocker = true;
    }

    /// <summary>
    /// ロッカーを出る時の処理
    /// </summary>
    public void ExitLocker()
    {
        canMove = true;
        CameraLookAtObject.transform.localPosition = defaultLookAtPosition;
        inLocker = false;
    }

    /// <summary>
    /// ゲームクリアの処理
    /// </summary>
    public void gameClear()
    {
        canMove = false;
        StartCoroutine(gameClearCamera());
    }

    /// <summary>
    /// アイテムのクールタイムの処理
    /// アイテムを使った後に、呼び出す。
    /// </summary>
    public void ItemCoolTime()
    {
        coolTimeTimer += Time.deltaTime;
        if(coolTimeTimer/ itemCooltime > 1f)
        {
            coolTimeTimer = itemCooltime;
        }
        ItemCoolTimePanel.GetComponent<Image>().fillAmount = 1 - (coolTimeTimer/itemCooltime);
    }

    /// <summary>
    /// ゲームクリア時のカメラの動き。
    /// </summary>
    /// <returns></returns>
    IEnumerator gameClearCamera()
    {
        for(int i = 0; i < 100; i++)
        {
            yield return null;
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        

    }

}
