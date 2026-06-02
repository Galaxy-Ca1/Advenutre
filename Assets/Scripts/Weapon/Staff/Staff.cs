using System;
using UnityEngine;

public class Staff : BaseWeapon
{
    [SerializeField] private ProjectileSO projectileSO;
    [SerializeField] private Transform projectileSpawnPoint;

    public event EventHandler OnStaffAttack;

    public override void Attack()
    {
        if (projectileSO == null || projectileSO.projectilePrefab == null)
        {
            Debug.LogError("[Staff] ProjectileSO or projectile prefab is missing.");
            return;
        }

        Transform spawnPoint = projectileSpawnPoint != null ? projectileSpawnPoint : transform;
        GameObject laserObject = Instantiate(projectileSO.projectilePrefab, spawnPoint.position, spawnPoint.rotation);

        if (laserObject.TryGetComponent(out MagicLaserProjectile laserProjectile))
        {
            laserProjectile.Init(projectileSO);
        }

        OnStaffAttack?.Invoke(this, EventArgs.Empty);
    }
}
