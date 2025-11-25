## 本地难度层级
>初盘评估每个格子的难度，每评估一个，不填入盘面

cellDifficultyMap：Dict<RowCol, HashSet
核心步骤：
- 根据难度评级得到 （row，col, difficulty). 迭代器
- 标记格子难度 rowcol -> set difficulty
- 遍历所有难度层级
- 剩余的没有标记的格子标记为 4级
- 返回对应的难度等级


难度层级同样4层，只是不会填数，而且具体操作也会有不同
能够复用的就是难度的评估
- 难度1
- 难度2
- 难度3

## 关卡难度
> 对关卡进行一定次数(n=1000)的模拟，统计平均难度分

模拟一次 SolverOne
- 本地难度计算   LocalDifficultyEvalutor
- 上一步选中位置.   LastSelectCoordinate
- 难度，和一些参数，上一步位置 => 对应的分数 （策略)  (difficulty, lastSelectCoordinate, boardData) => (row, col, score)
- 分数 -> 权重 （策略） (score) => weight
- 位置选择（位置，权重，选择策略）CoordinateSelector(RowColList, Weight) => (row, col)
- 更新
    - 填数
    - 分数更新
- 直到所有空格完成
  模拟一定次数


### 全局格子难度计算
难度4设计到递归
这里使用 栈 实现

在难度三，如果有唯一值，就直接填入，标记难度3
没有唯一值，就是选择候选数最少的，
然后填入，标记难度4，
然后 clone上下，标记下一个，继续

每个难度返回 bool 表示是有有处理

遍历难度层计算
只要有处理就继续重来
都没有处理
表示失败了，这是出栈，获取新的上下文，然后更新上下文，继续重复步骤

这与你想做的“difficulty pipeline”一致：
