using CrossMath.Core.Generators.Canvas;

namespace CrossMath.Core.Generators.CanvasHashProvider;

/// <summary>
/// 布局指纹提供器 —— 只需要知道“哪些格子被占了”就能唯一标识一个布局
/// 适用于所有基于“格子有值/无值”的画布实现
/// </summary>
public interface ICanvasHashProvider
{
    ulong ComputeHash(ICanvas canvas);
}