using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public Transform rightHand;
    public Transform leftHand;

    [Header("Optional Shield Reference")]
    public GameObject shieldObject;

    private GameObject currentWeapon;
    private GameObject currentPrefab;

    public GameObject CurrentWeaponPrefab { get; private set; }
    public bool IsTwoHanded { get; private set; }

    public void EquipWeapon(GameObject weaponPrefab, bool twoHanded)
    {
        if(weaponPrefab == null)
        {
            Debug.LogError("Weapon prefab is null.");
            return;
        }

        if (currentPrefab == weaponPrefab)
        {
            Debug.Log("Weapon already equipped.");
            return;
        }

        if(currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }

        currentWeapon = Instantiate(weaponPrefab, rightHand);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.transform.localScale = Vector3.one;

        currentPrefab = weaponPrefab;
        CurrentWeaponPrefab = weaponPrefab;
        IsTwoHanded = twoHanded;

        if (shieldObject != null)
        {
            shieldObject.SetActive(!twoHanded);
        }

        Debug.Log($"Equipped weapon: {weaponPrefab.name}, TwoHanded={IsTwoHanded}");
    }
}