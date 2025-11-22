> HoleDigger 负责给终盘挖空

核心流程
- 盘面 -> 算式布局提取
- 获取挖空位置 （row， col)
- 挖空，唯一解校验
- 校验成功，继续挖空
- 校验失败，挖空回填，继续挖空
- 合适停止 策略(挖空数，或者其他)

#### 挖空策略
```C#
public interface IHoleStrategy  
{  
    RowCol? GetNextHoleCoordinate(List<ExpressionView> expressions, HoleContext context);  
}

```

#### 唯一解校验
```C#
public interface IUniqueSolutionValidator  
{  
    bool HasUniqueSolution(BoardData board);  
}
```

#### 目标挖空数计算 —— 必须（但可以很简单）

你完全可以不用 IHoleTargetCalculator，就一个简单函数：
```C#
public delegate int HoleTargetCalculator(HoleContext context);`
```
