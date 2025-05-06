using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashController : MonoBehaviour
{
    private Vector2 muzzleFlashDirection;
    public void SetDirection(Vector2 dir)
    {
        muzzleFlashDirection = dir.normalized;
        float angle = Mathf.Atan2(muzzleFlashDirection.y, muzzleFlashDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.4f); 
        
    }

   
}
