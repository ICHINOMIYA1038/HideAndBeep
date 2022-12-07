using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static SoundManager;
using Cinemachine;
using Photon.Pun;

public class ZombieController : MonoBehaviour
{
    Rigidbody rb;
    Animator animator;
    NavMeshAgent agent;
    [SerializeField] GameObject effect;
    [SerializeField] CinemachineVirtualCamera vcam;
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
    [SerializeField]
    /// <summary>
    /// behaviorMode :0 Wait 1 MoveAround 2 Detect 3 MoveTo 4 Arrive 5 Find 
    /// </summary>
    [Range(0, 5)]
    public int behaviorMode = 0;
    public static readonly int WAITING_MODE = 0;
    public static readonly int MOVEAROUND_MODE = 1;
    public static readonly int DETECT_MODE = 2;
    public static readonly int MOVETO_MODE = 3;
    public static readonly int FIND_MODE = 4;
    public static readonly int SUSPECT_LOCKER_MODE = 5;
    public static readonly int DAMAGED_MODE = 6;
    float walkSpeed = 3.5f;
    float runSpeed = 10f;
    float detectSpeed = 5f;
    bool isMovie = false;
    bool canMove = true;
    public bool beingDamaged = false;
    [SerializeField]float detectRange;
    [SerializeField] AudioSource audiosource;
    [SerializeField] AudioClip damageSE;
    bool isSearching = false;

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
        if (behaviorMode == MOVETO_MODE)
        {
            MoveTo();
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

    void Wait()
    {
        isFinding = false;
        agent.speed = 0f;
        agent.SetDestination(agent.transform.position);
        if ((targetPosition - agent.transform.position).magnitude < 70f)
        {
            if (CatchSight())
            {
                return;
            }
        }
        if (HearSound())
        {
            behaviorMode = DETECT_MODE;;
        }



    }
    void MoveAround()
    {
        isFinding = false;
        agent.speed = walkSpeed;
        targetPosition = new Vector3(Random.Range(-50f, 100f), Random.Range(-50f, 100f), Random.Range(-50f, 100f));
        agent.SetDestination(targetPosition);
        if ((targetPosition - agent.transform.position).magnitude < 50f)
        {
            CatchSight();
        }

    }
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
            if (timer > 20f)
            {
                
                behaviorMode = WAITING_MODE;
                yield break;
            }
            

            yield return null;
        }
        StartCoroutine("SearchAround");

    }

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


    void MoveTo()
    {
        isFinding = false;
        agent.SetDestination(targetPosition);
        if ((targetPosition - agent.transform.position).magnitude < 50f)
        {
            behaviorMode = FIND_MODE;
        }
    }
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
    bool CatchSight()
    {
        //Quaternion.AngleAxis(deltaAngle, transform.up)

        float ViewAngle = 60f;
        float deltaAngle = -ViewAngle;
        RaycastHit hit;
        //Ray ray = new(agent.transform.position + new Vector3(0f, 15f, 0f), Quaternion.AngleAxis(deltaAngle, transform.up) * agent.transform.forward);
        //Ray ray2 = new Ray(agent.transform.position + new Vector3(0f, 15f, 0f), agent.transform.up);
        Ray ray3 = new Ray(agent.transform.position + new Vector3(0f, 10f, 0f), agent.transform.forward);


        while (deltaAngle < ViewAngle)
        {
            deltaAngle += 5f;
            ray3.direction = Quaternion.AngleAxis(deltaAngle, transform.up) * agent.transform.forward;
            if (Physics.Raycast(ray3, out hit, detectRange))
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

            //Debug.DrawLine(ray.origin, ray.direction * 30f, Color.blue, 1f);
            //Debug.DrawLine(ray2.origin, ray2.direction * 30f, Color.green, 1f);
            Debug.DrawLine(ray3.origin, ray3.origin + ray3.direction * 30f, Color.red, 1f);

        }

        return false;
    }

    public int getEnemyState()
    {
        return behaviorMode;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }
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
                isMovie = true;
                vcam.enabled = true;
                vcam.Priority = 30;
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

    public void gameOver()
    {
        gameManager.gameOver();
    }

    public void seeLocker(GameObject locker)
    {
        targetLocker = locker;
        agent.SetDestination(locker.transform.GetChild(4).position);
        StartCoroutine("openLocker");

        LockerScript lockerScript = locker.GetComponentInChildren<LockerScript>();

        lockerScript.open();
        

    }

    IEnumerator openLocker()
    {
        Debug.Log("startOpenLocker");
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 10)
            {
                Debug.Log("timeOut");
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

    public void OpenLocker(GameObject locker)
    {
        animator.SetTrigger("Open");
        isSearching = false;
        LockerScript lockerScript = locker.GetComponentInChildren<LockerScript>();
        if (lockerScript.playerExistsInLocker)
        {
            isMovie = true;
            vcam.enabled = true;
            vcam.transform.position += transform.up * 1f;
            vcam.Priority = 30;
            gameOver();
            lockerScript.getPlayerController().gameOver(this.gameObject);
            gameManager.sceneState = 0;
        }

    }

    public void Freeze()
    {
        canMove = false;
        agent.speed = 0f;
        agent.SetDestination(agent.transform.position);
        animator.SetFloat("speed", 0);

    }
    public void Free()
    {
        canMove = true;
        agent.speed = 0f;
        animator.SetFloat("speed", 0);
    }
    public void damaged()
    {
        behaviorMode = DAMAGED_MODE;
        Freeze();
        animator.SetTrigger("Damaged");
        beingDamaged = true;
        StartCoroutine("DamageEvent");
        audiosource.PlayOneShot(damageSE);
    }

    IEnumerator DamageEvent()
    {
        yield return new WaitForSeconds(3);
        beingDamaged = false;
        Free();
        behaviorMode = WAITING_MODE;
        yield break;
    }
    public bool changeStateEvent(int i)
    {
        if(behaviorMode == i)
        {
            return false;
        }
        if(behaviorMode != i)
        {
            return true;
        }
        return false;
    }

    public void SuspectLocker()
    {

        
        if (!isSearching)
        {
            SortLockerDistance();
            isSearching = true;
            seeLocker(lockers[0]);
        }
       
        /*
        agent.speed = 0f;
        float ViewAngle = 60f;
        float deltaAngle = -ViewAngle;
        RaycastHit hit;
        //Ray ray = new(agent.transform.position + new Vector3(0f, 15f, 0f), Quaternion.AngleAxis(deltaAngle, transform.up) * agent.transform.forward);
        //Ray ray2 = new Ray(agent.transform.position + new Vector3(0f, 15f, 0f), agent.transform.up);
        Ray ray3 = new Ray(agent.transform.position + new Vector3(0f, 10f, 0f), agent.transform.forward);


        while (deltaAngle < ViewAngle)
        {
         
            deltaAngle += 5f;
            if (Physics.Raycast(ray3, out hit, detectRange))
            {
                if (hit.transform.gameObject.tag == "Locker")
                {
                    targetLocker = hit.transform.gameObject;

                    InteractiveLocker = targetLocker.GetComponentInChildren<LockerScript>();

                    seeLocker(hit.transform.gameObject);
                }

            }

            //Debug.DrawLine(ray.origin, ray.direction * 30f, Color.blue, 1f);
            //Debug.DrawLine(ray2.origin, ray2.direction * 30f, Color.green, 1f);
            Debug.DrawLine(ray3.origin, ray3.origin + ray3.direction * 30f, Color.red, 1f);

        }
        */
    }
}