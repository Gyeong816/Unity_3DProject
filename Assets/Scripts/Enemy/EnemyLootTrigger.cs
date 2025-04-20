using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLootTrigger : MonoBehaviour
{
    private Enemy owner;

    private void Awake()
    {
        owner = GetComponentInParent<Enemy>(); 
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            UIManager.Instance.ShowLootPrompt(transform, owner);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.HideLootPrompt();
        }
    }
}
