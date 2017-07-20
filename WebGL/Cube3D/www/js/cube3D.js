/**
 * Bridge WebGL 3D Cube Demo
 * @version 1.0.0.0
 * @author Object.NET, Inc.
 * @copyright Copyright © Object.NET, Inc 2017
 * @compiler Bridge.NET 16.0.0-beta5
 */
Bridge.assembly("Cube3D", function ($asm, globals) {
    "use strict";

    Bridge.define("Cube3D.App", {
        statics: {
            ctors: {
                init: function () {
                    Bridge.ready(this.main);
                }
            },
            methods: {
                main: function () {
                    Cube3D.App.initCube("canvas1");
                },
                initCube: function (canvasId) {
                    var cube = new Cube3D.Cube();

                    Cube3D.App.initSettings(cube);

                    cube.canvas = Cube3D.App.getCanvasEl(canvasId);
                    cube.gl = Cube3D.App.create3DContext(cube.canvas);

                    if (cube.gl != null) {
                        cube.initShaders();
                        cube.initBuffers();
                        cube.initTexture();
                        cube.tick();

                        document.addEventListener("keydown", Bridge.fn.cacheBind(cube, cube.handleKeyDown));
                        document.addEventListener("keyup", Bridge.fn.cacheBind(cube, cube.handleKeyUp));
                    } else {
                        Cube3D.App.showError(cube.canvas, "<b>Either the browser doesn't support WebGL or it is disabled.<br>Please follow <a href=\"http://get.webgl.com\">Get WebGL</a>.</b>");
                    }
                },
                getCanvasEl: function (id) {
                    return document.getElementById(id);
                },
                create3DContext: function (canvas) {
                    var $t;
                    var names = System.Array.init(["webgl", "experimental-webgl", "webkit-3d", "moz-webgl"], System.String);

                    var context = null;

                    $t = Bridge.getEnumerator(names);
                    try {
                        while ($t.moveNext()) {
                            var name = $t.Current;
                            try {
                                context = canvas.getContext(name);
                            }
                            catch ($e1) {
                                $e1 = System.Exception.create($e1);
                            }

                            if (context != null) {
                                break;
                            }
                        }
                    } finally {
                        if (Bridge.is($t, System.IDisposable)) {
                            $t.System$IDisposable$dispose();
                        }
                    }
                    return context;
                },
                showError: function (canvas, message) {
                    var $t;
                    canvas.parentElement.replaceChild(($t = document.createElement('p'), $t.innerHTML = message, $t), canvas);
                },
                initSettings: function (cube) {
                    var useSettings = document.getElementById("settings");

                    if (useSettings == null || !useSettings.checked) {
                        return;
                    }

                    cube.useBlending = document.getElementById("blending").checked;
                    cube.alpha = parseFloat(document.getElementById("alpha").value);

                    cube.useLighting = document.getElementById("lighting").checked;

                    cube.ambientR = parseFloat(document.getElementById("ambientR").value);
                    cube.ambientG = parseFloat(document.getElementById("ambientG").value);
                    cube.ambientB = parseFloat(document.getElementById("ambientB").value);

                    cube.lightDirectionX = parseFloat(document.getElementById("lightDirectionX").value);
                    cube.lightDirectionY = parseFloat(document.getElementById("lightDirectionY").value);
                    cube.lightDirectionZ = parseFloat(document.getElementById("lightDirectionZ").value);

                    cube.directionalR = parseFloat(document.getElementById("directionalR").value);
                    cube.directionalG = parseFloat(document.getElementById("directionalG").value);
                    cube.directionalB = parseFloat(document.getElementById("directionalB").value);

                    cube["textureImageSrc"] = "crate.gif";
                }
            }
        },
        $entryPoint: true
    });

    Bridge.define("Cube3D.Cube", {
        fields: {
            canvas: null,
            gl: null,
            program: null,
            texture: null,
            useBlending: false,
            alpha: 0,
            useLighting: false,
            ambientR: 0,
            ambientG: 0,
            ambientB: 0,
            lightDirectionX: 0,
            lightDirectionY: 0,
            lightDirectionZ: 0,
            directionalR: 0,
            directionalG: 0,
            directionalB: 0,
            "textureImageSrc": null,
            mvMatrix: null,
            mvMatrixStack: null,
            pMatrix: null,
            vertexPositionAttribute: 0,
            vertexNormalAttribute: 0,
            textureCoordAttribute: 0,
            pMatrixUniform: null,
            mvMatrixUniform: null,
            nMatrixUniform: null,
            samplerUniform: null,
            useLightingUniform: null,
            ambientColorUniform: null,
            lightingDirectionUniform: null,
            directionalColorUniform: null,
            alphaUniform: null,
            cubeVertexPositionBuffer: null,
            cubeVertexNormalBuffer: null,
            cubeVertexTextureCoordBuffer: null,
            "cubeVertexIndexBuffer": null,
            xRotation: 0,
            xSpeed: 0,
            yRotation: 0,
            ySpeed: 0,
            z: 0,
            currentlyPressedKeys: null,
            lastTime: 0
        },
        ctors: {
            init: function () {
                this.useBlending = true;
                this.alpha = 1;
                this.useLighting = true;
                this.ambientR = 0.4;
                this.ambientG = 0.4;
                this.ambientB = 0.4;
                this.lightDirectionX = 0;
                this.lightDirectionY = 0;
                this.lightDirectionZ = -1;
                this.directionalR = 0.25;
                this.directionalG = 0.25;
                this.directionalB = 0.25;
                this["textureImageSrc"] = "crate.gif";
                this.mvMatrix = mat4.create();
                this.mvMatrixStack = System.Array.init([], System.Array.type(System.Double));
                this.pMatrix = mat4.create();
                this.xRotation = 0;
                this.xSpeed = 15;
                this.yRotation = 0;
                this.ySpeed = -15;
                this.z = -5.0;
                this.currentlyPressedKeys = System.Array.init([], System.Boolean);
                this.lastTime = 0;
            }
        },
        methods: {
            getShader: function (gl, id) {
                var shaderScript = document.getElementById(id);

                if (shaderScript == null) {
                    return null;
                }

                var str = "";
                var k = shaderScript.firstChild;

                while (k != null) {
                    if (k.nodeType === 3) {
                        str = System.String.concat(str, k.textContent);
                    }

                    k = k.nextSibling;
                }

                var shader;

                if (Bridge.referenceEquals(shaderScript.type, "x-shader/x-fragment")) {
                    shader = gl.CreateShader(gl.FRAGMENT_SHADER);
                } else if (Bridge.referenceEquals(shaderScript.type, "x-shader/x-vertex")) {
                    shader = gl.CreateShader(gl.VERTEX_SHADER);
                } else {
                    return null;
                }

                gl.ShaderSource(shader, str);
                gl.CompileShader(shader);

                if (!gl.GetShaderParameter(shader, gl["COMPILE_STATUS"])) {
                    Bridge.global.alert(gl.GetShaderInfoLog(shader));
                    return null;
                }

                return shader;
            },
            initShaders: function () {
                var fragmentShader = this.getShader(this.gl, "shader-fs");
                var vertexShader = this.getShader(this.gl, "shader-vs");
                var shaderProgram = this.gl.CreateProgram();

                if (Bridge.is(shaderProgram, System.Int32)) {
                    Bridge.global.alert("Could not initialise program");
                }

                this.gl.AttachShader(shaderProgram, vertexShader);
                this.gl.AttachShader(shaderProgram, fragmentShader);
                this.gl.LinkProgram(shaderProgram);

                if (!this.gl.GetProgramParameter(shaderProgram, this.gl["LINK_STATUS"])) {
                    Bridge.global.alert("Could not initialise shaders");
                }

                this.gl.UseProgram(shaderProgram);

                this.vertexPositionAttribute = this.gl.GetAttribLocation(shaderProgram, "aVertexPosition");
                this.vertexNormalAttribute = this.gl.GetAttribLocation(shaderProgram, "aVertexNormal");
                this.textureCoordAttribute = this.gl.GetAttribLocation(shaderProgram, "aTextureCoord");

                this.gl.EnableVertexAttribArray(this.vertexPositionAttribute);
                this.gl.EnableVertexAttribArray(this.vertexNormalAttribute);
                this.gl.EnableVertexAttribArray(this.textureCoordAttribute);

                this.pMatrixUniform = this.gl.GetUniformLocation(shaderProgram, "uPMatrix");
                this.mvMatrixUniform = this.gl.GetUniformLocation(shaderProgram, "uMVMatrix");
                this.nMatrixUniform = this.gl.GetUniformLocation(shaderProgram, "uNMatrix");
                this.samplerUniform = this.gl.GetUniformLocation(shaderProgram, "uSampler");
                this.useLightingUniform = this.gl.GetUniformLocation(shaderProgram, "uUseLighting");
                this.ambientColorUniform = this.gl.GetUniformLocation(shaderProgram, "uAmbientColor");
                this.lightingDirectionUniform = this.gl.GetUniformLocation(shaderProgram, "uLightingDirection");
                this.directionalColorUniform = this.gl.GetUniformLocation(shaderProgram, "uDirectionalColor");
                this.alphaUniform = this.gl.GetUniformLocation(shaderProgram, "uAlpha");

                this.program = shaderProgram;
            },
            handleLoadedTexture: function (image) {
                this.gl.PixelStorei(this.gl["UNPACK_FLIP_Y_WEBGL"], true);
                this.gl.BindTexture(this.gl.TEXTURE_2D, this.texture);
                this.gl.TexImage2D(this.gl.TEXTURE_2D, 0, this.gl.RGBA, this.gl.RGBA, this.gl["UNSIGNED_BYTE"], image);
                this.gl.TexParameteri(this.gl.TEXTURE_2D, this.gl["TEXTURE_MAG_FILTER"], this.gl["LINEAR"]);
                this.gl.TexParameteri(this.gl.TEXTURE_2D, this.gl["TEXTURE_MIN_FILTER"], this.gl["LINEAR_MIPMAP_NEAREST"]);
                this.gl.GenerateMipmap(this.gl.TEXTURE_2D);
                this.gl.BindTexture(this.gl.TEXTURE_2D, null);
            },
            initTexture: function () {
                this.texture = this.gl.CreateTexture();

                var textureHTMLImageElement = new Image();

                textureHTMLImageElement.onload = Bridge.fn.bind(this, function (ev) {
                    this.handleLoadedTexture(textureHTMLImageElement);
                });

                textureHTMLImageElement.src = this["textureImageSrc"];
            },
            setMatrixUniforms: function () {
                this.gl.UniformMatrix4fv(this.pMatrixUniform, false, this.pMatrix);
                this.gl.UniformMatrix4fv(this.mvMatrixUniform, false, this.mvMatrix);

                var normalMatrix = mat3.create();

                mat4.toInverseMat3(this.mvMatrix, normalMatrix);
                mat3.transpose(normalMatrix);

                this.gl.UniformMatrix3fv(this.nMatrixUniform, false, normalMatrix);
            },
            degToRad: function (degrees) {
                return degrees * Math["PI"] / 180;
            },
            handleKeyDown: function (e) {
                this.currentlyPressedKeys[System.Array.index(e.keyCode, this.currentlyPressedKeys)] = true;
            },
            handleKeyUp: function (e) {
                this.currentlyPressedKeys[System.Array.index(e.keyCode, this.currentlyPressedKeys)] = false;
            },
            handleKeys: function () {
                if (this.currentlyPressedKeys[System.Array.index(81, this.currentlyPressedKeys)]) {
                    this.z -= 0.05;
                }

                if (this.currentlyPressedKeys[System.Array.index(69, this.currentlyPressedKeys)]) {
                    this.z += 0.05;
                }

                if (this.currentlyPressedKeys[System.Array.index(65, this.currentlyPressedKeys)]) {
                    this.ySpeed = (this.ySpeed - 1) | 0;
                }

                if (this.currentlyPressedKeys[System.Array.index(68, this.currentlyPressedKeys)]) {
                    this.ySpeed = (this.ySpeed + 1) | 0;
                }

                if (this.currentlyPressedKeys[System.Array.index(87, this.currentlyPressedKeys)]) {
                    this.xSpeed = (this.xSpeed - 1) | 0;
                }

                if (this.currentlyPressedKeys[System.Array.index(83, this.currentlyPressedKeys)]) {
                    this.xSpeed = (this.xSpeed + 1) | 0;
                }
            },
            initBuffers: function () {
                this.cubeVertexPositionBuffer = this.gl.CreateBuffer();
                this.gl.BindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexPositionBuffer);

                var vertices = System.Array.init([-1.0, -1.0, 1.0, 1.0, -1.0, 1.0, 1.0, 1.0, 1.0, -1.0, 1.0, 1.0, -1.0, -1.0, -1.0, -1.0, 1.0, -1.0, 1.0, 1.0, -1.0, 1.0, -1.0, -1.0, -1.0, 1.0, -1.0, -1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, -1.0, -1.0, -1.0, -1.0, 1.0, -1.0, -1.0, 1.0, -1.0, 1.0, -1.0, -1.0, 1.0, 1.0, -1.0, -1.0, 1.0, 1.0, -1.0, 1.0, 1.0, 1.0, 1.0, -1.0, 1.0, -1.0, -1.0, -1.0, -1.0, -1.0, 1.0, -1.0, 1.0, 1.0, -1.0, 1.0, -1.0], System.Double);

                this.gl.BufferData(this.gl.ARRAY_BUFFER, new Float32Array(vertices), this.gl["STATIC_DRAW"]);

                this.cubeVertexNormalBuffer = this.gl.CreateBuffer();
                this.gl.BindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexNormalBuffer);

                var vertexNormals = System.Array.init([0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0], System.Double);

                this.gl.BufferData(this.gl.ARRAY_BUFFER, new Float32Array(vertexNormals), this.gl["STATIC_DRAW"]);

                this.cubeVertexTextureCoordBuffer = this.gl.CreateBuffer();
                this.gl.BindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexTextureCoordBuffer);

                var textureCoords = System.Array.init([0.0, 0.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 1.0, 1.0, 1.0, 1.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0], System.Double);

                this.gl.BufferData(this.gl.ARRAY_BUFFER, new Float32Array(textureCoords), this.gl["STATIC_DRAW"]);

                this["cubeVertexIndexBuffer"] = this.gl.CreateBuffer();
                this.gl.BindBuffer(this.gl.ELEMENT_ARRAY_BUFFER, this["cubeVertexIndexBuffer"]);

                var cubeVertexIndices = System.Array.init([0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7, 8, 9, 10, 8, 10, 11, 12, 13, 14, 12, 14, 15, 16, 17, 18, 16, 18, 19, 20, 21, 22, 20, 22, 23], System.Int32);

                this.gl.BufferData(this.gl.ELEMENT_ARRAY_BUFFER, new Uint16Array(cubeVertexIndices), this.gl["STATIC_DRAW"]);
            },
            drawScene: function () {
                this.gl.Viewport(0, 0, this.canvas.width, this.canvas.height);
                this.gl.Clear(this.gl["COLOR_BUFFER_BIT"] | this.gl["DEPTH_BUFFER_BIT"]);

                mat4.perspective(45, this.canvas.width / this.canvas.height, 0.1, 100, this.pMatrix);
                mat4.identity(this.mvMatrix);
                mat4.translate(this.mvMatrix, System.Array.init([0.0, 0.0, this.z], System.Double));
                mat4.rotate(this.mvMatrix, this.degToRad(this.xRotation), System.Array.init([1, 0, 0], System.Double));
                mat4.rotate(this.mvMatrix, this.degToRad(this.yRotation), System.Array.init([0, 1, 0], System.Double));

                this.gl.BindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexPositionBuffer);
                this.gl.VertexAttribPointer(this.vertexPositionAttribute, 3, this.gl.FLOAT, false, 0, 0);

                this.gl.BindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexNormalBuffer);
                this.gl.VertexAttribPointer(this.vertexNormalAttribute, 3, this.gl.FLOAT, false, 0, 0);

                this.gl.BindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexTextureCoordBuffer);
                this.gl.VertexAttribPointer(this.textureCoordAttribute, 2, this.gl.FLOAT, false, 0, 0);

                this.gl.ActiveTexture(this.gl.TEXTURE0);
                this.gl.BindTexture(this.gl.TEXTURE_2D, this.texture);

                this.gl.Uniform1i(this.samplerUniform, 0);

                // Add Blending
                if (this.useBlending) {
                    this.gl.BlendFunc(this.gl.SRC_ALPHA, this.gl.ONE);
                    this.gl.Enable(this.gl.BLEND);
                    this.gl.Disable(this.gl.DEPTH_TEST);
                    this.gl.Uniform1f(this.alphaUniform, this.alpha);
                } else {
                    this.gl.Disable(this.gl.BLEND);
                    this.gl.Enable(this.gl.DEPTH_TEST);
                    this.gl.Uniform1f(this.alphaUniform, 1);
                }

                // Add Lighting
                this.gl.Uniform1i(this.useLightingUniform, this.useLighting);

                if (this.useLighting) {
                    this.gl.Uniform3f(this.ambientColorUniform, this.ambientR, this.ambientG, this.ambientB);

                    var lightingDirection = System.Array.init([this.lightDirectionX, this.lightDirectionY, this.lightDirectionZ], System.Double);
                    var adjustedLD = vec3.create();

                    vec3.normalize(lightingDirection, adjustedLD);
                    vec3.scale(adjustedLD, -1);

                    this.gl.Uniform3fv(this.lightingDirectionUniform, adjustedLD);
                    this.gl.Uniform3f(this.directionalColorUniform, this.directionalR, this.directionalG, this.directionalB);
                }

                this.gl.BindBuffer(this.gl.ELEMENT_ARRAY_BUFFER, this["cubeVertexIndexBuffer"]);

                this.setMatrixUniforms();

                this.gl.DrawElements(this.gl["TRIANGLES"], 36, this.gl["UNSIGNED_SHORT"], 0);
            },
            animate: function () {
                var timeNow = new Date().getTime();

                if (this.lastTime !== 0) {
                    var elapsed = timeNow - this.lastTime;

                    this.xRotation += (this.xSpeed * elapsed) / 1000.0;
                    this.yRotation += (this.ySpeed * elapsed) / 1000.0;
                }

                this.lastTime = timeNow;
            },
            tick: function () {
                Cube3D.App.initSettings(this);
                this.handleKeys();
                this.drawScene();
                this.animate();
                Bridge.global.setTimeout(Bridge.fn.cacheBind(this, this.tick), 20);
            }
        }
    });
});
