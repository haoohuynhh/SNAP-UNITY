using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{

    [SerializeField] private int maxAmmo = 7;
    [SerializeField] private int currentAmmo = 7;
    [SerializeField] private float reloadTime = 1.5f;
    private bool isReloading = false;

    [SerializeField] private TextMeshProUGUI ammoText;
    public event System.Action<int,int> OnAmmoChanged;


    // Tham chiếu đến PlayerMovement để biết hướng nhìn của nhân vật
    private PlayerMovement playerMovement;
    [SerializeField] Animator animator;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    private AudioSource audioSource;
    
    void Start()
    {
        // Lấy tham chiếu đến component PlayerMovement từ đối tượng cha
        playerMovement = GetComponentInParent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        
    }

    void Update()
    {
       WeaponAiming();
    }
    
    void WeaponAiming()
    {
        // Lấy vị trí chuột trong không gian màn hình
        Vector3 mousepos = Input.mousePosition;
        
        // Lấy vị trí súng trên màn hình
        Vector3 gunposition = Camera.main.WorldToScreenPoint(transform.position);
        
        // Tính toán vector hướng từ súng đến chuột
        mousepos.x = mousepos.x - gunposition.x;
        mousepos.y = mousepos.y - gunposition.y;
        
        // Tính góc xoay cho súng
        float gunangle = Mathf.Atan2(mousepos.y, mousepos.x) * Mathf.Rad2Deg;
        
        // Kiểm tra nếu chuột ở bên trái súng trên màn hình
        bool isCursorOnLeft = Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x;
        
        // Kiểm tra hướng nhìn của nhân vật (nếu có tham chiếu)
        bool isPlayerFacingLeft = false;
        if (playerMovement != null)
        {
            // Kiểm tra hướng nhìn của player dựa vào eulerAngles.y
            isPlayerFacingLeft = Mathf.Approximately(playerMovement.transform.eulerAngles.y, 180f);
        }
        
        // Áp dụng rotation dựa vào vị trí chuột và hướng nhìn của nhân vật
        if (isCursorOnLeft)
        {
            transform.rotation = Quaternion.Euler(180f, 0f, -gunangle);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, gunangle);
        }
    }
    public bool Fire()
    {
        if(isReloading)
        
            return false;
        
        if(currentAmmo == 0)
        {
            StartCoroutine(Reload());
            return false;
            
        }
        animator.SetTrigger("shoot");
        audioSource.PlayOneShot(shootSound);
        Debug.Log("Fire!");
        currentAmmo--;
        ammoText.text = currentAmmo+ "/" + maxAmmo;
        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
        return true;
        
    }
    public void GunReload()
    {
        if(currentAmmo == maxAmmo || isReloading)
        return;
        StartCoroutine(Reload());
        
    }
    private IEnumerator Reload()
    {
        isReloading = true;
        if(ammoText != null)
        {
            ammoText.alignment = TextAlignmentOptions.Left;
            ammoText.text = "Reloading";
        }
        Debug.Log("Reloading...");
        animator.SetTrigger("Reload");
        audioSource.PlayOneShot(reloadSound);
        yield return new WaitForSeconds(reloadTime);
        

        currentAmmo = maxAmmo;

        ammoText.alignment = TextAlignmentOptions.Right;
        ammoText.text = currentAmmo + "/" + maxAmmo;
        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
        isReloading = false;
    }
}