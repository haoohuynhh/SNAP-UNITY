using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private Vector3 startPoint;
    private bool movingRight = true;

    [SerializeField] float waitTimeAtEdge = 1f;
    private float waitTimer = 0f;

    [Header("Follow Player")]
    public Transform player;
    public float followDistance = 5f;

    [Header("Attack")]
    public float attackDistance = 1.5f;
    public float StopFollowDistance = 2f;
    public float attackCooldown = 1f;
    public float attackTimer = 0f;
    
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float attackRate = 1f;
    [SerializeField] int attackDamage = 1;



    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rb2d;
    Quiz activeQuiz;
    void Start()
    {
        isAlive = true;
        activeQuiz = FindObjectOfType<Quiz>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);
        currentHealth = maxHealth;
        startPoint = transform.position;
        SetupQuiz();
        animator = GetComponent<Animator>();

        

        
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if(distanceToPlayer <= followDistance && isAlive == true)
        {
            followPlayer();
        }
         if(distanceToPlayer <= attackDistance && isAlive == true)
        {
            AttackPlayer();
        }
        
         else 
        {
            Patrol();
        }
       
        
        
        
    }
    void SetupQuiz()
    {
        if(quiz != null)
        {
            quiz.gameObject.SetActive(true);
        }
    }
    public void DamageTake()
    {
        
        if(activeQuiz != null)
        {
            int selectedIndex = activeQuiz.CurrentSelectedIndex;
            int correctIndex = activeQuiz.CurrentQuestion.GetCorrectAnswerIndex();
            if(selectedIndex == correctIndex)
            {
                
                    Debug.Log("Trung diem yeu!");
                    isAlive = false;
                    animator.SetTrigger("Died");
                    Destroy(gameObject,2f);
                
            }
            else
            {
                currentHealth--;
                Debug.Log("Trung Dan!");
                if(currentHealth <= 0)
                {
                    Debug.Log("Da chet!");
                    isAlive = false;
                    animator.SetTrigger("Died");
                    Destroy(gameObject,2f);
                }
                else
                {
                    isAlive = true;
                    animator.SetTrigger("Hurt");
                }
            }
        }
    }
    void Patrol()
    {
        if(isAlive == false)
        {
            return;
        }
        float leftBound = startPoint.x - patrolRange;
        float rightBound = startPoint.x + patrolRange;
        if(waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            animator.SetBool("IsWalking", false);
            return;
        }

        float direction = movingRight ? 1 : -1;

        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);
        animator.SetBool("IsWalking", true);

        if (transform.position.x > rightBound && movingRight)
        {
            waitTimer = waitTimeAtEdge;
            movingRight = false;
        }
        else if (transform.position.x < leftBound && !movingRight)
        {
            waitTimer = waitTimeAtEdge;
            movingRight = true;
        }
        else if (transform.position.x > rightBound && !movingRight)
        {
            waitTimer = waitTimeAtEdge;
            movingRight = false;
        }
        else if (transform.position.x < leftBound)
        {
            waitTimer = waitTimeAtEdge;
            movingRight = true;
        }
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer != null )
        {
            if(waitTimer <= 0)
            {
                if(movingRight)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
            }
        }

    }
   void followPlayer()
   {
    if(isAlive == false)
    {
         return;
    }
    Vector2 direction = (player.position - transform.position).normalized;
    Vector2 stopPosition = (Vector2)player.position - direction * StopFollowDistance;
    transform.position = Vector2.MoveTowards(transform.position, stopPosition, moveSpeed * Time.deltaTime);
    animator.SetBool("IsWalking", true);
    transform.rotation = Quaternion.Euler(0, player.position.x < transform.position.x ? 180 : 0, 0);
   }
   public void AttackPlayer()
   {
    if(isAlive == false)
    {
         return;
    }
    animator.SetBool("IsWalking", false);
    spriteRenderer.flipX = (player.position.x < transform.position.x);
    if(Time.time - attackTimer >= attackCooldown)
    {
        attackTimer = Time.time;
        animator.SetTrigger("Attack");
        Debug.Log("Attack Player!");
        StartCoroutine(PerformAttackDamage(0.5f)); // Thay đổi thời gian delay nếu cần
    }
   }
public void DamagePlayer(PlayerMovement playerMovement)
{
    playerMovement.currentHealth -= attackDamage;
    playerMovement.UpdateUI();
    

}
private IEnumerator PerformAttackDamage(float delay)
{
    // Đợi cho animation bắt đầu
    yield return new WaitForSeconds(delay);
    
    // Tìm tất cả player nằm trong vùng attack (OverlapCircle)
    Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
    
    // Lặp qua tất cả player bị trúng và gây sát thương
    foreach (Collider2D player in hitPlayers)
    {
        Debug.Log("Enemy hit " + player.name);
        
        // Lấy component PlayerHealth hoặc PlayerMovement
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        
        if (playerMovement != null)
        {
            // Gây sát thương cho player
            DamagePlayer(playerMovement);
            
        }
    }
}
   
    void OnDrawGizmosSelected()
{
    // Phạm vi tuần tra
    Gizmos.color = Color.green;
    Gizmos.DrawLine(transform.position + Vector3.left * patrolRange,
                    transform.position + Vector3.right * patrolRange);
    
    // Phạm vi tấn công
    if (attackPoint != null)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
}
