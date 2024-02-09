using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class WeaponSystem
{
    public static void DropWeapon(Weapon weapon){
        if (weapon != null){
            weapon.transform.parent = null;
            weapon.AddComponent<Rigidbody>();
        }
    }
    public static void DropWeapon(Weapon weapon, Vector3 throwDirection, float throwForce){
        if (weapon != null){
            weapon.transform.parent = null;
            weapon.AddComponent<Rigidbody>().AddForce(throwDirection * throwForce);
        }
    }

}
