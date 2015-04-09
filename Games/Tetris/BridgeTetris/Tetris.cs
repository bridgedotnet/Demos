/*
 * DISCLAIMER: This code was ported from JavaScript version made avalable by Jake Gordon on:
 * Website/howto: http://codeincomplete.com/posts/2011/10/10/javascript_tetris/
 * Github: https://github.com/jakesgordon/javascript-tetris/blob/master/index.html
 *
 * License for this code replicates original code license by obligation, and is as follows:

Copyright (c) 2011, 2012, 2013 Jake Gordon and contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

 *
 */
using Bridge;
using Bridge.Html5;
using System;
using System.Collections.Generic;

namespace BridgeTetris
{
    public class Tetris
    {
        private static void LoadPlayArea()
        {
            var menuDiv = new DivElement
            {
                Id = "menu"
            };

            var pressStartTitle = new AnchorElement
            {
                Href = "javascript:BridgeTetris.Tetris.play()",
                InnerHTML = "Press Space to Play."
            };

            menuDiv.AppendChild(BridgeTetris.Tetris.InsideParagraph(pressStartTitle, "start"));

            var nextPieceCanvas = new CanvasElement
            {
                Id = "upcoming"
            };

            menuDiv.AppendChild(BridgeTetris.Tetris.InsideParagraph(nextPieceCanvas));

            var scoreParagraph = new List<Node>();
            scoreParagraph.Add(new LabelElement
            {
                InnerHTML = "score "
            });

            scoreParagraph.Add(new SpanElement
            {
                Id = "score",
                InnerHTML = "00000"
            });

            menuDiv.AppendChild(BridgeTetris.Tetris.InsideParagraph(scoreParagraph));

            var rowsParagraph = new List<Node>();
            rowsParagraph.Add(new LabelElement
            {
                InnerHTML = "rows "
            });

            rowsParagraph.Add(new SpanElement
            {
                Id = "rows",
                InnerHTML = "0"
            });

            menuDiv.AppendChild(BridgeTetris.Tetris.InsideParagraph(rowsParagraph));

            var tetrisCourtCanvas = new CanvasElement
            {
                Id = "canvas",
                Width = 200,
                Height = 400
            };

            var tetrisDiv = new DivElement
            {
                Id = "tetris"
            };

            tetrisDiv.AppendChild(menuDiv);
            tetrisDiv.AppendChild(tetrisCourtCanvas);

            Document.Body.AppendChild(tetrisDiv);
        }

        private static Node InsideParagraph(List<Node> elementList, string paragraphID = null)
        {
            var currentParagraphElement = new ParagraphElement();

            if (!String.IsNullOrWhiteSpace(paragraphID))
            {
                currentParagraphElement.Id = paragraphID;
            }

            foreach (var element in elementList)
            {
                currentParagraphElement.AppendChild(element);
            }

            return currentParagraphElement;
        }

        private static Node InsideParagraph(Node element, string paragraphID = null)
        {
            return BridgeTetris.Tetris.InsideParagraph(new List<Node> { element }, paragraphID);
        }

        #region base helper methods

        // Note: This was a shorthand for GetElementById in the original code.
        private static Element Get(string id)
        {
            return Document.GetElementById(id);
        }

        private static void Hide(string id)
        {
            Get(id).Style.Visibility = Visibility.Hidden;
        }

        private static void Show(string id)
        {
            Get(id).Style.Visibility = Visibility.Inherit;
        }

        private static void Html(string id, string html)
        {
            Get(id).InnerHTML = html;
        }

        private static double Timestamp()
        {
            return new Date().GetTime();
        }

        private static double Random(int lowerBound, int uppeBound)
        {
            return (lowerBound + (Math.Random() * (uppeBound - lowerBound)));
        }

        #endregion

        #region game constants

        private static class Key
        {
            public const short Escape = 27;
            public const short Spacebar = 32;
            public const short LeftArrow = 37;
            public const short UpArrow = 38;
            public const short RightArrow = 39;
            public const short DownArrow = 40;
        }

        private static class Direction
        {
            public const short ToUp = 0;
            public const short ToRight = 1;
            public const short ToDown = 2;
            public const short Toleft = 3;
            public const short First = 0;
            public const short Last = 3;
        };

        private static CanvasElement canvas;
        private static CanvasElement upcomingPieceCanvas;

        private static CanvasRenderingContext2D canvasContext;
        private static CanvasRenderingContext2D upcomingPieceCanvasContext;

        private static void LoadCanvasContext()
        {
            // FIXME: Shouldn't it allow returning canvasElement without a cast??
            BridgeTetris.Tetris.canvas = Get("canvas").As<CanvasElement>();
            BridgeTetris.Tetris.upcomingPieceCanvas = Get("upcoming").As<CanvasElement>();

            BridgeTetris.Tetris.canvasContext = canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            BridgeTetris.Tetris.upcomingPieceCanvasContext = upcomingPieceCanvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
        }

        // how long before piece drops by 1 row (seconds)
        private static class Speed
        {
            public const float start = 0.6f;
            public const float decrement = 0.005f;
            public const float lowerBoundary = 0.1f;
        };

        private static int tetrisCourtWidth = 10;         // width of tetris court (in blocks)
        private static int tetrisCourtHeight = 20;        // height of tetris court (in blocks)
        private static int upcomingPreviewDimensions = 5; // width/heigth of upcoming preview (in blocks)

        #endregion

        #region tetris elements' abstraction classes
        private class Piece
        {
            public PieceType BlockType { get; set; }
            public short Orientation { get; set; }
            public int HorizontalPosition { get; set; }
            public int VerticalPosition { get; set; }
        }

        private class PieceType
        {
            public int Size { get; set; }
            public string Color { get; set; }
            public int[] Blocks { get; set; }
        }

        #endregion

        #region game variables (initialized during reset)

        private static int horizontalPixelDimension; // pixel width of a tetris block
        private static int verticalPixelDimension; // pixel height of a tetris block

        private static int currentScore; // the current score
        private static int incrementalInstantaneousScore; // the currently displayed score (catches up to score in small chunks like slot machine)
        
        private static int completedRowsCount; // number of completed rows in the current game

        // 2 dimensional array (nx*ny) representing tetris court - either empty block or occupied by a 'piece'
        private static BridgeTetris.Tetris.PieceType[,] tetrisCourt = new BridgeTetris.Tetris.PieceType[BridgeTetris.Tetris.tetrisCourtWidth, BridgeTetris.Tetris.tetrisCourtHeight];

        private static int[] userActionsQueue = new int[0]; // queue of user actions (inputs)

        private static BridgeTetris.Tetris.Piece currentPiece; // the current piece
        private static BridgeTetris.Tetris.Piece nextPiece; // the next piece

        private static bool isGamePlaying;   // true|false - game is in progress
        private static double playingTime; // time since starting the game
        private static double pieceAdvanceInterval; // how long before current piece drops by 1 row
        private static double currentTimeStamp; // current timestamp (used on frame())
        private static double previousTimeStamp; // last timestamp (used on frame())

        #endregion

        #region tetris pieces
        /*
         * blocks: each element represents a rotation of the piece (0, 90, 180, 270)
         *         each element is a 16bit integer where the 16 bits represent
         *         a 4x4 set of blocks, e.g. j.blocks[0] = 0x44C0
         *
         *   0100 = 0x4 << 3 = 0x4000
         *   0100 = 0x4 << 2 = 0x0400
         *   1100 = 0xC << 1 = 0x00C0
         *   0000 = 0x0 << 0 = 0x0000
         *           /\        ------
         *                     0x44C0
         */

        private static BridgeTetris.Tetris.PieceType iBlock = new BridgeTetris.Tetris.PieceType
        {
            Size = 4,
            Blocks = new int[]
            {
                0x0F00, // [ ]
                0x2222, // [ ]
                0x00F0, // [ ]
                0x4444  // [ ]
            },
            Color = "cyan"
        };

        private static BridgeTetris.Tetris.PieceType jBlock = new BridgeTetris.Tetris.PieceType
        {
            Size = 3,
            Blocks = new int[]
            {
                0x44C0, //    [ ]
                0x8E00, //    [ ]
                0x6440, // [ ][ ]
                0x0E20  //
            },
            Color = "blue"
        };

        private static BridgeTetris.Tetris.PieceType lBlock = new BridgeTetris.Tetris.PieceType
        {
            Size = 3,
            Blocks = new int[]
            {
                0x4460, // [ ]
                0x0E80, // [ ]
                0xC440, // [ ][ ]
                0x2E00  //
            },
            Color = "orange"
        };

        private static BridgeTetris.Tetris.PieceType oBlock = new BridgeTetris.Tetris.PieceType
        {
            Size = 2,
            Blocks = new int[]
            {
                0xCC00, //
                0xCC00, // [ ][ ]
                0xCC00, // [ ][ ]
                0xCC00  //
            },
            Color = "yellow"
        };

        private static BridgeTetris.Tetris.PieceType sBlock = new BridgeTetris.Tetris.PieceType
        {
            Size = 3,
            Blocks = new int[]
            {
                0x06C0, //
                0x8C40, //    [ ][ ]
                0x6C00, // [ ][ ]
                0x4620  //
            },
            Color = "green"
        };

        private static BridgeTetris.Tetris.PieceType tBlock = new BridgeTetris.Tetris.PieceType
        {
            Size = 3,
            Blocks = new int[]
            {
                0x0E40, //
                0x4C40, // [ ][ ][ ]
                0x4E00, //    [ ]
                0x4640  //
            },
            Color = "purple"
        };

        private static BridgeTetris.Tetris.PieceType zBlock = new BridgeTetris.Tetris.PieceType
        {
            Size = 3,
            Blocks = new int[]
            {
                0x0C60, //
                0x4C80, // [ ][ ]
                0xC600, //    [ ][ ]
                0x2640  //
            },
            Color = "red"
        };

        #endregion

        /// <summary>
        /// do the bit manipulation and iterate through each
        /// occupied block (x,y) for a given piece
        /// </summary>
        /// <param name="pieceType"></param>
        /// <param name="horizontalPositon"></param>
        /// <param name="verticalPosition"></param>
        /// <param name="direction"></param>
        /// <param name="fn"></param>
        private static Tuple<int, int>[] Eachblock(BridgeTetris.Tetris.PieceType pieceType, int horizontalPositon, int verticalPosition, short direction)
        {
            int row = 0;
            int column = 0;
            int blocks = pieceType.Blocks[direction];

            Tuple<int, int>[] result = new Tuple<int, int>[0];

            for (int bit = 0x8000; bit > 0; bit = bit >> 1)
            {
                if ((blocks & bit) > 0)
                {
                    result.Push(new Tuple<int, int>(horizontalPositon + column, verticalPosition + row));
                }

                if (++column == 4)
                {
                    column = 0;
                    ++row;
                }
            }

            return result;
        }

        // this is the function delegated to eachblock from occupied:
        private static bool PieceCanFit(int horizontalPosition, int verticalPosition)
        {
            return (horizontalPosition < 0 || horizontalPosition >= tetrisCourtWidth ||
                    verticalPosition < 0   || verticalPosition >= tetrisCourtHeight ||
                    BridgeTetris.Tetris.GetBlock(horizontalPosition, verticalPosition) != null);
        }

        // check if a piece can fit into a position in the grid
        private static bool Occupied(PieceType pieceType, int horizontalPosition, int verticalPosition, short direction)
        {
            var matchingCells = BridgeTetris.Tetris.Eachblock(pieceType, horizontalPosition, verticalPosition, direction);

            foreach (var tuple in matchingCells)
            {
                if (BridgeTetris.Tetris.PieceCanFit(tuple.Item1, tuple.Item2))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool Unoccupied(PieceType pieceType, int horizontalPosition, int verticalPosition, short direction)
        {
            return !BridgeTetris.Tetris.Occupied(pieceType, horizontalPosition, verticalPosition, direction);
        }

        // start with 4 instances of each piece and
        // pick randomly until the 'bag is empty'
        private static BridgeTetris.Tetris.PieceType[] pieces;

        private static BridgeTetris.Tetris.Piece RandomPiece()
        {
            BridgeTetris.Tetris.pieces = new BridgeTetris.Tetris.PieceType[]
            {
                iBlock,
                iBlock,
                iBlock,
                iBlock,

                jBlock,
                jBlock,
                jBlock,
                jBlock,

                lBlock,
                lBlock,
                lBlock,
                lBlock,

                oBlock,
                oBlock,
                oBlock,
                oBlock,

                sBlock,
                sBlock,
                sBlock,
                sBlock,

                tBlock,
                tBlock,
                tBlock,
                tBlock,

                zBlock,
                zBlock,
                zBlock,
                zBlock
            };

            // FIXME: This shouldn't need to be cast as Piece again
            var randomPieceType = (BridgeTetris.Tetris.PieceType)BridgeTetris.Tetris.pieces.Splice((int)BridgeTetris.Tetris.Random(0, BridgeTetris.Tetris.pieces.Length - 1), 1)[0];

            return new BridgeTetris.Tetris.Piece
            {
                Orientation = BridgeTetris.Tetris.Direction.ToUp,
                BlockType = randomPieceType,
                HorizontalPosition = (int)Math.Round(BridgeTetris.Tetris.Random(0, BridgeTetris.Tetris.tetrisCourtWidth - randomPieceType.Size)),
                VerticalPosition = 0
            };
        }

        #region GAME LOOP

        private static void Run()
        {
            BridgeTetris.Tetris.LoadCanvasContext();

            BridgeTetris.Tetris.AddEvents();

            var last = BridgeTetris.Tetris.currentTimeStamp = BridgeTetris.Tetris.Timestamp();

            BridgeTetris.Tetris.Resize(); // setup all our sizing information
            BridgeTetris.Tetris.Reset();  // reset the per-game variables
            BridgeTetris.Tetris.Frame();  // start the first frame
        }

        private static void Frame()
        {
            BridgeTetris.Tetris.currentTimeStamp = BridgeTetris.Tetris.Timestamp();

            // using requestAnimationFrame have to be able to handle large delta's caused when it 'hibernates' in a background or non-visible tab
            BridgeTetris.Tetris.Update(Math.Min(1, (BridgeTetris.Tetris.currentTimeStamp - BridgeTetris.Tetris.previousTimeStamp) / 1000.0));

            BridgeTetris.Tetris.Draw();

            BridgeTetris.Tetris.previousTimeStamp = BridgeTetris.Tetris.currentTimeStamp;

            Window.RequestAnimationFrame(animationFrame => BridgeTetris.Tetris.Frame());
        }

        private static void AddEvents()
        {
            Document.AddEventListener(EventType.KeyDown, BridgeTetris.Tetris.KeyDown, false);
            Document.AddEventListener(EventType.Resize, BridgeTetris.Tetris.Resize, false);
        }

        public static void Resize(Event evnt = null)
        {
            // set canvas logical size equal to its physical size
            BridgeTetris.Tetris.canvas.Width = BridgeTetris.Tetris.canvas.ClientWidth;
            BridgeTetris.Tetris.canvas.Height = BridgeTetris.Tetris.canvas.ClientHeight;

            BridgeTetris.Tetris.upcomingPieceCanvas.Width = BridgeTetris.Tetris.upcomingPieceCanvas.ClientWidth;
            BridgeTetris.Tetris.upcomingPieceCanvas.Height = BridgeTetris.Tetris.upcomingPieceCanvas.ClientHeight;

            // pixel size of a single tetris block
            BridgeTetris.Tetris.horizontalPixelDimension = BridgeTetris.Tetris.canvas.ClientWidth / BridgeTetris.Tetris.tetrisCourtWidth;
            BridgeTetris.Tetris.verticalPixelDimension = BridgeTetris.Tetris.canvas.ClientHeight / BridgeTetris.Tetris.tetrisCourtHeight;

            BridgeTetris.Tetris.Invalidate();
            BridgeTetris.Tetris.InvalidateNext();
        }

        public static void KeyDown(Event eventInformation)
        {
            var isKeyStrokeHandled = false;
            var keyboardEventInformation = eventInformation.As<KeyboardEvent>();

            if (BridgeTetris.Tetris.isGamePlaying)
            {
                switch (keyboardEventInformation.KeyCode)
                {
                    case BridgeTetris.Tetris.Key.LeftArrow:
                        BridgeTetris.Tetris.userActionsQueue.Push(BridgeTetris.Tetris.Direction.Toleft);
                        isKeyStrokeHandled = true;
                        break;
                    case BridgeTetris.Tetris.Key.RightArrow:
                        BridgeTetris.Tetris.userActionsQueue.Push(BridgeTetris.Tetris.Direction.ToRight);
                        isKeyStrokeHandled = true;
                        break;
                    case BridgeTetris.Tetris.Key.UpArrow:
                        BridgeTetris.Tetris.userActionsQueue.Push(BridgeTetris.Tetris.Direction.ToUp);
                        isKeyStrokeHandled = true;
                        break;
                    case BridgeTetris.Tetris.Key.DownArrow:
                        BridgeTetris.Tetris.userActionsQueue.Push(BridgeTetris.Tetris.Direction.ToDown);
                        isKeyStrokeHandled = true;
                        break;
                    case BridgeTetris.Tetris.Key.Escape:
                        BridgeTetris.Tetris.Lose();
                        isKeyStrokeHandled = true;
                        break;
                }
            }
            else
            {
                if (keyboardEventInformation.KeyCode == BridgeTetris.Tetris.Key.Spacebar)
                {
                    BridgeTetris.Tetris.Play();
                    isKeyStrokeHandled = true;
                }
            }

            if (isKeyStrokeHandled)
            {
                eventInformation.PreventDefault(); // prevent arrow keys from scrolling the page (supported in ie9+ and all other browsers)
            }
        }

        #endregion

        #region GAME LOGIC

        private static void Play()
        {
            BridgeTetris.Tetris.Hide("start");
            BridgeTetris.Tetris.Reset();
            BridgeTetris.Tetris.isGamePlaying = true;
        }

        private static void Lose()
        {
            BridgeTetris.Tetris.Show("start");
            BridgeTetris.Tetris.SetVisualScore();
            BridgeTetris.Tetris.isGamePlaying = false;
        }

        private static void SetVisualScore(int? scoreValue = null)
        {
            BridgeTetris.Tetris.incrementalInstantaneousScore = scoreValue ?? BridgeTetris.Tetris.currentScore;
            BridgeTetris.Tetris.InvalidateScore();
        }

        private static void SetScore(int scoreValue)
        {
            BridgeTetris.Tetris.currentScore = scoreValue;
            BridgeTetris.Tetris.SetVisualScore(scoreValue);
        }

        private static void AddScore(int scoreValue)
        {
            BridgeTetris.Tetris.currentScore += scoreValue;
        }

        private static void ClearScore()
        {
            BridgeTetris.Tetris.SetScore(0);
        }

        private static void ClearRows()
        {
            BridgeTetris.Tetris.SetRows(0);
        }

        private static void SetRows(int newRowsCount)
        {
            var speedMin = BridgeTetris.Tetris.Speed.lowerBoundary;
            var speedStart = BridgeTetris.Tetris.Speed.start;
            var speedDec = BridgeTetris.Tetris.Speed.decrement;
            var rowCount = BridgeTetris.Tetris.completedRowsCount;

            BridgeTetris.Tetris.completedRowsCount = newRowsCount;

            BridgeTetris.Tetris.pieceAdvanceInterval = Math.Max(speedMin, speedStart - (speedDec * rowCount));

            BridgeTetris.Tetris.InvalidateRows();
        }

        private static void AddRows(int rowsAmountToAdd)
        {
            BridgeTetris.Tetris.SetRows(BridgeTetris.Tetris.completedRowsCount + rowsAmountToAdd);
        }

        private static BridgeTetris.Tetris.PieceType GetBlock(int horizontalPosition, int verticalPosition)
        {
            BridgeTetris.Tetris.PieceType retval = null;

            if (horizontalPosition >= 0 && horizontalPosition < BridgeTetris.Tetris.tetrisCourtWidth &&
                BridgeTetris.Tetris.tetrisCourt.Length > ((horizontalPosition + 1) * BridgeTetris.Tetris.tetrisCourtWidth) &&
                verticalPosition > 0 && verticalPosition < BridgeTetris.Tetris.tetrisCourtHeight)
            {
                retval = BridgeTetris.Tetris.tetrisCourt[horizontalPosition, verticalPosition];
            }

            return retval;
        }

        private static void SetBlock(int horizontalPosition, int verticalPosition, BridgeTetris.Tetris.PieceType pieceType)
        {
            if (horizontalPosition >= 0 && horizontalPosition < BridgeTetris.Tetris.tetrisCourtWidth)
            {
                BridgeTetris.Tetris.tetrisCourt[horizontalPosition, verticalPosition] = pieceType;
            }

            BridgeTetris.Tetris.Invalidate();
        }

        private static void ClearBlocks()
        {
            BridgeTetris.Tetris.tetrisCourt = new BridgeTetris.Tetris.PieceType[BridgeTetris.Tetris.tetrisCourtWidth, BridgeTetris.Tetris.tetrisCourtHeight];
        }

        private static void ClearActions()
        {
            BridgeTetris.Tetris.userActionsQueue = new int[0];
        }

        private static void SetCurrentPiece(BridgeTetris.Tetris.Piece pieceType)
        {
            BridgeTetris.Tetris.currentPiece = pieceType ?? BridgeTetris.Tetris.RandomPiece();
            BridgeTetris.Tetris.Invalidate();
        }

        private static void SetNextPiece(BridgeTetris.Tetris.Piece pieceType = null)
        {
            BridgeTetris.Tetris.nextPiece = pieceType ?? BridgeTetris.Tetris.RandomPiece();
            BridgeTetris.Tetris.InvalidateNext();
        }

        private static void Reset()
        {
            BridgeTetris.Tetris.playingTime = 0;

            BridgeTetris.Tetris.ClearActions();
            BridgeTetris.Tetris.ClearBlocks();
            BridgeTetris.Tetris.ClearRows();
            BridgeTetris.Tetris.ClearScore();

            BridgeTetris.Tetris.SetCurrentPiece(BridgeTetris.Tetris.nextPiece);
            BridgeTetris.Tetris.SetNextPiece();
        }

        private static void Update(double timeDelta)
        {
            if (BridgeTetris.Tetris.isGamePlaying)
            {
                if (BridgeTetris.Tetris.incrementalInstantaneousScore < BridgeTetris.Tetris.currentScore)
                {
                    BridgeTetris.Tetris.SetVisualScore(BridgeTetris.Tetris.incrementalInstantaneousScore + 1);
                }

                BridgeTetris.Tetris.Handle(BridgeTetris.Tetris.userActionsQueue.Shift().As<int>()); // FIXME: Shift() should already return the type of the actions array.

                BridgeTetris.Tetris.playingTime = BridgeTetris.Tetris.playingTime + timeDelta;

                if (BridgeTetris.Tetris.playingTime > BridgeTetris.Tetris.pieceAdvanceInterval)
                {
                    BridgeTetris.Tetris.playingTime = BridgeTetris.Tetris.playingTime - BridgeTetris.Tetris.pieceAdvanceInterval;
                    BridgeTetris.Tetris.Drop();
                }
            }
        }

        private static void Handle(int action)
        {
            switch (action)
            {
                case BridgeTetris.Tetris.Direction.Toleft:
                case BridgeTetris.Tetris.Direction.ToRight:
                    BridgeTetris.Tetris.Move(action);
                    break;

                case BridgeTetris.Tetris.Direction.ToUp:
                    BridgeTetris.Tetris.Rotate();
                    break;

                case BridgeTetris.Tetris.Direction.ToDown:
                    BridgeTetris.Tetris.Drop();
                    break;
            }
        }

        private static bool Move(int direction)
        {
            var x = BridgeTetris.Tetris.currentPiece.HorizontalPosition;
            var y = BridgeTetris.Tetris.currentPiece.VerticalPosition;

            switch (direction)
            {
                case BridgeTetris.Tetris.Direction.ToRight:
                    x++;
                    break;

                case BridgeTetris.Tetris.Direction.Toleft:
                    x--;
                    break;

                case BridgeTetris.Tetris.Direction.ToDown:
                    y++;
                    break;
            }

            if (BridgeTetris.Tetris.Unoccupied(BridgeTetris.Tetris.currentPiece.BlockType, x, y, BridgeTetris.Tetris.currentPiece.Orientation))
            {
                BridgeTetris.Tetris.currentPiece.HorizontalPosition = x;
                BridgeTetris.Tetris.currentPiece.VerticalPosition = y;
                BridgeTetris.Tetris.Invalidate();

                return true;
            }
            else
            {
                return false;
            }
        }

        private static void Rotate()
        {
            short newdir;

            if (BridgeTetris.Tetris.currentPiece.Orientation == BridgeTetris.Tetris.Direction.Last)
            {
                newdir = BridgeTetris.Tetris.Direction.First;
            }
            else
            {
                newdir = (short)(BridgeTetris.Tetris.currentPiece.Orientation + 1);
            }

            if (BridgeTetris.Tetris.Unoccupied(BridgeTetris.Tetris.currentPiece.BlockType, BridgeTetris.Tetris.currentPiece.HorizontalPosition, BridgeTetris.Tetris.currentPiece.VerticalPosition, newdir))
            {
                BridgeTetris.Tetris.currentPiece.Orientation = newdir;
                BridgeTetris.Tetris.Invalidate();
            }
        }

        private static void Drop()
        {
            if (!BridgeTetris.Tetris.Move(BridgeTetris.Tetris.Direction.ToDown))
            {
                BridgeTetris.Tetris.AddScore(10);

                BridgeTetris.Tetris.DropPiece();
                BridgeTetris.Tetris.RemoveLines();

                BridgeTetris.Tetris.SetCurrentPiece(BridgeTetris.Tetris.nextPiece);
                BridgeTetris.Tetris.SetNextPiece(BridgeTetris.Tetris.RandomPiece());

                BridgeTetris.Tetris.ClearActions();

                if (BridgeTetris.Tetris.Occupied(BridgeTetris.Tetris.currentPiece.BlockType, BridgeTetris.Tetris.currentPiece.HorizontalPosition, BridgeTetris.Tetris.currentPiece.VerticalPosition,
                    BridgeTetris.Tetris.currentPiece.Orientation))
                {
                    BridgeTetris.Tetris.Lose();
                }
            }
        }

        private static void DropPiece()
        {
            var matchingCells = BridgeTetris.Tetris.Eachblock(BridgeTetris.Tetris.currentPiece.BlockType, BridgeTetris.Tetris.currentPiece.HorizontalPosition, BridgeTetris.Tetris.currentPiece.VerticalPosition,
                                          BridgeTetris.Tetris.currentPiece.Orientation);

            foreach (var tuple in matchingCells)
            {
                BridgeTetris.Tetris.SetBlock(tuple.Item1, tuple.Item2, BridgeTetris.Tetris.currentPiece.BlockType);
            }
        }

        private static void RemoveLines()
        {
            int n = 0;
            bool complete = false;

            for (var y = BridgeTetris.Tetris.tetrisCourtHeight; y > 0; --y)
            {
                complete = true;

                for (var x = 0; x < BridgeTetris.Tetris.tetrisCourtWidth; ++x)
                {
                    if (BridgeTetris.Tetris.GetBlock(x, y) == null)
                    {
                        complete = false;
                    }
                }

                if (complete)
                {
                    BridgeTetris.Tetris.RemoveLine(y);
                    y++; // recheck same line
                    n++;
                }
            }

            if (n > 0)
            {
                BridgeTetris.Tetris.AddRows(n);
                BridgeTetris.Tetris.AddScore(100 * (int)Math.Pow(2, n - 1)); // 1:100, 2:200, 3:400, 4:800
            }
        }

        private static void RemoveLine(int linePosition)
        {
            for (var y = linePosition; y >= 0; --y)
            {
                for (var x = 0; x < BridgeTetris.Tetris.tetrisCourtWidth; ++x)
                {
                    BridgeTetris.Tetris.SetBlock(x, y, (y == 0) ? null : BridgeTetris.Tetris.GetBlock(x, y - 1));
                }
            }
        }

        #endregion

        #region RENDERING

        private static class invalid
        {
            public static bool Court = false;
            public static bool Next = false;
            public static bool Score = false;
            public static bool Rows = false;
        }

        private static void Invalidate()
        {
            BridgeTetris.Tetris.invalid.Court = true;
        }

        private static void InvalidateNext()
        {
            BridgeTetris.Tetris.invalid.Next = true;
        }

        private static void InvalidateScore()
        {
            BridgeTetris.Tetris.invalid.Score = true;
        }

        private static void InvalidateRows()
        {
            BridgeTetris.Tetris.invalid.Rows = true;
        }

        private static void Draw()
        {
            BridgeTetris.Tetris.canvasContext.Save();
            BridgeTetris.Tetris.canvasContext.LineWidth = 1;
            BridgeTetris.Tetris.canvasContext.Translate(0.5, 0.5); // for crisp 1px black lines

            BridgeTetris.Tetris.DrawCourt();
            BridgeTetris.Tetris.DrawNext();
            BridgeTetris.Tetris.DrawScore();
            BridgeTetris.Tetris.DrawRows();

            BridgeTetris.Tetris.canvasContext.Restore();
        }

        private static void DrawCourt()
        {
            if (BridgeTetris.Tetris.invalid.Court)
            {
                BridgeTetris.Tetris.canvasContext.ClearRect(0, 0, BridgeTetris.Tetris.canvas.Width, BridgeTetris.Tetris.canvas.Height);

                if (BridgeTetris.Tetris.isGamePlaying)
                {
                    BridgeTetris.Tetris.DrawPiece(BridgeTetris.Tetris.canvasContext, BridgeTetris.Tetris.currentPiece.BlockType, BridgeTetris.Tetris.currentPiece.HorizontalPosition,
                              BridgeTetris.Tetris.currentPiece.VerticalPosition, BridgeTetris.Tetris.currentPiece.Orientation);
                }

                BridgeTetris.Tetris.PieceType block;

                for (int y = 0; y < BridgeTetris.Tetris.tetrisCourtHeight; y++)
                {
                    for (int x = 0; x < BridgeTetris.Tetris.tetrisCourtWidth; x++)
                    {
                        block = BridgeTetris.Tetris.GetBlock(x, y);

                        if (block != null)
                        {
                            BridgeTetris.Tetris.DrawBlock(BridgeTetris.Tetris.canvasContext, x, y, block.Color);
                        }
                    }
                }

                BridgeTetris.Tetris.canvasContext.StrokeRect(0, 0, (BridgeTetris.Tetris.tetrisCourtWidth * BridgeTetris.Tetris.horizontalPixelDimension) - 1,
                                         (BridgeTetris.Tetris.tetrisCourtHeight * BridgeTetris.Tetris.verticalPixelDimension) - 1); // court boundary

                BridgeTetris.Tetris.invalid.Court = false;
            }
        }

        private static void DrawNext()
        {
            if (BridgeTetris.Tetris.invalid.Next)
            {
                var padding = (BridgeTetris.Tetris.upcomingPreviewDimensions - nextPiece.BlockType.Size) / 2; // half-arsed attempt at centering next piece display

                BridgeTetris.Tetris.upcomingPieceCanvasContext.Save();

                BridgeTetris.Tetris.upcomingPieceCanvasContext.Translate(0.5, 0.5);
                BridgeTetris.Tetris.upcomingPieceCanvasContext.ClearRect(0, 0, BridgeTetris.Tetris.upcomingPreviewDimensions * BridgeTetris.Tetris.horizontalPixelDimension,
                                                     BridgeTetris.Tetris.upcomingPreviewDimensions * BridgeTetris.Tetris.verticalPixelDimension);

                BridgeTetris.Tetris.DrawPiece(BridgeTetris.Tetris.upcomingPieceCanvasContext, BridgeTetris.Tetris.nextPiece.BlockType, padding, padding, BridgeTetris.Tetris.nextPiece.Orientation);

                BridgeTetris.Tetris.upcomingPieceCanvasContext.StrokeStyle = "black";
                BridgeTetris.Tetris.upcomingPieceCanvasContext.StrokeRect(0, 0, (BridgeTetris.Tetris.upcomingPreviewDimensions * BridgeTetris.Tetris.horizontalPixelDimension) - 1,
                                                      (BridgeTetris.Tetris.upcomingPreviewDimensions * BridgeTetris.Tetris.verticalPixelDimension) - 1);

                BridgeTetris.Tetris.upcomingPieceCanvasContext.Restore();

                BridgeTetris.Tetris.invalid.Next = false;
            }
        }

        private static void DrawScore()
        {
            if (BridgeTetris.Tetris.invalid.Score)
            {
                BridgeTetris.Tetris.Html("score", ("00000" + Math.Floor(incrementalInstantaneousScore)).Slice(-5));
                BridgeTetris.Tetris.invalid.Score = false;
            }
        }

        private static void DrawRows()
        {
            if (BridgeTetris.Tetris.invalid.Rows)
            {
                BridgeTetris.Tetris.Html("rows", BridgeTetris.Tetris.completedRowsCount.ToString());
                BridgeTetris.Tetris.invalid.Rows = false;
            }
        }

        private static void DrawPiece(CanvasRenderingContext2D specifiedCanvasContext, BridgeTetris.Tetris.PieceType pieceType,
            int horizontalPosition, int verticalPosition, short direction)
        {
            var matchingCells = BridgeTetris.Tetris.Eachblock(pieceType, horizontalPosition, verticalPosition, direction);

            foreach (var tuple in matchingCells)
            {
                BridgeTetris.Tetris.DrawBlock(specifiedCanvasContext, tuple.Item1, tuple.Item2, pieceType.Color);
            }
        }

        private static void DrawBlock(CanvasRenderingContext2D specifiedCanvasContext, int horizontalPosition,
            int verticalPosition, string blockColor)
        {
            specifiedCanvasContext.FillStyle = blockColor;

            specifiedCanvasContext.FillRect(horizontalPosition * BridgeTetris.Tetris.horizontalPixelDimension,
                verticalPosition * BridgeTetris.Tetris.verticalPixelDimension, BridgeTetris.Tetris.horizontalPixelDimension, BridgeTetris.Tetris.verticalPixelDimension);

            specifiedCanvasContext.StrokeRect(horizontalPosition * BridgeTetris.Tetris.horizontalPixelDimension,
                verticalPosition * BridgeTetris.Tetris.verticalPixelDimension, BridgeTetris.Tetris.horizontalPixelDimension, BridgeTetris.Tetris.verticalPixelDimension);
        }

        #endregion

        /// <summary>
        /// Load the class upon page load. When DOM content is ready, actually.
        /// </summary>
        [Ready]
        public static void LoadGame()
        {
            BridgeTetris.Tetris.LoadPlayArea(); // load page's placeholders

            BridgeTetris.Tetris.Run();          // effectively start the game engine (will listen for 'spacebar' to begin game)
        }
    }
}