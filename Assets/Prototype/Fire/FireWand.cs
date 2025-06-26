using UnityEngine;

public class FireWand : MonoBehaviour
{
    [Header("设置")]
    public float cooldown = 0.8f;       // 攻击冷却
    public Animator fireZoneAnimator;   // 拖拽 FireAttackZone 对象到这里

    private float cooldownTimer = 0f;
    private Camera mainCamera;
    private Transform fireZoneTransform;

    void Start()
    {
        mainCamera = Camera.main;
        if (fireZoneAnimator != null)
        {
            fireZoneTransform = fireZoneAnimator.transform;
            fireZoneAnimator.gameObject.SetActive(false); // 确保开始时是禁用的
        }
    }

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0) && cooldownTimer <= 0)
        {
            Attack();
            cooldownTimer = cooldown;
        }
    }

    void Attack()
    {
        // 1. 激活攻击区域
        fireZoneAnimator.gameObject.SetActive(true);

        // 2. 计算方向并旋转攻击区域
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 attackDirection = (mousePosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
        fireZoneTransform.rotation = Quaternion.Euler(0, 0, angle);

        // 3. 触发动画
        fireZoneAnimator.SetTrigger("Attack");
    }
}