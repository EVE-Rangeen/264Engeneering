using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人管理器 - 负责敌人的生成、管理和销毁
/// 2025-06-25 肖沐奇 创建
/// </summary>
public class EnemyManager : MonoBehaviour
{
    private static EnemyManager _instance;
    public static EnemyManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [Header("玩家引用")]
    [SerializeField] private Transform _player;
    [SerializeField] private Camera _mainCamera;

    [Header("生成区域设置")]
    [SerializeField] private float _spawnDistanceMin = 15f; // 最小生成距离
    [SerializeField] private float _spawnDistanceMax = 20f; // 最大生成距离
    [SerializeField] private float _despawnDistance = 25f; // 销毁距离

    [Header("波次刷新设置")]
    [SerializeField] private List<WaveData> _waveList = new List<WaveData>();

    [Header("定时刷新设置")]
    [SerializeField] private List<TimedSpawnData> _timedSpawnList = new List<TimedSpawnData>();

    [Header("特殊敌人设置")]
    [SerializeField] private List<SpecialEnemyData> _specialEnemyList = new List<SpecialEnemyData>();

    [Header("调试设置")]
    [SerializeField] private bool _enableDebugGizmos = true;
    [SerializeField] private bool _enableDebugLogs = false;

    private List<GameObject> _activeEnemies = new List<GameObject>();
    private float _lastWaveSpawnTime;
    private float _gameStartTime;
    private List<float> _timedSpawnExecutedTimes = new List<float>();
    private List<float> _specialEnemyExecutedTimes = new List<float>();
    private int _currentWaveIndex = 0;
    private float _currentWaveStartTime;
    private bool _waveActive = false;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    void Start()
    {
        InitializeEnemyManager();
    }

    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    void Update()
    {
        HandleWaveSystem();
        HandleTimedSpawn();
        HandleSpecialEnemySpawn();
        CleanupDistantEnemies();
    }

    /// <summary>
    /// 初始化敌人管理器
    /// </summary>
    private void InitializeEnemyManager()
    {
        _gameStartTime = Time.time;

        // 自动查找玩家
        if (_player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _player = playerObject.transform;
            }
            else
            {
                Debug.LogError("EnemyManager: 未找到玩家对象！请确保玩家有'Player'标签。");
            }
        }

        // 自动查找主相机
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        // 初始化执行时间列表
        _timedSpawnExecutedTimes.Clear();
        _specialEnemyExecutedTimes.Clear();

        for (int i = 0; i < _timedSpawnList.Count; i++)
        {
            _timedSpawnExecutedTimes.Add(-1f);
        }

        for (int i = 0; i < _specialEnemyList.Count; i++)
        {
            _specialEnemyExecutedTimes.Add(-1f);
        }

        if (_enableDebugLogs)
        {
            Debug.Log("EnemyManager 初始化完成");
        }

        // 开始第一个波次
        StartNextWave();
    }

    /// <summary>
    /// 处理波次系统
    /// </summary>
    private void HandleWaveSystem()
    {
        if (_player == null || _waveList.Count == 0) return;

        // 检查当前波次是否结束
        if (_waveActive && Time.time - _currentWaveStartTime >= GetCurrentWave().duration)
        {
            EndCurrentWave();
            StartNextWave();
            return;
        }

        // 在波次活跃期间处理刷新
        if (_waveActive)
        {
            HandleWaveSpawn();
        }
    }

    /// <summary>
    /// 开始下一个波次
    /// </summary>
    private void StartNextWave()
    {
        if (_currentWaveIndex >= _waveList.Count)
        {
            // 所有波次已完成，可以循环或停止
            if (_enableDebugLogs)
            {
                Debug.Log("所有波次已完成");
            }
            _currentWaveIndex = 0; // 循环播放波次
        }

        _currentWaveStartTime = Time.time;
        _waveActive = true;
        _lastWaveSpawnTime = Time.time;

        if (_enableDebugLogs)
        {
            Debug.Log($"开始波次 {_currentWaveIndex + 1}");
        }
    }

    /// <summary>
    /// 结束当前波次
    /// </summary>
    private void EndCurrentWave()
    {
        _waveActive = false;

        if (_enableDebugLogs)
        {
            Debug.Log($"波次 {_currentWaveIndex + 1} 结束");
        }

        _currentWaveIndex++;
    }

    /// <summary>
    /// 获取当前波次数据
    /// </summary>
    private WaveData GetCurrentWave()
    {
        if (_currentWaveIndex < _waveList.Count)
        {
            return _waveList[_currentWaveIndex];
        }
        return null;
    }

    /// <summary>
    /// 处理波次内的刷新
    /// </summary>
    private void HandleWaveSpawn()
    {
        WaveData currentWave = GetCurrentWave();
        if (currentWave == null || currentWave.enemySpawnDataList.Count == 0) return;

        // 按间隔正常刷新
        if (Time.time - _lastWaveSpawnTime >= currentWave.spawnInterval)
        {
            SpawnWaveEnemies(currentWave);
            _lastWaveSpawnTime = Time.time;
        }
    }

    /// <summary>
    /// 生成波次敌人
    /// </summary>
    private void SpawnWaveEnemies(WaveData waveData)
    {
        if (waveData.enemySpawnDataList.Count == 0) return;

        // 计算总权重
        float totalWeight = 0f;
        foreach (var spawnData in waveData.enemySpawnDataList)
        {
            totalWeight += spawnData.spawnWeight;
        }

        if (totalWeight <= 0f)
        {
            Debug.LogWarning("波次中所有敌人的权重总和为0！");
            return;
        }

        // 随机选择要生成的敌人类型
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var spawnData in waveData.enemySpawnDataList)
        {
            currentWeight += spawnData.spawnWeight;
            if (randomValue <= currentWeight)
            {
                // 生成这种类型的敌人
                int spawnCount = Random.Range(spawnData.minSpawnCount, spawnData.maxSpawnCount + 1);

                for (int i = 0; i < spawnCount; i++)
                {
                    Vector3 spawnPosition = GetRandomSpawnPosition();
                    SpawnEnemy(spawnData.enemyPrefab, spawnPosition);
                }
                break;
            }
        }
    }

    /// <summary>
    /// 处理定时刷新
    /// </summary>
    private void HandleTimedSpawn()
    {
        if (_player == null) return;

        float currentGameTime = Time.time - _gameStartTime;

        for (int i = 0; i < _timedSpawnList.Count; i++)
        {
            var timedSpawn = _timedSpawnList[i];

            // 检查是否已经执行过
            if (_timedSpawnExecutedTimes[i] >= 0) continue;

            // 检查是否到达执行时间
            if (currentGameTime >= timedSpawn.triggerTime)
            {
                ExecuteTimedSpawn(timedSpawn);
                _timedSpawnExecutedTimes[i] = currentGameTime;

                if (_enableDebugLogs)
                {
                    Debug.Log($"执行定时刷新: {timedSpawn.enemyPrefab.name} x{timedSpawn.spawnCount}");
                }
            }
        }
    }

    /// <summary>
    /// 执行定时刷新
    /// </summary>
    private void ExecuteTimedSpawn(TimedSpawnData timedSpawn)
    {
        for (int i = 0; i < timedSpawn.spawnCount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            SpawnEnemy(timedSpawn.enemyPrefab, spawnPosition);
        }
    }

    /// <summary>
    /// 处理特殊敌人刷新
    /// </summary>
    private void HandleSpecialEnemySpawn()
    {
        if (_player == null) return;

        float currentGameTime = Time.time - _gameStartTime;

        for (int i = 0; i < _specialEnemyList.Count; i++)
        {
            var specialEnemy = _specialEnemyList[i];

            // 检查是否已经执行过
            if (_specialEnemyExecutedTimes[i] >= 0) continue;

            // 检查是否到达执行时间
            if (currentGameTime >= specialEnemy.triggerTime)
            {
                ExecuteSpecialEnemySpawn(specialEnemy);
                _specialEnemyExecutedTimes[i] = currentGameTime;

                if (_enableDebugLogs)
                {
                    Debug.Log($"生成特殊敌人: {specialEnemy.enemyPrefab.name}");
                }
            }
        }
    }

    /// <summary>
    /// 执行特殊敌人刷新
    /// </summary>
    private void ExecuteSpecialEnemySpawn(SpecialEnemyData specialEnemy)
    {
        Vector3 spawnPosition;

        switch (specialEnemy.spawnType)
        {
            case SpecialSpawnType.RandomAroundPlayer:
                spawnPosition = GetRandomSpawnPosition();
                break;
            case SpecialSpawnType.FixedPosition:
                spawnPosition = specialEnemy.fixedPosition;
                break;
            case SpecialSpawnType.PlayerPosition:
                spawnPosition = _player.position + specialEnemy.offsetFromPlayer;
                break;
            default:
                spawnPosition = GetRandomSpawnPosition();
                break;
        }

        GameObject spawnedEnemy = SpawnEnemy(specialEnemy.enemyPrefab, spawnPosition);

        // 为特殊敌人添加标记
        if (spawnedEnemy != null)
        {
            var specialTag = spawnedEnemy.AddComponent<SpecialEnemyTag>();
            specialTag.isSpecialEnemy = true;
        }
    }

    /// <summary>
    /// 生成敌人
    /// </summary>
    private GameObject SpawnEnemy(GameObject enemyPrefab, Vector3 position)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyManager: 敌人预制体为空！");
            return null;
        }

        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        _activeEnemies.Add(enemy);

        // 确保敌人有正确的标签
        if (!enemy.CompareTag("Enemy"))
        {
            enemy.tag = "Enemy";
        }

        return enemy;
    }

    /// <summary>
    /// 清理距离过远的敌人
    /// </summary>
    private void CleanupDistantEnemies()
    {
        if (_player == null) return;

        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            if (_activeEnemies[i] == null)
            {
                _activeEnemies.RemoveAt(i);
                continue;
            }

            float distance = Vector3.Distance(_player.position, _activeEnemies[i].transform.position);

            if (distance > _despawnDistance)
            {
                // 检查是否是特殊敌人
                var specialTag = _activeEnemies[i].GetComponent<SpecialEnemyTag>();
                if (specialTag != null && specialTag.isSpecialEnemy)
                {
                    // 特殊敌人不自动销毁
                    continue;
                }

                Destroy(_activeEnemies[i]);
                _activeEnemies.RemoveAt(i);

                if (_enableDebugLogs)
                {
                    Debug.Log("销毁距离过远的敌人");
                }
            }
        }
    }

    /// <summary>
    /// 获取随机生成位置（在视野范围外但不会太远）
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        if (_player == null) return Vector3.zero;

        Vector3 playerPosition = _player.position;

        // 生成一个随机方向
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        // 随机距离（在最小和最大生成距离之间）
        float distance = Random.Range(_spawnDistanceMin, _spawnDistanceMax);

        // 计算生成位置
        Vector3 spawnPosition = playerPosition + new Vector3(direction.x, direction.y, 0) * distance;

        return spawnPosition;
    }

    /// <summary>
    /// 手动生成敌人
    /// </summary>
    public GameObject ManualSpawnEnemy(GameObject enemyPrefab, Vector3 position)
    {
        return SpawnEnemy(enemyPrefab, position);
    }

    /// <summary>
    /// 清除所有敌人
    /// </summary>
    public void ClearAllEnemies()
    {
        foreach (var enemy in _activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        _activeEnemies.Clear();

        if (_enableDebugLogs)
        {
            Debug.Log("清除所有敌人");
        }
    }

    /// <summary>
    /// 获取当前敌人数量
    /// </summary>
    public int GetCurrentEnemyCount()
    {
        // 清理空引用
        _activeEnemies.RemoveAll(enemy => enemy == null);
        return _activeEnemies.Count;
    }

    /// <summary>
    /// 更新当前波次刷新间隔
    /// </summary>
    public void UpdateCurrentWaveSpawnInterval(float newSpawnInterval)
    {
        WaveData currentWave = GetCurrentWave();
        if (currentWave != null)
        {
            currentWave.spawnInterval = newSpawnInterval;
        }
    }

    /// <summary>
    /// 添加新的波次数据
    /// </summary>
    public void AddWaveData(WaveData waveData)
    {
        _waveList.Add(waveData);
    }

    /// <summary>
    /// 强制开始下一个波次
    /// </summary>
    public void ForceNextWave()
    {
        if (_waveActive)
        {
            EndCurrentWave();
        }
        StartNextWave();
    }



    /// <summary>
    /// 重置所有计时器
    /// </summary>
    public void ResetAllTimers()
    {
        _gameStartTime = Time.time;
        _lastWaveSpawnTime = 0f;

        for (int i = 0; i < _timedSpawnExecutedTimes.Count; i++)
        {
            _timedSpawnExecutedTimes[i] = -1f;
        }

        for (int i = 0; i < _specialEnemyExecutedTimes.Count; i++)
        {
            _specialEnemyExecutedTimes[i] = -1f;
        }
    }

    /// <summary>
    /// 在Scene视图中绘制调试信息
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (!_enableDebugGizmos || _player == null) return;

        Vector3 playerPosition = _player.position;

        // 绘制最小生成距离
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerPosition, _spawnDistanceMin);

        // 绘制最大生成距离
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerPosition, _spawnDistanceMax);

        // 绘制销毁距离
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerPosition, _despawnDistance);

        // 绘制当前活跃敌人位置
        Gizmos.color = Color.white;
        foreach (var enemy in _activeEnemies)
        {
            if (enemy != null)
            {
                Gizmos.DrawWireSphere(enemy.transform.position, 0.5f);
            }
        }
    }
}

/// <summary>
/// 波次数据
/// </summary>
[System.Serializable]
public class WaveData
{
    [Header("波次基础信息")]
    public float duration = 30f; // 波次持续时间
    public float spawnInterval = 2f; // 波次内刷新间隔
    public int minEnemyCount = 5; // 同屏最少敌人数量

    [Header("波次敌人配置")]
    public List<WaveEnemyData> enemySpawnDataList = new List<WaveEnemyData>();
}

/// <summary>
/// 波次敌人数据
/// </summary>
[System.Serializable]
public class WaveEnemyData
{
    [Header("基础设置")]
    public GameObject enemyPrefab;
    public float spawnWeight = 1f; // 刷新权重

    [Header("数量设置")]
    public int minSpawnCount = 1;
    public int maxSpawnCount = 3;
}

/// <summary>
/// 定时刷新数据
/// </summary>
[System.Serializable]
public class TimedSpawnData
{
    [Header("基础设置")]
    public GameObject enemyPrefab;
    public float triggerTime; // 触发时间（游戏开始后的秒数）
    public int spawnCount = 5; // 刷新数量
}

/// <summary>
/// 特殊敌人数据
/// </summary>
[System.Serializable]
public class SpecialEnemyData
{
    [Header("基础设置")]
    public GameObject enemyPrefab;
    public float triggerTime; // 触发时间

    [Header("生成位置设置")]
    public SpecialSpawnType spawnType = SpecialSpawnType.RandomAroundPlayer;
    public Vector3 fixedPosition = Vector3.zero; // 固定位置（当spawnType为FixedPosition时使用）
    public Vector3 offsetFromPlayer = Vector3.zero; // 相对玩家的偏移（当spawnType为PlayerPosition时使用）
}

/// <summary>
/// 特殊敌人生成类型
/// </summary>
public enum SpecialSpawnType
{
    RandomAroundPlayer, // 随机在玩家周围
    FixedPosition,      // 固定位置
    PlayerPosition      // 玩家位置+偏移
}

/// <summary>
/// 特殊敌人标记组件
/// </summary>
public class SpecialEnemyTag : MonoBehaviour
{
    public bool isSpecialEnemy = false;
}
