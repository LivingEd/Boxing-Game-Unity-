using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float Health = 100;
    private float Damage;
    public float MinDamage = 5, MaxDamage = 15;

    private Transform PlayerTrans;
    private Rigidbody PlayerRB;
    public float speed = 5;
    public LayerMask whatIsGround, whatIsPlayer;
    public float sightRange, attackRange, stopRange;
    public bool playerInSightRange, playerInAttackRange, playerInStopRange;
    public CharacterController TheCharacControler;
    public Transform SphereTransform;
    public Animator EnemyAnim;
    public bool HasAttacked = false;
    public float RateOfFire = 1.5f;
    public EnemyAttackPhase TheEnemyAttackPhase;
    private Player ThePlayer;
    private float PDamage;
    public float PMinDamage = 3, PMaxDamage = 6;
    //HealthBar
    public Gradient TheHealthGradient;
    public Slider TheHealthSlider;
    public Slider TheHealthSlider2;
    public Image HealthFill;
    public Image HealthFill2;
    Vector3 velocity;
    public float gravity = -50;
    public GameObject ExplosionParticle;
    void Start()
    {
        ThePlayer = GameObject.FindObjectOfType<Player>().GetComponent<Player>();
        PlayerTrans = GameObject.FindObjectOfType<Player>().GetComponent<Transform>();
        PlayerRB = GameObject.FindObjectOfType<Player>().GetComponent<Rigidbody>();
        TheHealthSlider.maxValue = Health;
        TheHealthSlider.value = Health;
        TheHealthSlider2.maxValue = Health;
        TheHealthSlider2.value = Health;
    }

    // Update is called once per frame
    void Update()
    {
        if (Health > 0)
        {  //Check for sight and attack range
            playerInSightRange = Physics.CheckSphere(SphereTransform.position, sightRange, whatIsPlayer, QueryTriggerInteraction.Ignore);
            playerInAttackRange = Physics.CheckSphere(SphereTransform.position, attackRange, whatIsPlayer, QueryTriggerInteraction.Ignore);
            playerInStopRange = Physics.CheckSphere(SphereTransform.position, stopRange, whatIsPlayer, QueryTriggerInteraction.Ignore);

            //if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }

        if (Health <= 0)
        {
            Die();
        }
        Movement();
    }

    private void ChasePlayer()
    {
        Debug.Log("ChasePlayer");
    }
    private void AttackPlayer()
    {
        Debug.Log("AttackPlayer");
        if (HasAttacked == false)
        {
            StartCoroutine(KillPlayer());
        }
    }

    private void Movement()
    {
        // Move
        if (!playerInStopRange && playerInSightRange)
        {
            float step = speed * Time.deltaTime; // calculate distance to move
            //this.gameObject.GetComponent<Rigidbody>().MovePosition (transform.position + (PlayerTrans.position * step));
            //transform.position = Vector3.MoveTowards(transform.position, PlayerTrans.position, step);
            // find the target position relative to the player:
            Vector3 dir = PlayerTrans.position - transform.position;
            // calculate movement at the desired speed:
            Vector3 movement = dir.normalized * step;
            // limit movement to never pass the target position:
            if (movement.magnitude > dir.magnitude) movement = dir;
            // move the character:
            TheCharacControler.Move(movement);
        }
        //Stop
        else
        {
            float step = speed * Time.deltaTime; // calculate distance to move
            //transform.position = Vector3.MoveTowards(transform.position, PlayerTrans.position, step);
            // find the target position relative to the player:
            Vector3 dir = transform.position - transform.position;
            // calculate movement at the desired speed:
            Vector3 movement = dir.normalized * step;
            // limit movement to never pass the target position:
            if (movement.magnitude > dir.magnitude) movement = dir;
            // move the character:
            TheCharacControler.Move(movement);
        }
        velocity.y += gravity * Time.deltaTime;
        TheCharacControler.Move(velocity * Time.deltaTime);

        if (playerInSightRange) 
        { FaceTarget(); }
        
    }
    public void FaceTarget()
    {
        Vector3 direction = (PlayerTrans.position + PlayerRB.velocity - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    //ATTACK
    public IEnumerator KillPlayer()
    {
        if (HasAttacked == false)
        {
            HasAttacked = true;
            yield return new WaitForSeconds(RateOfFire);

            Attack();

            HasAttacked = false;
        }
    }
    void Attack()
    {
        // Play The Animation
        if (Health > 0)
        {
            EnemyAnim.SetBool("IsAttack1", true);
            EnemyAnim.SetBool("IsIdle", false);
        }
    }

    private void Die()
    {
        if (!ThePlayer.IsSpecialMove)
        {
            Destroy(this.gameObject);
        }
        else
        {
            StartCoroutine(TimeOfExplosion());
        }
        
    }
    private IEnumerator TimeOfExplosion()
    {
        GameObject impactGo = Instantiate(ExplosionParticle, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        yield return new WaitForSeconds(2);
    }


    // Player Attack Trigger Hit
    private void OnTriggerEnter(Collider Triginfo)
    {
        if (Triginfo.gameObject.CompareTag("Player"))
        {
        }
    }
    public void TakeDamage()
    {
        Damage = Random.Range(MinDamage, MaxDamage);
        if(!ThePlayer.IsSpecialMove)
        { 
            Health -= Damage;
            ThePlayer.SpecialFill += 1;
        }
        else
        {
            Health -= (Damage *= 3);
        }
        
    }

    private void OnTriggerStay(Collider Triginfo)
    {
        if (Triginfo.gameObject.CompareTag("Player") && TheEnemyAttackPhase.Checkint == 1 && Triginfo.isTrigger == false)
        {
            if(ThePlayer.IsDodging == false && ThePlayer.IsSpecialMove == false)
            {
                PDamage = Random.Range(PMinDamage, PMaxDamage);
                ThePlayer.Health -= PDamage;
            }
            
            TheEnemyAttackPhase.Checkint = 0;
        }
    }

    private void LateUpdate()
    {
        EnemyAnim.SetBool("IsAttack1", false);
        TheHealthSlider.value = Health;
        TheHealthSlider2.value = Health;
        if (Health > 100 / 1.7)
        {
            HealthFill.color = TheHealthGradient.Evaluate(1f);
            HealthFill2.color = TheHealthGradient.Evaluate(1f);
        }
        else
        {
            HealthFill.color = TheHealthGradient.Evaluate(TheHealthSlider.normalizedValue);
            HealthFill2.color = TheHealthGradient.Evaluate(TheHealthSlider.normalizedValue);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(SphereTransform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(SphereTransform.position, attackRange);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(SphereTransform.position, stopRange);

    }
}
