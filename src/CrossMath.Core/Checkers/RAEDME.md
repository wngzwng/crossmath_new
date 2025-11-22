var components = LayoutUtils.FindConnectedComponents(layout);
```
### 空盘校验
- 大小 Size： 1. 位数是否足够； 2. 四周是否有支撑格
	- 最外圈的上下左右的格子数
- 连通性：    
	- 检查盘面是否连通
	- DFS count。 VS   所有合法格子数
- 毛刺检测： 毛刺（Spikes/Burrs）= 与主体盘面连通，但不构成任何合法表达式长度的格子
	- 它是 **合法格子**（不是墙、不是空洞）
	- 它 **有邻居**（因此连到盘面主体）
	- 但它所在方向的 **连续长度不满足允许的表达式长度集合（如 5 或 7）**
	- 步骤：
		- 1. 提取 ExpressionLayout
		- 2. set(所有合法的格子) - set(表达式提取的总格子) = 毛刺部分的格子 
- 交点类型检测
	- 错误交点类型(数字格x符号格子 数字格x等号 符号格x等号) 与 非法交点类型(只允许某个类型交叉格)
	- 做法：
		- 1. 提取ExpressionLayout
		- 2. (row, col) -> expression_layout_id
		- 3. 交点类型检测  
			- cross_type1 !== cross_type1 错误交点类型
			- cross_type1 == cross_type2 && cross_type1 not in allowCrossType. 非法焦点类型

## 终盘校验
#### 表达式评估检测
- 1. 提取ExpressionLayout
- 2. 校验 格子填充 与 格子类型 是否相符
- 3. 完整的表达式评估，是否合法  排除这种。3 / 0 = any，这种是不允许的

## 初盘校验
##### 候选数数量与空位数量校验
#### 初盘的唯一解校验（true，多解，唯一解，无解，不可知或错误）
