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
            if (this == Player.Instance.WeaponHandling.Weapon) return true;
            return false;
        }
    }

    private void Awake() {
        Player.Instance.WeaponHandling.OnShoot += OnPlayerShoot_Action;
    }

    private void Update() {
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
        Player.Instance.WeaponHandling.SetWeapon(this);
    }
    public string GetInteractText(){
        return "PickUpWeapon";
    }
    public Transform GetTransform(){
        return transform;
    }

}
