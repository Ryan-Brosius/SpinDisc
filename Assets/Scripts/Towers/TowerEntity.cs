using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

public class TowerEntity : MonoBehaviour
{
    public TowerSO tower;

    [SerializeField] private Transform firePos;

    [SerializeField] private BulletEntity pfBullet;

    private bool TowerOnCooldown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (TowerOnCooldown == false)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        // Aims/Gets target position, then fires projectile
        // Aim();
        Fire();
    }

    public void Fire()
    {
        print("Fire");
        BulletEntity bullet = Instantiate(pfBullet, firePos.transform.position, Quaternion.LookRotation(firePos.transform.forward));
        bullet.Initialize(10f, firePos.transform.forward);
        TowerOnCooldown = true;
        StartCoroutine(RefreshCooldown());
    }

    private IEnumerator RefreshCooldown()
    {
        yield return new WaitForSeconds(tower.FireRate);

        TowerOnCooldown = false;
    }
}