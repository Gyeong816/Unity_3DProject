using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunposition : MonoBehaviour
{
    public Transform shoulder; // 따라갈 어깨 (Transform)
    
    void LateUpdate()
    {
        if (shoulder == null) return;

        // 위치만 따라가고 회전은 고정
        transform.position = shoulder.position;
        // 회전은 유지 (원하는 경우 회전도 제어 가능)
    }
}
