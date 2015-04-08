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
    [FileName("TetrisClasses")]
    public class TetrisClasses
    {
        #region tetris elements' abstraction classes
        public class Piece
        {
            public PieceType type { get; set; }
            public short dir { get; set; }
            public int x { get; set; }
            public int y { get; set; }
        }

        public class PieceType
        {
            public int size { get; set; }
            public string color { get; set; }
            public int[] blocks { get; set; }

            public PieceType(int sz, int[] shape, string clr)
            {
                size = sz;
                color = clr;

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

                blocks = shape;
            }
        }

        #endregion
    }

    public class Tetris
    {
        private static void loadPlayArea()
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
            menuDiv.AppendChild(insideParagraph(pressStartTitle, "start"));

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

        private static Node insideParagraph(List<Node> elementList, string paragraphID = null)
        {
            var paraElem = new ParagraphElement();

            if (!String.IsNullOrWhiteSpace(paragraphID))
            {
                paraElem.Id = paragraphID;
            }

            foreach (var el in elementList)
            {
                paraElem.AppendChild(el);
            }

            return paraElem;
        }

        private static Node insideParagraph(Node element, string paragraphID = null)
        {
            return insideParagraph(new List<Node> { element }, paragraphID);
        }

        #region base helper methods

        // FIXME: Useless, as we have to prepend full class path anyway. Left just for reference to original code.
        private static Element get(string id)
        {
            return Document.GetElementById(id);
        }

        private static void hide(string id)
        {
            get(id).Style.Visibility = Visibility.Hidden;
        }

        private static void show(string id)
        {
            //get(id).Style.Visibility = null; // FIXME: this does not work!
            get(id).Style.Visibility = Visibility.Inherit;
        }

        private static void html(string id, string html)
        {
            get(id).InnerHTML = html;
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
                ESC = 27,
                SPACE = 32,
                LEFT = 37,
                UP = 38,
                RIGHT = 39,
                DOWN = 40;
        }

        private static class DIR
        {
            public const short
                UP = 0,
                RIGHT = 1,
                DOWN = 2,
                LEFT = 3,
                MIN = 0,
                MAX = 3;
        };

        // private start = new Stats() -- optional, included on another .js!

        private static CanvasElement canvas;
        private static CanvasElement ucanvas;

        private static CanvasRenderingContext2D ctx;
        private static CanvasRenderingContext2D uctx;

        private static void loadCanvasContext()
        {
            // FIXME: Shouldn't it allow returning canvasElement without a cast??
            canvas = get("canvas").As<CanvasElement>();
            ucanvas = get("upcoming").As<CanvasElement>();

            ctx = canvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            uctx = ucanvas.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
        }

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

        #region game variables (initialized during reset)

        private static int dx, // pixel width of a tetris block
                           dy, // pixel height of a tetris block
                        score, // the current score
                       vscore, // the currently displayed score (catches up to score in small chunks like slot machine)
                         rows; // number of completed rows in the current game

        // 2 dimensional array (nx*ny) representing tetris court - either empty block or occupied by a 'piece'
        private static TetrisClasses.PieceType[,] blocks = new TetrisClasses.PieceType[nx, ny];

        private static int[] actions = new int[0]; // queue of user actions (inputs)

        private static TetrisClasses.Piece current, // the current piece
                                                    next; // the next piece

        private static bool playing; // true|false - game is in progress
        private static double dt,    // time since starting the game
                            step,    // how long before current piece drops by 1 row
                             now,    // current timestamp (used on frame())
                            last;    // last timestamp (used on frame())

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

        private static TetrisClasses.PieceType
            i = new TetrisClasses.PieceType(4, new int[] { 0x0F00, 0x2222, 0x00F0, 0x4444 }, "cyan"),
            j = new TetrisClasses.PieceType(3, new int[] { 0x44C0, 0x8E00, 0x6440, 0x0E20 }, "blue"),
            l = new TetrisClasses.PieceType(3, new int[] { 0x4460, 0x0E80, 0xC440, 0x2E00 }, "orange"),
            o = new TetrisClasses.PieceType(2, new int[] { 0xCC00, 0xCC00, 0xCC00, 0xCC00 }, "yellow"),
            s = new TetrisClasses.PieceType(3, new int[] { 0x06C0, 0x8C40, 0x6C00, 0x4620 }, "green"),
            t = new TetrisClasses.PieceType(3, new int[] { 0x0E40, 0x4C40, 0x4E00, 0x4640 }, "purple"),
            z = new TetrisClasses.PieceType(3, new int[] { 0x0C60, 0x4C80, 0xC600, 0x2640 }, "red");
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
        private static Tuple<int, int>[] eachblock(TetrisClasses.PieceType type, int x, int y, short dir)
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
            return (x < 0 || x >= nx || y < 0 || y >= ny || getBlock(x, y) != null);
        }

        // check if a piece can fit into a position in the grid
        private static bool occupied(TetrisClasses.PieceType type, int x, int y, short dir)
        {
            var matchingCells = eachblock(type, x, y, dir); // FIXME: 'result' logic must be ensured to work like on JS
            foreach (var tuple in matchingCells)
            {
                if (pieceCanFit(tuple.Item1, tuple.Item2))
                {
                    return true;
                }
            }
            return false; // FIXME: maybe just return eachblock result??
        }

        private static bool unoccupied(TetrisClasses.PieceType type, int x, int y, short dir)
        {
            return !occupied(type, x, y, dir);
        }

        // start with 4 instances of each piece and
        // pick randomly until the 'bag is empty'
        //private static List<Piece> pieces = new List<Piece>();
        private static TetrisClasses.PieceType[] pieces;

        private static TetrisClasses.Piece randomPiece()
        {
            pieces = new TetrisClasses.PieceType[] { i, i, i, i, j, j, j, j, l, l, l, l, o, o, o, o, s, s, s, s, t, t, t, t, z, z, z, z };
            var type = (TetrisClasses.PieceType)pieces.Splice((int)random(0, pieces.Length - 1), 1)[0]; // This does not cast as Piece?

            return new TetrisClasses.Piece
            {
                dir = DIR.UP,
                type = type,
                x = (int)Math.Round(random(0, nx - type.size)),
                y = 0
            };
        }

        #region GAME LOOP

        private static void run()
        {
            //showStats(); // initialize FPS counter (defined in external .js)
            addEvents();

            var last = now = timestamp();

            resize(); // setup all our sizing information
            reset();  // reset the per-game variables
            frame();  // start the first frame
        }

        // FIXME: Frame() has a 'double' argument cause callback on Window.RequestAnimationFrame() can only
        //        be Action<double>
        private static void frame()
        {
            now = timestamp();

            // using requestAnimationFrame have to be able to handle large delta's caused when it 'hibernates' in a background or non-visible tab
            update(Math.Min(1, (now - last) / 1000.0));

            draw();

            last = now;

            Window.RequestAnimationFrame(af => frame());
        }

        /*TODO: this is not really the game and uses external javascript, so let's work on this later
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

        public static void resize(Event evnt = null) // 'event' is a reserved keyword
        {
            // set canvas logical size equal to its physical size
            canvas.Width = canvas.ClientWidth;
            canvas.Height = canvas.ClientHeight;

            ucanvas.Width = ucanvas.ClientWidth;
            ucanvas.Height = ucanvas.ClientHeight;

            // pixel size of a single tetris block
            dx = canvas.ClientWidth / nx;
            dy = canvas.ClientHeight / ny;

            invalidate();
            invalidateNext();
        }

        public static void keydown(Event ev)
        {
            var handled = false;
            var kev = ev.As<KeyboardEvent>();
            if (playing)
            {
                switch (kev.KeyCode)
                {
                    case KEY.LEFT:
                        actions.Push(DIR.LEFT);
                        handled = true;
                        break;
                    case KEY.RIGHT:
                        actions.Push(DIR.RIGHT);
                        handled = true;
                        break;
                    case KEY.UP:
                        actions.Push(DIR.UP);
                        handled = true;
                        break;
                    case KEY.DOWN:
                        actions.Push(DIR.DOWN);
                        handled = true;
                        break;
                    case KEY.ESC:
                        lose();
                        handled = true;
                        break;
                }
            }
            else
            {
                if (kev.KeyCode == KEY.SPACE)
                {
                    play();
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
            hide("start");
            reset();
            playing = true;
        }

        private static void lose()
        {
            show("start");
            setVisualScore();
            playing = false;
        }

        private static void setVisualScore(int? n = null)
        {
            vscore = n ?? score;
            invalidateScore();
        }

        private static void setScore(int n)
        {
            score = n;
            setVisualScore(n);
        }

        private static void addScore(int n)
        {
            score += n;
        }

        private static void clearScore()
        {
            setScore(0);
        }

        private static void clearRows()
        {
            setRows(0);
        }

        private static void setRows(int n)
        {
            var speedMin = Speed.min;
            var speedStart = Speed.start;
            var speedDec = Speed.decrement;
            var rowCount = rows;

            rows = n;
            step = Math.Max(speedMin, speedStart - (speedDec * rowCount));
            invalidateRows();
        }

        private static void addRows(int n)
        {
            setRows(rows + n);
        }

        private static TetrisClasses.PieceType getBlock(int x, int y)
        {
            TetrisClasses.PieceType retval = null;

            if (x >= 0 && x < nx && blocks.Length > ((x + 1) * nx)
                && y > 0 && y < ny) // FIXME: JavaScript logic removes requirements to check against y-axis here.
            {
                retval = blocks[x, y];
            }

            return retval;
        }

        private static void setBlock(int x, int y, TetrisClasses.PieceType type)
        {
            // FIXME: understand what this does and ensure whether it is needed or not!
            //        seems to be just a js trick to ensure the array has been allocated
            //blocks[x] = blocks[x] || null; // does not make much sense!
            // maybe the test below does what this would: (disallow allocating blocks outside court longitudinally)
            if (x >= 0 && x < nx)
            {
                blocks[x, y] = type;
            }
            invalidate();
        }

        private static void clearBlocks()
        {
            blocks = new TetrisClasses.PieceType[nx, ny];
        }

        private static void clearActions()
        {
            actions = new int[0];
        }

        private static void setCurrentPiece(TetrisClasses.Piece piece)
        {
            current = piece ?? randomPiece();
            invalidate();
        }

        private static void setNextPiece(TetrisClasses.Piece piece = null)
        {
            next = piece ?? randomPiece();
            invalidateNext();
        }

        private static void reset()
        {
            dt = 0;
            clearActions();
            clearBlocks();
            clearRows();
            clearScore();
            setCurrentPiece(next);
            setNextPiece();
        }

        private static void update(double idt)
        {
            if (playing)
            {
                if (vscore < score)
                {
                    setVisualScore(vscore + 1);
                }

                handle(actions.Shift().As<int>()); // FIXME: Shift() should already return the type of the actions array.

                dt = dt + idt;
                if (dt > step)
                {
                    dt = dt - step;
                    drop();
                }
            }
        }

        private static void handle(int action)
        {
            switch (action)
            {
                case DIR.LEFT:
                case DIR.RIGHT:
                    move(action);
                    break;

                case DIR.UP:
                    rotate();
                    break;

                case DIR.DOWN:
                    drop();
                    break;
            }
        }

        private static bool move(int dir)
        {
            var x = current.x;
            var y = current.y;

            switch (dir)
            {
                case DIR.RIGHT:
                    x++;
                    break;
                case DIR.LEFT:
                    x--;
                    break;
                case DIR.DOWN:
                    y++;
                    break;
            }

            if (unoccupied(current.type, x, y, current.dir))
            {
                current.x = x;
                current.y = y;
                invalidate();
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
            if (current.dir == DIR.MAX)
            {
                newdir = DIR.MIN;
            }
            else
            {
                newdir = (short)(current.dir + 1);
            }

            if (unoccupied(current.type, current.x, current.y, newdir))
            {
                current.dir = newdir;
                invalidate();
            }
        }

        private static void drop()
        {
            if (!move(DIR.DOWN))
            {
                addScore(10);
                dropPiece();
                removeLines();
                setCurrentPiece(next);
                setNextPiece(randomPiece());
                clearActions();
                if (occupied(current.type, current.x, current.y, current.dir))
                {
                    lose();
                }
            }
        }

        private static void dropPiece()
        {
            var matchingCells = eachblock(current.type, current.x, current.y, current.dir);

            foreach (var tuple in matchingCells)
            {
                setBlock(tuple.Item1, tuple.Item2, current.type);
            }
        }

        private static void removeLines()
        {
            int n = 0;
            bool complete = false;

            for (var y = ny; y > 0; --y)
            {
                complete = true;

                for (var x = 0; x < nx; ++x)
                {
                    if (getBlock(x, y) == null)
                    {
                        complete = false;
                    }
                }

                if (complete)
                {
                    removeLine(y);
                    y++; // recheck same line
                    n++;
                }
            }

            if (n > 0)
            {
                addRows(n);
                addScore(100 * (int)Math.Pow(2, n - 1)); // 1:100, 2:200, 3:400, 4:800
            }
        }

        private static void removeLine(int n)
        {
            for (var y = n; y >= 0; --y)
            {
                for (var x = 0; x < nx; ++x)
                {
                    setBlock(x, y, (y == 0) ? null : getBlock(x, y - 1));
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
            invalid.court = true;
        }

        private static void invalidateNext()
        {
            invalid.next = true;
        }

        private static void invalidateScore()
        {
            invalid.score = true;
        }

        private static void invalidateRows()
        {
            invalid.rows = true;
        }

        private static void draw()
        {
            ctx.Save();
            ctx.LineWidth = 1;
            ctx.Translate(0.5, 0.5); // for crisp 1px black lines

            drawCourt();
            drawNext();
            drawScore();
            drawRows();

            ctx.Restore();
        }

        private static void drawCourt()
        {
            if (invalid.court)
            {
                ctx.ClearRect(0, 0, canvas.Width, canvas.Height);

                if (playing)
                {
                    drawPiece(ctx, current.type, current.x, current.y, current.dir);
                }

                TetrisClasses.PieceType block;

                for (int y = 0; y < ny; y++)
                {
                    for (int x = 0; x < nx; x++)
                    {
                        block = getBlock(x, y);
                        if (block != null)
                        {
                            drawBlock(ctx, x, y, block.color);
                        }
                    }
                }

                ctx.StrokeRect(0, 0, (nx * dx) - 1, (ny * dy) - 1); // court boundary

                invalid.court = false;
            }
        }

        private static void drawNext()
        {
            if (invalid.next)
            {
                var padding = (nu - next.type.size) / 2; // half-arsed attempt at centering next piece display

                uctx.Save();
                uctx.Translate(0.5, 0.5);
                uctx.ClearRect(0, 0, nu * dx, nu * dy);

                drawPiece(uctx, next.type, padding, padding, next.dir);

                uctx.StrokeStyle = "black";
                uctx.StrokeRect(0, 0, (nu * dx) - 1, (nu * dy) - 1);
                uctx.Restore();

                invalid.next = false;
            }
        }

        private static void drawScore()
        {
            if (invalid.score)
            {
                html("score", ("00000" + Math.Floor(vscore)).Slice(-5));
                invalid.score = false;
            }
        }

        private static void drawRows()
        {
            if (invalid.rows)
            {
                html("rows", rows.ToString());
                invalid.rows = false;
            }
        }

        private static void drawPiece(CanvasRenderingContext2D ctx, TetrisClasses.PieceType type, int x, int y, short dir)
        {
            var matchingCells = eachblock(type, x, y, dir);

            foreach (var tuple in matchingCells)
            {
                drawBlock(ctx, tuple.Item1, tuple.Item2, type.color);
            }
        }

        private static void drawBlock(CanvasRenderingContext2D ctx, int x, int y, string color)
        {
            ctx.FillStyle = color;
            ctx.FillRect(x * dx, y * dy, dx, dy);
            ctx.StrokeRect(x * dx, y * dy, dx, dy);
        }

        #endregion

        /// <summary>
        /// Load the class upon page load. When DOM content is ready, actually.
        /// </summary>
        [Ready]
        public static void LoadGame()
        {
            loadPlayArea(); // load page's placeholders
            loadCanvasContext();
            run();          // effectively start the game engine (will listen for 'spacebar' to begin game)
        }
    }
}