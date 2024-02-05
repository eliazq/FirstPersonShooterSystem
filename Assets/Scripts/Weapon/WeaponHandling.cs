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
    [SerializeField] private int pistolMags = 3; // Change in future
    [SerializeField] private int subMachineMags = 3;
    public event EventHandler OnShoot;
    private float shootingCooldown;
    private float dropWeaponCooldown;
    private float weaponThrowRate = 1f;
    private float impactDestroyTime = 5f;
    public bool HasWeapon {
        get {
            return Weapon != null;
        }
    }

    private void Update() {
        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= shootingCooldown && HasWeapon && Weapon.magSize > 0 && !Weapon.isReloading){
            shootingCooldown = Time.time + 1f/Weapon.Data.fireRate;
            Shoot();
        }
        
        if (Input.GetKey(KeyCode.G) && Time.time >= dropWeaponCooldown && !Weapon.isReloading){
            dropWeaponCooldown = Time.time + 1f/weaponThrowRate;
            DropWeapon();
        }

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
                Weapon.transform.position = handTransform.position;
                Weapon.transform.parent = handTransform;
                Weapon.transform.rotation = handTransform.rotation;
            }
        }
        else {
            if (weapon.GetComponent<Rigidbody>() != null) Destroy(weapon.GetComponent<Rigidbody>());
            Weapon = weapon;
            Weapon.transform.position = handTransform.position;
            Weapon.transform.parent = handTransform;
            Weapon.transform.rotation = handTransform.rotation;
        }
        
    }

    public void DropWeapon(){
        if (Weapon != null){
            Weapon.transform.parent = null;
            Weapon.AddComponent<Rigidbody>().AddForce(Camera.main.transform.forward * WeaponThrowForce);
            Weapon = null;
        }
    }

    private void Shoot(){
        OnShoot?.Invoke(this, EventArgs.Empty);            
        // Check if hits object
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, Weapon.Data.shootingDistance))
        {
            SpawnBulletImpact(hit.point, Quaternion.LookRotation(hit.normal), impactDestroyTime);
            ShootBulletTrail(Weapon.ShootingPoint.position, hit.point);
            if (hit.transform.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddForce(-hit.normal * ShotImpactForce);
                Vector3 ForceDir = (hit.transform.position - Player.Instance.transform.position).normalized;
                rigidbody.AddForce(ForceDir * ShotImpactForce);
            }
        }
        else {
            // If didnt hit anything, still shoot the trail
            ShootBulletTrail(Weapon.ShootingPoint.position, Camera.main.transform.forward * 100f);
        }
    }

    private void SpawnBulletImpact(Vector3 position, Quaternion rotation, float timeToDestroy){
        GameObject impact = Instantiate(impactEffect, position, rotation);
        Destroy(impact, timeToDestroy);
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
}

}
