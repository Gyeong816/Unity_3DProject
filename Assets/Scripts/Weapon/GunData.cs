using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GunData
{
    /// <summary>
    /// 무기 이름
    /// </summary>
    public string weaponName;
    /// <summary>
    /// 발사속도
    /// </summary>
    public float fireRate = 0.05f;
    /// <summary>
    /// 데미지
    /// </summary>
    public int damage = 10;
    /// <summary>
    /// 탄창량
    /// </summary>
    public int totalAmmo = 30;
    /// <summary>
    /// 장전시간
    /// </summary>
    public float reloadTime = 2.0f;
    /// <summary>
    /// 유효공격범위
    /// </summary>
    public float range = 100.0f;
}
