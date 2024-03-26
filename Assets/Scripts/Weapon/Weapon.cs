using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform handlerTransform;
    private WeaponHandling weaponHandling;
    
    // Animation triggers
    const string shootTrigger = "Shoot";
    const string ReloadTrigger = "Reload";
    const string OutOfAmmoBool = "OutOfAmmo";
    public int magSize{ get; private set; }

    public bool isReloading {get; private set;}
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

    public bool IsPlayerWeapon {
        get{
            if (this == weaponHandling.Weapon) return true;
            return false;
        }
    }

    public Transform handlerGrip
    {
        get { return handlerTransform; }
    }

    private void Update() {
        if (weaponHandling == null) return;

        if (isReloading && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Reload")){
            // Just stopped reload animation
            magSize = Data.maxMagSize;
            isReloading = false;
        }

        if (IsPlayerWeapon && !isReloading){
            if (magSize <= 0)
                animator.SetBool(OutOfAmmoBool, true);
            else animator.SetBool(OutOfAmmoBool, false);
        }
        
    }

    private void OnPlayerShoot_Action(object sender, EventArgs e){
        if (IsPlayerWeapon)
        {
            animator.SetTrigger(shootTrigger);
            magSize -= 1;
        }
    }

    public void Reload(){
        if (!isReloading)
        {
            animator.SetTrigger(ReloadTrigger);
            StartCoroutine(SetReload());
        }
    }

    IEnumerator SetReload(){
        yield return new WaitForEndOfFrame();
        isReloading = true;
    }

    // IINTERACTABLE INTERFACE
    public void Interact(Transform interactorTransform){

        if (weaponHandling == null)
        {
            weaponHandling = interactorTransform.GetComponent<WeaponHandling>();
            weaponHandling.OnShoot += OnPlayerShoot_Action;
        }

        weaponHandling.SetWeapon(this);
    }
    public string GetInteractText(){
        return "PickUpWeapon";
    }
    public Transform GetTransform(){
        return transform;
    }

}
