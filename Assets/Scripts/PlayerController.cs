using UnityEngine;
using System.Collections;
using Photon.Pun;
using Cinemachine;

public class PlayerController: MonoBehaviourPun
{
    string playerName; 
    Quaternion targetRotation;
    public Animator animator;
    public float speed;
    public Vector3 velocity;
    Vector3 oldVelocity;
    Quaternion rotation;
    bool isJumping;
    public bool checkWait;
    public float walkSpeed = 1.0f;
    public float runSpeed = 2.0f;
    public bool canMove = true;
    GameObject myCamera;
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



    // Use this for initialization
    void Start()
    {
        if (!photonView.IsMine)
        {
            audioListener.enabled = false;
            return;
        }
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

    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }


        if (isMovie == true)
        {
            return;
        }
        

        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (animator == null)
        {
            TryGetComponent(out animator);
        }


        //Sound
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
        

        if (itemState == 0)
        {

        }
        if (itemState == 1)
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
        if (itemState == HasWhistle)
        {
            if (Input.GetMouseButtonDown(1) && !animator.GetCurrentAnimatorStateInfo(0).IsName("light"))
            {
                animator.SetTrigger("Open");
                
                seAudioSource.PlayOneShot(seWhistle);

                soundmanager.soundDetect(this.transform.position, 30f, 30f);

                gameManager.enemyDamaged(transform.position, 30f);


            }
        }


        //速度の取得





        if (canMove ==true)
        {
            //カメラの向きで補正した入力ベクトルの取得
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            var horizontalRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
            var velocity = horizontalRotation * new Vector3(horizontal, 0, vertical).normalized;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("jump"))
            {
                isJumping = true;
            }
            else
            {
                isJumping = false;
            }

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

            if (speed == walkSpeed)
            { animator.SetFloat("speed", 3f); }
            else if (speed == runSpeed)
            { animator.SetFloat("speed", 5f); }
            else if (speed == 0)
            { animator.SetFloat("speed", 0); }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("stand") && checkWait == false)
            {
                StartCoroutine(waitMotion());
                checkWait = true;
            }

            if (Input.GetButtonDown("Jump"))
            {
                isJumping = true;
                animator.SetTrigger("isJump");
            }

            if (Input.GetMouseButtonDown(0) && !animator.GetCurrentAnimatorStateInfo(0).IsName("punch1"))
            {
                animator.SetTrigger("punch");
            }
            if (Input.GetMouseButtonDown(0) && animator.GetCurrentAnimatorStateInfo(0).IsName("punch1"))
            {
                animator.SetTrigger("punch2");
            }
            if (Input.GetMouseButtonDown(0) && animator.GetCurrentAnimatorStateInfo(0).IsName("punch2"))
            {
                animator.SetTrigger("punch3");
            }

            ///Sound処理
            if (speed == runSpeed)
            {
                soundmanager.soundDetect(transform.position, 12f, 0.8f);
            }

        }



    }

    public bool getPhotonviewIsMine()
    {
        return photonView.IsMine;
    }

    public PhotonView getPhotonView()
    {
        return photonView;
    }
    public void OnItemChange(int index)
    {
        itemState = index;
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable["ItemState"] = index;
        hashtable["Message"] = "こんにちは";
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
        }
        if (index == Hasamulet)
        {
            ItemLight.SetActive(false);
            ItemAmulet.SetActive(true);
            ItemWhistle.SetActive(false);
        }
        if (index == HasWhistle)
        {
            ItemLight.SetActive(false);
            ItemAmulet.SetActive(false);
            ItemWhistle.SetActive(true);
        }
    }

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
            if (count > 3.0f)
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

    public void stopAction()
    {
        canMove = true;
        animator.SetTrigger("stop");
        rb.constraints = RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationZ;
    }

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
    public void Free()
    {
        canMove = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationZ;
    }

    public void setName(string name)
    {
        playerName = name;
    }
    public string getName()
    {
        return playerName;
    }

    public　void magic()
    {
        seAudioSource.PlayOneShot(seMagic);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision.gameObject.GetComponent<ZombieController>().beingDamaged == false&&itemState!=Hasamulet)
        {
            
            gameOver(collision.gameObject);
        }
    }

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

    public void EnterLocker(Vector3 cameraPosition)
    {
        canMove = false;
        speed = 0f;
        animator.SetFloat("speed", 0);
        CameraLookAtObject.transform.position = cameraPosition;
        inLocker = true;
    }

    public void ExitLocker()
    {
        canMove = true;
        CameraLookAtObject.transform.localPosition = defaultLookAtPosition;
        inLocker = false;
    }

    public void gameClear()
    {
        canMove = false;
        StartCoroutine(gameClearCamera());
    }

    IEnumerator gameClearCamera()
    {
        for(int i = 0; i < 100; i++)
        {
            yield return null;
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        

    }

}
