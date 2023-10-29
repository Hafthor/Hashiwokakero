using System.Diagnostics;

public class Hashiwokakero {
    private const char HORZ_ONE = '\u2500', HORZ_TWO = '\u2550', VERT_ONE = '\u2502', VERT_TWO = '\u2551';
    private readonly int height, width;
    private readonly List<Island> islands = new();
    private readonly List<Bridge> bridges = new();
    private static readonly Coord[] directionsNSEW = { new(0, -1), new(0, 1), new(-1, 0), new(1, 0) };

    public Hashiwokakero(string s) {
        var lines = s.Split('\n');
        height = lines.Length;
        width = lines[0].Length;
        // set islands
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (char.IsDigit(lines[y][x]))
                    islands.Add(new Island(new Coord(x, y), lines[y][x] - '0'));

        // build all possible bridges to a value of 0
        foreach (var island in islands) {
            for (int directionIndex = 0; directionIndex < 4; directionIndex++) {
                var direction = directionsNSEW[directionIndex];
                if (island.bridgesNSEW[directionIndex] is not null) continue;
                var nearest = FindNearestIsland(island, direction);
                if (nearest is null) continue;
                var min = Math.Min(2, Math.Min(island.value, nearest.value));
                var bridge = new Bridge(island, nearest);
                island.bridgesNSEW[directionIndex] = bridge;
                nearest.bridgesNSEW[directionIndex ^ 1] = bridge;
                if (min == 0) {
                    Debugger.Break();
                }
                bridge.SetMinMaxCount(0, min);
                bridges.Add(bridge);
            }
        }

        // find interfering bridges
        foreach (var bridge in bridges) {
            var rect = new Rect(bridge.tl.coord, bridge.br.coord);
            foreach (var otherBridge in bridges) {
                if (bridge == otherBridge) continue;
                var otherRect = new Rect(otherBridge.tl.coord, otherBridge.br.coord);
                if (!rect.Intersects(otherRect)) continue;
                bridge.interfering.Add(otherBridge);
                otherBridge.interfering.Add(bridge);
            }
        }
    }

    public bool IsSolved() {
        return islands.All(i => i.value == i.Count()) && // check that all islands have the correct number of bridges
               bridges.All(b => b.value == 0 || !b.AnyInterfering) && // check that no bridges interfere
               NetworkCount() == 1; // check that all islands are connected
    }

    public int NetworkCount() {
        HashSet<Island> visited = new();
        int count = 0;
        foreach (var island in islands)
            if (!visited.Contains(island))
                count += ExploreIsland(island, visited);
        return count;
    }

    private int ExploreIsland(Island island, HashSet<Island> visited) {
        visited.Add(island);
        foreach (var bridge in island.Bridges()) {
            var other = bridge.OtherIsland(island);
            if (!visited.Contains(other))
                ExploreIsland(other, visited);
        }
        return 1;
    }
    
    private Island FindNearestIsland(Island from, Coord direction) {
        return islands
            .Where(i => Math.Sign(i.coord.x - from.coord.x) == direction.x && Math.Sign(i.coord.y - from.coord.y) == direction.y)
            .MinBy(i => Math.Abs(i.coord.x - from.coord.x) + Math.Abs(i.coord.y - from.coord.y))!;
    }

    public void Solve() {
        for (int i = 0; i < 100; i++) {
            bool didSomething = false;
            foreach (var bridge in bridges)
                didSomething |= Solve(bridge);
            foreach (var island in islands)
                didSomething |= Solve(island);
            if (!didSomething) break;
        }
    }

    private bool Solve(Bridge bridge) {
        if (bridge.tl.value == bridge.tl.Count() || bridge.br.value == bridge.br.Count()) // if either island solved
            return bridge.SetCount(bridge.MinPossible);
        
        // do some more smart stuff here
        return false;
    }

    private bool Solve(Island island) {
        var count = island.Count();
        var remaining = island.value - count;
        if (remaining == 0) return false;

        var freeBridges = island.FreeBridges().ToList();
        var sumOfMax = freeBridges.Select(b => b.MaxPossible).Sum();

        if (island.value == sumOfMax) {
            bool didSomething = false;
            foreach (var bridge in freeBridges)
                didSomething |= bridge.SetCount(bridge.MaxPossible);
            return didSomething;
        }
        if (island.value > (freeBridges.Count - 1) * 2) {
            bool didSomething = false;
            foreach (var bridge in freeBridges)
                didSomething |= bridge.SetMinMaxCount(1, 2);
            return didSomething;
        }
        
        var availableBridges = freeBridges.Where(b => b.MaxPossible > b.MinPossible).ToList();
        if (availableBridges.Count == 1)
            return availableBridges[0].SetMinMaxCount(remaining, 2);

        // do some more smart stuff here
        return false;
    }

    public string Solution() {
        var grid = new char[height][];
        // initialize grid
        for (int y = 0; y < height; y++) {
            grid[y] = new char[width];
            for (int x = 0; x < width; x++)
                grid[y][x] = ' ';
        }
        // set islands
        foreach (var island in islands)
            grid[island.coord.y][island.coord.x] = (char)('0' + island.value);
        // set bridges
        foreach (var bridge in bridges) {
            Coord tl = bridge.tl.coord, br = bridge.br.coord;
            if (bridge.IsVertical) {
                var c = bridge.value switch { 1 => VERT_ONE, 2 => VERT_TWO, _ => ' ' };
                for (int y = tl.y + 1; y < br.y; y++)
                    grid[y][tl.x] = grid[y][tl.x] == ' ' ? c : '?';
            } else {
                var c = bridge.value switch { 1 => HORZ_ONE, 2 => HORZ_TWO, _ => ' ' };
                for (int x = tl.x + 1; x < br.x; x++)
                    grid[tl.y][x] = grid[tl.y][x] == ' ' ? c : '?';
            }
        }
        return string.Join('\n', grid.Select(line => new string(line)));
    }
    
    public void Print() => Console.WriteLine(Solution());

    public record Coord(int x, int y) {
        public readonly int x = x, y = y;
    }

    public record Rect {
        public readonly Coord tl, br;

        public Rect(Coord a, Coord b) {
            (tl, br) = a.x > b.x || a.y > b.y ? (b, a) : (a, b);
        }

        public bool Intersects(Rect other) {
            return tl.x < other.br.x && br.x > other.tl.x && tl.y < other.br.y && br.y > other.tl.y;
        }
    }

    public class Island {
        public readonly Coord coord;
        public readonly int value;
        public readonly Bridge[] bridgesNSEW = new Bridge[4];

        public Island(Coord coord, int value) {
            this.coord = coord;
            this.value = value;
        }

        public IEnumerable<Bridge> Bridges() => bridgesNSEW.Where(b => b is not null);

        public IEnumerable<Bridge> FreeBridges() => Bridges().Where(b => !b.AnyInterfering && b.MaxPossible > 0);

        public int Count() => Bridges().Select(b => b.value).Sum();
        
        public override string ToString() => $"Island({coord.x}, {coord.y}, {value})";
    }

    public class Bridge {
        public Island tl, br;
        public int value;
        public HashSet<Bridge> interfering = new();

        private int minPossible;
        private int maxPossible = 2;
        public int MinPossible => AnyInterfering ? 0 : minPossible;
        public int MaxPossible => AnyInterfering ? 0 : maxPossible;

        public bool AnyInterfering => interfering.Any(b => b.value > 0);
        public bool IsVertical => tl.coord.x == br.coord.x;

        public Bridge(Island a, Island b) {
            (tl, br) = a.coord.x < b.coord.x || a.coord.y < b.coord.y ? (a, b) : (b, a);
        }

        public bool SetCount(int c) {
            if (c > 0 && AnyInterfering) throw new Exception("interfering bridges");
            if (c > maxPossible || c < minPossible) throw new Exception("invalid count");
            var didSomething = value != c || minPossible != c || maxPossible != c;
            value = minPossible = maxPossible = c;
            //if (didSomething) Console.WriteLine("SetCount: " + this);
            return didSomething;
        }

        public bool SetMinMaxCount(int min, int max) {
            int newMin = Math.Max(min, minPossible), newMax = Math.Min(max, maxPossible);
            if (newMin > newMax) throw new Exception("min > max");
            var didSomething = min != minPossible || max != maxPossible;
            value = minPossible = newMin;
            maxPossible = newMax;
            //if (didSomething) Console.WriteLine("SetMinMaxCount: " + this);
            return didSomething;
        }

        public Island OtherIsland(Island island) => tl == island ? br : tl;

        public override string ToString() => tl + " <-> " + br + " (" + minPossible + "," + maxPossible + ")";
    }
}