using Bridge;
using Bridge.Html5;
using System;
using System.Collections.Generic;

namespace BridgeTetris
{
    public class Tetris
    {
        [Ready]
        public static void loadPlayArea()
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

        protected static Node insideParagraph(List<Node> elementList)
        {
            var paraElem = new ParagraphElement();
            
            foreach (var el in elementList)
            {
                paraElem.AppendChild(el);
            }

            return paraElem;
        }

        protected static Node insideParagraph(Node element)
        {
            return insideParagraph(new List<Node> { element });
        }

        #region base helper methods
        protected static Element get(string id)
        {
            return Document.GetElementById(id);
        }

        protected static void hide(string id)
        {
            get(id).Style.Visibility = Visibility.Hidden;
        }

        protected static void show(string id)
        {
            //get(id).Style.Visibility = null; this does not work!
            get(id).Style.Visibility = Visibility.Visible;
        }

        protected static void html(string id, string html)
        {
            get(id).InnerHTML = html;
        }

        protected static double timestamp()
        {
            return new Date().GetTime();
        }

        protected static double random(int min, int max)
        {
            return (min + (Math.Random() * (max - min)));
        }

        //protected static double randomChoice() // Not used!? Can't guess types if not used..

        #region See what to do here later (this is optional)

        public static void fixReqAnimFrame() // unnecessary on bridge.net?
        { // http://paulirish.com/2011/requestanimationframe-for-smart-animating/
            //if (Window.RequestAnimationFrame == null)
            //Window.WebKitRequestAnimationFrame
            //Window.mozRequestAnimationFrame
            //Window.ORequestAnimationFrame
            //Window.MSRequestAnimationFrame
            // callback function
        }

        protected static void callBackReqAnimFrame(Func<Func<string, Node>, Node> callback)
        {
            Window.SetTimeout(callback, 1000 / 60);
        }

        #endregion

        #endregion

        #region game constants
        protected static Object KEY = new
        {
            ESC = 27,
            SPACE = 32,
            LEFT = 37,
            UP = 38,
            RIGHT = 39,
            DOWN = 40
        };

        protected static Object DIR = new
        {
            UP = 0,
            RIGHT = 1,
            DOWN = 2,
            LEFT = 3,
            MIN = 0,
            MAX = 3
        };

        // protected start = new Stats() -- optional, included on another .js!

        protected static Element canvas = get("canvas");


        protected static Element ucanvas = get("upcoming");

        // FIXME: canvas has no getContext on Bridge :(
        //protected static wat ctx = canvas.GetContext('2d');
        //protected static wat uctx = ucanvas.GetContext('2d');

        // how long before piece drops by 1 row (seconds)
        protected static Object speed = new { start = 0.6, decrement = 0.005, min = 0.1 };

        protected static int nx = 10, // width of tetris court (in blocks)
                             ny = 20, // height of tetris court (in blocks)
                             nu = 5;  // width/heigth of upcoming preview (in blocks)
        #endregion

        #region tetris elements' abstraction classes
        protected class TetrisCourt
        {
            public bool[,] court;
            public TetrisCourt()
            {
                court = new bool[nx, ny];
            }
        }
        protected class Piece
        {
            public int size { get; set; }
            public string color { get; set; }
            public Dictionary<short, int> blocks { get; set; }
            public Piece(int sz, List<int> shape, string clr)
            {
                size = sz;
                color = clr;
                if (shape.Count < 1)
                {
                    shape.Add(0xCC00); // 2x2 square
                }
                if (shape.Count < 4)
                {
                    for (int i = shape.Count - 1; i < 4; i++) {
                        shape[i] = shape[i - 1];
                    }
                }
                blocks = new Dictionary<short, int>
                {
                    { 0, shape[0] },
                    { 90, shape[1] },
                    { 180, shape[2] },
                    { 270, shape[3] }
                };
            }
        }
        #endregion

        #region game variables (initialized during reset)
        protected static int dx, // pixel width of a tetris block
                             dy, // pixel height of a tetris block
                             dt, // time since starting the game
                         vscore, // the currently displayed score (catches up to score in small chunks like slot machine)
                           rows; // number of completed rows in the current game


        // 2 dimensional array (nx*ny) representing tetris court - either empty block or occupied by a 'piece'
        protected static TetrisCourt blocks = new TetrisCourt();

        protected static List<int> actions; // queue of user actions (inputs)

        protected static Piece current, // the current piece
                                  next; // the next piece

        protected static bool playing; // true|false - game is in progress
        protected static double step; // how long before current piece drops by 1 row
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

        protected static Piece
            i = new Piece(4, new List<int> { 0x0F00, 0x2222, 0x00F0, 0x4444 }, "cyan"),
            j = new Piece(3, new List<int> { 0x44C0, 0x8E00, 0x6440, 0x0E20 }, "blue"),
            l = new Piece(3, new List<int> { 0x4460, 0x0E80, 0xC440, 0x2E00 }, "orange"),
            o = new Piece(2, new List<int> { 0xCC00, 0xCC00, 0xCC00, 0xCC00 }, "yellow"),
            s = new Piece(3, new List<int> { 0x06C0, 0x8C40, 0x6C00, 0x4620 }, "green"),
            t = new Piece(3, new List<int> { 0x0E40, 0x4C40, 0x4E00, 0x4640 }, "purple"),
            z = new Piece(3, new List<int> { 0x0C60, 0x4C80, 0xC600, 0x2640 }, "red");
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
        protected static bool eachblock(Piece type, int x, int y, short dir, Func<int, int, bool> fn)
        {
            int row = 0,
                col = 0,
                blocks = type.blocks[dir];
            var result = true; // FIXME: 'result' logic must be ensured to work like on JS

            for (int bit = 0x8000; bit > 0; bit = bit >> 1)
            {
                if ((blocks & bit) > 0)
                {
                    // FIXME: 'result' logic must be ensured to work like on JS
                    //        return on first false is enough?
                    result = fn(x + col, y + row); // fixme> action or func? (returns nothing or something?)
                }
                if (++col == 4)
                {
                    col = 0;
                    ++row;
                }
            }
            return result; // FIXME: 'result' logic must be ensured to work like on JS
        }

        // this is the function delegated to eachblock from occupied:
        protected static bool pieceCanFit(int x, int y)
        {
            return (x < 0 || x >= nx || y < 0 || y >= ny || getBlock(x, y)) ? true : false;
        }

        // check if a piece can fit into a position in the grid
        protected static bool occupied(Piece type, int x, int y, short dir)
        {
            var result = false; // FIXME: 'result' logic must be ensured to work like on JS
            Func<int, int, bool> pCF = pieceCanFit;
            result = eachblock(type, x, y, dir, pCF); // FIXME: 'result' logic must be ensured to work like on JS
            return result; // FIXME: maybe just return eachblock result??
        }

        protected static bool unoccupied(Piece type, int x, int y, short dir)
        {
            return !occupied(type, x, y, dir);
        }

        // start with 4 instances of each piece and
        // pick randomly until the 'bag is empty'
        protected static List<Piece> pieces = new List<Piece>();

        protected class PieceLambda // FIXME: this should be Piece, Piece above might be PieceDefinition
        { 
            public Piece type { get; set; }
            public short dir { get; set; }
            public int x { get; set; }
            public int y { get; set; }
        }
        protected static PieceLambda randomPiece()
        {
            if (pieces.Count < 1)
            {
                foreach (var piece in new[] { i, j, l, o, s, t, z })
                {
                    for (var pi = 1; pi <= 4; pi++)
                    {
                        pieces.Add(piece);
                    }
                }
            }
            
            // FIXMEFIXMEFIXME: IENumerable.Splice is not returning anything!?
            //  seems it should return a list so the first element could be chosen as line 164.
            //var type = pieces.Splice((int)random(0, pieces.Count - 1), 1);
            return new PieceLambda
            {
                dir = DIR.UP,
                type = type,
                x = Math.Round(random(0, nx - type.size)),
                y = 0
            };
        }
        #region GAME LOOP
        #endregion

        #region GAME LOGIC
        private static bool getBlock(int x, int y)
        {
            throw new NotImplementedException();
        }
        #endregion
        public static void play()
        {
        }
    }
}