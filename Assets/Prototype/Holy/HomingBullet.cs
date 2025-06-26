using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public float speed = 8f;
    public float rotateSpeed = 200f;
    public float lifeTime = 5f;

    private Transform target;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        FindClosestEnemy();

        if (target == null)
        {
            // 如果没有目标，可以向前直飞或销毁
            transform.Translate(Vector2.up * speed * Time.deltaTime, Space.Self);
            return;
        }

        Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        transform.Rotate(0, 0, -rotateAmount * rotateSpeed * Time.deltaTime);

        transform.Translate(Vector2.up * speed * Time.deltaTime, Space.Self);
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDist = Mathf.Infinity;
        Transform closest = null;
        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy.transform;
            }
        }
        target = closest;
    }
}
