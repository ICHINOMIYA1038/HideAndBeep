using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static SoundManager;
using Cinemachine;
using Photon.Pun;

/// <summary>
/// ゾンビを動かすためのクラス
/// </summary>
public class ZombieController : MonoBehaviour
{
    /// <summary>
    /// エネミーの片方の視界。これを二倍したものが、実際の検知範囲
    /// </summary>
    private const float VIEW_ANGLE = 60f;
    private const float MOVE_AROUND_TIMER = 7f;
    private const float WAIT_TIMER = 1.5f;
    Animator animator;
    NavMeshAgent agent;
    [SerializeField] GameObject effect;
    /// <summary>
    /// 敵の顔をアップで映すスクリプト
    /// </summary>
    [SerializeField] CinemachineVirtualCamera enemyCam;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] bool isFinding;
    [SerializeField] GameObject prey;
    [SerializeField] SoundManager soundManager;
    [SerializeField] GameManager gameManager;
    [SerializeField] PhotonView photonView;
    [SerializeField] Collider zombieCollider;
    LockerScript InteractiveLocker;
    [SerializeField] GameObject targetLocker;
    [SerializeField]GameObject[] lockers;
    /// <summary>
    /// エネミーの状態を表すオートマトン
    /// behaviorMode :0 Wait 1 MoveAround 2 Detect 3 MoveTo 4 Arrive 5 Find 6Damaged
    /// </summary>
    [SerializeField]
    [Range(0, 6)]
    public int behaviorMode = 0;
    /// <summary>
    /// 待機モード
    /// </summary>
    public static readonly int WAITING_MODE = 0;
    /// <summary>
    /// ランダムで動き回るモード
    /// </summary>
    public static readonly int MOVEAROUND_MODE = 1;
    /// <summary>
    /// 音を検知した時のモード
    /// 変更時に、サブルーチンを呼び出す。フレームごとには何もしない
    /// </summary>
    public static readonly int DETECT_MODE = 2;
    /// <summary>
    /// 使用しない
    /// </summary>
    public static readonly int MOVETO_MODE = 3;
    /// <summary>
    /// プレイヤーを発見した時のモード
    /// </summary>
    public static readonly int FIND_MODE = 4;
    /// <summary>
    /// ロッカーに入っていることを疑っている時のモード
    /// </summary>
    public static readonly int SUSPECT_LOCKER_MODE = 5;
    /// <summary>
    /// プレイヤーのアイテムなどで怯んでいる時のモード
    /// </summary>
    public static readonly int DAMAGED_MODE = 6;
    /// <summary>
    /// 歩く速度
    /// </summary>
    float walkSpeed = 3.5f;
    /// <summary>
    /// 走る速度
    /// </summary>
    float runSpeed = 10f;
    float detectSpeed = 5f;
    /// <summary>
    /// 索敵時のタイマーで、この時間を超えると、待機モードに移行する。
    /// </summary>
    float detectTimer = 20f;
    /// <summary>
    /// ゲームオーバーあるいは、ゲームクリアの状態かどうか
    /// </summary>
    bool isMovie = false;
    /// <summary>
    /// canMoveがfalseだと敵は止まっている。
    /// </summary>
    bool canMove = true;
    /// <summary>
    /// ダメージを受けているかどうか
    /// これがtrueの時、エネミーはプレイヤーを攻撃できない
    /// </summary>
    public bool beingDamaged = false;
    /// <summary>
    /// プレイヤーを検知できる最大距離
    /// </summary>
    [SerializeField]float detectRange;
    /// <summary>
    /// 音源 うめき声などを再生する
    /// </summary>
    [SerializeField] AudioSource audiosource;
    /// <summary>
    /// ダメージを受けた時のうめき声
    /// </summary>
    [SerializeField] AudioClip damageSE;
    /// <summary>
    /// プレイヤー追跡状態に移行したかどうか
    /// </summary>
    bool isSearching = false;
    /// <summary>
    /// 検知する音の最小の閾値
    /// これを超えると検知する。
    /// </summary>
    [SerializeField]float detectSoundLevel;
    /// <summary>
    /// 待機する時のカウント
    /// これを超えると、moveAroundへ移行する
    /// </summary>
    float waitTimer = 0f;
    /// <summary>
    /// moveAroundにおいて、新しい目的地を設定するためのタイマー
    /// </summary>
    float moveAroundTimer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        lockers = GameObject.FindGameObjectsWithTag("Locker");

        TryGetComponent<Animator>(out animator);
        TryGetComponent<NavMeshAgent>(out agent);
        TryGetComponent<PhotonView>(out photonView);

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
        if (!canMove)
        {
            return;
        }

        if (prey != null)
        {   
            if(prey.GetComponent<PlayerController>().inLocker == true)
            {
                behaviorMode = SUSPECT_LOCKER_MODE;
                isFinding = false;
            }
        }

        if (behaviorMode == WAITING_MODE)
        {
            Wait();
        }
        if (behaviorMode == MOVEAROUND_MODE)
        {
            MoveAround();
        }
        if (behaviorMode == DETECT_MODE)
        {

        }
        if (behaviorMode == FIND_MODE)
        {
            Find();
        }
        if (behaviorMode == SUSPECT_LOCKER_MODE)
        {
            SuspectLocker();
        }
        if (behaviorMode == DAMAGED_MODE)
        {
            
        }

        //アニメーション制御
    　  //スピードが一定の値を下回った時に、待機のアニメーションに移行する。
       //0.5は閾値
        if (agent.speed < 0.5)
        {
            animator.SetBool("Wait", true);
        }
        if (agent.speed >= 0.5)
        {
            animator.SetBool("Wait", false);
        }
        if (isFinding)
        {
            agent.speed = runSpeed;
            animator.SetBool("Run", true);

        }
        if (!isFinding && animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            agent.speed = walkSpeed;
            animator.SetBool("Run", false);
        }

    }

    /// <summary>
    /// ロッカーを近い順に並べるスクリプト
    /// </summary>
    void SortLockerDistance()
    {

        List<KeyValuePair<float,GameObject>> ProvisionalValue = new List<KeyValuePair<float, GameObject>>();
        for(int i = 0; i < lockers.Length; i++)
        {
            ProvisionalValue.Add(new KeyValuePair<float, GameObject>((lockers[i].transform.position - agent.transform.position).magnitude, lockers[i]));
            
        }

        var orderdList = ProvisionalValue.OrderBy(x => x.Key);
        int index = 0;
        foreach(var locker in orderdList)
        {
            lockers[index] = locker.Value;
            Debug.Log(locker.Key);
            index += 1;
        }

    }

    /// <summary>
    /// 待機時の処理
    /// </summary>
    void Wait()
    {
        isFinding = false;
        agent.speed = 0f;
        agent.SetDestination(agent.transform.position);
        if ((targetPosition - agent.transform.position).magnitude < detectRange)
        {
            if (CatchSight())
            {
                return;
            }
        }
        else if (HearSound())
        {
            behaviorMode = DETECT_MODE;;
        }
        waitTimer += Time.deltaTime;
        if (waitTimer > WAIT_TIMER)
        {
            waitTimer = 0f;
            behaviorMode = MOVEAROUND_MODE;
            setDestination();
        }
 
    }
    /// <summary>
    /// 動き回る処理
    /// </summary>
    void MoveAround()
    {
        isFinding = false;
        agent.speed = walkSpeed;
        moveAroundTimer += Time.deltaTime;
        if ((targetPosition - agent.transform.position).magnitude < detectRange)
        {
            CatchSight();
        }
        else if (HearSound())
        {
            behaviorMode = DETECT_MODE;
        }
        else if (moveAroundTimer > MOVE_AROUND_TIMER)
        {
            moveAroundTimer = 0f;
            behaviorMode = WAITING_MODE;
            agent.SetDestination(agent.transform.position);
        }

    }

    /// <summary>
    /// 目的地の設定
    /// </summary>
    /// <returns></returns>
    void setDestination()
    {
            targetPosition = new Vector3(Random.Range(-50f, 100f), Random.Range(-50f, 100f), Random.Range(-50f, 100f));
            agent.SetDestination(targetPosition);
            
    }

    /// <summary>
    ///　音を検出して動き出すときの処理
    ///　タイマーを設定して、指定時間を超えたら待機モードにうつる。
    /// </summary>
    /// <returns></returns>
    IEnumerator Detect()
    {
        agent.speed = detectSpeed;
        agent.SetDestination(targetPosition);
        float timer = 0;
        while ((agent.transform.position - targetPosition).magnitude > 1f)
        {
            timer += Time.deltaTime;
            if (CatchSight())
            {
                yield break;
            }
            if (timer > detectTimer)
            {
                
                behaviorMode = WAITING_MODE;
                yield break;
            }
            

            yield return null;
        }
        StartCoroutine("SearchAround");

    }

    /// <summary>
    ///　ランダムで動き回る時のモード
    /// </summary>
    /// <returns></returns>
    IEnumerator SearchAround()
    {

        float deltaAngle = -5f;
        for (int i = 0; i < 16; i++)
        {
            transform.RotateAround(agent.transform.position, transform.up, deltaAngle);
            if (CatchSight())
            {
                yield break;
            }
            yield return null;
        }
        for (int i = 0; i < 32; i++)
        {
            transform.RotateAround(agent.transform.position, transform.up, -deltaAngle);
            if (CatchSight())
            {
                yield break;
            }
            yield return null;
        }
        behaviorMode = WAITING_MODE;



    }

    /// <summary>
    /// プレイヤーを発見した時のモード
    /// </summary>
    void Find()
    {
        isFinding = true;
        targetPosition = prey.transform.position;
        agent.SetDestination(targetPosition);
        if ((targetPosition - agent.transform.position).magnitude > detectRange)
        {
            behaviorMode = WAITING_MODE;
        }
        if (prey.GetComponent<PlayerController>().inLocker)
        {
            
            prey = null;
            isFinding = false;
            animator.SetBool("Run", false);
            animator.SetBool("Wait", true);
            agent.SetDestination(agent.transform.position);
            agent.speed = 0f;
            behaviorMode = SUSPECT_LOCKER_MODE;
        }
    }

    /// <summary>
    /// マップ上から音を検出する
    /// もし、大きな音がなったら、detectモードへ移行する
    /// </summary>
    /// <returns></returns>
    bool HearSound()
    {
        if (soundManager == null)
        {
            return false;
        }
        soundData mySoundData = soundManager.MaxPosition();
        if (mySoundData.Value > 10f)
        {
            targetPosition = mySoundData.Position;
            StartCoroutine("Detect");


            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// プレイヤーを視界に検知するスクリプト
    /// rayの角度をy軸周り(transform.up方向)で変えて、視界を検知する。
    /// プレイヤーがrayで検出された場合に、FIND_MODEへ移行する。
    /// </summary>
    /// <returns></returns>
    bool CatchSight()
    {

        float ViewAngle = VIEW_ANGLE;
        float deltaAngle = -ViewAngle;
        RaycastHit hit;
        Ray ray = new Ray(agent.transform.position + new Vector3(0f, 10f, 0f), agent.transform.forward);


        while (deltaAngle < ViewAngle)
        {
            deltaAngle += 5f;
            ray.direction = Quaternion.AngleAxis(deltaAngle, transform.up) * agent.transform.forward;
            if (Physics.Raycast(ray, out hit, detectRange))
            {
                if (hit.transform.gameObject.tag == "Player")
                {
                    prey = hit.transform.gameObject;
                    behaviorMode = FIND_MODE;
                    return true;
                }
                /*
                if (hit.transform.gameObject.tag == "Locker")
                {
                    prey = hit.transform.gameObject;

                    InteractiveLocker = hit.transform.gameObject.GetComponentInChildren<LockerScript>();
                    
                    seeLocker(hit.transform.gameObject);
                    
                    return true;
                }
                */

            }
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 30f, Color.red, 1f);

        }

        return false;
    }

    /// <summary>
    /// エネミーの現在の状況を取得する
    /// </summary>
    /// <returns></returns>
    public int getEnemyState()
    {
        return behaviorMode;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player")&&gameManager.sceneState==1)
        {
            PlayerController pcon;
            pcon = collision.gameObject.GetComponent<PlayerController>();
            Collider Pcollider = pcon.GetComponent<Collider>();
            if (pcon.itemState != PlayerController.Hasamulet)
            {
                if (beingDamaged)
                {
                    return;
                }
                if (!pcon.getPhotonviewIsMine())
                {
                    return;
                }
                isMovie = true;
                enemyCam.enabled = true;
                enemyCam.Priority = 30;
                gameOver();
                animator.SetTrigger("jump");
                gameManager.sceneState = 0;
                
            }
            else
            {
                pcon.magic();
                pcon.OnItemChange(0);
                Freeze();
                damaged();
                Instantiate(effect, agent.transform.position, Quaternion.identity);

            }


        }

        /*
        if (collision.gameObject.CompareTag("Locker") && gameManager.sceneState == 1)
        {
            animator.SetBool("Run", false);
            agent.SetDestination(agent.transform.position);
            agent.speed = 0f;
            behaviorMode = SUSPECT_LOCKER_MODE;
            seeLocker(collision.gameObject,collision.gameObject.GetComponent<LockerScript>());
        }
        */
    }

    /// <summary>
    /// ゲームオーバー時の処理
    /// </summary>
    public void gameOver()
    {
        gameManager.gameOver();
    }

    /// <summary>
    /// ロッカーに近づくスクリプト
    /// </summary>
    /// <param name="locker"></param>
    public void seeLocker(GameObject locker)
    {
        targetLocker = locker;
        agent.SetDestination(locker.transform.GetChild(4).position);
        StartCoroutine("openLocker");

        LockerScript lockerScript = locker.GetComponentInChildren<LockerScript>();

        lockerScript.open();
        

    }

    /// <summary>
    /// ロッカーを開けるスクリプト
    /// </summary>
    /// <returns></returns>
    IEnumerator openLocker()
    {
        Debug.Log("startOpenLocker");
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 10f)
            {
                Debug.Log("timeOut");
                behaviorMode = MOVEAROUND_MODE;
                yield break;
            }
            if ((targetLocker.transform.position - agent.transform.position).magnitude < 10f)
            {
                Debug.Log("reachLocker");
                transform.LookAt(targetLocker.transform);
                Freeze();
                OpenLocker(targetLocker);
                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// ロッカーを開けるスクリプト
    /// プレイヤーが入っていたら、ゲームオーバーに移行する。
    /// そうでないならば、別の動作をする
    /// </summary>
    /// <param name="locker"></param>
    public void OpenLocker(GameObject locker)
    {
        animator.SetTrigger("Open");
        isSearching = false;
        LockerScript lockerScript = locker.GetComponentInChildren<LockerScript>();
        lockerScript.Opend();
        if (lockerScript.playerExistsInLocker)
        {
            isMovie = true;
            enemyCam.enabled = true;
            enemyCam.transform.position += transform.up * 1f;
            enemyCam.Priority = 30;
            gameOver();
            lockerScript.getPlayerController().gameOver(this.gameObject);
            
            gameManager.sceneState = 0;
        }
        else
        {
            StartCoroutine(CantFindPlayerinLocker(lockerScript));
        }
        
    }

    IEnumerator CantFindPlayerinLocker(LockerScript lockerScript)
    {
        yield return new WaitForSeconds(1);
        animator.SetTrigger("Open");
        lockerScript.Closed();
        yield return new WaitForSeconds(1);
        behaviorMode = MOVEAROUND_MODE;
        setDestination();
    }

    /// <summary>
    /// エネミーをフリーズさせる関数
    /// </summary>
    public void Freeze()
    {
        canMove = false;
        agent.speed = 0f;
        agent.SetDestination(agent.transform.position);
        animator.SetFloat("speed", 0);

    }

    /// <summary>
    /// エネミーをフリーズから解放させる関数
    /// </summary>
    public void Free()
    {
        canMove = true;
        agent.speed = 0f;
        animator.SetFloat("speed", 0);
    }
    /// <summary>
    /// 攻撃を受けた時の関数
    /// </summary>
    public void damaged()
    {
        behaviorMode = DAMAGED_MODE;
        Freeze();
        animator.SetTrigger("Damaged");
        beingDamaged = true;
        StartCoroutine("DamageEvent");
        audiosource.PlayOneShot(damageSE);
    }

    ///ダメージを受けた時の非同期処理
    IEnumerator DamageEvent()
    {
        yield return new WaitForSeconds(3);
        beingDamaged = false;
        Free();
        behaviorMode = WAITING_MODE;
        yield break;
    }

    /// <summary>
    /// ロッカーに入っていることを疑っている時に処理される関数
    /// ロッカーをエネミーから近い順番に並べて、一番近いロッカーを探す。
    /// </summary>
    public void SuspectLocker()
    {
        if (!isSearching)
        {
            SortLockerDistance();
            isSearching = true;
            seeLocker(lockers[0]);
        }
    }
}