using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    public PlayerWeapon.WeaponType selectedWeaponType = PlayerWeapon.WeaponType.NONE;
    public PlayerEquipment.EquipmentType selectedHelmetType = PlayerEquipment.EquipmentType.NONEHELMET;
    public PlayerEquipment.EquipmentType selectedVestType = PlayerEquipment.EquipmentType.NONEVEST;
    
    public int playerDollar = 150;
    public int vendorDollar = 10000;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
}
