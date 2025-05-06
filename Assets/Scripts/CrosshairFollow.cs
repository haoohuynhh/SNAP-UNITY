using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairFollow : MonoBehaviour
{
    [SerializeField] Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false; // Hide the cursor
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.nearClipPlane; // Set the distance from the camera
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);

        worldPos.z = 0; // Set the z position to 0 to keep it in 2D space
        transform.position = worldPos; // Update the position of the crosshair
                Cursor.visible = false; // Hide the cursor

    }
}
