using UnityEngine;

public class PlayerAutoShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float shootInterval = 0.5f;

    private float timer = 0f;
    private Quaternion shootRotation = Quaternion.identity;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= shootInterval)
        {
            AimAtNearestEnemy();
            Shoot();
            timer = 0f;
        }
    }

    void AimAtNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            shootRotation = Quaternion.identity; // 如果没有敌人，朝向默认方向
            return;
        }

        Transform closest = null;
        float minDist = Mathf.Infinity;
        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy.transform;
            }
        }
        if (closest != null)
        {
            Vector2 direction = (Vector2)closest.position - (Vector2)transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            shootRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, transform.position, shootRotation);
    }
}