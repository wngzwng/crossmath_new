using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Evaluation;

    
public sealed class GlobalDifficultyEvaluator
{
    private readonly IReadOnlyList<IGlobalDifficultyLayer> _layers;

    public GlobalDifficultyEvaluator(IEnumerable<IGlobalDifficultyLayer> layers)
    {
        // 必须从小到大排序！
        _layers = layers.OrderBy(l => l.Difficulty).ToList();
    }
    
    public Dictionary<RowCol, int> Evaluate(GlobalDifficultyContext initialContext)
    {
        var search = new Stack<GlobalDifficultyContext>();
        search.Push(initialContext.Clone());

        while (search.TryPop(out var ctx))
        {
            while (TryApplyDeterministicStep(ctx)) { }  // 持续确定性填数

            if (ctx.Board.GetKind() == BoardKind.Answer)
                return ctx.DifficultyRecord;

            if (TryMakeOneGuess(ctx, out var branches))
            {
                foreach (var branch in branches)
                    search.Push(branch);
            }
        }

        throw new InvalidOperationException("无法评级");
    }
    
    private bool TryApplyDeterministicStep(GlobalDifficultyContext ctx)
    {
        foreach (var layer in _layers)
        {
            if (layer.TryEvaluate(ctx, out var branches))
            {
                return true;
            }

            if (branches is not null)
            {
                _pendingGuess = branches.ToList();
                return false;
            }
        }
        return false;
    }
    
    private List<GlobalDifficultyContext>? _pendingGuess;

    private bool TryMakeOneGuess(GlobalDifficultyContext ctx, out IReadOnlyList<GlobalDifficultyContext> branches)
    {
        if (_pendingGuess is { Count: > 0 })
        {
            branches = _pendingGuess;
            _pendingGuess = null;
            return true;
        }

        branches = Array.Empty<GlobalDifficultyContext>();
        return false;
    }

        // public Dictionary<RowCol, int> Evaluate(GlobalDifficultyContext initialContext)
        // {
        //     var stack = new Stack<GlobalDifficultyContext>();
        //     stack.Push(initialContext.Clone());     // 第一个节点
        //
        //     while (stack.Count > 0)
        //     {
        //         var ctx = stack.Pop();
        //
        //         bool madeProgress;
        //         do
        //         {
        //             madeProgress = false;
        //
        //             foreach (var layer in _layers)
        //             {
        //                 if (layer.TryEvaluate(ctx, out var branches))
        //                 {
        //                     madeProgress = true;
        //                     // 重要：确定性步骤后，立刻从头开始重新扫描所有层（因为新填的数可能解锁更低级的技巧）
        //                     break;
        //                 }
        //                 
        //                 if (branches is not null)
        //                 {
        //                     var branchList = branches.ToList();   // 强制立即执行并缓存
        //
        //                     if (branchList.Count > 0)
        //                     {
        //                         foreach (var branch in branchList)
        //                             stack.Push(branch);
        //
        //                         break;  // 猜数分支已生成，结束当前节点处理
        //                     }
        //                 }
        //             }
        //         } while (madeProgress);
        //
        //         // 所有层都用完了，检查是否完成
        //         if (ctx.Board.GetKind() == BoardKind.Answer)
        //         {
        //             return ctx.DifficultyRecord;   // 找到一条完整解路径，返回其难度记录
        //         }
        //
        //         // 如果走到这里且没有产生新分支，说明当前路径死掉，丢弃，继续回溯
        //     }
        //
        //     throw new InvalidOperationException("盘面无解或超出评级范围");
        // }
}
