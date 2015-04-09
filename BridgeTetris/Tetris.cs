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

            menuDiv.AppendChild(InsideParagraph(pressStartTitle, "start"));

            var nextPieceCanvas = new CanvasElement
            {
                Id = "upcoming"
            };

            menuDiv.AppendChild(InsideParagraph(nextPieceCanvas));

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

            menuDiv.AppendChild(InsideParagraph(scoreParagraph));

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

            menuDiv.AppendChild(InsideParagraph(rowsParagraph));

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
            return InsideParagraph(new List<Node> { element }, paragraphID);
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
            canvas = Get("canvas").As<CanvasElement>();
            upcomingPieceCanvas = Get("upcoming").As<CanvasElement>();

            canvasContext = canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            upcomingPieceCanvasContext = upcomingPieceCanvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
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
            public PieceType Type { get; set; }
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
        private static PieceType[,] tetrisCourt = new PieceType[tetrisCourtWidth, tetrisCourtHeight];

        private static int[] userActionsQueue = new int[0]; // queue of user actions (inputs)

        private static Piece currentPiece; // the current piece
        private static Piece nextPiece; // the next piece

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

        private static PieceType iBlock = new PieceType
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

        private static PieceType jBlock = new PieceType
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

        private static PieceType lBlock = new PieceType
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

        private static PieceType oBlock = new PieceType
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

        private static PieceType sBlock = new PieceType
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

        private static PieceType tBlock = new PieceType
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

        private static PieceType zBlock = new PieceType
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
        private static Tuple<int, int>[] Eachblock(PieceType pieceType, int horizontalPositon, int verticalPosition, short direction)
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
                    GetBlock(horizontalPosition, verticalPosition) != null);
        }

        // check if a piece can fit into a position in the grid
        private static bool Occupied(PieceType pieceType, int horizontalPosition, int verticalPosition, short direction)
        {
            var matchingCells = Eachblock(pieceType, horizontalPosition, verticalPosition, direction);

            foreach (var tuple in matchingCells)
            {
                if (PieceCanFit(tuple.Item1, tuple.Item2))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool Unoccupied(PieceType pieceType, int horizontalPosition, int verticalPosition, short direction)
        {
            return !Occupied(pieceType, horizontalPosition, verticalPosition, direction);
        }

        // start with 4 instances of each piece and
        // pick randomly until the 'bag is empty'
        private static PieceType[] pieces;

        private static Piece RandomPiece()
        {
            pieces = new PieceType[]
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
            var type = (PieceType)pieces.Splice((int)Random(0, pieces.Length - 1), 1)[0];

            return new Piece
            {
                Orientation = Direction.ToUp,
                Type = type,
                HorizontalPosition = (int)Math.Round(Random(0, tetrisCourtWidth - type.Size)),
                VerticalPosition = 0
            };
        }

        #region GAME LOOP

        private static void Run()
        {
            LoadCanvasContext();

            AddEvents();

            var last = currentTimeStamp = Timestamp();

            Resize(); // setup all our sizing information
            Reset();  // reset the per-game variables
            Frame();  // start the first frame
        }

        private static void Frame()
        {
            currentTimeStamp = Timestamp();

            // using requestAnimationFrame have to be able to handle large delta's caused when it 'hibernates' in a background or non-visible tab
            Update(Math.Min(1, (currentTimeStamp - previousTimeStamp) / 1000.0));

            Draw();

            previousTimeStamp = currentTimeStamp;

            Window.RequestAnimationFrame(animationFrame => Frame());
        }

        private static void AddEvents()
        {
            Document.AddEventListener(EventType.KeyDown, KeyDown, false);
            Document.AddEventListener(EventType.Resize, Resize, false);
        }

        public static void Resize(Event evnt = null)
        {
            // set canvas logical size equal to its physical size
            canvas.Width = canvas.ClientWidth;
            canvas.Height = canvas.ClientHeight;

            upcomingPieceCanvas.Width = upcomingPieceCanvas.ClientWidth;
            upcomingPieceCanvas.Height = upcomingPieceCanvas.ClientHeight;

            // pixel size of a single tetris block
            horizontalPixelDimension = canvas.ClientWidth / tetrisCourtWidth;
            verticalPixelDimension = canvas.ClientHeight / tetrisCourtHeight;

            Invalidate();
            InvalidateNext();
        }

        public static void KeyDown(Event eventInformation)
        {
            var isKeyStrokeHandled = false;
            var keyboardEventInformation = eventInformation.As<KeyboardEvent>();

            if (isGamePlaying)
            {
                switch (keyboardEventInformation.KeyCode)
                {
                    case Key.LeftArrow:
                        userActionsQueue.Push(Direction.Toleft);
                        isKeyStrokeHandled = true;
                        break;
                    case Key.RightArrow:
                        userActionsQueue.Push(Direction.ToRight);
                        isKeyStrokeHandled = true;
                        break;
                    case Key.UpArrow:
                        userActionsQueue.Push(Direction.ToUp);
                        isKeyStrokeHandled = true;
                        break;
                    case Key.DownArrow:
                        userActionsQueue.Push(Direction.ToDown);
                        isKeyStrokeHandled = true;
                        break;
                    case Key.Escape:
                        Lose();
                        isKeyStrokeHandled = true;
                        break;
                }
            }
            else
            {
                if (keyboardEventInformation.KeyCode == Key.Spacebar)
                {
                    Play();
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
            Hide("start");
            Reset();
            isGamePlaying = true;
        }

        private static void Lose()
        {
            Show("start");
            SetVisualScore();
            isGamePlaying = false;
        }

        private static void SetVisualScore(int? scoreValue = null)
        {
            incrementalInstantaneousScore = scoreValue ?? currentScore;
            InvalidateScore();
        }

        private static void SetScore(int scoreValue)
        {
            currentScore = scoreValue;
            SetVisualScore(scoreValue);
        }

        private static void AddScore(int scoreValue)
        {
            currentScore += scoreValue;
        }

        private static void ClearScore()
        {
            SetScore(0);
        }

        private static void ClearRows()
        {
            SetRows(0);
        }

        private static void SetRows(int newRowsCount)
        {
            var speedMin = Speed.lowerBoundary;
            var speedStart = Speed.start;
            var speedDec = Speed.decrement;
            var rowCount = completedRowsCount;

            completedRowsCount = newRowsCount;

            pieceAdvanceInterval = Math.Max(speedMin, speedStart - (speedDec * rowCount));

            InvalidateRows();
        }

        private static void AddRows(int rowsAmountToAdd)
        {
            SetRows(completedRowsCount + rowsAmountToAdd);
        }

        private static PieceType GetBlock(int horizontalPosition, int verticalPosition)
        {
            PieceType retval = null;

            if (horizontalPosition >= 0 && horizontalPosition < tetrisCourtWidth &&
                tetrisCourt.Length > ((horizontalPosition + 1) * tetrisCourtWidth) &&
                verticalPosition > 0 && verticalPosition < tetrisCourtHeight)
            {
                retval = tetrisCourt[horizontalPosition, verticalPosition];
            }

            return retval;
        }

        private static void SetBlock(int horizontalPosition, int verticalPosition, PieceType pieceType)
        {
            if (horizontalPosition >= 0 && horizontalPosition < tetrisCourtWidth)
            {
                tetrisCourt[horizontalPosition, verticalPosition] = pieceType;
            }

            Invalidate();
        }

        private static void ClearBlocks()
        {
            tetrisCourt = new PieceType[tetrisCourtWidth, tetrisCourtHeight];
        }

        private static void ClearActions()
        {
            userActionsQueue = new int[0];
        }

        private static void SetCurrentPiece(Piece pieceType)
        {
            currentPiece = pieceType ?? RandomPiece();
            Invalidate();
        }

        private static void SetNextPiece(Piece pieceType = null)
        {
            nextPiece = pieceType ?? RandomPiece();
            InvalidateNext();
        }

        private static void Reset()
        {
            playingTime = 0;
            
            ClearActions();
            ClearBlocks();
            ClearRows();
            ClearScore();
            
            SetCurrentPiece(nextPiece);
            SetNextPiece();
        }

        private static void Update(double timeDelta)
        {
            if (isGamePlaying)
            {
                if (incrementalInstantaneousScore < currentScore)
                {
                    SetVisualScore(incrementalInstantaneousScore + 1);
                }

                Handle(userActionsQueue.Shift().As<int>()); // FIXME: Shift() should already return the type of the actions array.

                playingTime = playingTime + timeDelta;

                if (playingTime > pieceAdvanceInterval)
                {
                    playingTime = playingTime - pieceAdvanceInterval;
                    Drop();
                }
            }
        }

        private static void Handle(int action)
        {
            switch (action)
            {
                case Direction.Toleft:
                case Direction.ToRight:
                    Move(action);
                    break;

                case Direction.ToUp:
                    Rotate();
                    break;

                case Direction.ToDown:
                    Drop();
                    break;
            }
        }

        private static bool Move(int direction)
        {
            var x = currentPiece.HorizontalPosition;
            var y = currentPiece.VerticalPosition;

            switch (direction)
            {
                case Direction.ToRight:
                    x++;
                    break;

                case Direction.Toleft:
                    x--;
                    break;

                case Direction.ToDown:
                    y++;
                    break;
            }

            if (Unoccupied(currentPiece.Type, x, y, currentPiece.Orientation))
            {
                currentPiece.HorizontalPosition = x;
                currentPiece.VerticalPosition = y;
                Invalidate();

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

            if (currentPiece.Orientation == Direction.Last)
            {
                newdir = Direction.First;
            }
            else
            {
                newdir = (short)(currentPiece.Orientation + 1);
            }

            if (Unoccupied(currentPiece.Type, currentPiece.HorizontalPosition, currentPiece.VerticalPosition, newdir))
            {
                currentPiece.Orientation = newdir;
                Invalidate();
            }
        }

        private static void Drop()
        {
            if (!Move(Direction.ToDown))
            {
                AddScore(10);

                DropPiece();
                RemoveLines();

                SetCurrentPiece(nextPiece);
                SetNextPiece(RandomPiece());

                ClearActions();

                if (Occupied(currentPiece.Type, currentPiece.HorizontalPosition, currentPiece.VerticalPosition,
                    currentPiece.Orientation))
                {
                    Lose();
                }
            }
        }

        private static void DropPiece()
        {
            var matchingCells = Eachblock(currentPiece.Type, currentPiece.HorizontalPosition, currentPiece.VerticalPosition,
                                          currentPiece.Orientation);

            foreach (var tuple in matchingCells)
            {
                SetBlock(tuple.Item1, tuple.Item2, currentPiece.Type);
            }
        }

        private static void RemoveLines()
        {
            int n = 0;
            bool complete = false;

            for (var y = tetrisCourtHeight; y > 0; --y)
            {
                complete = true;

                for (var x = 0; x < tetrisCourtWidth; ++x)
                {
                    if (GetBlock(x, y) == null)
                    {
                        complete = false;
                    }
                }

                if (complete)
                {
                    RemoveLine(y);
                    y++; // recheck same line
                    n++;
                }
            }

            if (n > 0)
            {
                AddRows(n);
                AddScore(100 * (int)Math.Pow(2, n - 1)); // 1:100, 2:200, 3:400, 4:800
            }
        }

        private static void RemoveLine(int linePosition)
        {
            for (var y = linePosition; y >= 0; --y)
            {
                for (var x = 0; x < tetrisCourtWidth; ++x)
                {
                    SetBlock(x, y, (y == 0) ? null : GetBlock(x, y - 1));
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
            invalid.Court = true;
        }

        private static void InvalidateNext()
        {
            invalid.Next = true;
        }

        private static void InvalidateScore()
        {
            invalid.Score = true;
        }

        private static void InvalidateRows()
        {
            invalid.Rows = true;
        }

        private static void Draw()
        {
            canvasContext.Save();
            canvasContext.LineWidth = 1;
            canvasContext.Translate(0.5, 0.5); // for crisp 1px black lines

            DrawCourt();
            DrawNext();
            DrawScore();
            DrawRows();

            canvasContext.Restore();
        }

        private static void DrawCourt()
        {
            if (invalid.Court)
            {
                canvasContext.ClearRect(0, 0, canvas.Width, canvas.Height);

                if (isGamePlaying)
                {
                    DrawPiece(canvasContext, currentPiece.Type, currentPiece.HorizontalPosition,
                              currentPiece.VerticalPosition, currentPiece.Orientation);
                }

                PieceType block;

                for (int y = 0; y < tetrisCourtHeight; y++)
                {
                    for (int x = 0; x < tetrisCourtWidth; x++)
                    {
                        block = GetBlock(x, y);

                        if (block != null)
                        {
                            DrawBlock(canvasContext, x, y, block.Color);
                        }
                    }
                }

                canvasContext.StrokeRect(0, 0, (tetrisCourtWidth * horizontalPixelDimension) - 1,
                                         (tetrisCourtHeight * verticalPixelDimension) - 1); // court boundary

                invalid.Court = false;
            }
        }

        private static void DrawNext()
        {
            if (invalid.Next)
            {
                var padding = (upcomingPreviewDimensions - nextPiece.Type.Size) / 2; // half-arsed attempt at centering next piece display

                upcomingPieceCanvasContext.Save();

                upcomingPieceCanvasContext.Translate(0.5, 0.5);
                upcomingPieceCanvasContext.ClearRect(0, 0, upcomingPreviewDimensions * horizontalPixelDimension,
                                                     upcomingPreviewDimensions * verticalPixelDimension);

                DrawPiece(upcomingPieceCanvasContext, nextPiece.Type, padding, padding, nextPiece.Orientation);

                upcomingPieceCanvasContext.StrokeStyle = "black";
                upcomingPieceCanvasContext.StrokeRect(0, 0, (upcomingPreviewDimensions * horizontalPixelDimension) - 1,
                                                      (upcomingPreviewDimensions * verticalPixelDimension) - 1);

                upcomingPieceCanvasContext.Restore();

                invalid.Next = false;
            }
        }

        private static void DrawScore()
        {
            if (invalid.Score)
            {
                Html("score", ("00000" + Math.Floor(incrementalInstantaneousScore)).Slice(-5));
                invalid.Score = false;
            }
        }

        private static void DrawRows()
        {
            if (invalid.Rows)
            {
                Html("rows", completedRowsCount.ToString());
                invalid.Rows = false;
            }
        }

        private static void DrawPiece(CanvasRenderingContext2D specifiedCanvasContext, PieceType pieceType,
            int horizontalPosition, int verticalPosition, short direction)
        {
            var matchingCells = Eachblock(pieceType, horizontalPosition, verticalPosition, direction);

            foreach (var tuple in matchingCells)
            {
                DrawBlock(specifiedCanvasContext, tuple.Item1, tuple.Item2, pieceType.Color);
            }
        }

        private static void DrawBlock(CanvasRenderingContext2D specifiedCanvasContext, int horizontalPosition,
            int verticalPosition, string blockColor)
        {
            specifiedCanvasContext.FillStyle = blockColor;

            specifiedCanvasContext.FillRect(horizontalPosition * horizontalPixelDimension,
                verticalPosition * verticalPixelDimension, horizontalPixelDimension, verticalPixelDimension);

            specifiedCanvasContext.StrokeRect(horizontalPosition * horizontalPixelDimension,
                verticalPosition * verticalPixelDimension, horizontalPixelDimension, verticalPixelDimension);
        }

        #endregion

        /// <summary>
        /// Load the class upon page load. When DOM content is ready, actually.
        /// </summary>
        [Ready]
        public static void LoadGame()
        {
            LoadPlayArea(); // load page's placeholders

            Run();          // effectively start the game engine (will listen for 'spacebar' to begin game)
        }
    }
}