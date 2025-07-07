using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 语言选择器（单按钮国旗切换模式）
/// 2025-07-07杜宜峰
/// </summary>
public class LanguageSelector : MonoBehaviour
{
    [Header("国旗切换UI")]
    [SerializeField] private Button _flagButton;
    [SerializeField] private Image _flagImage;
    [SerializeField] private Sprite _ukFlag;
    [SerializeField] private Sprite _chinaFlag;
    [SerializeField] private TMP_Text _currentLanguageText;

    [Header("语言显示名称")]
    [SerializeField] private string _chineseDisplayName = "中文";
    [SerializeField] private string _englishDisplayName = "English";

    void Start()
    {
        if (_flagButton != null)
        {
            _flagButton.onClick.AddListener(OnFlagButtonClick);
        }
        UpdateFlagUI();
    }

    /// <summary>
    /// 点击国旗按钮切换语言
    /// </summary>
    private void OnFlagButtonClick()
    {
        if (LocalizationManager.Instance == null) return;
        string nextLang = LocalizationManager.Instance.CurrentLanguage == "zh-CN" ? "en-US" : "zh-CN";
        SwitchLanguage(nextLang);
        UpdateFlagUI();
    }

    /// <summary>
    /// 切换语言
    /// </summary>
    /// <param name="language">目标语言</param>
    public void SwitchLanguage(string language)
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SwitchLanguage(language);
            UpdateFlagUI();
            Debug.Log($"语言已切换到: {language}");
        }
        else
        {
            Debug.LogError("LocalizationManager 实例不存在");
        }
    }

    /// <summary>
    /// 更新国旗和当前语言显示
    /// </summary>
    private void UpdateFlagUI()
    {
        if (LocalizationManager.Instance == null) return;
        string currentLang = LocalizationManager.Instance.CurrentLanguage;
        if (_flagImage != null)
        {
            _flagImage.sprite = currentLang == "zh-CN" ? _ukFlag : _chinaFlag;
        }
        if (_currentLanguageText != null)
        {
            _currentLanguageText.text = $"{GetLanguageDisplayName(currentLang)}";
        }
    }

    /// <summary>
    /// 获取语言显示名称
    /// </summary>
    /// <param name="languageCode">语言代码</param>
    /// <returns>显示名称</returns>
    private string GetLanguageDisplayName(string languageCode)
    {
        switch (languageCode)
        {
            case "zh-CN":
                return _chineseDisplayName;
            case "en-US":
                return _englishDisplayName;
            default:
                return languageCode;
        }
    }
}