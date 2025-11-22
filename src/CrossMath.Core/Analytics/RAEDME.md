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