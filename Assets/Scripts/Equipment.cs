using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public Transform LeftHandTarget;
    public Transform RightHandTarget;
    public Transform WeaponTransform;
   [Range(0f, 1f)] public float IKWeight = 1f;
}
