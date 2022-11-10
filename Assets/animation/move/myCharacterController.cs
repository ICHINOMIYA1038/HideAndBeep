using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myCharacterController : MonoBehaviour
{
    Animator animator;
   public float speed;
    public Vector3 velocity;
    Vector3 oldVelocity;
    Quaternion rotation;
    bool isJumping;
    public bool checkWait;
    public float walkSpeed = 3.0f;
    public float runSpeed = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        velocity= Vector3.zero;
        isJumping= false;
        checkWait= false;
        speed = 0f;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //animator.SetFloat("speed", speed);
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        rotation = Quaternion.AngleAxis(0, Vector3.up);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("jump"))
        {
            isJumping = true;
        }
        else 
        {
            isJumping = false;
        }

        velocity = Vector3.zero;
        if (y > 0)
        {
            speed = walkSpeed;
            velocity = transform.forward * speed * Time.deltaTime;
           
        }
        if (y < 0)
        {
            rotation = Quaternion.Euler(0, 180, 0);
        }
        if (x > 0)
        {
            //velocity = transform.right * speed * Time.deltaTime;
            rotation= Quaternion.Euler(0,90,0);

        }
        if (x < 0)
        {
            //velocity = -(transform.right * speed * Time.deltaTime);
            rotation = Quaternion.Euler(0, -90, 0);
        }
        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            animator.SetTrigger("isJump");
        }
        if (Input.GetKey(KeyCode.LeftShift)&&y>0)
        {
            speed = runSpeed;
        }
        if(!(y>0))
        {
            speed = 0f;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("stand")&&checkWait==false)
        {
            StartCoroutine(waitMotion());
            checkWait = true;
        }


        if (speed == walkSpeed)
        { animator.SetFloat("speed", 3f); }
        else if(speed==runSpeed)
        { animator.SetFloat("speed", 5f); }
        else if (speed == 0)
        { animator.SetFloat("speed", 0); }




        //velocity = Vector3.Lerp(oldVelocity, velocity, speed * Time.deltaTime);
        //transform.LookAt(transform.position + velocity);
        Quaternion qNow = this.transform.rotation;
        if (rotation != Quaternion.AngleAxis(0, Vector3.up))
        { 
            transform.rotation = Quaternion.Lerp(qNow, qNow * rotation,  Time.deltaTime);
        }
            if(isJumping==false)
        {
            transform.position = transform.position + velocity;
        }
            oldVelocity = velocity;
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

}