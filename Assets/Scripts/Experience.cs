using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 经验掉落物逻辑
/// 2025-06-26 肖沐奇 创建
/// </summary>
public class Experience : MonoBehaviour
{
    [Header("经验类型")]
    [SerializeField] private ExpType _expType = ExpType.Normal;

    private ExperienceConfig _experienceConfig;

    void Awake()
    {
        // 从Resources目录加载经验配置
        _experienceConfig = Resources.Load<ExperienceConfig>("ScriptableObjects/Exp Config");
        if (_experienceConfig == null)
        {
            Debug.LogError("无法从Resources/ScriptableObjects/Exp Config加载经验配置！");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("碰撞对象：" + collision.gameObject.name);
        // 检查碰撞对象或其父对象是否有Player标签，因为渲染Sprite的Collider是Player的子对象
        GameObject playerObject = null;

        // 先检查碰撞对象本身
        if (collision.gameObject.CompareTag("Player"))
        {
            playerObject = collision.gameObject;
        }
        // 如果碰撞对象不是Player，检查其父对象
        else if (collision.transform.parent != null && collision.transform.parent.CompareTag("Player"))
        {
            playerObject = collision.transform.parent.gameObject;
        }

        // 如果找到了玩家对象，则添加经验
        if (playerObject != null)
        {
            if (_experienceConfig != null)
            {
                int amount = _experienceConfig.GetExperienceAmount(_expType);
                ExperienceLevelController.instance.AddExperience(amount);
            }
            else
            {
                Debug.LogError("ExperienceConfig 未加载！请确保 Resources/ScriptableObjects/Exp Config 文件存在。");
            }
            Destroy(gameObject);
        }
    }
}
