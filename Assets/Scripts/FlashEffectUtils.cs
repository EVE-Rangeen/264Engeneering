using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 受击闪烁效果相关工具
/// 2025-07-02 肖沐奇 创建
/// </summary>
public static class FlashEffectUtils
{
    /// <summary>
    /// 创建闪烁材质
    /// </summary>
    public static Material CreateFlashMaterial(Color color)
    {
        // 创建一个新的材质实例，使用Unity内置的着色器 
        Material flashMaterial = new Material(Shader.Find("GUI/Text Shader"));
        flashMaterial.color = color;

        // 设置材质名称便于调试
        flashMaterial.name = $"{color} Flash Material";

        return flashMaterial;
    }
}
