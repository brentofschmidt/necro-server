using System.Numerics;
using Game.Core.World;

namespace Game.Server.World;

// Flat-cell spatial index per the architecture doc. Cell size ≈ interest
// radius; a Query returns the 3x3 (or 5x5) block of cells around the
// center. Lives on the tick thread — not thread-safe by design.
//
// Phase 4 implementation: dumb and correct. Profile-driven optimization
// (hierarchical grids, hysteresis, LOS culling) waits until something
// is actually slow.
public sealed class SpatialGrid
{
    public const float CellSize = 50f;

    private readonly Dictionary<(int, int), HashSet<EntityId>> _cells = new();
    private readonly Dictionary<EntityId, (int, int)> _entityCell = new();

    public void Update(EntityId id, Vector3 position)
    {
        var newCell = CellOf(position);
        if (_entityCell.TryGetValue(id, out var oldCell))
        {
            if (oldCell == newCell) return;
            if (_cells.TryGetValue(oldCell, out var oldSet))
            {
                oldSet.Remove(id);
                if (oldSet.Count == 0) _cells.Remove(oldCell);
            }
        }
        if (!_cells.TryGetValue(newCell, out var set))
        {
            set = new HashSet<EntityId>();
            _cells[newCell] = set;
        }
        set.Add(id);
        _entityCell[id] = newCell;
    }

    public void Remove(EntityId id)
    {
        if (!_entityCell.TryGetValue(id, out var cell)) return;
        if (_cells.TryGetValue(cell, out var set))
        {
            set.Remove(id);
            if (set.Count == 0) _cells.Remove(cell);
        }
        _entityCell.Remove(id);
    }

    // Returns every entity in the 3x3 cell block around `center`. Slight
    // over-fetch (corners of the 3x3 block extend past the literal radius)
    // is the whole point — bounded set size, no per-entity distance math.
    public IEnumerable<EntityId> Query(Vector3 center)
    {
        var (cx, cz) = CellOf(center);
        for (var dx = -1; dx <= 1; dx++)
        for (var dz = -1; dz <= 1; dz++)
        {
            if (_cells.TryGetValue((cx + dx, cz + dz), out var set))
            {
                foreach (var id in set) yield return id;
            }
        }
    }

    private static (int, int) CellOf(Vector3 pos)
        => ((int)System.Math.Floor(pos.X / CellSize), (int)System.Math.Floor(pos.Z / CellSize));
}
