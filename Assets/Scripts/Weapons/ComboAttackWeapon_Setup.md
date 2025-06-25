# 连击武器设置说明

## 武器架构

```
ComboAttackWeapon (主武器对象 - 只负责连击逻辑)
├── 攻击时动态克隆的物体：
│   ├── LeftSwordInstance (左剑攻击实例)
│   │   ├── Collider2D (IsTrigger = true)
│   │   └── EnemyDamager (负责伤害处理)
│   ├── RightSwordInstance (右剑攻击实例)
│   │   ├── Collider2D (IsTrigger = true)
│   │   └── EnemyDamager (负责伤害处理)
│   └── AoeAttackInstance (AOE攻击实例)
│       ├── Collider2D (IsTrigger = true)
│       └── EnemyDamager (负责伤害处理)
```

## 架构说明

### ComboAttackWeapon 职责
- ✅ **连击逻辑控制**：管理三段连击的时序
- ✅ **预制体克隆**：每次攻击都克隆新的攻击物体
- ✅ **自动攻击循环**：实现自动攻击功能
- ✅ **独立攻击时间**：剑攻击和AOE攻击使用不同的持续时间
- ✅ **物体生命周期管理**：攻击结束后自动销毁克隆的物体
- ❌ **不处理伤害**：伤害由各攻击区域的EnemyDamager处理

### EnemyDamager 职责
- ✅ **伤害处理**：对敌人造成伤害
- ✅ **击退效果**：应用击退力
- ✅ **碰撞检测**：检测敌人碰撞
- ✅ **动画效果**：处理生长动画

## 设置步骤

### 1. 创建武器对象
1. 创建一个空的GameObject，命名为"ComboAttackWeapon"
2. 将 `ComboAttackWeapon.cs` 脚本挂载到这个对象上

### 2. 创建攻击预制体 ⚠️ 重要步骤
1. **左剑攻击预制体**：
   - 创建一个空的GameObject，命名为"LeftSwordPrefab"
   - 添加Collider2D组件，勾选"IsTrigger"
   - 添加EnemyDamager组件
   - 调整碰撞体大小和位置（建议放在左侧）
   - 将整个物体拖拽到Project窗口创建预制体
   - 删除场景中的物体

2. **右剑攻击预制体**：
   - 创建一个空的GameObject，命名为"RightSwordPrefab"
   - 添加Collider2D组件，勾选"IsTrigger"
   - 添加EnemyDamager组件
   - 调整碰撞体大小和位置（建议放在右侧）
   - 将整个物体拖拽到Project窗口创建预制体
   - 删除场景中的物体

3. **AOE攻击预制体**：
   - 创建一个空的GameObject，命名为"AoeAttackPrefab"
   - 添加Collider2D组件，勾选"IsTrigger"
   - 添加EnemyDamager组件
   - 调整碰撞体大小（建议为圆形或方形，覆盖周围区域）
   - 将整个物体拖拽到Project窗口创建预制体
   - 删除场景中的物体

### 3. 配置ComboAttackWeapon脚本
在Inspector中设置以下参数：

#### 攻击预制体 ⚠️ 必须设置
- **Left Sword Prefab**：拖拽LeftSwordPrefab预制体
- **Right Sword Prefab**：拖拽RightSwordPrefab预制体
- **Aoe Attack Prefab**：拖拽AoeAttackPrefab预制体

#### 连击设置
- **Combo Time Window**：连击时间窗口（默认2秒）
- **Attack Cooldown**：攻击冷却时间（默认0.5秒）

#### 攻击持续时间设置
- **Sword Attack Duration**：剑攻击持续时间（默认0.3秒）
- **Aoe Attack Duration**：AOE攻击持续时间（默认0.5秒）

#### 自动攻击设置
- **Auto Attack**：是否启用自动攻击（默认true）
- **Auto Attack Interval**：自动攻击间隔（默认1秒）

### 4. 配置EnemyDamager组件 ⚠️ 重要配置
对每个攻击预制体的EnemyDamager组件进行配置：

#### 基础伤害设置
- **Damage Amount**：设置该攻击区域的伤害值
- **Destroy On Impact**：true ⚠️ 可以设为true，因为每次都是新克隆的物体
- **Life Time**：设置为大于对应攻击持续时间（如剑攻击0.3秒，AOE攻击0.5秒）
- **Should Knock Back**：是否启用击退
- **Knock Back Force**：击退力度
- **Damage Over Time**：false（建议）
- **Grow Speed**：10（快速生长动画）
- **Destroy Parent**：false ⚠️ 必须设为false

#### 敌人检测设置
- **Enemy Tag**：敌人标签（如"Enemy"）
- **Enemy Sorting Layer**：敌人排序层级（如"Enemy"）
- **Detection Method**：检测方式
  - 0 = 仅使用Tag
  - 1 = 仅使用SortingLayer
  - 2 = 同时使用Tag和SortingLayer

### 5. 设置敌人
根据检测方式设置敌人：

#### 使用Tag检测
- 给敌人对象设置正确的Tag（如"Enemy"）

#### 使用SortingLayer检测
- 给敌人的SpriteRenderer设置正确的SortingLayer（如"Enemy"）

#### 使用Both检测
- 同时设置Tag和SortingLayer

#### 通用要求
- 有Collider2D组件
- 有Rigidbody2D组件（用于击退效果）

## 工作原理

### 克隆机制 ⚠️ 核心特性
1. **每次攻击都克隆新物体**：确保动画每次都从头开始播放
2. **攻击结束后自动销毁**：避免物体积累和内存泄漏
3. **动画重置**：每次克隆的物体都有完整的初始状态

### 自动攻击模式（默认）
1. **自动循环**：武器会自动按照设定的间隔进行攻击
2. **连击系统**：在时间窗口内连续攻击形成连击
3. **预制体克隆**：每次攻击都克隆对应的预制体
4. **独立持续时间**：
   - 剑攻击使用Sword Attack Duration
   - AOE攻击使用Aoe Attack Duration
5. **伤害检测**：EnemyDamager组件自动检测碰撞并处理伤害
6. **自动销毁**：攻击持续时间结束后自动销毁克隆的物体

### 手动攻击模式
1. **手动触发**：按空格键或鼠标左键触发攻击
2. **连击系统**：在时间窗口内连续攻击形成连击
3. **其他流程**：与自动攻击模式相同

## 攻击序列

1. **第一段**：克隆左剑攻击预制体，持续Sword Attack Duration后销毁
2. **第二段**：克隆右剑攻击预制体，持续Sword Attack Duration后销毁
3. **第三段**：克隆AOE攻击预制体，持续Aoe Attack Duration后销毁
4. **重置**：完成连击后重置，重新开始

## 输出信息

当攻击命中敌人时，会在Console中输出：
- `对敌人 [敌人名称] 造成 [伤害值] 点伤害`
- 如果启用了击退效果：`对敌人施加了击退效果，力度：[击退力度]`

## 调试功能

在Scene视图中选中ComboAttackWeapon时，会显示：
- **红色方块**：左剑攻击区域（基于预制体大小）
- **蓝色方块**：右剑攻击区域（基于预制体大小）
- **黄色方块**：AOE攻击区域（基于预制体大小）

## 重要注意事项 ⚠️

### 预制体设置要求
1. **必须创建预制体**：不能直接使用场景中的物体
2. **预制体必须包含EnemyDamager**：确保伤害功能正常
3. **预制体位置设置**：在预制体中设置好相对位置
4. **验证预制体**：脚本启动时会验证预制体是否设置

### 生命周期管理
1. **自动克隆**：每次攻击都会克隆新的预制体实例
2. **自动销毁**：攻击结束后自动销毁克隆的物体
3. **内存管理**：不会产生物体积累，内存使用稳定
4. **动画重置**：每次攻击都有完整的动画效果

### 攻击持续时间设置建议
1. **剑攻击**：通常设置为0.2-0.4秒，快速攻击
2. **AOE攻击**：通常设置为0.4-0.8秒，较长的范围攻击
3. **连击时间窗口**：确保大于所有攻击持续时间之和
4. **EnemyDamager Life Time**：设置为大于对应的攻击持续时间

### 常见问题解决
1. **预制体未设置错误**：确保在Inspector中设置了所有三个预制体
2. **动画不正确**：每次克隆都会重置动画状态，确保动画正常
3. **伤害不生效**：检查EnemyDamager的配置，确保检测方式正确
4. **攻击时间不匹配**：确保EnemyDamager的Life Time大于对应的攻击持续时间
5. **物体不销毁**：脚本会自动销毁，如果手动销毁可能影响逻辑

### 优势
1. **动画完整性**：每次攻击都有完整的动画效果
2. **内存安全**：不会产生物体积累
3. **状态重置**：每次攻击都是全新的状态
4. **易于调试**：每个攻击实例都是独立的
5. **性能稳定**：避免了复杂的对象池管理

### 其他注意事项
1. 预制体必须包含完整的组件设置
2. 攻击区域的碰撞体大小决定了攻击范围
3. 根据Detection Method设置正确的Tag或SortingLayer
4. 剑攻击和AOE攻击可以使用不同的持续时间
5. 连击系统会自动管理攻击状态，无需手动控制
6. 伤害信息会通过Debug.Log输出到Console中
7. ComboAttackWeapon只负责连击逻辑，不处理伤害
8. 每个攻击区域的伤害由各自的EnemyDamager独立配置
9. 自动攻击模式下，武器会持续循环攻击
10. 手动攻击模式下，需要按键触发攻击
11. 每次攻击都克隆新物体，确保动画和状态完全重置
12. 攻击结束后自动销毁，避免内存泄漏 