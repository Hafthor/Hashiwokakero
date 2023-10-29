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
s = "" +
    " 2--4-2\n" +
    "1| 1# |\n" +
    "|| |4=4\n" +
    "|4=6=3|\n" +
    "2|1| ||\n" +
    "|1|1 |1\n" +
    "2-3--2 ";
// TODO: why does this result in:
//  2──4─2
// 1? 1║ │
// ││ │4═4
// │4═6═3│
// 2?1│ ││
// │1?1 ?1
// 2─3──2 
// instead of:
//  2──4─2
// 1│ 1║ │
// ││ │4═4
// │4═6═3│
// 2│1│ ││
// │1│1 │1
// 2─3──2 
// note that only single vertical bridges are affected
// note the first bridge affect, shows interference, but not the interfering bridge
var board = new Hashiwokakero(s);
board.Solve();
board.Print();
return board.IsSolved() ? 0 : 1;