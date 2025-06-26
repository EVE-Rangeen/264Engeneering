using UnityEngine;

/// <summary>
/// 经验配置数据
/// 2025-06-26 肖沐奇 创建
/// </summary>
[CreateAssetMenu(fileName = "Exp Config", menuName = "Game Data/Exp Config")]
public class ExperienceConfig : ScriptableObject
{
    [Header("普通经验配置")]
    [SerializeField] private int _normalExpAmount = 10;

    [Header("高级经验配置")]
    [SerializeField] private int _highExpAmount = 20;

    [Header("超高级经验配置")]
    [SerializeField] private int _veryHighExpAmount = 30;

    /// <summary>
    /// 获取指定类型经验的数值
    /// </summary>
    /// <param name="expType">经验类型</param>
    /// <returns>经验数值</returns>
    public int GetExperienceAmount(ExpType expType)
    {
        switch (expType)
        {
            case ExpType.Normal:
                return _normalExpAmount;
            case ExpType.High:
                return _highExpAmount;
            case ExpType.VeryHigh:
                return _veryHighExpAmount;
            default:
                Debug.LogWarning($"未知的经验类型：{expType}");
                return 0;
        }
    }

    /// <summary>
    /// 普通经验数量
    /// </summary>
    public int NormalExpAmount => _normalExpAmount;

    /// <summary>
    /// 高级经验数量
    /// </summary>
    public int HighExpAmount => _highExpAmount;

    /// <summary>
    /// 超高级经验数量
    /// </summary>
    public int VeryHighExpAmount => _veryHighExpAmount;
}