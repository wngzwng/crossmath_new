```C#
public interface ICompletionChecker
{
    bool IsComplete(ICanvas canvas);
}
```
表示收集那些盘面感兴趣的盘面
第一类：
1. 盘面大小类，给定盘面大小，收集这类盘面大小
2. 