using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 20f;
    [SerializeField] float bulletLifeTime = 2f;

    private Rigidbody2D bulletRigidbody;
    private Vector2 bulletDirection;
    private Vector2 previousPosition;

    public void SetDirection(Vector2 dir)
    {
        bulletDirection = dir.normalized;
        float angle = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Start()
    {
        bulletRigidbody = GetComponent<Rigidbody2D>();
        bulletRigidbody.velocity = bulletDirection * bulletSpeed;

        previousPosition = transform.position;

        Destroy(gameObject, bulletLifeTime);
    }

    void FixedUpdate()
    {
        RaycastCheck();
        previousPosition = transform.position;
    }

    void RaycastCheck()
{
    Vector2 currentPosition = transform.position;
    Vector2 direction = currentPosition - previousPosition;
    float distance = direction.magnitude;

    if (distance > 0.0001f)
    {
        Vector2 rayOrigin = previousPosition - direction.normalized * 0.01f;
        float rayDistance = distance + 0.02f;

        // Vẽ ray debug (chỉ thấy ở Scene view)
        Debug.DrawRay(rayOrigin, direction.normalized * rayDistance, Color.red, 1f);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction.normalized, rayDistance, LayerMask.GetMask("Enemy", "Ground"));

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyController enemy = hit.collider.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.DamageTake();
                }
            }

            // Dù trúng enemy hay ground thì đều hủy đạn
            Destroy(gameObject);
        }
    }
}

    

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.gameObject.CompareTag("Enemy"))
    //     {
    //         Debug.Log("Va chạm với enemy (trigger)!");
    //         Destroy(other.gameObject);
    //         Destroy(gameObject);
    //     }
    // }

  
   
}
