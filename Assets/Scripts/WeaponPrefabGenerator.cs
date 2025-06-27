/// <summary>
/// 谈恩萁创建
/// </summary>
using UnityEngine;

public class WeaponPrefabGenerator : MonoBehaviour
{
    public static WeaponPrefabGenerator instance;
    public GameObject weaponGameObject;

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 根据武器名称生成武器预制体
    /// </summary>
    /// <param name="weaponName">武器名称，对应Resources文件夹中的预制体名称</param>
    public void GenerateWeaponPrefab(string weaponName)
    {
        // 从Resources文件夹加载预制体
        GameObject weaponPrefab = Resources.Load<GameObject>(weaponName);
        
        if (weaponPrefab == null)
        {
            Debug.LogWarning($"未找到预制体: {weaponName}");
            return;
        }

        // 在weaponGameObject下实例化预制体
        GameObject weaponInstance = Instantiate(weaponPrefab, weaponGameObject.transform);
        
        // 可选：设置生成的武器实例的名称
        weaponInstance.name = weaponName + "_Instance";
    }
    
} 