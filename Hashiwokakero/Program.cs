// string shows solution, but it is not used
// you can replace paths with spaces and it will still work
var s = "" +
        " 1--2 1\n" +
        "2--1| |\n" +
        "| 2-6=5\n" +
        "| | # #\n" +
        "3-4=4 #\n" +
        "|1-2--3\n" +
        "2-2-1  ";
var board = new Hashiwokakero(s);
board.Solve();
board.Print();
return board.IsSolved() ? 0 : 1;