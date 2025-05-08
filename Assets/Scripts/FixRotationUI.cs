using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRotationUI : MonoBehaviour
{
    Canvas canvas;
    // Start is called before the first frame update
    void Awake()
    {
        canvas = GetComponent<Canvas>();
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;
        
    }
}
