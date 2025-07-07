using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 本地化系统测试脚本
/// 用于验证多语言功能是否正常工作
/// </summary>
public class LocalizationTest : MonoBehaviour
{
    private TMP_FontAsset _chineseFontAsset;

    [Header("测试UI组件")]
    [SerializeField] private TMP_Text _testText1;
    [SerializeField] private TMP_Text _testText2;
    [SerializeField] private TMP_Text _testText3;
    [SerializeField] private TMP_Text _currentLanguageText;

    [Header("测试按钮")]
    [SerializeField] private Button _chineseButton;
    [SerializeField] private Button _englishButton;
    [SerializeField] private Button _testButton;

    [Header("测试参数")]
    [SerializeField] private int _testLevel = 5;
    [SerializeField] private int _testCoins = 100;
    [SerializeField] private string _testWeaponName = "大剑";

    void Awake()
    {
        // 直接写死加载字体
        _chineseFontAsset = Resources.Load<TMP_FontAsset>("Fonts/Uranus_Pixel_11Px SDF");
        if (_chineseFontAsset == null)
        {
            Debug.LogError("未找到中文TMP字体: Assets/Fonts/Uranus_Pixel_11Px SDF.asset");
        }
    }

    void Start()
    {
        SetupTestUI();
        SetupButtons();
        UpdateTestDisplay();
    }

    /// <summary>
    /// 设置测试UI
    /// </summary>
    private void SetupTestUI()
    {
        // 创建测试UI（如果不存在）
        if (_testText1 == null)
        {
            CreateTestUI();
        }
    }

    /// <summary>
    /// 创建测试UI
    /// </summary>
    private void CreateTestUI()
    {
        // 创建Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("TestCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 创建测试面板
        GameObject testPanel = new GameObject("TestPanel");
        testPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = testPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 0.5f);
        panelRect.anchorMax = new Vector2(1, 1);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // 添加背景
        Image panelImage = testPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);

        // 创建标题
        CreateText(testPanel, "Localization Test", new Vector2(0, 0.4f), 24, Color.white);

        // 创建测试文本1
        _testText1 = CreateText(testPanel, "", new Vector2(0, 0.2f), 18, Color.white);

        // 创建测试文本2
        _testText2 = CreateText(testPanel, "", new Vector2(0, 0.1f), 18, Color.white);

        // 创建测试文本3
        _testText3 = CreateText(testPanel, "", new Vector2(0, 0), 18, Color.white);

        // 创建当前语言显示
        _currentLanguageText = CreateText(testPanel, "", new Vector2(0, -0.1f), 16, Color.yellow);

        // 创建按钮
        CreateButtons(testPanel);
    }

    /// <summary>
    /// 创建文本组件
    /// </summary>
    private TMP_Text CreateText(GameObject parent, string text, Vector2 position, int fontSize, Color color)
    {
        GameObject textObj = new GameObject("TestText");
        textObj.transform.SetParent(parent.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = new Vector2(position.x * 400, position.y * 200);
        textRect.sizeDelta = new Vector2(400, 50);

        TMP_Text tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = fontSize;
        tmpText.color = color;
        tmpText.alignment = TextAlignmentOptions.Center;
        if (_chineseFontAsset != null)
            tmpText.font = _chineseFontAsset;
        return tmpText;
    }

    /// <summary>
    /// 创建按钮
    /// </summary>
    private void CreateButtons(GameObject parent)
    {
        // 中文按钮
        _chineseButton = CreateButton(parent, "中文", new Vector2(-0.3f, -0.3f), Color.green);

        // 英文按钮
        _englishButton = CreateButton(parent, "English", new Vector2(0, -0.3f), Color.blue);

        // 测试按钮
        _testButton = CreateButton(parent, "Test", new Vector2(0.3f, -0.3f), Color.red);
    }

    /// <summary>
    /// 创建按钮
    /// </summary>
    private Button CreateButton(GameObject parent, string text, Vector2 position, Color color)
    {
        GameObject buttonObj = new GameObject("Button");
        buttonObj.transform.SetParent(parent.transform, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(position.x * 400, position.y * 200);
        buttonRect.sizeDelta = new Vector2(120, 40);

        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = color;

        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonImage;

        // 创建按钮文本
        GameObject textObj = new GameObject("ButtonText");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TMP_Text buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 16;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        if (_chineseFontAsset != null)
            buttonText.font = _chineseFontAsset;
        return button;
    }

    /// <summary>
    /// 设置按钮事件
    /// </summary>
    private void SetupButtons()
    {
        if (_chineseButton != null)
        {
            _chineseButton.onClick.AddListener(() =>
            {
                LocalizationManager.Instance.SwitchLanguage("zh-CN");
                UpdateTestDisplay();
                Debug.Log("切换到中文");
            });
        }

        if (_englishButton != null)
        {
            _englishButton.onClick.AddListener(() =>
            {
                LocalizationManager.Instance.SwitchLanguage("en-US");
                UpdateTestDisplay();
                Debug.Log("切换到英文");
            });
        }

        if (_testButton != null)
        {
            _testButton.onClick.AddListener(() =>
            {
                TestLocalization();
            });
        }
    }

    /// <summary>
    /// 更新测试显示
    /// </summary>
    private void UpdateTestDisplay()
    {
        if (LocalizationManager.Instance == null)
        {
            Debug.LogError("LocalizationManager 不存在！");
            return;
        }

        // 测试文本1: 等级显示
        if (_testText1 != null)
        {
            _testText1.text = LocalizationManager.Instance.GetText("level", _testLevel);
        }

        // 测试文本2: 金币显示
        if (_testText2 != null)
        {
            _testText2.text = LocalizationManager.Instance.GetText("coins", _testCoins);
        }

        // 测试文本3: 武器等级格式
        if (_testText3 != null)
        {
            _testText3.text = LocalizationManager.Instance.GetText("weapon_level_format", _testWeaponName, 3, 5);
        }

        // 当前语言显示
        if (_currentLanguageText != null)
        {
            string currentLang = LocalizationManager.Instance.CurrentLanguage;
            string displayName = currentLang == "zh-CN" ? "中文" : "English";
            _currentLanguageText.text = $"当前语言: {displayName} ({currentLang})";
        }
    }

    /// <summary>
    /// 测试本地化功能
    /// </summary>
    private void TestLocalization()
    {
        Debug.Log("=== 本地化系统测试 ===");

        if (LocalizationManager.Instance == null)
        {
            Debug.LogError("LocalizationManager 不存在！");
            return;
        }

        // 测试基本文本
        Debug.Log($"等级文本: {LocalizationManager.Instance.GetText("level", 10)}");
        Debug.Log($"金币文本: {LocalizationManager.Instance.GetText("coins", 500)}");
        Debug.Log($"生存时间: {LocalizationManager.Instance.GetText("survival_time", "05:30")}");

        // 测试游戏模式
        Debug.Log($"普通模式: {LocalizationManager.Instance.GetText("normal_mode")}");
        Debug.Log($"神力模式: {LocalizationManager.Instance.GetText("power_mode")}");

        // 测试升级系统
        Debug.Log($"选择升级: {LocalizationManager.Instance.GetText("select_upgrade")}");
        Debug.Log($"当前等级: {LocalizationManager.Instance.GetText("current_level", 3, 5)}");
        Debug.Log($"已满级: {LocalizationManager.Instance.GetText("max_level_reached")}");

        // 测试人物属性
        Debug.Log($"最大生命值: {LocalizationManager.Instance.GetText("max_health", 100.5f)}");
        Debug.Log($"护甲值: {LocalizationManager.Instance.GetText("armor", 15.0f)}");
        Debug.Log($"移动速度: {LocalizationManager.Instance.GetText("move_speed", 8.5f)}");

        // 测试武器等级格式
        Debug.Log($"武器等级: {LocalizationManager.Instance.GetText("weapon_level_format", "大剑", 3, 5)}");
        Debug.Log($"饰品等级: {LocalizationManager.Instance.GetText("accessory_level_format", "护甲", 2, 4)}");

        Debug.Log("=== 测试完成 ===");
    }

    void Update()
    {
        // 按T键进行测试
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestLocalization();
        }

        // 按C键切换到中文
        if (Input.GetKeyDown(KeyCode.C))
        {
            LocalizationManager.Instance.SwitchLanguage("zh-CN");
            UpdateTestDisplay();
        }

        // 按E键切换到英文
        if (Input.GetKeyDown(KeyCode.E))
        {
            LocalizationManager.Instance.SwitchLanguage("en-US");
            UpdateTestDisplay();
        }
    }
}