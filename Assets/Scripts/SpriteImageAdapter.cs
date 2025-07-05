/// <summary>
/// 谈恩萁创建
/// </summary>
using UnityEngine;
using UnityEngine.UI;
 
[RequireComponent(typeof(Camera))]
public class SpriteImageAdapter : MonoBehaviour {
 
    [System.Serializable]
    public class SpriteInfo
    {
        [Header("适配对象")]
        [Tooltip("SpriteRenderer组件（用于场景中的精灵）")]
        public SpriteRenderer SpriteRenderer = null;
        [Tooltip("Image组件（用于UI界面）")]
        public Image UIImage = null;
        
        [Header("适配模式")]
        public EFillModel Model = EFillModel.ShowAll;
        
        /// <summary>
        /// 获取当前使用的组件类型
        /// </summary>
        public bool IsUsingSpriteRenderer => SpriteRenderer != null;
        public bool IsUsingUIImage => UIImage != null;
    }
 
    public enum EFillModel
    {
        /// <summary>
        /// 显示图片的所有内容
        /// </summary>
        ShowAll,
        /// <summary>
        /// 使图片内容填满屏幕
        /// </summary>
        Full,
        /// <summary>
        /// 根据图片高度填充屏幕
        /// </summary>
        WithHeight,
        /// <summary>
        /// 根据图片宽度填充屏幕
        /// </summary>
        WithWidth,
        /// <summary>
        /// 拉伸图片以填充整个屏幕（不保持宽高比）
        /// </summary>
        Stretch
    }
 
    public enum EUpdateType
    {
        /// <summary>
        /// 只在唤醒时更新一次
        /// </summary>
        UpdateOnAwake,
        /// <summary>
        /// 再每次视口发生变化的时候更新一次
        /// </summary>
        UpdateOnViewportChanged
    }
 
    public EUpdateType TickType = EUpdateType.UpdateOnAwake;
    public SpriteInfo[] Members;
    Camera Viewport;
    float ScreenRatio;
 
    void Awake () {
        Viewport = GetComponent<Camera>();
        UpdateAll();
    }
 
    private void LateUpdate()
    {
        if (TickType != EUpdateType.UpdateOnViewportChanged) return;
        if (ScreenRatio != Viewport.aspect)
        {
            UpdateAll();
        }
    } 
 
    /// <summary>
    /// 更新所有应用屏幕适配的图片
    /// </summary>
    void UpdateAll()
    {
        for (int i = 0; i < Members.Length; i++)
        {
            if (Members[i].IsUsingSpriteRenderer)
            {
                AdaptSpriteRenderer(Members[i]);
            }
            else if (Members[i].IsUsingUIImage)
            {
                AdaptUIImage(Members[i]);
            }
        }
 
        ScreenRatio = Viewport.aspect;
    }
 
    /// <summary>
    /// 适配SpriteRenderer组件
    /// </summary>
    private void AdaptSpriteRenderer(SpriteInfo _spriteInfo)
    {
        SpriteRenderer spriteRenderer = _spriteInfo.SpriteRenderer;
        Vector3 scale = spriteRenderer.transform.localScale;
        float cameraheight = Viewport.orthographicSize * 2;
        float camerawidth = cameraheight * Viewport.aspect;
        float texr = (float)spriteRenderer.sprite.texture.width / spriteRenderer.sprite.texture.height;
        float viewr = camerawidth / cameraheight;
        
        switch (_spriteInfo.Model)
        {
            case EFillModel.WithHeight:
                //> 根据图片高度进行填充
                scale *= cameraheight / spriteRenderer.bounds.size.y;
                break;
            case EFillModel.WithWidth:
                //> 根据图片宽度进行填充
                scale *= camerawidth / spriteRenderer.bounds.size.x;
                break;
            case EFillModel.Full: 
                //> 填满整个屏幕
                if (viewr >= texr)
                {
                    if(viewr >= 1 && texr >= 1 || texr < 1)
                        scale *= camerawidth / spriteRenderer.bounds.size.x;
                    else
                        scale *= cameraheight / spriteRenderer.bounds.size.y;
                }
                else
                {
                    if (viewr <= 1 || texr > 1)
                        scale *= cameraheight / spriteRenderer.bounds.size.y; 
                    else
                        scale *= camerawidth / spriteRenderer.bounds.size.x;
                }
                break;
            case EFillModel.Stretch:
                //> 拉伸图片以填充整个屏幕（不保持宽高比）
                // 获取sprite的原始纹理尺寸（像素单位）
                float textureWidth = spriteRenderer.sprite.texture.width;
                float textureHeight = spriteRenderer.sprite.texture.height;
                
                // 获取sprite的像素单位转换为世界单位的比例
                float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;
                
                // 计算sprite在世界坐标系中的原始尺寸
                float spriteWorldWidth = textureWidth / pixelsPerUnit;
                float spriteWorldHeight = textureHeight / pixelsPerUnit;
                
                // 计算需要的缩放比例
                float scaleX = camerawidth / spriteWorldWidth;
                float scaleY = cameraheight / spriteWorldHeight;
                
                // 设置缩放
                scale.x = scaleX;
                scale.y = scaleY;
                break;
            default:
                //> 在屏幕上显示图片的全部内容
                if (viewr >= texr)
                {
                    scale *= cameraheight / spriteRenderer.bounds.size.y;
                }
                else
                {
                    scale *= camerawidth / spriteRenderer.bounds.size.x;
                }
                break;
        }
        spriteRenderer.transform.localScale = scale; 
    }
    
    /// <summary>
    /// 适配UI Image组件
    /// </summary>
    private void AdaptUIImage(SpriteInfo _spriteInfo)
    {
        Image uiImage = _spriteInfo.UIImage;
        RectTransform rectTransform = uiImage.GetComponent<RectTransform>();
        
        // 获取屏幕尺寸
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float screenRatio = screenWidth / screenHeight;
        
        // 获取图片原始尺寸
        Sprite sprite = uiImage.sprite;
        if (sprite == null) return;
        
        float imageWidth = sprite.texture.width;
        float imageHeight = sprite.texture.height;
        float imageRatio = imageWidth / imageHeight;
        
        Vector2 sizeDelta = rectTransform.sizeDelta;
        
        switch (_spriteInfo.Model)
        {
            case EFillModel.WithHeight:
                //> 根据图片高度进行填充
                sizeDelta.y = screenHeight;
                sizeDelta.x = screenHeight * imageRatio;
                break;
            case EFillModel.WithWidth:
                //> 根据图片宽度进行填充
                sizeDelta.x = screenWidth;
                sizeDelta.y = screenWidth / imageRatio;
                break;
            case EFillModel.Full:
                //> 填满整个屏幕
                if (screenRatio >= imageRatio)
                {
                    // 屏幕更宽，按宽度填充
                    sizeDelta.x = screenWidth;
                    sizeDelta.y = screenWidth / imageRatio;
                }
                else
                {
                    // 屏幕更高，按高度填充
                    sizeDelta.y = screenHeight;
                    sizeDelta.x = screenHeight * imageRatio;
                }
                break;
            case EFillModel.Stretch:
                //> 拉伸图片以填充整个屏幕（不保持宽高比）
                // 设置锚点为填满整个父容器
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                
                // 清除所有偏移，让图片完全填满锚点区域
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                
                // 将sizeDelta设置为零，这样图片会完全填满锚点定义的区域
                sizeDelta = Vector2.zero;
                break;
            default: // ShowAll
                //> 在屏幕上显示图片的全部内容
                if (screenRatio >= imageRatio)
                {
                    // 屏幕更宽，按高度适配
                    sizeDelta.y = screenHeight;
                    sizeDelta.x = screenHeight * imageRatio;
                }
                else
                {
                    // 屏幕更高，按宽度适配
                    sizeDelta.x = screenWidth;
                    sizeDelta.y = screenWidth / imageRatio;
                }
                break;
        }
        
        rectTransform.sizeDelta = sizeDelta;
    }
}