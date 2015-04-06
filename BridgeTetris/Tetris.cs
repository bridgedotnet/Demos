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
        public static void LoadPlayArea()
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
            menuDiv.AppendChild(insideParagraph(pressStartTitle));

            var nextPieceCanvas = new CanvasElement
            {
                Id = "upcoming"
            };
            menuDiv.AppendChild(insideParagraph(nextPieceCanvas));

            var scorePara = new List<Node>();
            scorePara.Add(new LabelElement
            {
                InnerHTML = "score "
            });

            scorePara.Add(new SpanElement
            {
                Id = "score",
                InnerHTML = "00000"
            });
            menuDiv.AppendChild(insideParagraph(scorePara));

            var rowsPara = new List<Node>();
            rowsPara.Add(new LabelElement
            {
                InnerHTML = "rows "
            });

            rowsPara.Add(new SpanElement
            {
                Id = "rows",
                InnerHTML = "0"
            });
            menuDiv.AppendChild(insideParagraph(rowsPara));

            var pieceSlideCanvas = new CanvasElement { Width = 200, Height = 400 };
            Document.Body.AppendChild(insideParagraph(pieceSlideCanvas));
        }

        private static Node insideParagraph(List<Node> elementList)
        {
            var paraElem = new ParagraphElement();

            foreach (var el in elementList)
            {
                paraElem.AppendChild(el);
            }

            return paraElem;
        }

        private static Node insideParagraph(Node element)
        {
            return BridgeTetris.Tetris.insideParagraph(new List<Node> { element });
        }

        #region base helper methods

        // FIXME: Useless, as we have to prepend full class path anyway. Left just for reference to original code.
        private static Element get(string id)
        {
            return Document.GetElementById(id);
        }

        private static void hide(string id)
        {
            BridgeTetris.Tetris.get(id).Style.Visibility = Visibility.Hidden;
        }

        private static void show(string id)
        {
            //get(id).Style.Visibility = null; // FIXME: this does not work!
            BridgeTetris.Tetris.get(id).Style.Visibility = Visibility.Inherit;
        }

        private static void html(string id, string html)
        {
            BridgeTetris.Tetris.get(id).InnerHTML = html;
        }

        private static double timestamp()
        {
            return new Date().GetTime();
        }

        private static double random(int min, int max)
        {
            return (min + (Math.Random() * (max - min)));
        }

        //private static double randomChoice() // Not used!? Can't guess types if not used..

        #region See what to do here later (this is optional)

        private static void fixReqAnimFrame() // unnecessary on bridge.net?
        {
            Script.Write(@"if (!window.requestAnimationFrame) { // http://paulirish.com/2011/requestanimationframe-for-smart-animating/
                window.requestAnimationFrame = window.webkitRequestAnimationFrame ||
                                               window.mozRequestAnimationFrame    ||
                                               window.oRequestAnimationFrame      ||
                                               window.msRequestAnimationFrame     ||
                                               function(callback, element) {
                                                   window.setTimeout(callback, 1000 / 60);
                                               }
            }");
        }

        private static void callBackReqAnimFrame(Func<Func<string, Node>, Node> callback)
        {
            Window.SetTimeout(callback, 1000 / 60);
        }

        #endregion

        #endregion

        #region game constants

        private static class KEY
        {
            public const short
                ESC   = 27,
                SPACE = 32,
                LEFT  = 37,
                UP    = 38,
                RIGHT = 39,
                DOWN  = 40;
        }

        private static class DIR
        {
            public const short
                UP    = 0,
                RIGHT = 1,
                DOWN  = 2,
                LEFT  = 3,
                MIN   = 0,
                MAX   = 3;
        };

        // private start = new Stats() -- optional, included on another .js!

        // FIXME Should allow returning canvasElement without casting??
        private static CanvasElement canvas = BridgeTetris.Tetris.get("canvas").As<CanvasElement>();
        private static CanvasElement ucanvas = BridgeTetris.Tetris.get("upcoming").As<CanvasElement>();

        // FIXME: it could do casting automatically!
        private static CanvasRenderingContext2D ctx = BridgeTetris.Tetris.canvas.GetContext("2d").As<CanvasRenderingContext2D>();
        private static CanvasRenderingContext2D uctx = BridgeTetris.Tetris.ucanvas.GetContext("2d").As<CanvasRenderingContext2D>();

        // how long before piece drops by 1 row (seconds)
        private static class Speed
        {
            public const float
                start = 0.6f,
                decrement = 0.005f,
                min = 0.1f;
        };

        private static int nx = 10, // width of tetris court (in blocks)
                           ny = 20, // height of tetris court (in blocks)
                           nu = 5;  // width/heigth of upcoming preview (in blocks)

        #endregion

        #region tetris elements' abstraction classes
        private class Piece
        {
            public PieceType type { get; set; }
            public short dir { get; set; }
            public int x { get; set; }
            public int y { get; set; }
        }

        private class PieceType
        {
            public int size { get; set; }
            public string color { get; set; }
            public int[] blocks { get; set; }

            public PieceType(int sz, int[] shape, string clr)
            {
                this.size = sz;
                this.color = clr;

                if (shape.Length < 1)
                {
                    shape[0] = 0xCC00; // 2x2 square
                }

                if (shape.Length < 4)
                {
                    for (int i = shape.Length - 1; i < 4; i++)
                    {
                        shape[i] = shape[i - 1];
                    }
                }

                this.blocks = shape;
            }
        }

        #endregion

        #region game variables (initialized during reset)

        private static int dx, // pixel width of a tetris block
                           dy, // pixel height of a tetris block
                        score, // the current score
                       vscore, // the currently displayed score (catches up to score in small chunks like slot machine)
                         rows; // number of completed rows in the current game

        // 2 dimensional array (nx*ny) representing tetris court - either empty block or occupied by a 'piece'
        private static BridgeTetris.Tetris.PieceType[,] blocks = new BridgeTetris.Tetris.PieceType[nx, ny];

        private static int[] actions = new int[0]; // queue of user actions (inputs)

        private static BridgeTetris.Tetris.Piece current, // the current piece
                                                    next; // the next piece

        private static bool playing; // true|false - game is in progress
        private static double dt,    // time since starting the game
                            step;    // how long before current piece drops by 1 row

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

        private static BridgeTetris.Tetris.PieceType
            i = new BridgeTetris.Tetris.PieceType(4, new int[] { 0x0F00, 0x2222, 0x00F0, 0x4444 }, "cyan"),
            j = new BridgeTetris.Tetris.PieceType(3, new int[] { 0x44C0, 0x8E00, 0x6440, 0x0E20 }, "blue"),
            l = new BridgeTetris.Tetris.PieceType(3, new int[] { 0x4460, 0x0E80, 0xC440, 0x2E00 }, "orange"),
            o = new BridgeTetris.Tetris.PieceType(2, new int[] { 0xCC00, 0xCC00, 0xCC00, 0xCC00 }, "yellow"),
            s = new BridgeTetris.Tetris.PieceType(3, new int[] { 0x06C0, 0x8C40, 0x6C00, 0x4620 }, "green"),
            t = new BridgeTetris.Tetris.PieceType(3, new int[] { 0x0E40, 0x4C40, 0x4E00, 0x4640 }, "purple"),
            z = new BridgeTetris.Tetris.PieceType(3, new int[] { 0x0C60, 0x4C80, 0xC600, 0x2640 }, "red");
        #endregion

        /// <summary>
        /// do the bit manipulation and iterate through each
        /// occupied block (x,y) for a given piece
        /// </summary>
        /// <param name="type"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dir"></param>
        /// <param name="fn"></param>
        private static Tuple<int, int>[] eachblock(BridgeTetris.Tetris.PieceType type, int x, int y, short dir)
        {
            int row = 0,
                col = 0,
                blocks = type.blocks[dir];
            Tuple<int, int>[] result = new Tuple<int, int>[0]; // FIXME: 'result' logic must be ensured to work like on JS

            for (int bit = 0x8000; bit > 0; bit = bit >> 1)
            {
                if ((blocks & bit) > 0)
                {
                    result.Push(new Tuple<int, int>(x + col, y + row));
                }
                if (++col == 4)
                {
                    col = 0;
                    ++row;
                }
            }
            return result;
        }

        // this is the function delegated to eachblock from occupied:
        private static bool pieceCanFit(int x, int y)
        {
            return (x < 0 || x >= nx || y < 0 || y >= ny || BridgeTetris.Tetris.getBlock(x, y) != null);
        }

        // check if a piece can fit into a position in the grid
        private static bool occupied(BridgeTetris.Tetris.PieceType type, int x, int y, short dir)
        {
            var matchingCells = BridgeTetris.Tetris.eachblock(type, x, y, dir); // FIXME: 'result' logic must be ensured to work like on JS
            foreach (var tuple in matchingCells)
            {
                if (BridgeTetris.Tetris.pieceCanFit(tuple.Item1, tuple.Item2))
                {
                    return true;
                }
            }
            return false; // FIXME: maybe just return eachblock result??
        }

        private static bool unoccupied(BridgeTetris.Tetris.PieceType type, int x, int y, short dir)
        {
            return !BridgeTetris.Tetris.occupied(type, x, y, dir);
        }

        // start with 4 instances of each piece and
        // pick randomly until the 'bag is empty'
        //private static List<Piece> pieces = new List<Piece>();
        private static BridgeTetris.Tetris.PieceType[] pieces;

        private static BridgeTetris.Tetris.Piece randomPiece()
        {
            pieces = new BridgeTetris.Tetris.PieceType[] { i, i, i, i, j, j, j, j, l, l, l, l, o, o, o, o, s, s, s, s, t, t, t, t, z, z, z, z };
            var type = (BridgeTetris.Tetris.PieceType)pieces.Splice((int)random(0, pieces.Length - 1), 1)[0]; // This does not cast as Piece?

            return new BridgeTetris.Tetris.Piece
            {
                dir = BridgeTetris.Tetris.DIR.UP,
                type = type,
                x = (int)Math.Round(random(0, nx - type.size)),
                y = 0
            };
        }

        #region GAME LOOP

        public static void Run()
        {
            //showStats(); // initialize FPS counter (defined in external .js)
            BridgeTetris.Tetris.addEvents();
        }

        /*FIXME: this is not really the game and uses external javascript, so let's work on this later
         * private static void showStats()
        {
            Bridge.Script.Write("stats.domElement.id = 'stats'");
            get('menu').AppendChild('stats.domElement');
        }*/

        private static void addEvents()
        {
            Document.AddEventListener(EventType.KeyDown, keydown, false);
            Document.AddEventListener(EventType.Resize, resize, false);
        }

        public static void resize(Event evnt) // 'event' is a reserved keyword
        {
            // set canvas logical size equal to its physical size
            canvas.Width = BridgeTetris.Tetris.canvas.ClientWidth;
            canvas.Height = BridgeTetris.Tetris.canvas.ClientHeight;

            ucanvas.Width = BridgeTetris.Tetris.ucanvas.ClientWidth;
            ucanvas.Height = BridgeTetris.Tetris.ucanvas.ClientHeight;

            // pixel size of a single tetris block
            dx = BridgeTetris.Tetris.canvas.ClientWidth / BridgeTetris.Tetris.nx;
            dy = BridgeTetris.Tetris.canvas.ClientHeight / BridgeTetris.Tetris.ny;

            invalidate();
            invalidateNext();
        }

        public static void keydown(Event ev)
        {
            var handled = false;
            var kev = ev.As<KeyboardEvent>();
            if (BridgeTetris.Tetris.playing)
            {
                switch (kev.KeyCode)
                {
                    case BridgeTetris.Tetris.KEY.LEFT:
                        actions.Push(BridgeTetris.Tetris.DIR.LEFT);
                        handled = true;
                        break;
                    case BridgeTetris.Tetris.KEY.RIGHT:
                        actions.Push(BridgeTetris.Tetris.DIR.RIGHT);
                        handled = true;
                        break;
                    case BridgeTetris.Tetris.KEY.UP:
                        actions.Push(BridgeTetris.Tetris.DIR.UP);
                        handled = true;
                        break;
                }
            }
            else
            {
                if (kev.KeyCode == BridgeTetris.Tetris.KEY.SPACE)
                {
                    BridgeTetris.Tetris.play();
                    handled = true;
                }
            }

            if (handled)
            {
                ev.PreventDefault(); // prevent arrow keys from scrolling the page (supported in ie9+ and all other browsers)
            }
        }

        #endregion

        #region GAME LOGIC

        private static void play()
        {
            BridgeTetris.Tetris.hide("start");
            BridgeTetris.Tetris.reset();
            BridgeTetris.Tetris.playing = true;
        }

        private static void lose()
        {
            BridgeTetris.Tetris.show("start");
            BridgeTetris.Tetris.setVisualScore();
            BridgeTetris.Tetris.playing = false;
        }

        private static void setVisualScore(int? n = null)
        {
            vscore = n ?? score;
            BridgeTetris.Tetris.invalidateScore();
        }

        private static void setScore(int n)
        {
            BridgeTetris.Tetris.score = n;
            BridgeTetris.Tetris.setVisualScore(n);
        }

        private static void addScore(int n)
        {
            BridgeTetris.Tetris.score += n;
        }

        private static void clearScore()
        {
            BridgeTetris.Tetris.setScore(0);
        }

        private static void clearRows()
        {
            BridgeTetris.Tetris.setRows(0);
        }

        private static void setRows(int n)
        {
            var speedMin = BridgeTetris.Tetris.Speed.min;
            var speedStart = BridgeTetris.Tetris.Speed.start;
            var speedDec = BridgeTetris.Tetris.Speed.decrement;
            var rowCount = BridgeTetris.Tetris.rows;

            BridgeTetris.Tetris.rows = n;
            BridgeTetris.Tetris.step = Math.Max(speedMin, speedStart - (speedDec * rowCount));
            BridgeTetris.Tetris.invalidateRows();
        }

        private static void addRows(int n)
        {
            BridgeTetris.Tetris.setRows(BridgeTetris.Tetris.rows + n);
        }

        private static BridgeTetris.Tetris.PieceType getBlock(int x, int y)
        {
            BridgeTetris.Tetris.PieceType retval = null;

            if (x >= 0 && x < BridgeTetris.Tetris.nx && blocks.Length > ((x + 1) * BridgeTetris.Tetris.nx))
            {
                retval = blocks[x, y];
            }

            return retval;
        }

        private static void setBlock(int x, int y, BridgeTetris.Tetris.PieceType type)
        {
            // FIXME: understand what this does and ensure whether it is needed or not!
            //        seems to be just a js trick to ensure the array has been allocated
            //blocks[x] = blocks[x] || null; // does not make much sense!
            // maybe the test below does what this would: (disallow allocating blocks outside court longitudinally)
            if (x >= 0 && x < BridgeTetris.Tetris.nx)
            {
                blocks[x, y] = type;
            }
            BridgeTetris.Tetris.invalidate();
        }

        private static void clearBlocks()
        {
            blocks = new BridgeTetris.Tetris.PieceType[BridgeTetris.Tetris.nx, BridgeTetris.Tetris.ny];
        }

        private static void clearActions()
        {
            BridgeTetris.Tetris.actions = new int[0];
        }

        private static void setCurrentPiece(BridgeTetris.Tetris.Piece piece)
        {
            BridgeTetris.Tetris.current = piece ?? BridgeTetris.Tetris.randomPiece();
            BridgeTetris.Tetris.invalidate();
        }

        private static void setNextPiece(BridgeTetris.Tetris.Piece piece = null)
        {
            BridgeTetris.Tetris.next = piece ?? BridgeTetris.Tetris.randomPiece();
            BridgeTetris.Tetris.invalidateNext();
        }

        private static void reset()
        {
            BridgeTetris.Tetris.dt = 0;
            BridgeTetris.Tetris.clearActions();
            BridgeTetris.Tetris.clearBlocks();
            BridgeTetris.Tetris.clearRows();
            BridgeTetris.Tetris.clearScore();
            BridgeTetris.Tetris.setCurrentPiece(next);
            BridgeTetris.Tetris.setNextPiece();
        }

        private static void update(double idt)
        {
            if (BridgeTetris.Tetris.playing)
            {
                if (BridgeTetris.Tetris.vscore < BridgeTetris.Tetris.score)
                {
                    BridgeTetris.Tetris.setVisualScore(BridgeTetris.Tetris.vscore + 1);
                }

                BridgeTetris.Tetris.handle(BridgeTetris.Tetris.actions.Shift().As<int>()); // FIXME: Shift() should already return the type of the actions array.

                BridgeTetris.Tetris.dt = BridgeTetris.Tetris.dt + idt;
                if (BridgeTetris.Tetris.dt > BridgeTetris.Tetris.step)
                {
                    BridgeTetris.Tetris.dt = BridgeTetris.Tetris.dt - BridgeTetris.Tetris.step;
                    BridgeTetris.Tetris.drop();
                }
            }
        }

        private static void handle(int action)
        {
            switch (action)
            {
                case BridgeTetris.Tetris.DIR.LEFT:
                case BridgeTetris.Tetris.DIR.RIGHT:
                    BridgeTetris.Tetris.move(action);
                    break;

                case BridgeTetris.Tetris.DIR.UP:
                    BridgeTetris.Tetris.rotate();
                    break;

                case BridgeTetris.Tetris.DIR.DOWN:
                    BridgeTetris.Tetris.drop();
                    break;
            }
        }

        private static bool move(int dir)
        {
            var x = BridgeTetris.Tetris.current.x;
            var y = BridgeTetris.Tetris.current.y;

            switch (dir)
            {
                case BridgeTetris.Tetris.DIR.RIGHT:
                    x++;
                    break;
                case BridgeTetris.Tetris.DIR.LEFT:
                    x--;
                    break;
                case BridgeTetris.Tetris.DIR.DOWN:
                    y++;
                    break;
            }

            if (BridgeTetris.Tetris.unoccupied(BridgeTetris.Tetris.current.type, x, y, BridgeTetris.Tetris.current.dir))
            {
                BridgeTetris.Tetris.current.x = x;
                BridgeTetris.Tetris.current.y = y;
                BridgeTetris.Tetris.invalidate();
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void rotate()
        {
            short newdir;
            if (BridgeTetris.Tetris.current.dir == DIR.MAX)
            {
                newdir = BridgeTetris.Tetris.DIR.MIN;
            }
            else
            {
                newdir = (short)(BridgeTetris.Tetris.current.dir + 1);
            }

            if (BridgeTetris.Tetris.unoccupied(BridgeTetris.Tetris.current.type, BridgeTetris.Tetris.current.x, BridgeTetris.Tetris.current.y, newdir))
            {
                BridgeTetris.Tetris.current.dir = newdir;
                BridgeTetris.Tetris.invalidate();
            }
        }

        private static void drop()
        {
            if (!BridgeTetris.Tetris.move(BridgeTetris.Tetris.DIR.DOWN))
            {
                BridgeTetris.Tetris.addScore(10);
                BridgeTetris.Tetris.dropPiece();
                BridgeTetris.Tetris.removeLines();
                BridgeTetris.Tetris.setCurrentPiece(BridgeTetris.Tetris.next);
                BridgeTetris.Tetris.setNextPiece(BridgeTetris.Tetris.randomPiece());
                BridgeTetris.Tetris.clearActions();
                if (BridgeTetris.Tetris.occupied(BridgeTetris.Tetris.current.type, BridgeTetris.Tetris.current.x, BridgeTetris.Tetris.current.y, BridgeTetris.Tetris.current.dir))
                {
                    BridgeTetris.Tetris.lose();
                }
            }
        }

        private static void dropPiece()
        {
            var matchingCells = BridgeTetris.Tetris.eachblock(BridgeTetris.Tetris.current.type, BridgeTetris.Tetris.current.x, BridgeTetris.Tetris.current.y, BridgeTetris.Tetris.current.dir);

            foreach (var tuple in matchingCells)
            {
                BridgeTetris.Tetris.setBlock(tuple.Item1, tuple.Item2, BridgeTetris.Tetris.current.type);
            }
        }

        private static void removeLines()
        {
            int n = 0;
            bool complete = false;

            for (var y = BridgeTetris.Tetris.ny; y > 0; --y)
            {
                complete = true;

                for (var x = 0; x < nx; ++x)
                {
                    if (BridgeTetris.Tetris.getBlock(x, y) == null)
                    {
                        complete = false;
                    }
                }

                if (complete)
                {
                    BridgeTetris.Tetris.removeLine(y);
                    y++; // recheck same line
                    n++;
                }
            }

            if (n > 0)
            {
                BridgeTetris.Tetris.addRows(n);
                BridgeTetris.Tetris.addScore(100 * (int)Math.Pow(2, n - 1)); // 1:100, 2:200, 3:400, 4:800
            }
        }

        private static void removeLine(int n)
        {
            for (var y = n; y >= 0; --y)
            {
                for (var x = 0; x < nx; ++x)
                {
                    BridgeTetris.Tetris.setBlock(x, y, (y == 0) ? null : BridgeTetris.Tetris.getBlock(x, y - 1));
                }
            }
        }

        #endregion

        #region RENDERING

        private static class invalid
        {
            public static bool
                court = false,
                next = false,
                score = false,
                rows = false;
        }

        private static void invalidate()
        {
            BridgeTetris.Tetris.invalid.court = true;
        }

        private static void invalidateNext()
        {
            BridgeTetris.Tetris.invalid.next = true;
        }

        private static void invalidateScore()
        {
            BridgeTetris.Tetris.invalid.score = true;
        }

        private static void invalidateRows()
        {
            BridgeTetris.Tetris.invalid.rows = true;
        }

        private static void draw()
        {
            BridgeTetris.Tetris.ctx.Save();
            BridgeTetris.Tetris.ctx.LineWidth = 1;
            BridgeTetris.Tetris.ctx.Translate(0.5, 0.5); // for crisp 1px black lines

            BridgeTetris.Tetris.drawCourt();
            BridgeTetris.Tetris.drawNext();
            BridgeTetris.Tetris.drawScore();
            BridgeTetris.Tetris.drawRows();

            BridgeTetris.Tetris.ctx.Restore();
        }

        private static void drawCourt()
        {
            if (BridgeTetris.Tetris.invalid.court)
            {
                BridgeTetris.Tetris.ctx.ClearRect(0, 0, BridgeTetris.Tetris.canvas.Width, BridgeTetris.Tetris.canvas.Height);

                if (BridgeTetris.Tetris.playing)
                {
                    BridgeTetris.Tetris.drawPiece(BridgeTetris.Tetris.ctx, BridgeTetris.Tetris.current.type, BridgeTetris.Tetris.current.x, BridgeTetris.Tetris.current.y, BridgeTetris.Tetris.current.dir);
                }

                BridgeTetris.Tetris.PieceType block;

                for (int y = 0; y < BridgeTetris.Tetris.ny; y++)
                {
                    for (int x = 0; x < BridgeTetris.Tetris.nx; x++)
                    {
                        block = BridgeTetris.Tetris.getBlock(x, y);
                        if (block != null)
                        {
                            BridgeTetris.Tetris.drawBlock(BridgeTetris.Tetris.ctx, x, y, block.color);
                        }
                    }
                }

                BridgeTetris.Tetris.ctx.StrokeRect(0, 0, (BridgeTetris.Tetris.nx * BridgeTetris.Tetris.dx) - 1, (BridgeTetris.Tetris.ny * BridgeTetris.Tetris.dy) - 1); // court boundary

                BridgeTetris.Tetris.invalid.court = false;
            }
        }

        private static void drawNext()
        {
            if (BridgeTetris.Tetris.invalid.next)
            {
                var padding = (BridgeTetris.Tetris.nu - BridgeTetris.Tetris.next.type.size) / 2; // half-arsed attempt at centering next piece display

                BridgeTetris.Tetris.uctx.Save();
                BridgeTetris.Tetris.uctx.Translate(0.5, 0.5);
                BridgeTetris.Tetris.uctx.ClearRect(0, 0, BridgeTetris.Tetris.nu * BridgeTetris.Tetris.dx, BridgeTetris.Tetris.nu * BridgeTetris.Tetris.dy);

                BridgeTetris.Tetris.drawPiece(BridgeTetris.Tetris.uctx, BridgeTetris.Tetris.next.type, padding, padding, BridgeTetris.Tetris.next.dir);

                BridgeTetris.Tetris.uctx.StrokeStyle = "black";
                BridgeTetris.Tetris.uctx.StrokeRect(0, 0, (BridgeTetris.Tetris.nu * BridgeTetris.Tetris.dx) - 1, (BridgeTetris.Tetris.nu * BridgeTetris.Tetris.dy) - 1);
                BridgeTetris.Tetris.uctx.Restore();

                BridgeTetris.Tetris.invalid.next = false;
            }
        }

        private static void drawScore()
        {
            if (BridgeTetris.Tetris.invalid.score)
            {
                BridgeTetris.Tetris.html("score", ("00000" + Math.Floor(vscore)).Slice(-5));
                BridgeTetris.Tetris.invalid.score = false;
            }
        }

        private static void drawRows()
        {
            if (BridgeTetris.Tetris.invalid.rows)
            {
                BridgeTetris.Tetris.html("rows", rows.ToString());
                BridgeTetris.Tetris.invalid.rows = false;
            }
        }

        private static void drawPiece(CanvasRenderingContext2D ctx, BridgeTetris.Tetris.PieceType type, int x, int y, short dir)
        {
            var matchingCells = BridgeTetris.Tetris.eachblock(type, x, y, dir);

            foreach (var tuple in matchingCells)
            {
                BridgeTetris.Tetris.drawBlock(BridgeTetris.Tetris.ctx, tuple.Item1, tuple.Item2, type.color);
            }
        }

        private static void drawBlock(CanvasRenderingContext2D ctx, int x, int y, string color)
        {
            BridgeTetris.Tetris.ctx.FillStyle = color;
            BridgeTetris.Tetris.ctx.FillRect(x * BridgeTetris.Tetris.dx, y * BridgeTetris.Tetris.dy, BridgeTetris.Tetris.dx, BridgeTetris.Tetris.dy);
            BridgeTetris.Tetris.ctx.StrokeRect(x * BridgeTetris.Tetris.dx, y * BridgeTetris.Tetris.dy, BridgeTetris.Tetris.dx, BridgeTetris.Tetris.dy);
        }

        #endregion

        /// <summary>
        /// Load the class upon page load. When DOM content is ready, actually.
        /// </summary>
        [Ready]
        public static void Main()
        {
            BridgeTetris.Tetris.LoadPlayArea(); // load page's placeholders
            BridgeTetris.Tetris.Run();          // effectively start the game engine (will listen for 'spacebar' to begin game)
        }
    }
}