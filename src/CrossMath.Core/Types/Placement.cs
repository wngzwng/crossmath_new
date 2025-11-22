namespace CrossMath.Core.Types;

// 放置点
public readonly record struct Placement  
(  
    int Row,   
    int Col,   
    Direction Direction,   
    int Length  
);