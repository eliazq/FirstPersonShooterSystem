using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private Animator animator;
    
    // Animation triggers
    const string shootTrigger = "Shoot";
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

    private void Awake() {
        Player.Instance.WeaponHandling.OnShoot += OnPlayerShoot_Action;
    }

    private void OnPlayerShoot_Action(object sender, EventArgs e){
        if (this == Player.Instance.WeaponHandling.Weapon)
        {
            animator.SetTrigger(shootTrigger);
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
