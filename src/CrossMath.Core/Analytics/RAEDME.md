## 空盘信息
#### 基本信息
- 大小 Size
- 格子数  CellCount
- 交点数  CrossCount
- 算式数  FormulaCount
- 环数。  RingCount。tips：存在长度为7或者交点为符号格时，公式是否适用
- Sigma
- 次圈 上下左右数
- 最外圈  上下左右。sigma
#### 结构信息 需要算式布局信息辅助  ExpressionLayout
- 算式统计 FormulaStatistics 12，3    长度5算式数，长度7算式数
- 交点统计 CrossStatistics 23，4   数字交叉格数，符号格交叉数
- L型特殊算式数 LSpecialCount
- Z型特殊算式数 ZSpecialCount

## 终盘信息
- 运算符统计  OperatorStatistics
- 盘面数字友好度  BoardNumberFriendly
- 最小值。 MinValue
- 最大值。  MaxValue

## 初盘信息
#### 基本信息
- 空格数   HoleCount
- 候选数  Candidates
- 候选数数字友好度 CandidateFriendly
#### 全局格子难度信息
- 全局难度统计 GCellDifficultyStatistics
- 全局最大难度 GMaxCellDifficulty
- 全局最小难度 GMinCellDifficulty
- 全局总难度 GTotalCellDifficulty
#### 本地格子难度信息
- 本地难度统计 LCellDifficultyStatistics
- 本地最大难度 LMaxCellDifficulty
- 本地最小难度 LMinCellDifficulty
- 本地总难度。 LTotalCellDifficulty
#### 关卡难度
- 关卡难度  LevelDifficulty
- ? 卡点统计 PainPointStatistics
- ？卡点数。 PainPointCount
- ？首个卡点位置。 FirstPainPointPos
- ？首个卡点百分比 FirstPainPointPercentage






需要计算的字段：list[Filed]

计算的映射
filed，computer

读取Dict
获取需要的字段：
BoardLayout layout = build（orgindata， filedadapt）

需要计算的字段：
emptyscheme


字段数据：
1. 类型
2. 稳定的名字
4. 数据来源：layout，终盘，初盘

字段稳定名字表：

字段导出名映射表：
稳定名字 -> 导出名or读取名

字段映射器（可外部传入json覆盖）
外部字段名 -> 稳定名字


字段计算上下文:
Board，
Layout，
其他计算的中间结果存取

字段来源：注册器：  字段 ： 字段来源
1. 尝试 从 provider 中直接读取
2. 没有，进入计算
   3. 获取分析器，计算结果
   4. 从结果中析出字段，中间结果放入 上下文中缓存，由上下文传递

字段Runner(上下文，要求的字段列表)






