using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour, IInteractable
{
    [SerializeField] private WeaponData.WeaponType ammoType;

    public string GetInteractText()
    {
        return ammoType.ToString() + " ammo";
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(Transform interactorTransform)
    {
        switch(ammoType){
            case WeaponData.WeaponType.Pistol:
                interactorTransform.GetComponent<WeaponHandling>().pistolMags += 1;
                break;
            case WeaponData.WeaponType.SubMachine:
                interactorTransform.GetComponent<WeaponHandling>().subMachineMags += 1;
                break;
        }
        Destroy(gameObject);
    }
}
