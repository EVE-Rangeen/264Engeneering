using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 武器图标生成器
/// 负责在挂载的物体下添加Image类型的子物体
/// </summary>
public class WeaponIconGenerator : MonoBehaviour
{
    /// <summary>
    /// 添加一个武器图标子物体
    /// </summary>
    /// <param name="weaponData">武器数据</param>
    /// <param name="iconName">图标名称</param>
    public void AddWeaponIcon(WeaponData weaponData, string iconName = "WeaponIcon")
    {
        if (weaponData == null)
        {
            Debug.LogWarning($"武器数据为空，无法创建图标: {iconName}");
            return;
        }

        // 创建新的GameObject作为子物体
        GameObject iconObject = new GameObject(iconName);
        
        // 设置父物体
        iconObject.transform.SetParent(this.transform);
        
        // 添加Image组件
        Image iconImage = iconObject.AddComponent<Image>();
        
        // 设置图标精灵
        if (weaponData.WeaponIcon != null)
        {
            iconImage.sprite = weaponData.WeaponIcon;
        }
        else
        {
            Debug.LogWarning($"武器 {weaponData.WeaponName} 没有设置图标");
        }

        // 设置RectTransform属性
        RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        rectTransform.localPosition = Vector3.zero;

        Debug.Log($"成功添加武器图标: {weaponData.WeaponName}");
    }
} 