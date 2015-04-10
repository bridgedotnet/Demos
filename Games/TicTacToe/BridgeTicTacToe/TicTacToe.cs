using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;
using System;

namespace BridgeTicTacToe
{
    public class TicTacToe
    {
        #region Types
        private enum Turn
        {
            Human,
            Computer
        }

        private enum Player
        {
            Human,
            Computer,
            None
        }
        #endregion

        #region Variables
        private const string human = "x";
        private const string computer = "o";

        private static Turn turn = Turn.Human;

        // Tic-Tac-Toe 3x3 board
        private static string[,] board = new string[3, 3];

        private static string[,] boardMap = new string[,]
        {
            { "a1", "a2", "a3" },
            { "b1", "b2", "b3" },
            { "c1", "c2", "c3" }
        };

        private static Player winner = Player.None;

        private static int winsCount = 0;
        private static int defeatsCount = 0;
        private static int tiesCount = 0;

        private enum ScoreType
        {
            Win,
            Defeat,
            Tie
        }

        #endregion

        #region Game Logic

        private static void GameFlow()
        {
            var hmn = BridgeTicTacToe.TicTacToe.human;

            var ongoingGame = BridgeTicTacToe.TicTacToe.winner == Player.None;
            var emptySlot = string.IsNullOrWhiteSpace(jQuery.This.Text());

            if (jQuery.This.Is("td") && ongoingGame && turn == Turn.Human && emptySlot)
            {
                jQuery.This.Text(hmn);
                BridgeTicTacToe.TicTacToe.BoardCheck();
                turn = Turn.Computer;

                if (!CheckWin())
                {
                    BridgeTicTacToe.TicTacToe.CompMove();
                    BridgeTicTacToe.TicTacToe.BoardCheck();
                    BridgeTicTacToe.TicTacToe.CheckWin();
                }
            }
        }

        /// <summary>
        /// Initializes the game flow.
        /// </summary>
        [Ready]
        private static void PageReady()
        {
            jQuery.Select("#restart").Click(BridgeTicTacToe.TicTacToe.ClearBoard);
            jQuery.Select("td").Click(BridgeTicTacToe.TicTacToe.GameFlow);

            BridgeTicTacToe.TicTacToe.GameFlow();

            jQuery.Select("#gameStatus").Css("background", "#bef");
            jQuery.Select("#gameStatus").Text("Make your move!");

            jQuery.Select("#restart").Css("visibility", "hidden");

            Console.Log("Upper bounds: " + board.GetUpperBound(0)); // FIXME: This returns NaN!?!?!?
        }

        /// <summary>
        /// compMove's basic AI checks if there's a winning sequence, if there's a way to block
        /// lose, or randomly picks a cell.
        /// </summary>
        private static void CompMove()
        {
            var cpu = BridgeTicTacToe.TicTacToe.computer;
            var hmn = BridgeTicTacToe.TicTacToe.human;

            var brd = BridgeTicTacToe.TicTacToe.board;
            var bm = BridgeTicTacToe.TicTacToe.boardMap;

            if (turn == Turn.Human)
            {
                return;
            }

            if (!FillGap(cpu) && !FillGap(hmn))
            {
                var freeCellList = new string[0];

                for (var i = 0; i <= brd.GetUpperBound(0); i++)
                {
                    for (var j = 0; j <= brd.GetUpperBound(1); j++)
                    {
                        if (string.IsNullOrWhiteSpace(brd[i, j]))
                        {
                            freeCellList.Push(bm[i, j]);
                        }
                    }
                }

                if (freeCellList.Length > 0)
                {
                    var chosenCell = (int)Math.Floor(Math.Random() * freeCellList.Length);

                    jQuery.Select("#" + freeCellList[chosenCell]).Text(cpu);
                }
            }

            // Finished turn. Next turn will be human's.
            turn = Turn.Human;
        }

        private static bool FillGap(string CheckChar)
        {
            var brd = BridgeTicTacToe.TicTacToe.board;
            var bm = BridgeTicTacToe.TicTacToe.boardMap;

            var myCheckMark = BridgeTicTacToe.TicTacToe.computer;

            // [i,j] positions of component spaces to fill a row and win
            int iCmp1;
            int iCmp2;
            int jCmp1;
            int jCmp2;

            string cmp1;
            string cmp2;

            var iBound = brd.GetUpperBound(0);
            var jBound = brd.GetUpperBound(1);

            var iCount = iBound + 1;
            var jCount = jBound + 1;

            for (var i = 0; i <= iBound; i++)
            {
                iCmp1 = (i + 1) % iCount;
                iCmp2 = (i + 2) % iCount;
                for (var j = 0; j <= jBound; j++)
                {

                    // If the inspected cell is empty
                    if (string.IsNullOrWhiteSpace(brd[i, j]))
                    {
                        jCmp1 = (j + 1) % jCount;
                        jCmp2 = (j + 2) % jCount;

                        // Check if the column has two CheckChars already
                        cmp1 = brd[iCmp1, j];
                        cmp2 = brd[iCmp2, j];

                        if (cmp1 == CheckChar && cmp2 == CheckChar)
                        {
                            jQuery.Select("#" + bm[i, j]).Text(myCheckMark);

                            return true;
                        }

                        // Check if the line has two CheckChars already
                        cmp1 = brd[i, jCmp1];
                        cmp2 = brd[i, jCmp2];

                        if (cmp1 == CheckChar && cmp2 == CheckChar)
                        {
                            jQuery.Select("#" + bm[i, j]).Text(myCheckMark);

                            return true;
                        }

                        // If corners, check diagonals
                        if ((i == 0 || i == iBound) && (j == 0 || j == jBound))
                        {

                            if (i == j)
                            {
                                // For i=j (0 or 2), we check cmps (1,1), (2,2) or (0,0), (1,1) resp.
                                cmp1 = brd[iCmp1, jCmp1];
                                cmp2 = brd[iCmp2, jCmp2];
                            }
                            else
                            {
                                // For i!=j (0,2 or 2,0) we check cmps (1,2), (2,1) or (0,1), (1,0)
                                cmp1 = brd[iCmp1, jCmp2];
                                cmp2 = brd[iCmp2, jCmp1];
                            }

                            if (cmp1 == CheckChar && cmp2 == CheckChar)
                            {
                                jQuery.Select("#" + bm[i, j]).Text(myCheckMark);

                                return true;
                            }
                        }
                        else
                        {
                            // If not corners and i=j (1,1, center), then check both diagonals
                            if (i == j)
                            {
                                if ((brd[0, 0] == CheckChar && brd[2, 2] == CheckChar) ||
                                    (brd[0, 2] == CheckChar && brd[2, 0] == CheckChar))
                                {
                                    jQuery.Select("#" + bm[i, j]).Text(myCheckMark);

                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Tracks checked cells in the game
        /// </summary>
        private static void BoardCheck()
        {
            var brd = BridgeTicTacToe.TicTacToe.board;
            var bm = BridgeTicTacToe.TicTacToe.boardMap;

            for (var i = 0; i <= brd.GetUpperBound(0); i++)
            {
                for (var j = 0; j <= brd.GetUpperBound(1); j++)
                {
                    BridgeTicTacToe.TicTacToe.board[i, j] = jQuery.Select("#" + bm[i, j]).Text();
                }
            }
        }

        private static bool CheckWin()
        {
            var brd = BridgeTicTacToe.TicTacToe.board;

            var rowMatch = new bool[]
            {
                brd[0, 0] == brd[0, 1] && brd[0, 1] == brd[0, 2],
                brd[1, 0] == brd[1, 1] && brd[1, 1] == brd[1, 2],
                brd[2, 0] == brd[2, 1] && brd[2, 1] == brd[2, 2]
            };

            var colMatch = new bool[]
            {
                brd[0, 0] == brd[1, 0] && brd[1, 0] == brd[2, 0],
                brd[0, 1] == brd[1, 1] && brd[1, 1] == brd[2, 1],
                brd[0, 2] == brd[1, 2] && brd[1, 2] == brd[2, 2]
            };

            var dgnMatch = new bool[] {
                brd[0, 0] == brd[1, 1] && brd[1, 1] == brd[2, 2],
                brd[0, 2] == brd[1, 1] && brd[1, 1] == brd[2, 0]
            };

            foreach (var mark in new string[] { human, computer })
            {
                if (
                    BridgeTicTacToe.TicTacToe.winner == Player.None &&
                    ((rowMatch[0] || colMatch[0]) && brd[0, 0] == mark) ||
                    (rowMatch[1] && brd[1, 0] == mark) ||
                    (rowMatch[2] && brd[2, 0] == mark) ||
                    (colMatch[1] && brd[0, 1] == mark) ||
                    (colMatch[2] && brd[0, 2] == mark) ||
                    ((dgnMatch[0] || dgnMatch[1]) && brd[1, 1] == mark)
                   )
                {
                    BridgeTicTacToe.TicTacToe.WinAlert(mark);
                }
            }

            var draw = true;

            for (var i = 0; i <= brd.GetUpperBound(0); i++)
            {
                for (var j = 0; j <= brd.GetUpperBound(1); j++)
                {
                    if (string.IsNullOrWhiteSpace(brd[i, j]))
                    {
                        draw = false;
                    }
                }
            }

            if (draw)
            {
                BridgeTicTacToe.TicTacToe.WinAlert("none");

                return true;
            }

            return (winner != Player.None);
        }

        /// <summary>
        /// Declares who won the match.
        /// </summary>
        private static void WinAlert(string who)
        {
            Player whoWon;

            if (who == human)
            {
                whoWon = Player.Human;

                jQuery.Select("#gameStatus").Css("background", "#cfc");
                jQuery.Select("#gameStatus").Text("Victory is yours!");

                BridgeTicTacToe.TicTacToe.updateScoreBoard(ScoreType.Win);
            }
            else if (who == computer)
            {
                whoWon = Player.Computer;

                jQuery.Select("#gameStatus").Css("background", "#fbb");
                jQuery.Select("#gameStatus").Text("Better luck next time!");

                BridgeTicTacToe.TicTacToe.updateScoreBoard(ScoreType.Defeat);
            }
            else
            {
                whoWon = Player.None;

                jQuery.Select("#gameStatus").Css("background", "#eef");
                jQuery.Select("#gameStatus").Text("Cat's game!");

                BridgeTicTacToe.TicTacToe.updateScoreBoard(ScoreType.Tie);
            }

            BridgeTicTacToe.TicTacToe.winner = whoWon;

            jQuery.Select("#restart").Css("visibility", "visible");
        }

        private static void ClearBoard()
        {
            var iBound = BridgeTicTacToe.TicTacToe.board.GetUpperBound(0);
            var jBound = BridgeTicTacToe.TicTacToe.board.GetUpperBound(1);

            var bm = BridgeTicTacToe.TicTacToe.boardMap;

            for (var i = 0; i <= iBound; i++)
            {
                for (var j = 0; j <= jBound; j++)
                {
                    BridgeTicTacToe.TicTacToe.board[i, j] = String.Empty;

                    jQuery.Select("#" + bm[i, j]).Text(String.Empty);
                }
            }

            BridgeTicTacToe.TicTacToe.winner = Player.None;

            jQuery.Select("#gameStatus").Css("background", "#bef");
            jQuery.Select("#gameStatus").Text("Make your move!");

            BridgeTicTacToe.TicTacToe.GameFlow();

            if (turn == Turn.Computer)
            {
                BridgeTicTacToe.TicTacToe.CompMove();
            }

            jQuery.Select("#restart").Css("visibility", "hidden");
        }

        private static void updateScoreBoard(ScoreType which)
        {
            int newValue = 0;

            // There's no Enum.GetName() support yet, so we just map enum to string names
            string scoreName = "";

            switch (which) {
                case ScoreType.Win:
                    newValue = ++BridgeTicTacToe.TicTacToe.winsCount;
                    scoreName = "win";
                    break;
                case ScoreType.Defeat:
                    newValue = ++BridgeTicTacToe.TicTacToe.defeatsCount;
                    scoreName = "defeat";
                    break;
                case ScoreType.Tie:
                    newValue = ++BridgeTicTacToe.TicTacToe.tiesCount;
                    scoreName = "tie";
                    break;
            }

            if (newValue != 0)
            {
                jQuery.Select("#" + scoreName + "Counter").Text(newValue.ToString());
            }
        }

        #endregion
    }
}
