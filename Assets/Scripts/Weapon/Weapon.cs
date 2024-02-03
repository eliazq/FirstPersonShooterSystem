using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform shootingPoint;
    public Transform ShootingPoint
    {
        get{
            return shootingPoint;
        }
        private set{
            shootingPoint = value;
        }
    }
    [SerializeField] private WeaponData weaponData;
    public WeaponData Data
    {
        get{
            return weaponData;
        }
        private set{
            weaponData = value;
        }
    }

    // IINTERACTABLE INTERFACE
    public void Interact(Transform interactorTransform){
        Player.Instance.WeaponHandling.SetWeapon(this);
    }
    public string GetInteractText(){
        return "PickUpWeapon";
    }
    public Transform GetTransform(){
        return transform;
    }

}
