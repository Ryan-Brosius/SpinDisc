using UnityEngine;

public class Mace : TowerEntity
{
    private int bulletCount = 15;

    public override void Fire()
    {

        for (int i = 0; i < bulletCount; i++)
        {
            float spreadAngle = ((45f / (bulletCount - 1)) * i) - (45f / 2f);
            Quaternion localRotationOffset = Quaternion.Euler(0f, spreadAngle, 0f);

            Quaternion finalRotation = firePos.transform.rotation * localRotationOffset;

            BulletEntity bullet = Instantiate(pfBullet, firePos.transform.position, finalRotation);
            bullet.Initialize(tower.Speed, tower.Spread, tower.Range, tower.Damage, gameObject);
        }

        TowerOnCooldown = true;
        StartCoroutine(RefreshCooldown());
    }
}
