using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHandling : MonoBehaviour
{
    // TODO! : Different Mags, for pistols, AR etc.. and NaughtyAttribute organization
    public Weapon Weapon {get; private set;}
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private GameObject bulletTrailPrefab;
    [SerializeField] private float trailTime = 30f;
    [SerializeField] private Transform handTransform;
    [SerializeField] private float WeaponThrowForce = 300f;
    [SerializeField] private float ShotImpactForce = 200f;
    public int pistolMags {get; set;} = 0;
    public int subMachineMags {get; set;} = 0;
    public event EventHandler OnShoot;
    private float shootingCooldown;
    private float dropWeaponCooldown;
    private float weaponThrowRate = 1f;
    private float impactDestroyTime = 60f;
    [SerializeField] private string groundLayerName = "Ground";
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
            Shoot(Camera.main.transform.position, Camera.main.transform.forward, Player.Instance.transform.position);
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
        
    }

    private void DropWeapon(){
        WeaponSystem.DropWeapon(Weapon, Camera.main.transform.forward, WeaponThrowForce);
        Weapon = null;
    }

    private void Shoot(Vector3 shootingPosition, Vector3 shootDirection, Vector3 ShooterPosition){
        OnShoot?.Invoke(this, EventArgs.Empty);            
        // Check if hits object
        if (Physics.Raycast(shootingPosition, shootDirection, out RaycastHit hit, Weapon.Data.shootingDistance))
        {
            if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer(groundLayerName)))
            {
                SpawnBulletImpact(hit.point, Quaternion.LookRotation(hit.normal));
            }
            ShootBulletTrail(Weapon.ShootingPoint.position, hit.point);
            if (hit.transform.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddForce(-hit.normal * ShotImpactForce);
                Vector3 ForceDir = (hit.transform.position - ShooterPosition).normalized;
                rigidbody.AddForce(ForceDir * ShotImpactForce);
            }
        }
        else {
            // If didnt hit anything, still shoot the trail
            Vector3 targetPos = shootingPosition + shootDirection * 100f;
            ShootBulletTrail(Weapon.ShootingPoint.position, targetPos);
        }
    }

    private void SpawnBulletImpact(Vector3 position, Quaternion rotation){
        GameObject impact = Instantiate(impactEffect, position, rotation);
        Destroy(impact, impactDestroyTime);
    }

    private void ShootBulletTrail(Vector3 startPosition, Vector3 endPosition){
        GameObject trail = Instantiate(bulletTrailPrefab, startPosition, Quaternion.identity);
        MoveTrailSmooth(trail, endPosition);
    }

    private void MoveTrailSmooth(GameObject trail, Vector3 targetPos) {
    // You can adjust the duration based on how fast you want the trail to move
    float duration = trailTime;

    // Store the initial position of the trail
    Vector3 initialPos = trail.transform.position;

    // Use a coroutine to smoothly move the trail over time
    StartCoroutine(MoveTrailCoroutine(trail, initialPos, targetPos, duration));
}

private IEnumerator MoveTrailCoroutine(GameObject trail, Vector3 initialPos, Vector3 targetPos, float duration) {
    float elapsed = 0f;

    while (elapsed < duration) {
        // Interpolate the position between initialPos and targetPos over time
        trail.transform.position = Vector3.Lerp(initialPos, targetPos, elapsed / duration);

        // Increment the elapsed time
        elapsed += Time.deltaTime;

        // Wait for the next frame
        yield return null;
    }

    // Ensure the trail reaches the exact target position
    trail.transform.position = targetPos;
    Destroy(trail);
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
