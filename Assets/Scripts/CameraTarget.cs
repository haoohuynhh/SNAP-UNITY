using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Transform player;
    [SerializeField] float xThreshold;
    [SerializeField] float yThreshold;
    void Update()
    {
       AimLogic(); 
    }
    public void AimLogic()
    {
        Vector3 mousePos =cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPos = (player.position + mousePos) / 2;

        targetPos.x = Mathf.Clamp(targetPos.x, player.position.x - xThreshold, player.position.x + xThreshold);
        targetPos.y = Mathf.Clamp(targetPos.y, player.position.y - yThreshold, player.position.y + yThreshold);
        this.transform.position = targetPos;
    }
}
