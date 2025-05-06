using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPart : MonoBehaviour
{
    public EnemyController owner;  
    public PlayerHP player;
    public Parts partType;
    
    public float damageValue;

    public float damageReductionRate;
    private void Start()
    {
        owner = GetComponentInParent<EnemyController>();
        player = GetComponentInParent<PlayerHP>();
    }

    private void OnValidate()
    {
        switch (partType)
        {
            case Parts.Head:
                damageValue = 0f;
                break;
            case Parts.Body:
                damageValue = 1.5f;
                break;
            case Parts.Arm:
                damageValue = 1f;
                break;
            case Parts.Leg:
                damageValue = 1f;
                break;
            case Parts.Helmet1:
                damageValue = 2f;
                damageReductionRate = 0.5f;
                break;
            case Parts.Helmet2:
                damageValue = 2f;
                damageReductionRate = 0.7f;
                break;
            case Parts.Vest1:
                damageValue = 1.5f;
                damageReductionRate = 0.5f;
                break;
            case Parts.Vest2:
                damageValue = 1.5f;
                damageReductionRate = 0.7f;
                break;
            case Parts.Vest3:
                damageValue = 1.5f;
                damageReductionRate = 0.9f;
                break;
            default:
                damageValue = 0f;
                break;
        }
    }
}
