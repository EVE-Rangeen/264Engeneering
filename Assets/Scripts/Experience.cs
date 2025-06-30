using UnityEngine;

/// <summary>
/// 经验掉落物逻辑
/// 2025-06-26 肖沐奇 创建
/// </summary>
public class Experience : Pickup
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

    protected override void DataUpdate()
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
    }
}
