using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class Ak47 : MonoBehaviour
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
    public float MaxAmmo = 30;

    private Quaternion originalLocalRotation;

    void Start()
    {
        TotalAmmo = MaxAmmo;
    }

    void Update()
    {
        Debug.DrawRay(Muzzle.position, Muzzle.forward * data.range, Color.red);
    }

    public void Fire()
    {
        originalLocalRotation = transform.localRotation;
        Recoil();
        ShowMuzzleFlash();

        CombatSystem.Instance.AlertEnemiesByGunSound(Muzzle.position, data.soundRadius);
       
        
        Vector3 fireDirection = Muzzle.forward;
        Ray ray = new Ray(Muzzle.position, fireDirection);

        // 플레이어가 쏘는 총: "Player"는 제외
        int raycastMask = LayerMask.GetMask("Enemy", "Ground", "Object");

        if (Physics.Raycast(ray, out RaycastHit hit, data.range, raycastMask, QueryTriggerInteraction.Ignore))
        {
            CombatSystem.Instance.ApplyDamage(hit, data.damage);
  

            Quaternion rotation = Quaternion.LookRotation(hit.normal);
            int hitLayer = hit.collider.gameObject.layer;

            switch (hitLayer)
            {
                case int e when e == LayerMask.NameToLayer("Enemy"):
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

    private void Recoil()
    {
        Sequence recoilSequence = DOTween.Sequence();

        float pitch = Random.Range(-2f, -1f);
        float yaw = Random.Range(-3f, 3f);
        float roll = Random.Range(-2f, 2f);

        Vector3 recoilBackOffset = new Vector3(0f, 0.004f, 0.310f);
        Vector3 recoilReturnOffset = new Vector3(0f, 0.004f, 0.358f);

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
