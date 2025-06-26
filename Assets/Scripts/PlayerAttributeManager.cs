using UnityEngine;
using static ES3;

public class PlayerAttributeManager : MonoBehaviour
{
    public static PlayerAttributeManager instance;
    
    [Header("玩家组件引用")]
    [SerializeField] private Player _playerComponent;
    public Player PlayerComponent => _playerComponent;
    
    void Awake()
    {
        instance = this;
        
        // 查找并获取Player组件
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        _playerComponent = playerObject.GetComponent<Player>();

        // 游戏开始时从存档加载并应用属性
        LoadAndApplyPlayerAttributes();
    }
    
    /// <summary>
    /// 从ES3存档中读取数据并应用到玩家属性
    /// </summary>
    public void LoadAndApplyPlayerAttributes()
    {
        try
        {
            // 读取最大生命值 (UpgradeableAttributes.Attribute_0.CurrentUpgradeValue)
            float maxHealthValue = Load("UpgradeableAttributes.Attribute_0.CurrentUpgradeValue", 100f);
            
            // 读取恢复值 (UpgradeableAttributes.Attribute_1.CurrentUpgradeValue)
            float recoveryValue = Load("UpgradeableAttributes.Attribute_1.CurrentUpgradeValue", 0f);
            
            // 读取护甲值 (UpgradeableAttributes.Attribute_2.CurrentUpgradeValue)
            float armorValue = Load("UpgradeableAttributes.Attribute_2.CurrentUpgradeValue", 0f);
            
            // 读取力量因子 (UpgradeableAttributes.Attribute_3.CurrentUpgradeValue)
            float powerFactorValue = Load("UpgradeableAttributes.Attribute_3.CurrentUpgradeValue", 0f);
            
            // 读取移动速度 (UpgradeableAttributes.Attribute_4.CurrentUpgradeValue)
            float moveSpeedValue = Load("UpgradeableAttributes.Attribute_4.CurrentUpgradeValue", 0f);
            
            // 读取吸取范围因子 (UpgradeableAttributes.Attribute_5.CurrentUpgradeValue)
            float magnetAreaFactorValue = Load("UpgradeableAttributes.Attribute_5.CurrentUpgradeValue", 0f);
            
            // 读取幸运增量 (UpgradeableAttributes.Attribute_6.CurrentUpgradeValue)
            float luckIncrementValue = Load("UpgradeableAttributes.Attribute_6.CurrentUpgradeValue", 0f);
            
            // 应用到Player组件
            _playerComponent.MaxHealth = maxHealthValue;
            _playerComponent.Recovery = recoveryValue;
            _playerComponent.Armor = armorValue;
            _playerComponent.PowerFactor = powerFactorValue;
            _playerComponent.MoveSpeed = moveSpeedValue;
            _playerComponent.MagnetAreaFactor = magnetAreaFactorValue;
            _playerComponent.LuckIncrement = luckIncrementValue;
            
            Debug.Log($"- 最大生命值: {_playerComponent.MaxHealth}");
            Debug.Log($"- 恢复值: {_playerComponent.Recovery}");
            Debug.Log($"- 护甲值: {_playerComponent.Armor}");
            Debug.Log($"- 力量因子: {_playerComponent.PowerFactor}");
            Debug.Log($"- 移动速度: {_playerComponent.MoveSpeed}");
            Debug.Log($"- 吸取范围因子: {_playerComponent.MagnetAreaFactor}");
            Debug.Log($"- 幸运增量: {_playerComponent.LuckIncrement}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载玩家属性时出现错误: {e.Message}");
            Debug.LogWarning("将使用默认属性值");
        }
    }
} 