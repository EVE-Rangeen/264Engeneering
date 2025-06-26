using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// GameObject生成配置
/// </summary>
[System.Serializable]
public class ObjectSpawnConfig
{
    [Header("基础设置")]
    public GameObject prefab; // 预制体
    public string objectName; // 物体名称

    [Header("生成设置")]
    public int minCount = 1; // 每个chunk最小生成数量
    public int maxCount = 3; // 每个chunk最大生成数量
    public float spawnChance = 1f; // 生成概率 (0-1)

    [Header("位置设置")]
    public bool canSpawnOnSpecial = false; // 是否可以在特殊地块上生成
    public bool canSpawnOnNormal = true; // 是否可以在普通地块上生成
    public float minDistance = 1f; // 与其他物体的最小距离

    [Header("限制设置")]
    public bool hasGlobalLimit = false; // 是否有全局数量限制
    public int globalLimit = 100; // 全局数量限制
}

/// <summary>
/// 生成的物体信息
/// </summary>
[System.Serializable]
public class SpawnedObjectInfo
{
    public Vector3 position;
    public string configName;
    public GameObject gameObject;

    public SpawnedObjectInfo(Vector3 pos, string name, GameObject obj)
    {
        position = pos;
        configName = name;
        gameObject = obj;
    }
}

/// <summary>
/// 地图管理器 - 使用柏林噪声生成无限地图
/// 挂在在游戏关卡场景的Grid下
/// 2025-06-25 肖沐奇 创建
/// </summary>
public class MapManager : MonoBehaviour
{

    private static MapManager _instance;
    public static MapManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [Header("地图生成设置")]
    [SerializeField] private int _chunkSize = 16; // 地图块大小
    [SerializeField] private int _viewDistance = 3; // 视野距离（加载多少个地图块）
    [SerializeField] private float _updateInterval = 0.1f; // 更新间隔（秒）

    [Header("柏林噪声参数")]
    [SerializeField] private float _noiseScale = 0.1f; // 噪声缩放
    [SerializeField] private int _octaves = 4; // 噪声层数
    [SerializeField] private float _persistence = 0.5f; // 噪声持久性
    [SerializeField] private float _lacunarity = 2f; // 噪声间隙
    [SerializeField] private int _seed = 0; // 随机种子
    [SerializeField] private Vector2 _offset = Vector2.zero; // 噪声偏移

    [Header("地块类型")]
    [SerializeField] private TileBase _normalTile; // 普通瓦片
    [SerializeField] private TileBase _specialTile; // 特殊瓦片
    [SerializeField] private float _specialTileThreshold = 0.3f; // 特殊瓦片的阈值

    [Header("引用组件")]
    [SerializeField] private Transform _player; // 玩家对象
    [SerializeField] private Camera _camera; // 主相机

    [Header("GameObject生成设置")]
    [SerializeField] private ObjectSpawnConfig[] _spawnConfigs; // 生成配置数组
    [SerializeField] private Transform _objectParent; // 生成物体的父对象
    [SerializeField] private int _maxSpawnAttempts = 50; // 每个物体的最大生成尝试次数

    // 私有字段
    private Grid _grid;
    private Tilemap _tilemap;
    private TilemapRenderer _tilemapRenderer;
    private Dictionary<Vector2Int, bool> _generatedChunks = new Dictionary<Vector2Int, bool>();
    private Vector2Int _lastPlayerChunk = new Vector2Int(int.MaxValue, int.MaxValue);
    private float _lastUpdateTime;
    private System.Random _random;

    // 噪声偏移数组，用于打破对称性
    private Vector2[] _octaveOffsets;

    // GameObject生成管理
    private Dictionary<Vector2Int, List<SpawnedObjectInfo>> _chunkObjects = new Dictionary<Vector2Int, List<SpawnedObjectInfo>>();
    private Dictionary<string, int> _globalObjectCounts = new Dictionary<string, int>(); // 全局物体计数

    /// <summary>
    /// 初始化组件和随机数生成器
    /// </summary>
    void Awake()
    {
        // 单例模式实现
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        // 获取Grid组件引用
        _grid = GetComponent<Grid>();

        // 查找BaseTilemap子对象
        Transform baseTilemapTransform = transform.Find("BaseTilemap");
        if (baseTilemapTransform != null)
        {
            _tilemap = baseTilemapTransform.GetComponent<Tilemap>();
            _tilemapRenderer = baseTilemapTransform.GetComponent<TilemapRenderer>();
        }

        // 验证组件
        if (_grid == null)
        {
            Debug.LogError("MapManager: Grid组件未找到！");
        }
        if (_tilemap == null)
        {
            Debug.LogError("MapManager: BaseTilemap的Tilemap组件未找到！");
        }
        if (_tilemapRenderer == null)
        {
            Debug.LogError("MapManager: BaseTilemap的TilemapRenderer组件未找到！");
        }

        // 初始化随机数生成器
        _random = new System.Random(_seed);

        // 初始化噪声偏移数组
        InitializeNoiseOffsets();


    }

    /// <summary>
    /// 开始时的初始化
    /// </summary>
    void Start()
    {
        // 自动查找玩家和相机
        if (_player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _player = playerObject.transform;
            }
        }

        if (_camera == null)
        {
            _camera = Camera.main;
        }

        // 如果没有设置默认瓦片，尝试从资源中加载
        if (_normalTile == null)
        {
            _normalTile = Resources.Load<TileBase>("Tiles/Grass/Grass_0");
            if (_normalTile == null)
            {
                Debug.LogWarning("MapManager: 未设置普通瓦片，请在Inspector中设置NormalTile！");
            }
        }

        // 创建物体父对象
        if (_objectParent == null)
        {
            GameObject parentObject = new GameObject("Generated Objects");
            parentObject.transform.SetParent(transform);
            _objectParent = parentObject.transform;
        }

        // 初始化全局物体计数
        foreach (var config in _spawnConfigs)
        {
            if (config.hasGlobalLimit)
            {
                _globalObjectCounts[config.objectName] = 0;
            }
        }

        // 生成初始地图
        if (_player != null)
        {
            GenerateInitialMap();
        }
    }

    /// <summary>
    /// 更新地图生成
    /// </summary>
    void Update()
    {
        if (_player == null || _tilemap == null) return;

        // 限制更新频率
        if (Time.time - _lastUpdateTime < _updateInterval) return;
        _lastUpdateTime = Time.time;

        Vector2Int currentPlayerChunk = GetChunkFromWorldPosition(_player.position);

        // 如果玩家移动到新的地图块，更新地图
        if (currentPlayerChunk != _lastPlayerChunk)
        {
            UpdateMap(currentPlayerChunk);
            _lastPlayerChunk = currentPlayerChunk;
        }
    }

    /// <summary>
    /// 生成初始地图
    /// </summary>
    private void GenerateInitialMap()
    {
        Vector2Int playerChunk = GetChunkFromWorldPosition(_player.position);
        UpdateMap(playerChunk);
        _lastPlayerChunk = playerChunk;
    }

    /// <summary>
    /// 更新地图 - 生成新的地图块，移除远程的地图块
    /// </summary>
    /// <param name="centerChunk">中心地图块坐标</param>
    private void UpdateMap(Vector2Int centerChunk)
    {
        // 生成视野范围内的地图块
        for (int x = centerChunk.x - _viewDistance; x <= centerChunk.x + _viewDistance; x++)
        {
            for (int y = centerChunk.y - _viewDistance; y <= centerChunk.y + _viewDistance; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(x, y);
                if (!_generatedChunks.ContainsKey(chunkCoord))
                {
                    GenerateChunk(chunkCoord);
                    _generatedChunks[chunkCoord] = true;
                }
            }
        }

        // 移除距离过远的地图块
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (var chunk in _generatedChunks.Keys)
        {
            float distance = Vector2Int.Distance(chunk, centerChunk);
            if (distance > _viewDistance + 1) // 留出一定缓冲区
            {
                chunksToRemove.Add(chunk);
            }
        }

        foreach (var chunk in chunksToRemove)
        {
            RemoveChunk(chunk);
            _generatedChunks.Remove(chunk);
        }
    }

    /// <summary>
    /// 生成单个地图块
    /// </summary>
    /// <param name="chunkCoord">地图块坐标</param>
    private void GenerateChunk(Vector2Int chunkCoord)
    {
        Vector3Int startPosition = new Vector3Int(chunkCoord.x * _chunkSize, chunkCoord.y * _chunkSize, 0);

        for (int x = 0; x < _chunkSize; x++)
        {
            for (int y = 0; y < _chunkSize; y++)
            {
                Vector3Int tilePosition = new Vector3Int(startPosition.x + x, startPosition.y + y, 0);

                // 使用柏林噪声生成地形
                float noiseValue = GenerateNoiseValue(tilePosition.x, tilePosition.y);

                // 根据噪声值选择瓦片类型
                TileBase tileToPlace = GetTileFromNoiseValue(noiseValue);

                if (tileToPlace != null)
                {
                    _tilemap.SetTile(tilePosition, tileToPlace);
                }
            }
        }

        // 生成GameObject
        GenerateObjectsInChunk(chunkCoord);
    }

    /// <summary>
    /// 移除地图块
    /// </summary>
    /// <param name="chunkCoord">地图块坐标</param>
    private void RemoveChunk(Vector2Int chunkCoord)
    {
        Vector3Int startPosition = new Vector3Int(chunkCoord.x * _chunkSize, chunkCoord.y * _chunkSize, 0);
        BoundsInt area = new BoundsInt(startPosition.x, startPosition.y, 0, _chunkSize, _chunkSize, 1);

        // 清除该区域的所有瓦片
        _tilemap.SetTilesBlock(area, new TileBase[_chunkSize * _chunkSize]);

        // 移除该chunk中的所有GameObject
        RemoveObjectsInChunk(chunkCoord);
    }

    /// <summary>
    /// 在指定chunk中生成GameObject
    /// </summary>
    /// <param name="chunkCoord">chunk坐标</param>
    private void GenerateObjectsInChunk(Vector2Int chunkCoord)
    {
        if (_spawnConfigs == null || _spawnConfigs.Length == 0) return;

        // 检查是否已经生成过物体
        if (_chunkObjects.ContainsKey(chunkCoord))
        {
            // 重新激活已存在的物体
            RestoreObjectsInChunk(chunkCoord);
            return;
        }

        List<SpawnedObjectInfo> spawnedObjects = new List<SpawnedObjectInfo>();
        List<Vector3> occupiedPositions = new List<Vector3>();

        Vector3Int chunkStartPos = new Vector3Int(chunkCoord.x * _chunkSize, chunkCoord.y * _chunkSize, 0);

        foreach (var config in _spawnConfigs)
        {
            if (config.prefab == null) continue;

            // 检查全局限制
            if (config.hasGlobalLimit && _globalObjectCounts.ContainsKey(config.objectName))
            {
                if (_globalObjectCounts[config.objectName] >= config.globalLimit)
                {
                    continue; // 已达到全局限制，跳过
                }
            }

            // 检查生成概率
            if (_random.NextDouble() > config.spawnChance) continue;

            // 确定要生成的数量
            int spawnCount = _random.Next(config.minCount, config.maxCount + 1);

            for (int i = 0; i < spawnCount; i++)
            {
                // 检查全局限制（每次生成前检查）
                if (config.hasGlobalLimit && _globalObjectCounts.ContainsKey(config.objectName))
                {
                    if (_globalObjectCounts[config.objectName] >= config.globalLimit)
                    {
                        break; // 已达到全局限制，停止生成
                    }
                }

                Vector3 spawnPosition = FindValidSpawnPosition(chunkStartPos, config, occupiedPositions);

                if (spawnPosition != Vector3.zero)
                {
                    GameObject spawnedObject = Instantiate(config.prefab, spawnPosition, Quaternion.identity, _objectParent);
                    spawnedObject.name = $"{config.objectName}_{chunkCoord.x}_{chunkCoord.y}_{i}";

                    SpawnedObjectInfo objectInfo = new SpawnedObjectInfo(spawnPosition, config.objectName, spawnedObject);
                    spawnedObjects.Add(objectInfo);
                    occupiedPositions.Add(spawnPosition);

                    // 更新全局计数
                    if (config.hasGlobalLimit)
                    {
                        if (!_globalObjectCounts.ContainsKey(config.objectName))
                        {
                            _globalObjectCounts[config.objectName] = 0;
                        }
                        _globalObjectCounts[config.objectName]++;
                    }
                }
            }
        }

        // 保存生成的物体信息
        _chunkObjects[chunkCoord] = spawnedObjects;
    }

    /// <summary>
    /// 移除指定chunk中的所有GameObject
    /// </summary>
    /// <param name="chunkCoord">chunk坐标</param>
    private void RemoveObjectsInChunk(Vector2Int chunkCoord)
    {
        if (!_chunkObjects.ContainsKey(chunkCoord)) return;

        foreach (var objectInfo in _chunkObjects[chunkCoord])
        {
            if (objectInfo.gameObject != null)
            {
                objectInfo.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 恢复指定chunk中的GameObject
    /// </summary>
    /// <param name="chunkCoord">chunk坐标</param>
    private void RestoreObjectsInChunk(Vector2Int chunkCoord)
    {
        if (!_chunkObjects.ContainsKey(chunkCoord)) return;

        foreach (var objectInfo in _chunkObjects[chunkCoord])
        {
            if (objectInfo.gameObject != null)
            {
                objectInfo.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 寻找有效的生成位置
    /// </summary>
    /// <param name="chunkStartPos">chunk起始位置</param>
    /// <param name="config">生成配置</param>
    /// <param name="occupiedPositions">已占用的位置</param>
    /// <returns>有效位置，如果找不到返回Vector3.zero</returns>
    private Vector3 FindValidSpawnPosition(Vector3Int chunkStartPos, ObjectSpawnConfig config, List<Vector3> occupiedPositions)
    {
        for (int attempt = 0; attempt < _maxSpawnAttempts; attempt++)
        {
            // 在chunk内随机选择一个位置
            int randomX = _random.Next(0, _chunkSize);
            int randomY = _random.Next(0, _chunkSize);
            Vector3Int tilePos = new Vector3Int(chunkStartPos.x + randomX, chunkStartPos.y + randomY, 0);
            Vector3 worldPos = _grid.CellToWorld(tilePos) + _grid.cellSize * 0.5f; // 居中到tile

            // 检查tile类型是否符合要求
            TileBase tile = _tilemap.GetTile(tilePos);
            if (!IsValidTileForSpawn(tile, config)) continue;

            // 检查与其他物体的距离
            bool tooClose = false;
            foreach (var pos in occupiedPositions)
            {
                if (Vector3.Distance(worldPos, pos) < config.minDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                return worldPos;
            }
        }

        return Vector3.zero; // 找不到有效位置
    }

    /// <summary>
    /// 检查tile是否适合生成物体
    /// </summary>
    /// <param name="tile">要检查的tile</param>
    /// <param name="config">生成配置</param>
    /// <returns>是否适合生成</returns>
    private bool IsValidTileForSpawn(TileBase tile, ObjectSpawnConfig config)
    {
        if (tile == null) return false;

        // 根据tile类型判断
        if (tile == _normalTile && config.canSpawnOnNormal) return true;
        if (tile == _specialTile && config.canSpawnOnSpecial) return true;

        return false;
    }

    /// <summary>
    /// 初始化噪声偏移数组，为每个噪声层生成不同的偏移值
    /// </summary>
    private void InitializeNoiseOffsets()
    {
        _octaveOffsets = new Vector2[_octaves];

        for (int i = 0; i < _octaves; i++)
        {
            // 为每个噪声层生成不同的偏移值，基于种子和层索引
            float offsetX = _random.Next(-100000, 100000) + _offset.x;
            float offsetY = _random.Next(-100000, 100000) + _offset.y;
            _octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
    }

    /// <summary>
    /// 生成多层柏林噪声值
    /// </summary>
    /// <param name="x">世界X坐标</param>
    /// <param name="y">世界Y坐标</param>
    /// <returns>0-1之间的噪声值</returns>
    private float GenerateNoiseValue(int x, int y)
    {
        if (_octaveOffsets == null || _octaveOffsets.Length != _octaves)
        {
            InitializeNoiseOffsets();
        }

        float amplitude = 1f;
        float frequency = _noiseScale;
        float noiseValue = 0f;
        float maxValue = 0f;

        for (int i = 0; i < _octaves; i++)
        {
            // 使用每层独特的偏移值
            float sampleX = (x + _octaveOffsets[i].x) * frequency;
            float sampleY = (y + _octaveOffsets[i].y) * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
            noiseValue += perlinValue * amplitude;

            maxValue += amplitude;
            amplitude *= _persistence;
            frequency *= _lacunarity;
        }

        return noiseValue / maxValue;
    }

    /// <summary>
    /// 根据噪声值选择瓦片类型
    /// </summary>
    /// <param name="noiseValue">噪声值 (0-1)</param>
    /// <returns>对应的瓦片</returns>
    private TileBase GetTileFromNoiseValue(float noiseValue)
    {
        if (noiseValue < _specialTileThreshold)
        {
            return _specialTile; // 水（特殊地块）
        }
        else
        {
            return _normalTile; // 草地（普通地块）
        }
    }

    /// <summary>
    /// 从世界坐标获取地图块坐标
    /// </summary>
    /// <param name="worldPosition">世界坐标</param>
    /// <returns>地图块坐标</returns>
    private Vector2Int GetChunkFromWorldPosition(Vector3 worldPosition)
    {
        Vector3Int cellPosition = _grid.WorldToCell(worldPosition);
        return new Vector2Int(
            Mathf.FloorToInt((float)cellPosition.x / _chunkSize),
            Mathf.FloorToInt((float)cellPosition.y / _chunkSize)
        );
    }

    /// <summary>
    /// 设置新的随机种子并重新生成地图
    /// </summary>
    /// <param name="newSeed">新的种子值</param>
    public void SetSeed(int newSeed)
    {
        _seed = newSeed;
        _random = new System.Random(_seed);

        // 重新初始化噪声偏移数组
        InitializeNoiseOffsets();

        // 清除所有已生成的地图块
        foreach (var chunk in _generatedChunks.Keys)
        {
            RemoveChunk(chunk);
        }
        _generatedChunks.Clear();

        // 清除所有生成的物体
        foreach (var chunkObjects in _chunkObjects.Values)
        {
            foreach (var objectInfo in chunkObjects)
            {
                if (objectInfo.gameObject != null)
                {
                    Destroy(objectInfo.gameObject);
                }
            }
        }
        _chunkObjects.Clear();

        // 重置全局物体计数
        foreach (var config in _spawnConfigs)
        {
            if (config.hasGlobalLimit)
            {
                _globalObjectCounts[config.objectName] = 0;
            }
        }

        // 重新生成地图
        if (_player != null)
        {
            GenerateInitialMap();
        }
    }

    /// <summary>
    /// 设置玩家引用
    /// </summary>
    /// <param name="player">玩家Transform</param>
    public void SetPlayer(Transform player)
    {
        _player = player;
        if (_player != null)
        {
            GenerateInitialMap();
        }
    }

    /// <summary>
    /// 销毁时清理单例引用
    /// </summary>
    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    /// <summary>
    /// 在编辑器中绘制调试信息
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (_player == null) return;

        Vector2Int playerChunk = GetChunkFromWorldPosition(_player.position);

        // 绘制当前玩家所在的地图块
        Gizmos.color = Color.red;
        Vector3 chunkWorldPos = new Vector3(playerChunk.x * _chunkSize, playerChunk.y * _chunkSize, 0);
        Gizmos.DrawWireCube(chunkWorldPos + Vector3.one * _chunkSize * 0.5f, Vector3.one * _chunkSize);

        // 绘制视野范围
        Gizmos.color = Color.yellow;
        for (int x = playerChunk.x - _viewDistance; x <= playerChunk.x + _viewDistance; x++)
        {
            for (int y = playerChunk.y - _viewDistance; y <= playerChunk.y + _viewDistance; y++)
            {
                Vector3 pos = new Vector3(x * _chunkSize, y * _chunkSize, 0);
                Gizmos.DrawWireCube(pos + Vector3.one * _chunkSize * 0.5f, Vector3.one * _chunkSize);
            }
        }
    }
}
