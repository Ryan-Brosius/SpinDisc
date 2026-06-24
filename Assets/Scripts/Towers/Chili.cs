using UnityEngine;

public class Chili : TowerEntity
{
    private int bulletCount = 24;

   protected override void Fire()
   {
        for (int i = 0; i < bulletCount; i++)
        {
            BulletEntity bullet = Instantiate(pfBullet, firePos.transform.position, Quaternion.Euler(0f, (360f / bulletCount)*i, 0f));
            bullet.Initialize(tower.Speed, tower.Spread, tower.Range);
        }

        TowerOnCooldown = true;
        StartCoroutine(RefreshCooldown());
   }
}
