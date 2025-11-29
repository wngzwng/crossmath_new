using CrossMath.Core.CandidateDomains;
using CrossMath.Core.CSP;
using CrossMath.Core.Evaluation;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.ExpressionSolvers;
using CrossMath.Core.ExpressionSolvers.ExpressionValidators;
using CrossMath.Core.ExpressionSolvers.NumberPools;
using CrossMath.Core.ExpressionSolvers.OperatorPools;
using CrossMath.Core.ExpressionSolvers.SolverProviders;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.BoardSolvers;

public class BoardSolver: IBoardSolver
{
    protected record SearchNode(BoardData board, Dictionary<RowCol, string> answers);
        
    private HashSet<int> _allowExpLengths = new HashSet<int>([5, 7]);
    public IReadOnlySet<int> AllowExpLengths => _allowExpLengths;

    private List<string> _initCellCandidates = Enumerable.Range(1, 240).Select(i => i.ToString()).ToList()
        .Concat(SymbolManager.AllOperatorSymbols).ToList();
                         
    private CrossMathCSP _csp = new ();

    private List<ExpressionLayout> _layouts = new ();

    private HashSet<string> answersSet = new ();


    private string AnswersHash(Dictionary<RowCol, string> answers)
    {
        var orderAnswers = answers.OrderBy(pair => pair.Key).ToList();
        return string.Join(",", orderAnswers.Select(pair => pair.Value));
    }
    
    
    public IEnumerable<BoardSolution> Solve(BoardData board, ExpressionSolverProvider solver)
    {
        Init(board);
        
        var _statck = new Stack<SearchNode>();
        _statck.Push(new SearchNode(board.Clone(), new Dictionary<RowCol, string>()));
        
        while (_statck.TryPop(out var node))
        {
            Again:
            // 确定性求解
            if (!TryApplyDeterministicStep(node!, solver)) continue;

            if (node.board.Holes.Count == 0)
            {
                string hash = AnswersHash(node.answers);
                if (answersSet.Contains(hash)) continue;

                answersSet.Add(hash);
                yield return new BoardSolution(board, node.answers);
                continue;
            }
            
            var manager = BuildCandidateDomainManager(node.board, solver);
            var cspResult = _csp.RunPropagation(manager, _initCellCandidates);

            if (TryProcessSingleCandidateTables(node, cspResult.ExpressionCandidates))
            {
                goto Again;  // 约束传播后有唯一解
            }
            
            if (cspResult.VariableDomains.Count <= 0) break;
            
            var minDomainVariable = cspResult.VariableDomains
                .Where(kvp => kvp.Value.Count > 1)  // 只考虑多候选域的变量
                .MinBy(kvp => kvp.Value.Count);
            foreach (var branch in CreateBranches(node, minDomainVariable.Key, minDomainVariable.Value))
            {
                _statck.Push(branch);
            }
        }
    }

    public void Init(BoardData board)
    {
         _layouts = ExpressionLayoutBuilder.ExtractLayouts(board.Layout, _allowExpLengths);
         answersSet.Clear();
    }


    public ExpressionSolverProvider CreateDefaultExpressionSolverProvider()
    {
        return ExpressionSolverProvider.CreateDefault();
    }
    

    public ExpressionSolveContext CreateDefaultExpressionSolveContext(BoardData board)
    {
        return new ExpressionSolveContext()
        {
            NumPool = NumberPoolFactory.Create(board.GetAnswerNumbers()),
            OpPool = OperatorPoolFactory.Discrete(board.GetAllOperators()),
            Validator = new ExpressionValidator(ValidationMode.FullDiscreteConsume)
        };
    }


    public CandidateDomainManager<RowCol, string> BuildCandidateDomainManager(BoardData board, ExpressionSolverProvider solver)
    {
        var manager = new CandidateDomainManager<RowCol, string>();
        var solvedCtx = CreateDefaultExpressionSolveContext(board);
        foreach (var layout in _layouts)
        {
            if (layout.EmptyCellCount(board) <= 0) continue;
            
            var exp = layout.ToExpression(board);
            var solvedExps = solver.Solve(exp, solvedCtx);
            var table = layout.BuildCandidateTable(exp, solvedExps);
            manager.Add(layout.ToString(), table);
        }
        return manager;
    } 

    protected bool TryApplyDeterministicStep(SearchNode node, ExpressionSolverProvider solver)
    {
        var solvedCtx = CreateDefaultExpressionSolveContext(node.board);
        var board = node.board;
        foreach (var layout in _layouts)
        {
            if (layout.EmptyCellCount(board) <= 0) continue;
            
            var exp = layout.ToExpression(board);
            var solvedExps = solver.Solve(exp, solvedCtx);
            var top2SolvedExps = solvedExps.Take(2).ToList();
            if (top2SolvedExps.Count == 0) return false; // 无解

            if (top2SolvedExps.Count == 1) //有唯一数，填写空格
            {
                var record = layout.ExtractCandidateRow(exp, top2SolvedExps[0]).ToDictionary();
                FillBoard(node, record);
            }
        }

        return true;
    }

    protected void FillBoard(SearchNode node, Dictionary<RowCol, string> answers)
    {
        var board = node.board;
        foreach (var hole in answers.Keys)
        {
            // 添加防护检查，确保该位置尚未被填充
            if (!node.answers.ContainsKey(hole))
            {
                board.SetValueOnly(hole, answers[hole]);
                board.PossibleAnswers.Remove(answers[hole]);
                node.answers.Add(hole, answers[hole]);
            }
        }
    }
    
    private bool TryProcessSingleCandidateTables(SearchNode node, CandidateDomainManager<RowCol, string> domainManager)
    {
        var modeProcess = false;
        foreach (var table in domainManager.Tables)
        {
            if (table.IsSingle)
            {
                var map = table[0].ToDictionary();
                FillBoard(node, map);
                modeProcess = true;   
            }
        }

        return modeProcess;
    }


    protected IEnumerable<SearchNode> CreateBranches(SearchNode node, RowCol position, IEnumerable<string> candidateValues)
    {
        foreach (var value in candidateValues)
        {
            var newNode = new SearchNode(node.board.Clone(), new Dictionary<RowCol, string>(node.answers));
            var answers= new Dictionary<RowCol, string>();
            answers.Add(position, value);   
            FillBoard(newNode, answers);

            yield return newNode;
        }
    }
    
    
    
}