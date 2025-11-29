using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public Transform rightHand;
    public Transform leftHand;

    private GameObject currentWeapon;
    private GameObject currentPrefab;

    public GameObject CurrentWeaponPrefab { get; private set; }
    public bool IsTwoHanded { get; private set; }

    public void EquipWeapon(GameObject weaponPrefab, bool twoHanded)
    {
        if (currentPrefab == weaponPrefab)
        {
            Debug.Log("Weapon already equipped.");
            return;
        }

        if(currentWeapon != null)
            Destroy(currentWeapon);
        

        currentWeapon = Instantiate(weaponPrefab);

        // attach weapon depending on type
        if (!twoHanded)
        {
            currentWeapon.transform.SetParent(rightHand);
        }
        else
        {
            currentWeapon.transform.SetParent(leftHand);
        }

        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        currentPrefab = weaponPrefab;
        CurrentWeaponPrefab = weaponPrefab;
        IsTwoHanded = twoHanded;
    }
}