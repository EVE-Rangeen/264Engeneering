using UnityEngine;

public class HolyLight : MonoBehaviour
{
    public float radius = 1.5f;
    public float damagePerSecond = 20f;
    // public LayerMask enemyLayer; // No longer needed

    void Update()
    {
        // Find all GameObjects with the "Enemy" tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemyObject in enemies)
        {
            // Calculate the distance between the holy light and the enemy
            float distance = Vector2.Distance(transform.position, enemyObject.transform.position);

            // If the enemy is within the radius
            if (distance <= radius)
            {
                // Get the enemy's health component and apply damage
                EnemyHealth enemyHealth = enemyObject.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damagePerSecond * Time.deltaTime);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}