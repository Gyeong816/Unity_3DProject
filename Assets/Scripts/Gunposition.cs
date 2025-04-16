using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunposition : MonoBehaviour
{
    public Transform shoulder; // 따라갈 어깨 (Transform)
    
    void LateUpdate()
    {
        if (shoulder == null) return;

   
        transform.position = shoulder.position;
       
    }
}
