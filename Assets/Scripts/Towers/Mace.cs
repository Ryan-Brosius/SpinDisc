using UnityEngine;

public class Mace : TowerEntity
{
    private int bulletCount = 15;

    public override void Fire()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            BulletEntity bullet = Instantiate(pfBullet, firePos.transform.position, Quaternion.Euler(0f, ((45f / (bulletCount - 1)) * i) - (45f/2f), 0f));
            bullet.Initialize(tower.Speed, tower.Spread, tower.Range, tower.Damage);
        }

        TowerOnCooldown = true;
        StartCoroutine(RefreshCooldown());
    }
}
