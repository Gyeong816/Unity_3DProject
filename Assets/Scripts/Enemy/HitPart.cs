using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPart : MonoBehaviour
{
    public Enemy owner;       
    public Parts partType;
    public float damageValue;
    
    private void OnValidate()
    {
        switch (partType)
        {
            case Parts.Head:
                damageValue = 0f;
                break;
            case Parts.Body:
                damageValue = 2f;
                break;
            case Parts.Arm:
                damageValue = 1f;
                break;
            case Parts.Leg:
                damageValue = 1f;
                break;
            default:
                damageValue = 0f;
                break;
        }
    }
}
public enum Parts
{
    Unknown,
    Body,
    Head,
    Leg,
    Arm
}