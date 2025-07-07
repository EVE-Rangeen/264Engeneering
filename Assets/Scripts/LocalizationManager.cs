using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 本地化管理器
/// 负责加载和管理多语言文本
/// 2025-07-05杜宜峰
/// </summary>
public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    [Header("本地化设置")]
    [SerializeField] private string _defaultLanguage = "zh-CN";
    [SerializeField] private string _currentLanguage = "zh-CN";

    private Dictionary<string, string> _localizedTexts = new Dictionary<string, string>();
    private List<LocalizedText> _localizedTextComponents = new List<LocalizedText>();

    /// <summary>
    /// 当前语言
    /// </summary>
    public string CurrentLanguage => _currentLanguage;

    /// <summary>
    /// 支持的语言列表
    /// </summary>
    public string[] SupportedLanguages => new string[] { "zh-CN", "en-US" };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLocalization();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初始化本地化系统
    /// </summary>
    private void InitializeLocalization()
    {
        // 从PlayerPrefs加载上次选择的语言，如果没有则使用默认语言
        _currentLanguage = PlayerPrefs.GetString("SelectedLanguage", _defaultLanguage);
        LoadLanguage(_currentLanguage);
    }

    /// <summary>
    /// 加载指定语言的文本
    /// </summary>
    /// <param name="language">语言代码</param>
    public void LoadLanguage(string language)
    {
        _currentLanguage = language;
        PlayerPrefs.SetString("SelectedLanguage", language);
        PlayerPrefs.Save();

        // 清空当前文本字典
        _localizedTexts.Clear();

        // 加载语言文件
        TextAsset languageFile = Resources.Load<TextAsset>($"Localization/{language}");

        if (languageFile == null)
        {
            Debug.LogError($"找不到语言文件: Localization/{language}");
            // 如果找不到指定语言文件，尝试加载默认语言
            if (language != _defaultLanguage)
            {
                LoadLanguage(_defaultLanguage);
                return;
            }
            return;
        }

        // 解析JSON文件
        try
        {
            LocalizationData data = JsonUtility.FromJson<LocalizationData>(languageFile.text);
            foreach (var item in data.texts)
            {
                _localizedTexts[item.key] = item.value;
            }
            Debug.Log($"成功加载语言: {language}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"解析语言文件失败: {e.Message}");
            return;
        }

        // 刷新所有本地化文本组件
        RefreshAllLocalizedTexts();
    }

    /// <summary>
    /// 获取本地化文本
    /// </summary>
    /// <param name="key">文本键</param>
    /// <returns>本地化文本</returns>
    public string GetText(string key)
    {
        if (_localizedTexts.TryGetValue(key, out string value))
        {
            return value;
        }

        Debug.LogWarning($"找不到本地化文本: {key}");
        return key; // 返回键名作为后备
    }

    /// <summary>
    /// 获取带参数的本地化文本
    /// </summary>
    /// <param name="key">文本键</param>
    /// <param name="parameters">参数数组</param>
    /// <returns>格式化后的本地化文本</returns>
    public string GetText(string key, params object[] parameters)
    {
        string text = GetText(key);
        return string.Format(text, parameters);
    }

    /// <summary>
    /// 注册本地化文本组件
    /// </summary>
    /// <param name="localizedText">本地化文本组件</param>
    public void RegisterLocalizedText(LocalizedText localizedText)
    {
        if (!_localizedTextComponents.Contains(localizedText))
        {
            _localizedTextComponents.Add(localizedText);
        }
    }

    /// <summary>
    /// 注销本地化文本组件
    /// </summary>
    /// <param name="localizedText">本地化文本组件</param>
    public void UnregisterLocalizedText(LocalizedText localizedText)
    {
        _localizedTextComponents.Remove(localizedText);
    }

    /// <summary>
    /// 刷新所有本地化文本组件
    /// </summary>
    public void RefreshAllLocalizedTexts()
    {
        foreach (var localizedText in _localizedTextComponents)
        {
            if (localizedText != null)
            {
                localizedText.Refresh();
            }
        }
    }

    /// <summary>
    /// 切换语言
    /// </summary>
    /// <param name="language">目标语言</param>
    public void SwitchLanguage(string language)
    {
        if (language != _currentLanguage)
        {
            LoadLanguage(language);
        }
    }
}

/// <summary>
/// 本地化数据结构
/// </summary>
[System.Serializable]
public class LocalizationData
{
    public LocalizationItem[] texts;
}

/// <summary>
/// 本地化项目
/// </summary>
[System.Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
}