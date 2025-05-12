using System.Collections;
using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerMovement : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 4;
    public int currentHealth;
    [SerializeField] public Image[] hearts;
    [SerializeField] public Sprite fullHeart;
    [SerializeField] public Sprite emptyHeart;
    public bool isDeath = false;
   

    [Header("Movement")]
    Vector2 moveInput;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpSpeed = 20f;
    [SerializeField] float climbSpeed = 5f;
    
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] Transform gun;
    private WeaponController weaponController;
 


    [SerializeField] SpriteRenderer GunSpriteRenderer;
    
    float gravityScaleAtStart;
    Rigidbody2D myRigidbody;
    BoxCollider2D myBodyCollider;

    Animator myAnimator;
    SpriteRenderer spriteRenderer;
    [SerializeField] bool isFree;

    public float attackRate = 3f;
    public float jumpRate = 2f;
    public float nextJumpTime = 0f;
    public float nextAttackTime = 0f;
    [Header("Audio")]
    [SerializeField] public AudioClip hitsound;
    [SerializeField] public AudioClip deathsound;

    public AudioSource audioSource;
    InputManager inputManager;



    
     

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        inputManager = FindObjectOfType<InputManager>();
        currentHealth = maxHealth;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Enemy"), true);
        // Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullets"), LayerMask.NameToLayer("Enemy"), true);
        spriteRenderer = GetComponent<SpriteRenderer>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
        weaponController = GetComponentInChildren<WeaponController>();
        isFree = true;
        isDeath = false;

        inputManager = FindObjectOfType<InputManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if(inputManager.isPaused || isDeath) {return;}
        
        
        UpdateUI();
        if(currentHealth <= 0 )
        {
            DiedState();
            isDeath = true;
            GunSpriteRenderer.enabled = false;
            audioSource.PlayOneShot(deathsound);
            Invoke("ShowDeathMenuDelayed", 1.5f);
            return;
            
        }
       
        
        OnRun();
        FlipSprite();
        UpdateJumpAnimation();
        ClimbLadder();
        RunReverse();
        
        
        // UpdateClimbAnimation();
    }
    void ShowDeathMenuDelayed()
{
    if (inputManager != null)
    {
        
        inputManager.ShowDeathMenu();
    }
}
    void OnFire(InputValue value)
    {
        if(inputManager.isPaused) {return;}
        if(!isFree) {return;}
        if(isDeath) {return;}
        if(Time.time >= nextAttackTime){
            bool canFire = weaponController.Fire();
        if (canFire)
        {
            // Nếu có thể
// Lấy vị trí chuột
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mousePos.z = 0; // Đảm bảo z = 0 trong không gian 2D
    
    // Tính hướng từ vị trí súng đến chuột
    Vector2 direction = (mousePos - gun.position).normalized;
    
    // Tạo đạn tại vị trí súng
    GameObject newMuzzleFlash = Instantiate(muzzleFlash, gun.position, Quaternion.identity);
    GameObject newBullet = Instantiate(bullet, gun.position, Quaternion.identity);
    
    // Thiết lập hướng bay cho đạn
    Bullet bulletComponent = newBullet.GetComponent<Bullet>();
    MuzzleFlashController muzzleFlashComponent = newMuzzleFlash.GetComponent<MuzzleFlashController>();
    
    if (bulletComponent && muzzleFlashComponent != null)
    {
        muzzleFlashComponent.SetDirection(direction);
        bulletComponent.SetDirection(direction);
        

    }
    
  
    nextAttackTime = Time.time + 1f / attackRate;
        }
        }
    }
    void OnGunReload(InputValue value)
    {
        if(inputManager.isPaused) {return;}
        if(!isFree) {return;}
        weaponController.GunReload();
    }
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }
    
    void OnJump(InputValue value )
    {
        if(inputManager.isPaused) {return;}
        if(!isFree) {return;}
        if(isDeath) {return;}
        if(!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){return;}
        if(Time.time >= nextJumpTime){
        if(value.isPressed )
        {
            myRigidbody.velocity += new Vector2(0f,jumpSpeed);
            myAnimator.SetBool("IsJumpingUp",true);
            myAnimator.SetBool("IsJumpingFall",false);
            
        }
        nextJumpTime = Time.time + 1f / jumpRate;
        }
    }
    void OnRun()
    {
        if(inputManager.isPaused) {return;}
        Vector2 playerVelocity = new Vector2 (moveInput.x * moveSpeed,myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;
        

    }
    public void UpdateUI()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
    
        
    
    public void DiedState()
    {
        if(isDeath)
        {return;}
        
       
        
            
            myAnimator.SetTrigger("Died");
            GunSpriteRenderer.enabled = false;
            
            
        
        
        
    }
   
    void FlipSprite()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    if (mousePos.x < transform.position.x)
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180f, transform.eulerAngles.z);
    }
    else
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0f, transform.eulerAngles.z);
    }



    }
    void UpdateJumpAnimation()
    {
        if(myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            myAnimator.SetBool("IsJumpingUp",false);
            myAnimator.SetBool("IsJumpingFall",false);
            return;
        }
        if(myRigidbody.velocity.y < 0)
        {
            myAnimator.SetBool("IsJumpingUp",false);
            myAnimator.SetBool("IsJumpingFall",true);

        }
        else if(myRigidbody.velocity.y >= 0)
        {
            myAnimator.SetBool("IsJumpingUp", true);
            myAnimator.SetBool("IsJumpingFall", false);
        }
        
    }
    void ClimbLadder()
    {
        if(!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing",false);
            myAnimator.SetBool("ClimbRelaxing",false);
            GunSpriteRenderer.enabled = true; 
            isFree = true;

            return;}
        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x,moveInput.y * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;
        
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon; 
        if(playerHasVerticalSpeed)
        {
            myAnimator.SetBool("IsJumpingUp",false);
            myAnimator.SetBool("IsJumpingFall",false);
            myAnimator.SetBool("ClimbRelaxing",false);
            myAnimator.SetBool("isClimbing",true);
            GunSpriteRenderer.enabled = false; 
            isFree = false;
        }
        else if(!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || myAnimator.GetBool("isClimbing") == true)
        {
            myAnimator.SetBool("IsJumpingUp",false);
            myAnimator.SetBool("IsJumpingFall",false);
            myAnimator.SetBool("ClimbRelaxing",true);
            myAnimator.SetBool("isClimbing",false);
            isFree = false;
        }
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Hazard")))
        {
            currentHealth = 0;
        }
      
        
    }
void RunReverse()
{
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    // Hướng nhân vật đang nhìn (dựa trên localScale.x)
    float playerDirection = transform.localScale.x; // 1 = facing right, -1 = facing left

    // Hướng chuột so với nhân vật
    float mouseDirection = Mathf.Sign(mousePos.x - transform.position.x); // 1 = chuột ở bên phải, -1 = bên trái

    // Hướng di chuyển thực tế (dựa trên vận tốc, không dựa trên moveInput)
    float moveDirection = Mathf.Sign(myRigidbody.velocity.x); // 1 = đi phải, -1 = đi trái

    // Kiểm tra xem nhân vật có đang di chuyển không (dùng vận tốc thực tế)
    bool isMoving = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
    bool isRunningBackwards = isMoving &&                       // Đang di chuyển
                              mouseDirection == playerDirection && // Chuột và nhân vật nhìn cùng hướng
                              moveDirection != playerDirection && myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));   // Di chuyển ngược với hướng nhìn

    if(!isMoving)
    {
        myAnimator.SetBool("IsRunningBack",false);
        myAnimator.SetBool("IsrunningGun",false);
        return;
        
        
    }
     if (isRunningBackwards)
    {
        // Đang chạy lùi
        myAnimator.SetBool("IsRunningBack", true);
        myAnimator.SetBool("IsrunningGun", false);
    }
    else
    {
        // Đang chạy bình thường
        myAnimator.SetBool("IsRunningBack", false);
        myAnimator.SetBool("IsrunningGun", true);
    }
}

}
