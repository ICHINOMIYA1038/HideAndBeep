using UnityEngine;
using System.Collections;
using Photon.Pun;

public class PlayerController: MonoBehaviourPun
{
    Quaternion targetRotation;
    Animator animator;
    public float speed;
    public Vector3 velocity;
    Vector3 oldVelocity;
    Quaternion rotation;
    bool isJumping;
    public bool checkWait;
    public float walkSpeed = 1.0f;
    public float runSpeed = 2.0f;
    bool canMove = true;
    GameObject myCamera;
    // Use this for initialization
    void Start()
    {
        TryGetComponent(out animator);
        targetRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        if (!photonView.IsMine)
        {
            return;
        }
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

        //速度の取得

        speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        if(horizontal==0 && vertical == 0)
        {
            speed = 0f;
        }
        var rotationSpeed = 600 * Time.deltaTime;



        if(canMove ==true)
        {
            transform.position += velocity * speed * 0.01f;

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
        }
        


        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("searchChest");
            speed = 0f;
            canMove = false;
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

    }

}
