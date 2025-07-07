using UnityEngine;
using TMPro;

/// <summary>
/// 本地化文本组件
/// 自动根据当前语言更新UI文本
/// 2025-07-05杜宜峰
/// </summary>
public class LocalizedText : MonoBehaviour
{
    private static TMP_FontAsset _chineseFontAsset;

    [Header("本地化设置")]
    [SerializeField] private string _localizationKey = "";
    [SerializeField] private bool _useParameters = false;
    [SerializeField] private string[] _parameters = new string[0];

    private TMP_Text _tmpText;
    private UnityEngine.UI.Text _uiText;

    /// <summary>
    /// 本地化键
    /// </summary>
    public string LocalizationKey
    {
        get => _localizationKey;
        set
        {
            _localizationKey = value;
            Refresh();
        }
    }

    void Awake()
    {
        // 获取字体（静态缓存）
        if (_chineseFontAsset == null)
        {
            _chineseFontAsset = Resources.Load<TMP_FontAsset>("Fonts/Uranus_Pixel_11Px SDF");
            if (_chineseFontAsset == null)
            {
                Debug.LogError("未找到中文TMP字体: Assets/Fonts/Uranus_Pixel_11Px SDF.asset");
            }
        }
        // 获取文本组件
        _tmpText = GetComponent<TMP_Text>();
        _uiText = GetComponent<UnityEngine.UI.Text>();
        if (_tmpText == null && _uiText == null)
        {
            Debug.LogError($"LocalizedText 组件需要 Text 或 TMP_Text 组件: {gameObject.name}");
        }
        // 强制指定字体
        if (_tmpText != null && _chineseFontAsset != null)
        {
            _tmpText.font = _chineseFontAsset;
        }
    }

    void Start()
    {
        // 注册到本地化管理器
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.RegisterLocalizedText(this);
            Refresh();
        }
        else
        {
            Debug.LogWarning("LocalizationManager 实例不存在，延迟注册");
            Invoke(nameof(DelayedRegister), 0.1f);
        }
    }

    void OnDestroy()
    {
        // 注销本地化管理器
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.UnregisterLocalizedText(this);
        }
    }

    /// <summary>
    /// 延迟注册
    /// </summary>
    private void DelayedRegister()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.RegisterLocalizedText(this);
            Refresh();
        }
    }

    /// <summary>
    /// 刷新文本
    /// </summary>
    public void Refresh()
    {
        if (string.IsNullOrEmpty(_localizationKey))
        {
            return;
        }

        string localizedText;

        if (_useParameters && _parameters.Length > 0)
        {
            // 使用参数格式化文本
            localizedText = LocalizationManager.Instance.GetText(_localizationKey, _parameters);
        }
        else
        {
            // 直接获取文本
            localizedText = LocalizationManager.Instance.GetText(_localizationKey);
        }

        // 更新文本组件
        if (_tmpText != null)
        {
            _tmpText.text = localizedText;
        }
        else if (_uiText != null)
        {
            _uiText.text = localizedText;
        }
    }

    /// <summary>
    /// 设置参数并刷新
    /// </summary>
    /// <param name="parameters">参数数组</param>
    public void SetParameters(params string[] parameters)
    {
        _parameters = parameters;
        _useParameters = true;
        Refresh();
    }

    /// <summary>
    /// 设置参数并刷新（使用object数组）
    /// </summary>
    /// <param name="parameters">参数数组</param>
    public void SetParameters(params object[] parameters)
    {
        _parameters = new string[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            _parameters[i] = parameters[i]?.ToString() ?? "";
        }
        _useParameters = true;
        Refresh();
    }

    /// <summary>
    /// 清除参数
    /// </summary>
    public void ClearParameters()
    {
        _parameters = new string[0];
        _useParameters = false;
        Refresh();
    }
}