using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

public class TowerEntity : MonoBehaviour
{
    [SerializeField] protected TowerSO tower;

    [SerializeField] protected Transform firePos;

    [SerializeField] protected BulletEntity pfBullet;

    protected bool TowerOnCooldown = false;

    // Update is called once per frame
    protected virtual void Update()
    {
        if (TowerOnCooldown == false)
        {
            Fire();
        }
    }

    protected virtual void Fire()
    {
        BulletEntity bullet = Instantiate(pfBullet, firePos.transform.position, Quaternion.LookRotation(firePos.transform.forward));
        bullet.Initialize(tower.Speed, tower.Spread, tower.Range);
        TowerOnCooldown = true;
        StartCoroutine(RefreshCooldown());
    }

    protected IEnumerator RefreshCooldown()
    {
        yield return new WaitForSeconds(tower.FireRate);

        TowerOnCooldown = false;
    }
}