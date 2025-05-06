using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class M3 : MonoBehaviour
{
    public GunData data;

    public GameObject nomalImpactEffect;
    public GameObject bloodImpactEffect;   
    public GameObject groundImpactEffect;  
    public GameObject muzzleFlash;

    public Transform LeftHandTarget;
    public Transform RightHandTarget;

    public Transform WeaponTransform;
    public Transform Muzzle;

    [Range(0f, 1f)] public float IKWeight = 1f;

    public float TotalAmmo = 0; 
    public float MaxAmmo = 8;  // 산탄총은 보통 적은 탄 수

    
    public float pelletCount = 5;               
    public float spreadAngle = 6f;   
    private Quaternion originalLocalRotation;

    private void Start()
    {
        TotalAmmo = MaxAmmo;
    }

    private void Update()
    {
        Debug.DrawRay(Muzzle.position, Muzzle.forward * data.range, Color.green);
    }

    public void Fire()
    {
        Debug.Log("M3 Fire");

        originalLocalRotation = transform.localRotation;
        Recoil();
        ShowMuzzleFlash();

        CombatSystem.Instance.AlertEnemiesByGunSound(Muzzle.position, data.soundRadius);
        
        int raycastMask = LayerMask.GetMask("Enemy", "Ground", "Default", "Object");

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 spreadDir = GetSpreadDirection(Muzzle.forward, spreadAngle);
            Ray ray = new Ray(Muzzle.position, spreadDir);

            if (Physics.Raycast(ray, out RaycastHit hit, data.range, raycastMask, QueryTriggerInteraction.Ignore))
            {
                CombatSystem.Instance.ApplyDamage(hit, data.damage); 

                Quaternion rotation = Quaternion.LookRotation(hit.normal);
                int hitLayer = hit.collider.gameObject.layer;

                switch (hitLayer)
                {
                    case int n when n == LayerMask.NameToLayer("Enemy"):
                        Instantiate(bloodImpactEffect, hit.point, rotation);
                        break;

                    case int g when g == LayerMask.NameToLayer("Ground"):
                        Instantiate(groundImpactEffect, hit.point, rotation);
                        break;

                    case int o when o == LayerMask.NameToLayer("Object"):
                        Instantiate(nomalImpactEffect, hit.point, rotation);
                        break;
                    case int c when c == LayerMask.NameToLayer("Default"):
                        Instantiate(nomalImpactEffect, hit.point, rotation);
                        break;
                    default:
                        Instantiate(nomalImpactEffect, hit.point, rotation);
                        break;
                }
            }
        }
    }

    private Vector3 GetSpreadDirection(Vector3 forward, float spreadAngle)
    {
        
        float yaw = Random.Range(-spreadAngle, spreadAngle);
        float pitch = Random.Range(-spreadAngle, spreadAngle);
        float roll = Random.Range(-spreadAngle, spreadAngle);

        Quaternion spreadRotation = Quaternion.Euler(pitch, yaw, roll);
        return spreadRotation * forward;
    }
    

    private void Recoil()
    {

        Sequence recoilSequence = DOTween.Sequence();

        float pitch = Random.Range(-5f, -4f);   
        float yaw = Random.Range(-3f, 3f);    
        float roll = Random.Range(-2f, 2f);
        
        Vector3 recoilBackOffset = new Vector3(-0.0067f, -0.0179f, 0.418f);
        Vector3 recoilReturnOffset = new Vector3(-0.0067f, -0.0179f, 0.468f);

        recoilSequence.Append(
            transform.DOLocalRotateQuaternion(originalLocalRotation * Quaternion.Euler(pitch, yaw, roll), 0.1f)
        );
        recoilSequence.Join(
            WeaponTransform.DOLocalMove(recoilBackOffset, 0.1f)
        );

        recoilSequence.Append(
            transform.DOLocalRotateQuaternion(originalLocalRotation, 0.1f)
        );
        recoilSequence.Join(
            WeaponTransform.DOLocalMove(recoilReturnOffset, 0.1f)
        );
    }

    private void ShowMuzzleFlash()
    {
        if (muzzleFlash == null) return;

        muzzleFlash.SetActive(true);
        CancelInvoke(nameof(HideMuzzleFlash));
        Invoke(nameof(HideMuzzleFlash), 0.05f);
    }

    private void HideMuzzleFlash()
    {
        muzzleFlash.SetActive(false);
    }
}
