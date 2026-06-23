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
            Fire();
        }
    }

    public void Fire()
    {
        BulletEntity bullet = Instantiate(pfBullet, firePos.transform.position, Quaternion.LookRotation(firePos.transform.forward));
        bullet.Initialize(tower.Speed, tower.Spread, tower.Range);
        TowerOnCooldown = true;
        StartCoroutine(RefreshCooldown());
    }

    private IEnumerator RefreshCooldown()
    {
        yield return new WaitForSeconds(tower.FireRate);

        TowerOnCooldown = false;
    }
}