namespace Test;

[TestClass]
public class Tests {
    [TestMethod]
    public void Test1() {
        var solution = "" +
                       " 1──2 1\n" +
                       "2──1│ │\n" +
                       "│ 2─6═5\n" +
                       "│ │ ║ ║\n" +
                       "3─4═4 ║\n" +
                       "│1─2──3\n" +
                       "2─2─1  ";
        // no cheating
        var puzzle = solution
            .Replace("─", " ")
            .Replace("═", " ")
            .Replace("│", " ")
            .Replace("║", " ");
        var board = new Hashiwokakero(puzzle);
        board.Solve();
        Assert.IsTrue(board.IsSolved());
        Assert.AreEqual(solution, board.Solution());
    }

    [TestMethod]
    public void Test2() {
        var solution = "" +
                       " 2──4─2\n" +
                       "1│ 1║ │\n" +
                       "││ │4═4\n" +
                       "│4═6═3│\n" +
                       "2│1│ ││\n" +
                       "│1│1 │1\n" +
                       "2─3──2 ";
        // no cheating
        var puzzle = solution
            .Replace("─", " ")
            .Replace("═", " ")
            .Replace("│", " ")
            .Replace("║", " ");
        var board = new Hashiwokakero(puzzle);
        board.Solve();
        Assert.IsTrue(board.IsSolved());
        Assert.AreEqual(solution, board.Solution());
    }
}