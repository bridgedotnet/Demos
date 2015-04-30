using Bridge;
using Bridge.Html5;
using Bridge.WebGL;
using Html5.TypedArrays;
using System;

namespace Cube3D
{
    public static class Draw
    {
        public static CanvasElement canvas;
        public static WebGLRenderingContext GL;
        public static WebGLProgram program;
        public static WebGLTexture texture;

        public static double[] mvMatrix = Script.Call<double[]>("mat4.create");
        public static double[][] mvMatrixStack = new double[][] { };
        public static double[] pMatrix = Script.Call<double[]>("mat4.create");

        public static int vertexPositionAttribute, vertexNormalAttribute, textureCoordAttribute;

        public static WebGLUniformLocation pMatrixUniform, mvMatrixUniform, nMatrixUniform, samplerUniform;
        public static WebGLBuffer cubeVertexPositionBuffer, cubeVertexNormalBuffer, cubeVertexTextureCoordBuffer, cubeVertexIndexBuffer;

        public static double xRot = 0;
        public static int xSpeed = 3;

        public static double yRot = 0;
        public static int ySpeed = -3;

        public static double z = -5.0;
        public static bool[] currentlyPressedKeys = new bool[] { };

        public static double lastTime = 0;

        public static WebGLRenderingContext Create3DContext(CanvasElement canvas)
        {
            string[] names = new string[] { "webgl", "experimental-webgl", "webkit-3d", "moz-webgl" };
            WebGLRenderingContext context = null;

            foreach (string name in names)
            {
                try
                {
                    context = canvas.GetContext(name).As<WebGLRenderingContext>();
                }
                catch (Exception e) { }

                if (context != null)
                {
                    break;
                }
            }

            return context;
        }

        public static void InitGL(CanvasElement canvas)
        {
            var gl = Draw.Create3DContext(canvas);

            if (gl == null)
            {
                Window.Alert("Could not initialise WebGL, sorry :-(");
            }

            Draw.GL = gl;
        }

        public static WebGLShader GetShader(WebGLRenderingContext gl, string id)
        {
            var shaderScript = Document.GetElementById(id).As<ScriptElement>();

            if (shaderScript == null)
            {
                return null;
            }

            var str = "";
            var k = shaderScript.FirstChild;

            while (k != null)
            {
                if (k.NodeType == NodeType.Text)
                {
                    str += k.TextContent;
                }

                k = k.NextSibling;
            }

            WebGLShader shader;

            if (shaderScript.Type == "x-shader/x-fragment")
            {
                shader = gl.CreateShader(gl.FRAGMENT_SHADER);
            }
            else if (shaderScript.Type == "x-shader/x-vertex")
            {
                shader = gl.CreateShader(gl.VERTEX_SHADER);
            }
            else
            {
                return null;
            }

            gl.ShaderSource(shader, str);
            gl.CompileShader(shader);

            if (!gl.GetShaderParameter(shader, gl.COMPILE_STATUS).As<bool>())
            {
                Window.Alert(gl.GetShaderInfoLog(shader));
                return null;
            }

            return shader;
        }

        public static void InitShaders()
        {
            var gl = Draw.GL;
            var fragmentShader = Draw.GetShader(gl, "shader-fs");
            var vertexShader = Draw.GetShader(gl, "shader-vs");

            var shaderProgram = gl.CreateProgram().As<WebGLProgram>();

            if (shaderProgram.Is<int>())
            {
                Window.Alert("Could not initialise program");
            }

            gl.AttachShader(shaderProgram, vertexShader);
            gl.AttachShader(shaderProgram, fragmentShader);
            gl.LinkProgram(shaderProgram);

            if (!gl.GetProgramParameter(shaderProgram, gl.LINK_STATUS).As<bool>())
            {
                Window.Alert("Could not initialise shaders");
            }

            gl.UseProgram(shaderProgram);

            Draw.vertexPositionAttribute = gl.GetAttribLocation(shaderProgram, "aVertexPosition");
            gl.EnableVertexAttribArray(Draw.vertexPositionAttribute);

            Draw.vertexNormalAttribute = gl.GetAttribLocation(shaderProgram, "aVertexNormal");
            gl.EnableVertexAttribArray(Draw.vertexNormalAttribute);

            Draw.textureCoordAttribute = gl.GetAttribLocation(shaderProgram, "aTextureCoord");
            gl.EnableVertexAttribArray(Draw.textureCoordAttribute);

            Draw.pMatrixUniform = gl.GetUniformLocation(shaderProgram, "uPMatrix");
            Draw.mvMatrixUniform = gl.GetUniformLocation(shaderProgram, "uMVMatrix");
            Draw.nMatrixUniform = gl.GetUniformLocation(shaderProgram, "uNMatrix");
            Draw.samplerUniform = gl.GetUniformLocation(shaderProgram, "uSampler");

            Draw.program = shaderProgram;
        }


        public static void HandleLoadedTexture(ImageElement image)
        {
            var gl = Draw.GL;

            //gl.PixelStorei(gl.UNPACK_FLIP_Y_WEBGL, gl.ONE);
            Script.Call("gl.pixelStorei", gl.UNPACK_FLIP_Y_WEBGL, true);
            gl.BindTexture(gl.TEXTURE_2D, Draw.texture);
            gl.TexImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, image);
            gl.TexParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
            gl.TexParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR_MIPMAP_NEAREST);
            gl.GenerateMipmap(gl.TEXTURE_2D);
            gl.BindTexture(gl.TEXTURE_2D, null);
        }

        public static void InitTexture()
        {
            var gl = Draw.GL;

            Draw.texture = gl.CreateTexture();

            var textureImageElement = new ImageElement();

            textureImageElement.OnLoad = (e) =>
            {
                Draw.HandleLoadedTexture(textureImageElement);
            };

            textureImageElement.Src = "crate.gif";
        }

        public static void MVPushMatrix()
        {
            var copy = Script.Call<object>("mat4.create");
            Script.Call("mat4.set", mvMatrix, copy);
            Draw.mvMatrixStack.Push(copy);
        }

        public static void MVPopMatrix()
        {
            if (mvMatrixStack.Length == 0)
            {
                throw new Exception("Invalid popMatrix!");
            }

            Draw.mvMatrix = mvMatrixStack.Pop().As<double[]>();
        }

        public static void SetMatrixUniforms()
        {
            var gl = Draw.GL;

            gl.UniformMatrix4fv(Draw.pMatrixUniform, false, pMatrix);
            gl.UniformMatrix4fv(Draw.mvMatrixUniform, false, mvMatrix);

            var normalMatrix = Script.Call<double[]>("mat3.create");

            Script.Call<object>("mat4.toInverseMat3", mvMatrix, normalMatrix);
            Script.Call<object>("mat3.transpose", normalMatrix);
            gl.UniformMatrix3fv(Draw.nMatrixUniform, false, normalMatrix);
        }

        public static double DegToRad(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static void HandleKeyDown(Event e)
        {
            Draw.currentlyPressedKeys[e.As<KeyboardEvent>().KeyCode] = true;
        }

        public static void HandleKeyUp(Event e)
        {
            Draw.currentlyPressedKeys[e.As<KeyboardEvent>().KeyCode] = false;
        }

        public static void HandleKeys()
        {
            if (currentlyPressedKeys[33])
            {
                // Page Up
                z -= 0.05;
            }

            if (currentlyPressedKeys[34])
            {
                // Page Down
                z += 0.05;
            }

            if (currentlyPressedKeys[37])
            {
                // Left cursor key
                ySpeed -= 1;
            }

            if (currentlyPressedKeys[39])
            {
                // Right cursor key
                ySpeed += 1;
            }

            if (currentlyPressedKeys[38])
            {
                // Up cursor key
                xSpeed -= 1;
            }

            if (currentlyPressedKeys[40])
            {
                // Down cursor key
                xSpeed += 1;
            }
        }

        public static void InitBuffers()
        {
            var gl = Draw.GL;

            Draw.cubeVertexPositionBuffer = gl.CreateBuffer();
            gl.BindBuffer(gl.ARRAY_BUFFER, cubeVertexPositionBuffer);

            var vertices = new double[] {
                // Front face
                -1.0, -1.0,  1.0,
                 1.0, -1.0,  1.0,
                 1.0,  1.0,  1.0,
                -1.0,  1.0,  1.0,

                // Back face
                -1.0, -1.0, -1.0,
                -1.0,  1.0, -1.0,
                 1.0,  1.0, -1.0,
                 1.0, -1.0, -1.0,

                // Top face
                -1.0,  1.0, -1.0,
                -1.0,  1.0,  1.0,
                 1.0,  1.0,  1.0,
                 1.0,  1.0, -1.0,

                // Bottom face
                -1.0, -1.0, -1.0,
                 1.0, -1.0, -1.0,
                 1.0, -1.0,  1.0,
                -1.0, -1.0,  1.0,

                // Right face
                 1.0, -1.0, -1.0,
                 1.0,  1.0, -1.0,
                 1.0,  1.0,  1.0,
                 1.0, -1.0,  1.0,

                // Left face
                -1.0, -1.0, -1.0,
                -1.0, -1.0,  1.0,
                -1.0,  1.0,  1.0,
                -1.0,  1.0, -1.0,
            };

            gl.BufferData(gl.ARRAY_BUFFER, new Float32Array(vertices), gl.STATIC_DRAW);
            Draw.cubeVertexNormalBuffer = gl.CreateBuffer();

            gl.BindBuffer(gl.ARRAY_BUFFER, cubeVertexNormalBuffer);

            var vertexNormals = new double[] {
                // Front face
                 0.0,  0.0,  1.0,
                 0.0,  0.0,  1.0,
                 0.0,  0.0,  1.0,
                 0.0,  0.0,  1.0,

                // Back face
                 0.0,  0.0, -1.0,
                 0.0,  0.0, -1.0,
                 0.0,  0.0, -1.0,
                 0.0,  0.0, -1.0,

                // Top face
                 0.0,  1.0,  0.0,
                 0.0,  1.0,  0.0,
                 0.0,  1.0,  0.0,
                 0.0,  1.0,  0.0,

                // Bottom face
                 0.0, -1.0,  0.0,
                 0.0, -1.0,  0.0,
                 0.0, -1.0,  0.0,
                 0.0, -1.0,  0.0,

                // Right face
                 1.0,  0.0,  0.0,
                 1.0,  0.0,  0.0,
                 1.0,  0.0,  0.0,
                 1.0,  0.0,  0.0,

                // Left face
                -1.0,  0.0,  0.0,
                -1.0,  0.0,  0.0,
                -1.0,  0.0,  0.0,
                -1.0,  0.0,  0.0
            };

            gl.BufferData(gl.ARRAY_BUFFER, new Float32Array(vertexNormals), gl.STATIC_DRAW);
            Draw.cubeVertexTextureCoordBuffer = gl.CreateBuffer();

            gl.BindBuffer(gl.ARRAY_BUFFER, cubeVertexTextureCoordBuffer);

            var textureCoords = new double[] {
                // Front face
                0.0, 0.0,
                1.0, 0.0,
                1.0, 1.0,
                0.0, 1.0,

                // Back face
                1.0, 0.0,
                1.0, 1.0,
                0.0, 1.0,
                0.0, 0.0,

                // Top face
                0.0, 1.0,
                0.0, 0.0,
                1.0, 0.0,
                1.0, 1.0,

                // Bottom face
                1.0, 1.0,
                0.0, 1.0,
                0.0, 0.0,
                1.0, 0.0,

                // Right face
                1.0, 0.0,
                1.0, 1.0,
                0.0, 1.0,
                0.0, 0.0,

                // Left face
                0.0, 0.0,
                1.0, 0.0,
                1.0, 1.0,
                0.0, 1.0
            };

            gl.BufferData(gl.ARRAY_BUFFER, new Float32Array(textureCoords), gl.STATIC_DRAW);

            Draw.cubeVertexIndexBuffer = gl.CreateBuffer();
            gl.BindBuffer(gl.ELEMENT_ARRAY_BUFFER, cubeVertexIndexBuffer);
            var cubeVertexIndices = new int[] {
                0, 1, 2,      0, 2, 3,    // Front face
                4, 5, 6,      4, 6, 7,    // Back face
                8, 9, 10,     8, 10, 11,  // Top face
                12, 13, 14,   12, 14, 15, // Bottom face
                16, 17, 18,   16, 18, 19, // Right face
                20, 21, 22,   20, 22, 23  // Left face
            };

            gl.BufferData(gl.ELEMENT_ARRAY_BUFFER, new Uint16Array(cubeVertexIndices), gl.STATIC_DRAW);
        }

        public static void DrawScene()
        {
            var gl = Draw.GL;

            gl.Viewport(0, 0, canvas.Width, canvas.Height);

            gl.Clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);

            Script.Call("mat4.perspective", 45, (double)canvas.Width / canvas.Height, 0.1, 100.0, pMatrix);
            Script.Call("mat4.identity", mvMatrix);

            Script.Call("mat4.translate", mvMatrix, new double[] { 0.0, 0.0, z });
            Script.Call("mat4.rotate", mvMatrix, Draw.DegToRad(xRot), new int[] { 1, 0, 0 });
            Script.Call("mat4.rotate", mvMatrix, Draw.DegToRad(yRot), new int[] { 0, 1, 0 });

            gl.BindBuffer(gl.ARRAY_BUFFER, Draw.cubeVertexPositionBuffer);
            gl.VertexAttribPointer(Draw.vertexPositionAttribute, 3, gl.FLOAT, false, 0, 0);

            gl.BindBuffer(gl.ARRAY_BUFFER, Draw.cubeVertexNormalBuffer);
            gl.VertexAttribPointer(Draw.vertexNormalAttribute, 3, gl.FLOAT, false, 0, 0);

            gl.BindBuffer(gl.ARRAY_BUFFER, Draw.cubeVertexTextureCoordBuffer);
            gl.VertexAttribPointer(Draw.textureCoordAttribute, 2, gl.FLOAT, false, 0, 0);

            gl.ActiveTexture(gl.TEXTURE0);
            gl.BindTexture(gl.TEXTURE_2D, Draw.texture);

            gl.Uniform1i(Draw.samplerUniform, 0);

            gl.BindBuffer(gl.ELEMENT_ARRAY_BUFFER, Draw.cubeVertexIndexBuffer);

            Draw.SetMatrixUniforms();

            gl.DrawElements(gl.TRIANGLES, 36, gl.UNSIGNED_SHORT, 0);
        }

        public static void Animate()
        {
            var timeNow = new Date().GetTime();

            if (Draw.lastTime != 0)
            {
                var elapsed = timeNow - lastTime;

                xRot += (xSpeed * elapsed) / 1000.0;
                yRot += (ySpeed * elapsed) / 1000.0;
            }

            Draw.lastTime = timeNow;
        }

        public static void Tick()
        {
            Script.Write("requestAnimFrame(Cube3D.Draw.tick);");
            Draw.HandleKeys();
            Draw.DrawScene();
            Draw.Animate();
        }

        [Ready]
        public static void WebGLStart()
        {
            Draw.canvas = Document.GetElementById("lesson07-canvas").As<CanvasElement>();
            
            Draw.InitGL(Draw.canvas);
            Draw.InitShaders();
            Draw.InitBuffers();
            Draw.InitTexture();

            var gl = Draw.GL;

            gl.ClearColor(1, 1, 1, 1);
            gl.Enable(gl.DEPTH_TEST);

            Document.OnKeyDown = Draw.HandleKeyDown;
            Document.OnKeyUp = Draw.HandleKeyUp;

            Draw.Tick();
        }
    }
}
