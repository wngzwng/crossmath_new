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