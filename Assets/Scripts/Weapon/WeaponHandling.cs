using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class WeaponHandling : MonoBehaviour
{
    public Weapon Weapon {get; private set;}
    [Header("Settings")]
    [SerializeField] private Transform handTransform;
    [SerializeField] private float WeaponThrowForce = 300f;
    [SerializeField] private float ShotImpactForce = 200f;

    [Header("Hand IK")]
    [SerializeField] private TwoBoneIKConstraint twoBoneIKConstraint;
    [SerializeField] private RigBuilder rigBuilder;
    [SerializeField] private Transform emptyHandTransform;

    public int pistolMags {get; set;} = 0;
    public int subMachineMags {get; set;} = 0;
    public event EventHandler OnShoot;
    private float shootingCooldown;
    private float dropWeaponCooldown;
    private float weaponThrowRate = 1f;
    public bool HasWeapon {
        get {
            return Weapon != null;
        }
    }

    private void Update() {

        HandleWeapon();

    }

    private void HandleWeapon(){
        if (HasWeapon)
        {
            CheckShooting();

            CheckWeaponDrop();

            CheckReload(); 
        }
    }

    private void CheckShooting(){
        // Check if player shoots and if its possible
        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= shootingCooldown && Weapon.magSize > 0 && !Weapon.isReloading){
            shootingCooldown = Time.time + 1f/Weapon.Data.fireRate;
            OnShoot?.Invoke(this, EventArgs.Empty);

            if (WeaponSystem.Instance.Shoot(Camera.main.transform.position, Camera.main.transform.forward,
                transform.position, Weapon.ShootingPoint.position, Weapon.Data.shootingDistance, ShotImpactForce, out RaycastHit hit))
                {
                    // TODO: Damage Logic Here, IDamageable.Damage
                     
                }
            
        }
    }

    private void CheckWeaponDrop(){
        if (Input.GetKey(KeyCode.G) && Time.time >= dropWeaponCooldown && !Weapon.isReloading){
            dropWeaponCooldown = Time.time + 1f/weaponThrowRate;
            DropWeapon(); 
        }
    }

    private void CheckReload(){
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadWithRightMag();
        }
    }

    private void ReloadWithRightMag(){
        if (Weapon.Data.weaponType == WeaponData.WeaponType.Pistol)
        {
            if (pistolMags > 0 && Weapon.magSize < Weapon.Data.maxMagSize){
            pistolMags -= 1;
            Weapon.Reload();
            }
        }
        else if (Weapon.Data.weaponType == WeaponData.WeaponType.SubMachine){
            if (subMachineMags > 0 && Weapon.magSize < Weapon.Data.maxMagSize){
            subMachineMags -= 1;
            Weapon.Reload();
            }
        }
    }

    public void SetWeapon(Weapon weapon){
        if (HasWeapon){
            if (!Weapon.isReloading) {
                Weapon.transform.parent = null;
                Weapon.AddComponent<Rigidbody>();
                if (weapon.GetComponent<Rigidbody>() != null) Destroy(weapon.GetComponent<Rigidbody>());
                Weapon = weapon;

                StartCoroutine(MoveWeaponDynamically(5f));
                StartCoroutine(RotateWeaponDynamically(5f));

                Weapon.transform.parent = handTransform;
            }
        }
        else {
            if (weapon.GetComponent<Rigidbody>() != null) Destroy(weapon.GetComponent<Rigidbody>());
            Weapon = weapon;

            StartCoroutine(MoveWeaponDynamically(5f));
            StartCoroutine(RotateWeaponDynamically(5f));

            Weapon.transform.parent = handTransform;
        }

        SetHandIKTarget(Weapon.handlerGrip);
        
    }

    private void DropWeapon(){
        WeaponSystem.DropWeapon(Weapon, Camera.main.transform.forward, WeaponThrowForce);
        Weapon = null;
        SetHandIKTarget(emptyHandTransform);
    }

    private void SetHandIKTarget(Transform target)
    {
        twoBoneIKConstraint.data.target = target;
        rigBuilder.Build();
    }

    private IEnumerator MoveWeaponDynamically(float smoothness){
        float elapsedTime = 0f;

        while (elapsedTime < smoothness){
            Vector3 targetPosition = handTransform.position;
            Weapon.transform.position = Vector3.Lerp(Weapon.transform.position, targetPosition, elapsedTime / smoothness);
            float positionThreshold = 0.01f;
            float distance = Vector3.Distance(Weapon.transform.position, targetPosition);
            if (distance < positionThreshold){
                Weapon.transform.position = targetPosition;
                break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator RotateWeaponDynamically(float smoothness){
        float elapsedTime = 0f;

        while (elapsedTime < smoothness){
            Quaternion targetRotation = handTransform.rotation;
            Weapon.transform.rotation = Quaternion.Slerp(Weapon.transform.rotation, targetRotation, elapsedTime / smoothness);
            float rotationThreshold = 0.01f;
            float angle = Quaternion.Angle(Weapon.transform.rotation, targetRotation);
            if (angle < rotationThreshold){
                Weapon.transform.rotation = targetRotation;
                break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


}
