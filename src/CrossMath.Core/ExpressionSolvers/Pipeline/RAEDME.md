ExpressionSolvePipeline（统一求解管线）

表达式求解流程是固定的：

1. 处理运算符
2. 处理值
3. 验证
   你的表达式求解有 6 个不稳定点：

不稳定行为	解耦策略
运算符填补方式不同	IOperatorStrategy
值枚举方式不同	IValueStrategy
表达式结构不同（Expression5 vs Expression7）	专用策略类
验证逻辑不同	IExpressionValidator
数字范围不同	ExpressionSolveOptions
数字池使用规则不同	策略化的 NumberPool