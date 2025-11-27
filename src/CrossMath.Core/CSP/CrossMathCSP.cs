    using CrossMath.Core.CandidateDomains;
    using CrossMath.Core.Expressions.Layout;
    using CrossMath.Core.Models;
    using CrossMath.Core.Types;

    namespace CrossMath.Core.CSP;

    public class CrossMathCSP
    {
        private Dictionary<RowCol, HashSet<string>> _posToRelatedLayouts = new();
        private Dictionary<string, HashSet<RowCol>> _layoutToHoles = new();
        private Dictionary<RowCol, HashSet<string>> _holeDomains = new();
        private Dictionary<string, ExpressionLayout> _idToLayouts = new();
        
        public CSPResult RunPropagation(CandidateDomainManager<RowCol, string> manager, List<string> candidates)
        {
            BuildLayout(manager);
            InitHoleCandidate(candidates);
            
            var holes = _holeDomains.Keys.ToList();
            foreach (var pos in holes)
            {
                SpreadFromCell(pos, manager);
            }

            return new CSPResult(_holeDomains, manager);
        }

        public void BuildLayout(CandidateDomainManager<RowCol, string> manager)
        {
            var layouts = manager.Layouts.ToArray();
            _posToRelatedLayouts = ExpressionLayoutGraphUtils.BuildPosToExprMap(layouts);

            foreach (var layout in layouts)
            {
                var table = manager.Get(layout);
                var domain = table.Domain;
                _layoutToHoles[layout.Id.Value] = domain.Keys.ToHashSet();

                _idToLayouts[layout.Id.Value] = layout;
            }
        }

        public void InitHoleCandidate(List<string> candidates)
        {
            foreach (var pos in _posToRelatedLayouts.Keys)
            {
                _holeDomains[pos] = candidates.ToHashSet();
            }
        }
        
        private void SpreadFromCell(RowCol cell, CandidateDomainManager<RowCol, string> manager)
        {
            var effectLayoutIds = new HashSet<string>();
            
            var relatedLayoutIds = _posToRelatedLayouts[cell];
            foreach (var relatedId in relatedLayoutIds)
            {
                var table = manager.Get(relatedId);
                var domain = table.Domain;
                
                var holes = _layoutToHoles[relatedId];
                foreach (var hole in holes)
                {
                    var holeDomain = _holeDomains[hole];
                    var oldCount = holeDomain.Count;
                    holeDomain.IntersectWith(domain[hole]);
                    var updatedCount = holeDomain.Count;

                    if (updatedCount != oldCount)
                    {
                        effectLayoutIds.Add(relatedId);
                    }
                }
            }

            if (effectLayoutIds.Count > 0)
            {
                foreach (var id in effectLayoutIds)
                {
                    SpreadFromLayout(id, manager);
                }
             
            }
            
        }

        private void SpreadFromLayout(string layoutId, CandidateDomainManager<RowCol, string> manager)
        {
            var effectHoles = new HashSet<RowCol>();
            
            var table = manager.Get(layoutId);
            var domain = table.Domain;

            foreach (var hole in domain.Keys)
            {
                var holeDomain = _holeDomains[hole];
                var oldCount = holeDomain.Count;
                holeDomain.IntersectWith(domain[hole]);
                var updatedCount = holeDomain.Count;

                if (updatedCount != oldCount)
                {
                    effectHoles.Add(hole);
                }
            }

            if (effectHoles.Count > 0)
            {
                // 更新table
                table = table.Where((candidateRow) =>
                {
                    
                    var rowToValue = candidateRow.ToDictionary();
                    foreach (var rowCol in rowToValue.Keys)
                    {
                        if (!effectHoles.Contains(rowCol)) continue;
                        
                        var holeAllowValues = _holeDomains[rowCol];
                        if (!holeAllowValues.Contains(rowToValue[rowCol])) return false;
                    }
                    return true;
                });
                manager.Add(_idToLayouts[layoutId], table);


                foreach (var effectHole in effectHoles)
                {
                    SpreadFromCell(effectHole, manager  );
                }
            }
            
        }
    }