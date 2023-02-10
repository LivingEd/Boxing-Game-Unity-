using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float Health = 100;
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    Vector3 velocity;
    public bool isGrounded;

    public PlayerAttackPhase ThePlayerAttackPhase;
    private Enemy CurEnemy;
    public Animator PlayerAnim;

    //Dash & Movement
    public bool IsDodging = false;
    public float DashSpeed = 20;
    public float DashTime = 1;
    public Transform cam;
    public Vector3 moveDir;
    public float DodgeFill;
    public string CurAttackAnimId = "IsAttack1";
    private string GetCurAttackAnimId = "IsAttack";
    private int CurAttackIdNum = 1;
    // Look
    public GameObject target;
    public Transform bestTarget = null;
    public Rigidbody bestTargetRB = null;
    //HealthBar
    public Gradient TheHealthGradient;
    public Slider TheHealthSlider;
    public Image HealthFill;
    private bool HasDied = false;
    public GameObject DodgeDot1, DodgeDot2, DodgeDot3;
    public GameObject DodgeDotO1, DodgeDotO2, DodgeDotO3;

    public Camera TheCamera;
    private float FirstFOV;
    public bool IsSpecialMove = false;
    public float SpecialFill;

    //New Inputs
    public PlayerControlsBox ThePlayerControlsBox;
    private InputAction moveInput;
    private InputAction AttackInput;
    private InputAction DodgeInput;
    //Particle
    public GameObject FiringParticle;

   
    private void Awake()
    {
        ThePlayerControlsBox = new PlayerControlsBox();
        Time.timeScale = 1;
        FirstFOV = TheCamera.fieldOfView;
        TheCamera.fieldOfView = 175;
        StartCoroutine(TimeFOV());
    }
    #region - Enable & Disable -
    private void OnEnable()
    {
        moveInput = ThePlayerControlsBox.Player.Move;
        moveInput.Enable();

        AttackInput = ThePlayerControlsBox.Player.AttackCall;
        AttackInput.Enable();
        AttackInput.performed += AttackCall;

        DodgeInput = ThePlayerControlsBox.Player.DodgeCall;
        DodgeInput.Enable();
        DodgeInput.performed += DodgeCall;
    }
    private void OnDisable()
    {
        moveInput.Disable();

    }
    #endregion
    void Start()
    {
        TheHealthSlider.maxValue = 100;
        TheHealthSlider.value = Health;
    }

    // Update is called once per frame
    void Update()
    {
        if (HasDied == true)
        {
            return;
        }
        //MOVEMENT
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");

        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveInput.ReadValue<Vector2>().x + transform.forward * moveInput.ReadValue<Vector2>().y;

        controller.Move(move * speed * Time.deltaTime);


        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

       /* if (Input.GetButton("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }*/

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        DashMovement();
        //Health
        HealthLogic();
        //CONTROLS
        //PCInputs();
        //Look
        FaceTarget();
    }

    /* private void PCInputs()
     {
         if (Input.GetKeyDown("j"))
         {
             if (!ThePlayerAttackPhase.IsPerformingAttack)
             {
                 //ComboHaseset
                 if(ThePlayerAttackPhase.ComboCountDown <= 0)
                 {
                     CurAttackAnimId = "IsAttack1";
                     CurAttackIdNum = 1;
                 }
                 PlayerAnim.SetBool(CurAttackAnimId, true);
                 PlayerAnim.SetBool("IsIdle", false);
                 CurAttackIdNum += 1;
                 GetCurAttackAnimId = (GetCurAttackAnimId += CurAttackIdNum.ToString());
                 CurAttackAnimId = GetCurAttackAnimId;

                 GetCurAttackAnimId = "IsAttack";
                 if (CurAttackIdNum > 5)
                 {
                     CurAttackIdNum = 1;
                     CurAttackAnimId = "IsAttack1";
                     ThePlayerAttackPhase.ComboCountDown = 0;
                 }
                 if(!IsSpecialMove && SpecialFill >= 5)
                 {
                     StartCoroutine(TimeSpecialMove());
                     SpecialFill = 0;
                 }

             }

         }

         if (Input.GetKeyDown("k"))
         {
             if (IsDodging == false)
             {
                 StartCoroutine(TimeDodge());
             }
         }
     }*/
    private void AttackCall(InputAction.CallbackContext context)
    {
        Attack();
    }
    private void DodgeCall(InputAction.CallbackContext context)
    {
        Dodge();
    }

    private void Attack()
    {
       if (!ThePlayerAttackPhase.IsPerformingAttack)
       {
           //ComboHaseset
           if (ThePlayerAttackPhase.ComboCountDown <= 0)
           {
               CurAttackAnimId = "IsAttack1";
               CurAttackIdNum = 1;
           }
           PlayerAnim.SetBool(CurAttackAnimId, true);
           PlayerAnim.SetBool("IsIdle", false);
           CurAttackIdNum += 1;
           GetCurAttackAnimId = (GetCurAttackAnimId += CurAttackIdNum.ToString());
           CurAttackAnimId = GetCurAttackAnimId;

           GetCurAttackAnimId = "IsAttack";
           if (CurAttackIdNum > 5)
           {
               CurAttackIdNum = 1;
               CurAttackAnimId = "IsAttack1";
               ThePlayerAttackPhase.ComboCountDown = 0;
           }
           if (!IsSpecialMove && SpecialFill >= 10)
           {
               StartCoroutine(TimeSpecialMove());
               SpecialFill = 0;
           }

       }
    }
    private void Dodge()
    {
        if (IsDodging == false)
        {
            StartCoroutine(TimeDodge());
        }
    }

    /* private void ComboIncrement()
     {
         if(PlayerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && !PlayerAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
         {
             ComboState += 1;
         }
     }*/

    private IEnumerator TimeDodge()
    {
        if(DodgeFill >= 1)
        {
            DodgeFill -= 1;
            IsDodging = true;
            StartCoroutine(TimeDash());
            yield return new WaitForSeconds(0.5f);
            IsDodging = false;
        }
        
    }
    private IEnumerator TimeDash()
    {
        float startTime = Time.time;
        while(Time.time < startTime + DashTime)
        {
            controller.Move(moveDir * DashSpeed * Time.deltaTime);
            yield return null;
        }
    }
    private void DashMovement()
    {
        if(DodgeFill < 3)
        {
            DodgeFill += (Time.deltaTime * 0.25f);
        }
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(h, 0, v).normalized;

        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        }
    }
    private IEnumerator TimeSpecialMove()
    {
        IsSpecialMove = true;
        FiringParticle.SetActive(true);
        StartCoroutine(TimeStopSpecialMove());
        float startTime = Time.time;
        while (Time.time < startTime + 5)
        {
            //controller.Move(bestTarget.transform.position * 5 * Time.deltaTime);
            float step = 500 * Time.deltaTime; 
            Vector3 dir = bestTarget.position - transform.position;
            Vector3 movement = dir.normalized * step;
            if (movement.magnitude > dir.magnitude) movement = dir;
            controller.Move(movement);
            yield return null;
        }
    }
    private IEnumerator TimeStopSpecialMove()
    {
        yield return new WaitForSeconds(5);
        IsSpecialMove = false;
        FiringParticle.SetActive(false);
    }
    private void HealthLogic()
    {
        //Health
        if (Health <= 0 && HasDied == false)
        {
            KillPlayer();
            HasDied = true;
        }
        TheHealthSlider.value = Health;
        if (Health > 100 / 1.6)
        {
            HealthFill.color = TheHealthGradient.Evaluate(1f);
        }
        else
        {
            HealthFill.color = TheHealthGradient.Evaluate(TheHealthSlider.normalizedValue);
        }
        if (this.gameObject.transform.position.y <= -300)
        {
            Health = 0;
        }
    }
    private void KillPlayer()
    {
        StartCoroutine(TimeOfDeath());
    }
    private IEnumerator TimeOfDeath()
    {
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(1);
        Retry();
    }
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void FaceTarget()
    {
        Vector3 direction = (bestTarget.position + bestTargetRB.velocity - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

    }
    public GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        target = closest;
        bestTarget = target.GetComponent<Transform>();
        bestTargetRB = target.GetComponent<Rigidbody>();
        return closest;

    }
    private IEnumerator TimeFOV()
    {
        float startTime = Time.time;
        while (Time.time < startTime + 5)
        {
            TheCamera.fieldOfView -= Time.deltaTime * 100;
            if(TheCamera.fieldOfView < FirstFOV) { TheCamera.fieldOfView = FirstFOV; }
            yield return null;
        }
    }
    private void LateUpdate()
    {
        /* if (Input.GetKeyDown("h") || Input.GetKeyDown("u") || Input.GetKeyDown("j") || Input.GetKeyDown("i"))
         {}
         else
         {
             PlayerAnim.SetBool("IsAttack1", false);
             PlayerAnim.SetBool("IsAttack2", false);
             PlayerAnim.SetBool("IsAttack3", false);
             PlayerAnim.SetBool("IsAttack4", false);
             PlayerAnim.SetBool("IsAttack5", false);
             PlayerAnim.SetBool("IsIdle", true); 
         } */
        if (DodgeFill >= 1) { DodgeDot1.SetActive(true); DodgeDotO1.SetActive(false); } else { DodgeDot1.SetActive(false); DodgeDotO1.SetActive(true); }
        if (DodgeFill >= 2) { DodgeDot2.SetActive(true); DodgeDotO2.SetActive(false); } else { DodgeDot2.SetActive(false); DodgeDotO2.SetActive(true); }
        if (DodgeFill >= 3) { DodgeDot3.SetActive(true); DodgeDotO3.SetActive(false); } else { DodgeDot3.SetActive(false); DodgeDotO3.SetActive(true); }
        FindClosestEnemy();//This Sometimes Is Unreachable Code
        
    }
    private void OnTriggerStay(Collider Triginfo)
    {
        if (Triginfo.gameObject.CompareTag("Enemy") && ThePlayerAttackPhase.Checkint == 1)
        {
            CurEnemy = Triginfo.GetComponent<Enemy>();
            CurEnemy.TakeDamage();

            ThePlayerAttackPhase.Checkint = 0;
        }

        if(Triginfo.gameObject.CompareTag("LookObject") && bestTarget == null)
        {
            bestTarget = Triginfo.transform;
            bestTargetRB = Triginfo.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerExit(Collider Triginfo)
    {
        if (Triginfo.gameObject.CompareTag("LookObject"))
        {
            bestTarget = null;
            bestTargetRB = null;
        }
    }



}
