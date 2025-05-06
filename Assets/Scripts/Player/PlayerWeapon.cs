using System.Collections;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon")]
    public EmptyGun emptyGun;
    public Ak47 ak47;
    public M3 m3;
    public CameraController cameraController;

    [Header("Status")]
    public bool noBullet = false;

    public enum WeaponType { NONE, AK47, M3 }
    public enum BulletType { NONE, AK47Bullet, M3Bullet }

    public float IKWeight = 1f;
    public Transform LeftHandTarget;
    public Transform RightHandTarget;

    private WeaponType currentWeapon = WeaponType.NONE;
    private BulletType currentBullet = BulletType.NONE;

    private float currentBullets = 0;
    private float nextFireTime = 0;

    private BulletUI currentBulletUI;

    private void Start()
    {
        SetWeapon(WeaponType.NONE);           
        SetBullet(BulletType.NONE, null);       

        if (GameData.Instance.selectedWeaponType != PlayerWeapon.WeaponType.NONE)
        {
            SetWeapon(GameData.Instance.selectedWeaponType); 
        }

    }
    public Transform GetCurrentWeaponTransform()
    {
        switch (currentWeapon)
        {
            case WeaponType.NONE:
                return emptyGun.transform;
            case WeaponType.AK47:
                return ak47.transform;
            case WeaponType.M3:
                return m3.transform;
            default:
                return null;
        }
    }
    public void SetWeapon(WeaponType weaponType)
    {
        currentWeapon = weaponType;

        emptyGun.gameObject.SetActive(weaponType == WeaponType.NONE);
        ak47.gameObject.SetActive(weaponType == WeaponType.AK47);
        m3.gameObject.SetActive(weaponType == WeaponType.M3);

        switch (weaponType)
        {
            case WeaponType.NONE:
                LeftHandTarget = emptyGun.LeftHandTarget;
                RightHandTarget = emptyGun.RightHandTarget;
                break;
            case WeaponType.AK47:
                LeftHandTarget = ak47.LeftHandTarget;
                RightHandTarget = ak47.RightHandTarget;
                currentBullets = 0;
                break;
            case WeaponType.M3:
                LeftHandTarget = m3.LeftHandTarget;
                RightHandTarget = m3.RightHandTarget;
                currentBullets = 0;
                break;
        }
    }

    public void SetBullet(BulletType bulletType, BulletUI bulletUI = null)
    {
        currentBullet = bulletType;
        currentBulletUI = bulletUI;
        noBullet = bulletType == BulletType.NONE;

        if (bulletType == BulletType.NONE)
        {
            currentBullets = 0; 
        }
    }

    public void AddReserveAmmo(int amount)
    {
        currentBullets += amount;
    }

    public int GetReserveAmmo()
    {
        return Mathf.FloorToInt(currentBullets);
    }

    public WeaponType GetCurrentWeaponType()
    {
        return currentWeapon;
    }

    public void Fire()
    {
        if (Time.time < nextFireTime) return;

        switch (currentWeapon)
        {
            case WeaponType.NONE:
                emptyGun.Fire();
                break;

            case WeaponType.AK47:
                if (ak47.TotalAmmo <= 0) return;
                nextFireTime = Time.time + ak47.data.fireRate;
                ak47.TotalAmmo--;
                ak47.Fire();
                cameraController.SetShake();
                break;

            case WeaponType.M3:
                if (m3.TotalAmmo <= 0) return;
                nextFireTime = Time.time + m3.data.fireRate;
                m3.TotalAmmo--;
                m3.Fire();
                cameraController.SetShake();
                break;
        }
    }

    public void Reload()
    {
        float neededAmmo = 0;
        float ammoToLoad = 0;

        switch (currentWeapon)
        {
            case WeaponType.AK47:
                if (currentBullet != BulletType.AK47Bullet)
                {
                    noBullet = true;
                    return;
                }
                neededAmmo = ak47.MaxAmmo - ak47.TotalAmmo;
                break;

            case WeaponType.M3:
                if (currentBullet != BulletType.M3Bullet)
                {
                    noBullet = true;
                    return;
                }
                neededAmmo = m3.MaxAmmo - m3.TotalAmmo;
                break;

            default:
                return;
        }

        if (neededAmmo <= 0 || currentBullets <= 0) return;

        ammoToLoad = Mathf.Min(neededAmmo, currentBullets);
        currentBullets -= ammoToLoad;

        switch (currentWeapon)
        {
            case WeaponType.AK47:
                ak47.TotalAmmo += ammoToLoad;
                break;
            case WeaponType.M3:
                m3.TotalAmmo += ammoToLoad;
                break;
        }

        TryReduceBulletUI((int)ammoToLoad);
    }

    private void TryReduceBulletUI(int amount)
    {
        if (currentBulletUI != null)
        {
            if (currentBulletUI.gameObject != null)
            {
                currentBulletUI.ReduceAmmo(amount);

                
                if (currentBulletUI == null || currentBulletUI.gameObject == null)
                {
                    currentBulletUI = null;
                }
            }
            else
            {
                currentBulletUI = null;
            }
        }
    }
}
