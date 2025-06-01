using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
// using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int maxHealth = 3;
    [SerializeField] int currentHealth;
    bool isAlive;

    [Header("Quiz")]
    [SerializeField] Quiz quiz;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float patrolRange = 3f;

    private Vector2 startPoint;
    [SerializeField] private float leftBound;
    [SerializeField] private float rightBound;
    private bool movingRight = true;

    [SerializeField] float waitTimeAtEdge = 1f;
    private float waitTimer = 0f;

    [Header("Follow Player")]
    public Transform player;
    public float followDistance = 5f;
    public float followSpeed = 8f;
    private bool isReturningToPatrol = false;

    [Header("Attack")]
    public float attackDistance = 1.5f;
    public float StopFollowDistance = 2f;
    public float attackCooldown = 1f;
    public float attackTimer = 0f;
    [SerializeField] float attackDelay = 0.5f;
    

    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] int attackDamage = 1;

    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rb2d;

    [Header("SoundEffect")]
    public AudioClip hurtSound;
    public AudioClip dieSound;
    private AudioSource audioSource;

    void Awake()
    {
        isAlive = true;
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);
        currentHealth = maxHealth;
        startPoint = transform.position;
        leftBound = startPoint.x - patrolRange;
        rightBound = startPoint.x + patrolRange;
        SetupQuiz();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!isAlive)
        {
            rb2d.velocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (playerMovement.isDeath)
        {
            if (!isReturningToPatrol)
            {
                isReturningToPatrol = true;
                Debug.Log("Player dead → Return to patrol");
            }
        }

        if (isReturningToPatrol)
        {
            ReturnToPatrol();
        }
        else if (distanceToPlayer <= attackDistance)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= followDistance)
        {
            followPlayer();
        }
        else
        {
            Patrol();
        }
    }

    void SetupQuiz()
    {
        if (quiz != null)
        {
            quiz.gameObject.SetActive(true);
        }
    }

    public void DamageTake()
    {
        if (!isAlive) return;

        if (quiz != null)
        {
            int selectedIndex = quiz.CurrentSelectedIndex;
            int correctIndex = quiz.CurrentQuestion.GetCorrectAnswerIndex();

            if (selectedIndex == correctIndex)
            {
                audioSource.PlayOneShot(dieSound);
                DisableComponents();
                Debug.Log("Trúng điểm yếu!");
                isAlive = false;
                DisableComponents();
                currentHealth = 0;
                animator.SetTrigger("Died");
                Destroy(gameObject, 2f);
            }
            else
            {
                audioSource.PlayOneShot(hurtSound);
                currentHealth--;
                Debug.Log("Trúng đạn!");
                if (currentHealth <= 0)
                {
                    audioSource.PlayOneShot(dieSound);
                    DisableComponents();
                    Debug.Log("Đã chết!");
                    isAlive = false;
                    currentHealth = 0;
                    DisableComponents();
                    animator.SetTrigger("Died");
                    Destroy(gameObject, 2f);
                }
                else
                {
                    animator.SetTrigger("Hurt");
                }
            }
        }
    }

    public void DisableComponents()
    {
        animator.ResetTrigger("Hurt");
        animator.ResetTrigger("Attack");
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
        
        StopAllCoroutines();
        attackPoint.gameObject.SetActive(false);
        rb2d.isKinematic = true;
        quiz.gameObject.SetActive(false);
    }

    void Patrol()
    {
        if (!isAlive)
        {
            rb2d.velocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
            return;
        }

        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            rb2d.velocity = Vector2.zero;
            animator.SetBool("IsWalking", false);

            if (waitTimer <= 0)
            {
                movingRight = !movingRight;
                transform.rotation = movingRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
                animator.SetBool("IsWalking", true);
            }
            return;
        }

        float direction = movingRight ? 1 : -1;
        rb2d.velocity = new Vector2(direction * moveSpeed, rb2d.velocity.y);
        animator.SetBool("IsWalking", true);

        if ((transform.position.x > rightBound && movingRight) ||
            (transform.position.x < leftBound && !movingRight))
        {
            waitTimer = waitTimeAtEdge;
            rb2d.velocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
        }
    }

    void followPlayer()
    {
        if (!isAlive)
        {
            rb2d.velocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
        
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > followDistance)
        {
            isReturningToPatrol = true;
            Debug.Log("Return to patrol!");
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb2d.velocity = new Vector2(direction.x * followSpeed, rb2d.velocity.y);
        animator.SetBool("IsWalking", true);
        transform.rotation = direction.x > 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
    }

    void ReturnToPatrol()
    {
        if (!isAlive)
        {
            rb2d.velocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
            return;
        }

        float distanceToStart = Vector2.Distance(transform.position, startPoint);

        if (distanceToStart > 0.1f)
        {
            Vector2 dir = (startPoint - (Vector2)transform.position).normalized;
            rb2d.velocity = new Vector2(dir.x * moveSpeed, rb2d.velocity.y);
            transform.rotation = dir.x > 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            animator.SetBool("IsWalking", true);
        }
        else
        {
            isReturningToPatrol = false;
            transform.position = new Vector3(startPoint.x, transform.position.y, transform.position.z);
            movingRight = true;
            waitTimer = waitTimeAtEdge;
            rb2d.velocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
        }
    }

    void AttackPlayer()
    {
        if (!isAlive || playerMovement.isDeath)
        {
            DisableComponents();
            return;
        }
        if (Time.time - attackTimer >= attackCooldown)
        {
            attackTimer = Time.time;

            if (isAlive)
            {
                animator.SetTrigger("Attack");
                Debug.Log("Attack Player!");

                StartCoroutine(PerformAttackDamage(attackDelay));
            }
        }
    }

    public void DamagePlayer(PlayerMovement playerMovement)
    {
        playerMovement.audioSource.PlayOneShot(playerMovement.hitsound);
        playerMovement.currentHealth -= attackDamage;
        playerMovement.UpdateUI();
        if (playerMovement.blinkCoroutine != null)
        {
            playerMovement.StopCoroutine(playerMovement.blinkCoroutine);
        }
            playerMovement.blinkCoroutine = playerMovement.StartCoroutine(playerMovement.Blink());
    }

    private IEnumerator PerformAttackDamage(float delay)
{
    yield return new WaitForSeconds(delay);

    if (!isAlive || playerMovement.isDeath) yield break;

    Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

    foreach (Collider2D player in hitPlayers)
    {
        Debug.Log("Enemy hit " + player.name);
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            DamagePlayer(playerMovement);
        }
    }
}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + Vector3.left * patrolRange,
                        transform.position + Vector3.right * patrolRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}