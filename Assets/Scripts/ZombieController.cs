using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static SoundManager;

public class ZombieController : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] bool isFinding;
    [SerializeField] GameObject prey;
    [SerializeField] SoundManager soundManager;
    /// <summary>
    /// behaviorMode :0 Wait 1 MoveAround 2 Detect 3 MoveTo 4 Arrive 5 Find 
    /// </summary>
    [SerializeField]
    [Range(0, 5)]
    int behaviorMode = 0;
    static readonly int WAITING_MODE = 0;
    static readonly int MOVEAROUND_MODE = 1;
    static readonly int DETECT_MODE = 2;
    static readonly int MOVETO_MODE = 3;
    static readonly int FIND_MODE = 4;

    float walkSpeed = 3.5f;
    float runSpeed = 10f;
    float detectSpeed = 5f;


    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent<Animator>(out animator);
        TryGetComponent<NavMeshAgent>(out agent);

    }

    // Update is called once per frame
    void Update()
    {

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

    void Wait()
    {
        isFinding = false;
        agent.speed = 0f;
        agent.SetDestination(agent.transform.position);
        if ((targetPosition - agent.transform.position).magnitude < 50f)
        {
            CatchSight();
        }
        if (HearSound())
        {
            behaviorMode = DETECT_MODE;
;
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
       
        while ((agent.transform.position - targetPosition).magnitude >1f)
        {
            if (CatchSight())
            {
                yield break;
            }
            
            yield return null;
        }
        StartCoroutine("SearchAround");

    }

    IEnumerator SearchAround()
    {
        
        float deltaAngle = -5f;
        for(int i = 0; i < 16; i++)
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
        if ((targetPosition - agent.transform.position).magnitude > 50f)
        {
            behaviorMode = WAITING_MODE;
        }
    }

    bool HearSound()
    {
        if (soundManager == null)
        {
            return false;
        }
        soundData mySoundData = soundManager.MaxPosition();
        if(mySoundData.Value > 10f)
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
        Ray ray = new Ray(agent.transform.position + new Vector3(0f, 7f, 0f), Quaternion.AngleAxis(deltaAngle, transform.up) * agent.transform.forward);
        Ray ray2 = new Ray(agent.transform.position + new Vector3(0f, 7f, 0f), agent.transform.up);
        Ray ray3 = new Ray(agent.transform.position + new Vector3(0f, 7f, 0f), agent.transform.forward);

        
        while (deltaAngle < ViewAngle)
        {
            deltaAngle += 5f;
            if (Physics.Raycast(ray, out hit, 50f))
            {
                if (hit.transform.gameObject.tag == "Player")
                {
                    prey = hit.transform.gameObject;
                    Debug.Log(prey.name);
                    behaviorMode = FIND_MODE;
                    return true;
                }

            }
            
            Debug.DrawLine(ray.origin, ray.direction * 30f, Color.blue, 1f);
            Debug.DrawLine(ray2.origin, ray2.direction * 30f, Color.green, 1f);
            Debug.DrawLine(ray3.origin, ray3.direction * 30f, Color.red, 1f);
            
        }

        return false;
    }

    
}