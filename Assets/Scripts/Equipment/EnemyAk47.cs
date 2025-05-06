using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class EnemyAk47 : MonoBehaviour
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



    private Quaternion originalLocalRotation;



    void Update()
    {
        Debug.DrawRay(Muzzle.position, Muzzle.forward * data.range, Color.red);
    }

    public void Fire()
    {
        
        
        originalLocalRotation = transform.localRotation;
        Recoil();
        ShowMuzzleFlash();

        //CombatSystem.Instance.AlertEnemiesByGunSound(Muzzle.position, data.soundRadius);
        Vector3 fireDirection = GetInaccurateDirection(Muzzle.forward);
        Ray ray = new Ray(Muzzle.position, fireDirection);

        
        int raycastMask = LayerMask.GetMask("Player", "Ground", "Object", "Default");

        if (Physics.Raycast(ray, out RaycastHit hit, data.range, raycastMask, QueryTriggerInteraction.Ignore))
        {
            
            if (hit.collider.GetComponentInParent<PlayerController>() != null)
            {
                CombatSystem.Instance.ApplyDamage(hit, data.damage);
            }

            Quaternion rotation = Quaternion.LookRotation(hit.normal);
            int hitLayer = hit.collider.gameObject.layer;

            switch (hitLayer)
            {
                case int p when p == LayerMask.NameToLayer("Player"):
                    Instantiate(bloodImpactEffect, hit.point, rotation);
                    break;

                case int g when g == LayerMask.NameToLayer("Ground"):
                    Instantiate(groundImpactEffect, hit.point, rotation);
                    break;

                case int o when o == LayerMask.NameToLayer("Object"):
                    Instantiate(nomalImpactEffect, hit.point, rotation);
                    break;
                case int o when o == LayerMask.NameToLayer("Default"):
                    Instantiate(nomalImpactEffect, hit.point, rotation);
                    break;

                default:
                    Instantiate(nomalImpactEffect, hit.point, rotation);
                    break;
            }
        }
    }

    private Vector3 GetInaccurateDirection(Vector3 forward)
    {
        float spreadAngle = 0.5f;
        Quaternion spread = Quaternion.Euler(
            Random.Range(-spreadAngle, spreadAngle),
            Random.Range(-spreadAngle, spreadAngle),
            0f
        );
        return spread * forward;
    }

    private void Recoil()
    {
        Sequence recoilSequence = DOTween.Sequence();

        float pitch = Random.Range(-2f, -1f);
        float yaw = Random.Range(-3f, 3f);
        float roll = Random.Range(-2f, 2f);

        // 적은 반동 없음
        Vector3 recoilBackOffset = Vector3.zero;
        Vector3 recoilReturnOffset = Vector3.zero;

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
    
    private void OnDisable()
    {
        DOTween.Kill(transform);
        DOTween.Kill(WeaponTransform);
    }
}
