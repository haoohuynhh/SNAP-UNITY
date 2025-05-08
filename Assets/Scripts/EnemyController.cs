using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
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
    public float followSpeed = 5f;
    private bool isReturningToPatrol = false;

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
    void Awake()
    {
        isAlive = true;
        activeQuiz = FindObjectOfType<Quiz>();
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

    // Update is called once per frame
    void Update()
    {
        
    if (!isAlive) return;

    float distanceToPlayer = Vector2.Distance(transform.position, player.position);

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
        // isReturningToPatrol = false;
        followPlayer();
    }
    else
    {
        Patrol();
    }    }
    
       
        
        
        
    
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
    if(!isAlive)
        {
            rb2d.velocity = Vector2.zero; // Dừng lại ngay lập tức
            return;
        }
    // Kiểm tra nếu đang trong thời gian chờ
    if(waitTimer > 0)
    {
        
        // Đang trong thời gian chờ - dừng di chuyển
        waitTimer -= Time.deltaTime;
        rb2d.velocity = Vector2.zero;
        animator.SetBool("IsWalking", false);
        
        // Nếu thời gian chờ kết thúc, thực hiện quay đầu
        if(waitTimer <= 0)
        {
            // Sau khi kết thúc thời gian chờ, đổi hướng
            movingRight = !movingRight;
            transform.rotation = movingRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            animator.SetBool("IsWalking", true);
        }
        return;
    }
    
    // Thực hiện di chuyển bình thường
    float direction = movingRight ? 1 : -1;
    rb2d.velocity = new Vector2(direction * moveSpeed, rb2d.velocity.y);
    animator.SetBool("IsWalking", true);
    
    // Kiểm tra nếu đến biên
    if((transform.position.x > rightBound && movingRight) || 
       (transform.position.x < leftBound && !movingRight))
    {
        // Đến biên - bắt đầu thời gian chờ
        waitTimer = waitTimeAtEdge;
        rb2d.velocity = Vector2.zero; // Dừng lại ngay lập tức
        animator.SetBool("IsWalking", false);
        
    }
}
    
   void followPlayer()
{
    if (!isAlive) return;

    float distanceToPlayer = Vector2.Distance(transform.position, player.position);

    // Nếu player rời khỏi vùng follow → quay về patrol
    if (distanceToPlayer > followDistance)
    {
        isReturningToPatrol = true;
        Debug.Log("Return to patrol!");
        return;
    }

    // Theo dõi player
    Vector2 direction = (player.position - transform.position).normalized;
    rb2d.velocity = new Vector2(direction.x * followSpeed, rb2d.velocity.y); 
    Debug.Log("Follow Player!");
    animator.SetBool("IsWalking", true);
    transform.rotation = direction.x > 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
}
void ReturnToPatrol()
{
    if (!isAlive) return;
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
        // Khi đã về gần vị trí ban đầu → reset patrol
        isReturningToPatrol = false;
        transform.position = new Vector3(startPoint.x, transform.position.y, transform.position.z); // đảm bảo khớp chính xác
        movingRight = true;
        waitTimer = waitTimeAtEdge;
        rb2d.velocity = Vector2.zero;
    }
}
    



void AttackPlayer()
{
    if (!isAlive) return;
    // Kiểm tra cooldown tấn công
    if (Time.time - attackTimer >= attackCooldown)
    {
        attackTimer = Time.time;
        animator.SetTrigger("Attack");
        Debug.Log("Attack Player!");
        
        // Gọi coroutine thực hiện sát thương sau một khoảng thời gian delay
        StartCoroutine(PerformAttackDamage(0.5f));
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