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

        public static HTMLCanvasElement GetCanvasEl(string id)
        {
            return Document.GetElementById(id).As<HTMLCanvasElement>();
        }

        public static WebGLRenderingContext Create3DContext(HTMLCanvasElement canvas)
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

        public static void ShowError(HTMLCanvasElement canvas, string message)
        {
            canvas.ParentElement.ReplaceChild(new HTMLParagraphElement { InnerHTML = message }, canvas);
        }

        public static void InitSettings(Cube cube)
        {
            var useSettings = Document.GetElementById("settings").As<HTMLInputElement>();

            if (useSettings == null || !useSettings.Checked)
            {
                return;
            }

            cube.useBlending = Document.GetElementById("blending").As<HTMLInputElement>().Checked;
            cube.alpha = Global.ParseFloat(Document.GetElementById("alpha").As<HTMLInputElement>().Value);

            cube.useLighting = Document.GetElementById("lighting").As<HTMLInputElement>().Checked;

            cube.ambientR = Global.ParseFloat(Document.GetElementById("ambientR").As<HTMLInputElement>().Value);
            cube.ambientG = Global.ParseFloat(Document.GetElementById("ambientG").As<HTMLInputElement>().Value);
            cube.ambientB = Global.ParseFloat(Document.GetElementById("ambientB").As<HTMLInputElement>().Value);

            cube.lightDirectionX = Global.ParseFloat(Document.GetElementById("lightDirectionX").As<HTMLInputElement>().Value);
            cube.lightDirectionY = Global.ParseFloat(Document.GetElementById("lightDirectionY").As<HTMLInputElement>().Value);
            cube.lightDirectionZ = Global.ParseFloat(Document.GetElementById("lightDirectionZ").As<HTMLInputElement>().Value);

            cube.directionalR = Global.ParseFloat(Document.GetElementById("directionalR").As<HTMLInputElement>().Value);
            cube.directionalG = Global.ParseFloat(Document.GetElementById("directionalG").As<HTMLInputElement>().Value);
            cube.directionalB = Global.ParseFloat(Document.GetElementById("directionalB").As<HTMLInputElement>().Value);

            cube.textureImageSrc = "crate.gif";
        }
    }
}
