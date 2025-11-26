using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace CrossMath.Core.Generators.CanvasHashProvider;

// ======================================================
// ZobristCanvasHashProvider.cs
// 实现：只用 HasValue 就能完美去重
// 原理：每个格子位置预先生成一个随机 ulong，被占了就异或进去
// ======================================================
public sealed class ZobristCanvasHashProvider : ICanvasHashProvider
{   
    // 种子 = “CrossMath” 的灵魂 + 诞生之日
    // 2025年11月23日 —— 这份代码在这一天被真正写
    private static readonly int  Seed = "CrossMath".GetHashCode() ^ 20251123;
    
    private static readonly object Empty = new(); // 哨兵

    public static readonly ZobristCanvasHashProvider Instance = new();

    public static readonly ZobristHash<object> Zobrist = new(Seed);
    private ZobristCanvasHashProvider() { }

    public ulong ComputeHash(ICanvas canvas)
    {
        var z = Zobrist;
        ulong hash = 0;

        for (int r = 0; r < canvas.Height; r++)
        for (int c = 0; c < canvas.Width; c++)
        {
            var pos = RowCol.At(r, c);
            var state = canvas.HasValue(pos)
                ? canvas.GetValue(pos) ?? Empty
                : Empty;

            hash ^= z[c, r, state]; // 像访问三维数组一样自然！
        }

        return hash;
    }
}

/* 使用方式
全局单例 —— 永远只有这一个实例
   private static readonly ICanvasHashProvider Hasher = ZobristCanvasHashProvider.Instance;
   
   private static readonly HashSet<ulong> SeenLayouts = new();
  
使用1:
foreach (var canvas in layoutGenerator.Generate())
   {
       ulong fingerprint = Hasher.ComputeHash(canvas);
   
       if (SeenLayouts.Add(fingerprint))
       {
           // 恭喜！这是一个全新的、从未出现过的布局！
           SaveToDatabase(canvas);
           YieldToPlayer(canvas);
       }
       // else：重复布局，自动丢弃，0成本
   }
   

使用2:
var uniqueLayouts = layoutGenerator
       .Generate()
       .DistinctBy(canvas => Hasher.ComputeHash(canvas));
       
   foreach (var canvas in uniqueLayouts)
   {
       Save(canvas);
   }
   
使用3: 生成 N 个不重复的布局
public IEnumerable<ICanvas> GenerateUnique(int count)
   {
       SeenLayouts.Clear(); // 可选：每次重新开始
   
       foreach (var canvas in layoutGenerator.Generate())
       {
           if (SeenLayouts.Add(Hasher.ComputeHash(canvas)))
           {
               yield return canvas;
   
               if (SeenLayouts.Count >= count)
                   yield break;
           }
       }
   }
   
   // 使用
   foreach (var layout in GenerateUnique(1000))
   {
       Console.WriteLine("第 {SeenLayouts.Count} 个全新布局！");
   }
   
*/

/*
old
  // 全局唯一实例
     public static readonly ZobristCanvasHashProvider Instance = new();

     // 预生成：每个格子一个随机值（支持最大 20x20 也没问题）
     private readonly ulong[,] _cellKeys;

     private ZobristCanvasHashProvider()
     {
         const int maxSize = 20;
         _cellKeys = new ulong[maxSize, maxSize];
         
         // 种子 = “CrossMath” 的灵魂 + 诞生之日
         // 2025年11月23日 —— 这份代码在这一天被真正写下、完成、封存
         var seed = "CrossMath".GetHashCode() ^ 20251123;
         var rnd = new Random(seed);

         for (int r = 0; r < maxSize; r++)
         for (int c = 0; c < maxSize; c++)
         {
             _cellKeys[r, c] = NextUlong(rnd);
         }
     }

     public ulong ComputeHash(ICanvas canvas)
     {
         ulong hash = 0;

         for (int r = 0; r < canvas.Height; r++)
         for (int c = 0; c < canvas.Width; c++)
         {
             var pos = RowCol.At(r, c);
             if (canvas.HasValue(pos))
             {
                 hash ^= _cellKeys[r, c];
             }
         }

         return hash;
     }

     private static ulong NextUlong(Random rnd)
     {
         var bytes = new byte[8];
         rnd.NextBytes(bytes);
         return BitConverter.ToUInt64(bytes);
     }
*/