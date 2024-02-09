using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public static WeaponSystem Instance;

    [Header("Shooting Data")]
    [SerializeField] string groundLayerName = "Ground";
    [SerializeField] GameObject trailPrefab;
    [SerializeField] GameObject impactEffect;
    [SerializeField] float impactDestroyTime = 50f;
    [SerializeField] float trailTime = 0.15f;
    [SerializeField] float impactForce = 250f;
    
    private void Start() {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this);
    }
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

    public void Shoot(Vector3 shootingPosition, Vector3 shootDirection, Vector3 ShooterPosition, Vector3 trailStartPoint, float shotDistance, float shotImpactForce){
        
        if (Physics.Raycast(shootingPosition, shootDirection, out RaycastHit hit, shotDistance))
        {
            if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer(groundLayerName)))
            {
                SpawnBulletImpact(impactEffect, hit.point, Quaternion.LookRotation(hit.normal), impactDestroyTime);
            }
            ShootBulletTrail(trailPrefab, trailStartPoint, hit.point, trailTime);
            if (hit.transform.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddForce(-hit.normal * shotImpactForce);
                Vector3 ForceDir = (hit.transform.position - ShooterPosition).normalized;
                rigidbody.AddForce(ForceDir * shotImpactForce);
            }
        }
        else {
            // If didnt hit anything, still shoot the trail
            Vector3 targetPos = shootingPosition + shootDirection * 100f;
            ShootBulletTrail(trailPrefab, trailStartPoint, targetPos, trailTime);
        }

    }

    public void Shoot(Vector3 shootingPosition, Vector3 shootDirection, Vector3 ShooterPosition, Vector3 trailStartPoint, float shotDistance){

        if (Physics.Raycast(shootingPosition, shootDirection, out RaycastHit hit, shotDistance))
        {
            if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer(groundLayerName)))
            {
                SpawnBulletImpact(impactEffect, hit.point, Quaternion.LookRotation(hit.normal), impactDestroyTime);
            }
            ShootBulletTrail(trailPrefab, trailStartPoint, hit.point, trailTime);
            if (hit.transform.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddForce(-hit.normal * impactForce);
                Vector3 ForceDir = (hit.transform.position - ShooterPosition).normalized;
                rigidbody.AddForce(ForceDir * impactForce);
            }
        }
        else {
            // If didnt hit anything, still shoot the trail
            Vector3 targetPos = shootingPosition + shootDirection * 100f;
            ShootBulletTrail(trailPrefab, trailStartPoint, targetPos, trailTime);
        }

    }

    public bool Shoot(Vector3 shootingPosition, Vector3 shootDirection, Vector3 ShooterPosition, Vector3 trailStartPoint, float shotDistance, float shotImpactForce, out RaycastHit hit){
        
        if (Physics.Raycast(shootingPosition, shootDirection, out RaycastHit hitInfo, shotDistance))
        {
            hit = hitInfo;
            if (hitInfo.collider.gameObject.layer.Equals(LayerMask.NameToLayer(groundLayerName)))
            {
                SpawnBulletImpact(impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal), impactDestroyTime);
            }
            ShootBulletTrail(trailPrefab, trailStartPoint, hitInfo.point, trailTime);
            if (hitInfo.transform.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddForce(-hitInfo.normal * shotImpactForce);
                Vector3 ForceDir = (hitInfo.transform.position - ShooterPosition).normalized;
                rigidbody.AddForce(ForceDir * shotImpactForce);
            }
            return true;
        }
        else {
            // If didnt hit anything, still shoot the trail
            Vector3 targetPos = shootingPosition + shootDirection * 100f;
            ShootBulletTrail(trailPrefab, trailStartPoint, targetPos, trailTime);
            hit = new RaycastHit();
            return false;
        }
    }

    public void SpawnBulletImpact(GameObject impactEffect, Vector3 position, Quaternion rotation, float impactDestroyTime){
        GameObject impact = Instantiate(impactEffect, position, rotation);
        Destroy(impact, impactDestroyTime);
    }

    public void ShootBulletTrail(GameObject bulletTrailPrefab, Vector3 startPosition, Vector3 endPosition, float trailFlyTime){
        GameObject trail = Instantiate(bulletTrailPrefab, startPosition, Quaternion.identity);
        MoveTrailSmooth(trail, endPosition, trailFlyTime);
    }

    private void MoveTrailSmooth(GameObject trail, Vector3 targetPos, float trailTime) {
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
    

}
