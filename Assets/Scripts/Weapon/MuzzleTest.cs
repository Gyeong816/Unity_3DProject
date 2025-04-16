using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleTest : MonoBehaviour
{


    public float rayLength = 100f;

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * rayLength, Color.yellow);
    }
}
