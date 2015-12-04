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
        }

        public static void InitCube(string canvasId)
        {
            var cube = new Cube();

            App.InitSettings(cube);

            cube.canvas = App.GetCanvasEl(canvasId);
            cube.gl = App.Create3DContext(cube.canvas);

            if (cube.gl != null)
            {
                cube.InitShaders();
                cube.InitBuffers();
                cube.InitTexture();
                cube.Tick();

                Document.AddEventListener(EventType.KeyDown, cube.HandleKeyDown);
                Document.AddEventListener(EventType.KeyUp, cube.HandleKeyUp);
            }
            else
            {
                App.ShowError(cube.canvas, "<b>Either the browser doesn't support WebGL or it is disabled.<br>Please follow <a href=\"http://get.webgl.com\">Get WebGL</a>.</b>");
            }
        }

        public static CanvasElement GetCanvasEl(string id)
        {
            return Document.GetElementById(id).As<CanvasElement>();
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
                catch { }

                if (context != null)
                {
                    break;
                }
            }

            return context;
        }

        public static void ShowError(CanvasElement canvas, string message)
        {
            canvas.ParentElement.ReplaceChild(new ParagraphElement { InnerHTML = message }, canvas);
        }

        public static void InitSettings(Cube cube)
        {
            var useSettings = Document.GetElementById("settings").As<InputElement>();

            if (useSettings == null || !useSettings.Checked)
            {
                return;
            }

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

            cube.textureImageSrc = "crate.gif";
        }
    }
}
