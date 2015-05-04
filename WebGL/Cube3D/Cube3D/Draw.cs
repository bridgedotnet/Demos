using Bridge;
using Bridge.Html5;
using Bridge.WebGL;
using System;

namespace Cube3D
{
    public class App
    {
        [Ready]
        public static void Main()
        {
            App.InitCube("canvas1");
            App.InitCube("canvas2");
            App.InitCube("canvas3");
        }

        public static void InitCube(string canvasId)
        {
            var cube = new Cube();

            App.InitSettings(cube);

            cube.canvas = App.GetCanvasEl(canvasId);
            cube.gl = App.InitGL(cube.canvas);
            cube.InitShaders();
            cube.InitBuffers();
            cube.InitTexture();
            cube.Tick();

            Document.AddEventListener(EventType.KeyDown, cube.HandleKeyDown);
            Document.AddEventListener(EventType.KeyUp, cube.HandleKeyUp);
        }

        public static CanvasElement GetCanvasEl(string id)
        {
            return Document.GetElementById(id).As<CanvasElement>();
        }

        public static WebGLRenderingContext InitGL(CanvasElement canvas)
        {
            var gl = App.Create3DContext(canvas);

            if (gl == null)
            {
                Global.Alert("Could not initialise WebGL, sorry :-(");
            }

            return gl;
        }

        public static WebGLRenderingContext Create3DContext(CanvasElement canvas)
        {
            string[] names = new string[] 
            { 
                "webgl", 
                "experimental-webgl", 
                "webkit-3d", 
                "moz-webgl" 
            };

            WebGLRenderingContext context = null;

            foreach (string name in names)
            {
                try
                {
                    context = canvas.GetContext(name).As<WebGLRenderingContext>();
                }
                catch (Exception ex) { }

                if (context != null)
                {
                    break;
                }
            }

            return context;
        }

        public static void InitSettings(Cube cube)
        {
            cube.useBlending = Document.GetElementById("blending").As<InputElement>().Checked;
            cube.alpha = Global.ParseFloat(Document.GetElementById("alpha").As<InputElement>().Value);

            cube.useLighting = Document.GetElementById("lighting").As<InputElement>().Checked;

            cube.ambientR = Global.ParseFloat(Document.GetElementById("ambientR").As<InputElement>().Value);
            cube.ambientG = Global.ParseFloat(Document.GetElementById("ambientG").As<InputElement>().Value);
            cube.ambientB = Global.ParseFloat(Document.GetElementById("ambientB").As<InputElement>().Value);

            cube.lightDirectionX = Global.ParseFloat(Document.GetElementById("lightDirectionX").As<InputElement>().Value);
            cube.lightDirectionY = Global.ParseFloat(Document.GetElementById("lightDirectionY").As<InputElement>().Value);
            cube.lightDirectionZ = Global.ParseFloat(Document.GetElementById("lightDirectionZ").As<InputElement>().Value);

            cube.directionalR = Global.ParseFloat(Document.GetElementById("directionalR").As<InputElement>().Value);
            cube.directionalG = Global.ParseFloat(Document.GetElementById("directionalG").As<InputElement>().Value);
            cube.directionalB = Global.ParseFloat(Document.GetElementById("directionalB").As<InputElement>().Value);

            cube.textureImageSrc = "crate.png";
        }
    }
}
