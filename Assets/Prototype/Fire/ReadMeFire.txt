火焰魔杖隔一段冷却时间进行一次喷火，喷出来的火对怪物进行范围伤害攻击。


代码可能有bug仔细检查


教程
步骤 1：设置游戏对象层级
在你的玩家 (Player) 对象下，创建一个空的子对象，命名为 FireAttackZone。
将 FireAttackZone 的 Transform > Position 重置为 (0, 0, 0)，这样它就和玩家的位置重合了。
为 FireAttackZone 添加两个组件：
Sprite Renderer：用来显示火焰的序列帧动画。
Animator：用来控制动画播放。
默认禁用 FireAttackZone：在 Inspector 中，取消勾选 FireAttackZone 对象名前面的复选框，让它在游戏开始时处于隐藏状态。
步骤 2：创建动画和动画控制器
创建动画控制器 (Animator Controller)：在 Assets 文件夹中右键 -> Create -> Animator Controller，命名为 FireWand_AC。
创建动画片段 (Animation Clip)：
选中 FireAttackZone 对象。
打开 Window -> Animation -> Animation 窗口。
点击 "Create" 按钮，创建一个新动画，命名为 FireBurstAnim。
将你的火焰序列帧图片拖拽到动画时间轴上，制作一个完整的喷火动画。
设置动画控制器：
双击打开 FireWand_AC。你会看到 Any State, Entry, Exit。
将你刚才创建的 FireBurstAnim 拖拽到 Animator 窗口中，它会成为默认状态。我们需要修改一下。
在 Animator 窗口中右键 -> Create State -> Empty，创建一个新状态，命名为 Idle。右键 Idle -> Set as Layer Default State。
在左侧的 Parameters 标签页中，点击 + 号，创建一个 Trigger 类型的参数，命名为 Attack。
创建一条从 Any State 到 FireBurstAnim 的过渡线 (Transition)。选中这条线，在 Inspector 中：
找到 Conditions，点击 + 号，选择我们刚才创建的 Attack 触发器。
取消勾选 Has Exit Time。
创建一条从 FireBurstAnim 回到 Idle 的过渡线。选中这条线，确保 Has Exit Time 是勾选的，并且 Exit Time 设置为 1。
添加动画事件：
选中 FireAttackZone，再次打开 Animation 窗口，并选择 FireBurstAnim 动画。
在时间轴上，找到你希望造成伤害的那一帧（比如火焰最大的时候）。
在时间轴上方点击 "Add Event" 按钮（一个带加号的小书签图标）。
在 Inspector 中，你会看到一个 Function 字段。我们稍后会在这里填上函数名。
关联控制器：将 FireWand_AC 拖拽到 FireAttackZone 的 Animator 组件的 Controller 字段上。
步骤 3：编写脚本
现在我们需要两个脚本：一个挂在玩家身上（FireWand.cs），一个挂在攻击区域上（FireAttackZone.cs）。