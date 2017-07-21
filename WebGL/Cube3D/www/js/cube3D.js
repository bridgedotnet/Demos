/**
 * Bridge WebGL 3D Cube Demo
 * @version 1.0.0.0
 * @author Object.NET, Inc.
 * @copyright Copyright 2008-2015 Object.NET, Inc.
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

                    cube.textureImageSrc = "crate.gif";
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
            textureImageSrc: null,
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
            cubeVertexIndexBuffer: null,
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
                this.textureImageSrc = "crate.gif";
                this.mvMatrix = mat4.create();
                this.mvMatrixStack = System.Array.init([], System.Array.type(System.Double));
                this.pMatrix = mat4.create();
                this.xRotation = 0;
                this.xSpeed = 15;
                this.yRotation = 0;
                this.ySpeed = -15;
                this.z = -5.0;
                this.currentlyPressedKeys = new (System.Collections.Generic.Dictionary$2(System.Int32,System.Boolean))();
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
                    shader = gl.createShader(gl.FRAGMENT_SHADER);
                } else if (Bridge.referenceEquals(shaderScript.type, "x-shader/x-vertex")) {
                    shader = gl.createShader(gl.VERTEX_SHADER);
                } else {
                    return null;
                }

                gl.shaderSource(shader, str);
                gl.compileShader(shader);

                if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
                    Bridge.global.alert(gl.getShaderInfoLog(shader));
                    return null;
                }

                return shader;
            },
            initShaders: function () {
                var fragmentShader = this.getShader(this.gl, "shader-fs");
                var vertexShader = this.getShader(this.gl, "shader-vs");
                var shaderProgram = this.gl.createProgram();

                if (Bridge.is(shaderProgram, System.Int32)) {
                    Bridge.global.alert("Could not initialise program");
                }

                this.gl.attachShader(shaderProgram, vertexShader);
                this.gl.attachShader(shaderProgram, fragmentShader);
                this.gl.linkProgram(shaderProgram);

                if (!this.gl.getProgramParameter(shaderProgram, this.gl.LINK_STATUS)) {
                    Bridge.global.alert("Could not initialise shaders");
                }

                this.gl.useProgram(shaderProgram);

                this.vertexPositionAttribute = this.gl.getAttribLocation(shaderProgram, "aVertexPosition");
                this.vertexNormalAttribute = this.gl.getAttribLocation(shaderProgram, "aVertexNormal");
                this.textureCoordAttribute = this.gl.getAttribLocation(shaderProgram, "aTextureCoord");

                this.gl.enableVertexAttribArray(this.vertexPositionAttribute);
                this.gl.enableVertexAttribArray(this.vertexNormalAttribute);
                this.gl.enableVertexAttribArray(this.textureCoordAttribute);

                this.pMatrixUniform = this.gl.getUniformLocation(shaderProgram, "uPMatrix");
                this.mvMatrixUniform = this.gl.getUniformLocation(shaderProgram, "uMVMatrix");
                this.nMatrixUniform = this.gl.getUniformLocation(shaderProgram, "uNMatrix");
                this.samplerUniform = this.gl.getUniformLocation(shaderProgram, "uSampler");
                this.useLightingUniform = this.gl.getUniformLocation(shaderProgram, "uUseLighting");
                this.ambientColorUniform = this.gl.getUniformLocation(shaderProgram, "uAmbientColor");
                this.lightingDirectionUniform = this.gl.getUniformLocation(shaderProgram, "uLightingDirection");
                this.directionalColorUniform = this.gl.getUniformLocation(shaderProgram, "uDirectionalColor");
                this.alphaUniform = this.gl.getUniformLocation(shaderProgram, "uAlpha");

                this.program = shaderProgram;
            },
            handleLoadedTexture: function (image) {
                this.gl.pixelStorei(this.gl.UNPACK_FLIP_Y_WEBGL, true);
                this.gl.bindTexture(this.gl.TEXTURE_2D, this.texture);
                this.gl.texImage2D(this.gl.TEXTURE_2D, 0, this.gl.RGBA, this.gl.RGBA, this.gl.UNSIGNED_BYTE, image);
                this.gl.texParameteri(this.gl.TEXTURE_2D, this.gl.TEXTURE_MAG_FILTER, this.gl.LINEAR);
                this.gl.texParameteri(this.gl.TEXTURE_2D, this.gl.TEXTURE_MIN_FILTER, this.gl.LINEAR_MIPMAP_NEAREST);
                this.gl.generateMipmap(this.gl.TEXTURE_2D);
                this.gl.bindTexture(this.gl.TEXTURE_2D, null);
            },
            initTexture: function () {
                this.texture = this.gl.createTexture();

                var textureImageElement = new Image();

                textureImageElement.onload = Bridge.fn.bind(this, function (ev) {
                    this.handleLoadedTexture(textureImageElement);
                });

                textureImageElement.src = this.textureImageSrc;
            },
            setMatrixUniforms: function () {
                this.gl.uniformMatrix4fv(this.pMatrixUniform, false, this.pMatrix);
                this.gl.uniformMatrix4fv(this.mvMatrixUniform, false, this.mvMatrix);

                var normalMatrix = mat3.create();

                mat4.toInverseMat3(this.mvMatrix, normalMatrix);
                mat3.transpose(normalMatrix);

                this.gl.uniformMatrix3fv(this.nMatrixUniform, false, normalMatrix);
            },
            degToRad: function (degrees) {
                return degrees * Math.PI / 180;
            },
            handleKeyDown: function (e) {
                this.currentlyPressedKeys.set(e.keyCode, true);
            },
            handleKeyUp: function (e) {
                this.currentlyPressedKeys.set(e.keyCode, false);
            },
            handleKeys: function () {
                if (this.checkPressedKey(81)) {
                    this.z -= 0.05;
                }

                if (this.checkPressedKey(69)) {
                    this.z += 0.05;
                }

                if (this.checkPressedKey(65)) {
                    this.ySpeed = (this.ySpeed - 1) | 0;
                }

                if (this.checkPressedKey(68)) {
                    this.ySpeed = (this.ySpeed + 1) | 0;
                }

                if (this.checkPressedKey(87)) {
                    this.xSpeed = (this.xSpeed - 1) | 0;
                }

                if (this.checkPressedKey(83)) {
                    this.xSpeed = (this.xSpeed + 1) | 0;
                }
            },
            checkPressedKey: function (key) {
                var b = { v : false };

                this.currentlyPressedKeys.tryGetValue(key, b);

                return b.v;
            },
            initBuffers: function () {
                this.cubeVertexPositionBuffer = this.gl.createBuffer();
                this.gl.bindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexPositionBuffer);

                var vertices = System.Array.init([-1.0, -1.0, 1.0, 1.0, -1.0, 1.0, 1.0, 1.0, 1.0, -1.0, 1.0, 1.0, -1.0, -1.0, -1.0, -1.0, 1.0, -1.0, 1.0, 1.0, -1.0, 1.0, -1.0, -1.0, -1.0, 1.0, -1.0, -1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, -1.0, -1.0, -1.0, -1.0, 1.0, -1.0, -1.0, 1.0, -1.0, 1.0, -1.0, -1.0, 1.0, 1.0, -1.0, -1.0, 1.0, 1.0, -1.0, 1.0, 1.0, 1.0, 1.0, -1.0, 1.0, -1.0, -1.0, -1.0, -1.0, -1.0, 1.0, -1.0, 1.0, 1.0, -1.0, 1.0, -1.0], System.Double);

                this.gl.bufferData(this.gl.ARRAY_BUFFER, new Float32Array(vertices), this.gl.STATIC_DRAW);

                this.cubeVertexNormalBuffer = this.gl.createBuffer();
                this.gl.bindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexNormalBuffer);

                var vertexNormals = System.Array.init([0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0], System.Double);

                this.gl.bufferData(this.gl.ARRAY_BUFFER, new Float32Array(vertexNormals), this.gl.STATIC_DRAW);

                this.cubeVertexTextureCoordBuffer = this.gl.createBuffer();
                this.gl.bindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexTextureCoordBuffer);

                var textureCoords = System.Array.init([0.0, 0.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 1.0, 1.0, 1.0, 1.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0], System.Double);

                this.gl.bufferData(this.gl.ARRAY_BUFFER, new Float32Array(textureCoords), this.gl.STATIC_DRAW);

                this.cubeVertexIndexBuffer = this.gl.createBuffer();
                this.gl.bindBuffer(this.gl.ELEMENT_ARRAY_BUFFER, this.cubeVertexIndexBuffer);

                var cubeVertexIndices = System.Array.init([0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7, 8, 9, 10, 8, 10, 11, 12, 13, 14, 12, 14, 15, 16, 17, 18, 16, 18, 19, 20, 21, 22, 20, 22, 23], System.Int32);

                this.gl.bufferData(this.gl.ELEMENT_ARRAY_BUFFER, new Uint16Array(cubeVertexIndices), this.gl.STATIC_DRAW);
            },
            drawScene: function () {
                this.gl.viewport(0, 0, this.canvas.width, this.canvas.height);
                this.gl.clear(this.gl.COLOR_BUFFER_BIT | this.gl.DEPTH_BUFFER_BIT);

                mat4.perspective(45, this.canvas.width / this.canvas.height, 0.1, 100, this.pMatrix);
                mat4.identity(this.mvMatrix);
                mat4.translate(this.mvMatrix, System.Array.init([0.0, 0.0, this.z], System.Double));
                mat4.rotate(this.mvMatrix, this.degToRad(this.xRotation), System.Array.init([1, 0, 0], System.Double));
                mat4.rotate(this.mvMatrix, this.degToRad(this.yRotation), System.Array.init([0, 1, 0], System.Double));

                this.gl.bindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexPositionBuffer);
                this.gl.vertexAttribPointer(this.vertexPositionAttribute, 3, this.gl.FLOAT, false, 0, 0);

                this.gl.bindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexNormalBuffer);
                this.gl.vertexAttribPointer(this.vertexNormalAttribute, 3, this.gl.FLOAT, false, 0, 0);

                this.gl.bindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexTextureCoordBuffer);
                this.gl.vertexAttribPointer(this.textureCoordAttribute, 2, this.gl.FLOAT, false, 0, 0);

                this.gl.activeTexture(this.gl.TEXTURE0);
                this.gl.bindTexture(this.gl.TEXTURE_2D, this.texture);

                this.gl.uniform1i(this.samplerUniform, 0);

                // Add Blending
                if (this.useBlending) {
                    this.gl.blendFunc(this.gl.SRC_ALPHA, this.gl.ONE);
                    this.gl.enable(this.gl.BLEND);
                    this.gl.disable(this.gl.DEPTH_TEST);
                    this.gl.uniform1f(this.alphaUniform, this.alpha);
                } else {
                    this.gl.disable(this.gl.BLEND);
                    this.gl.enable(this.gl.DEPTH_TEST);
                    this.gl.uniform1f(this.alphaUniform, 1);
                }

                // Add Lighting
                this.gl.uniform1i(this.useLightingUniform, this.useLighting);

                if (this.useLighting) {
                    this.gl.uniform3f(this.ambientColorUniform, this.ambientR, this.ambientG, this.ambientB);

                    var lightingDirection = System.Array.init([this.lightDirectionX, this.lightDirectionY, this.lightDirectionZ], System.Double);
                    var adjustedLD = vec3.create();

                    vec3.normalize(lightingDirection, adjustedLD);
                    vec3.scale(adjustedLD, -1);

                    this.gl.uniform3fv(this.lightingDirectionUniform, adjustedLD);
                    this.gl.uniform3f(this.directionalColorUniform, this.directionalR, this.directionalG, this.directionalB);
                }

                this.gl.bindBuffer(this.gl.ELEMENT_ARRAY_BUFFER, this.cubeVertexIndexBuffer);

                this.setMatrixUniforms();

                this.gl.drawElements(this.gl.TRIANGLES, 36, this.gl.UNSIGNED_SHORT, 0);
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

    var $m = Bridge.setMetadata,
        $n = [System,Bridge.WebGL,System.Collections.Generic,Cube3D];
    $m($n[3].Cube, function () { return {"att":1048577,"a":2,"m":[{"a":2,"isSynthetic":true,"n":".ctor","t":1,"sn":"ctor"},{"a":2,"n":"Animate","t":8,"sn":"animate","rt":$n[0].Void},{"a":1,"n":"CheckPressedKey","t":8,"pi":[{"n":"key","pt":$n[0].Int32,"ps":0}],"sn":"checkPressedKey","rt":$n[0].Boolean,"p":[$n[0].Int32]},{"a":2,"n":"DegToRad","t":8,"pi":[{"n":"degrees","pt":$n[0].Double,"ps":0}],"sn":"degToRad","rt":$n[0].Double,"p":[$n[0].Double]},{"a":2,"n":"DrawScene","t":8,"sn":"drawScene","rt":$n[0].Void},{"a":2,"n":"GetShader","t":8,"pi":[{"n":"gl","pt":$n[1].WebGLRenderingContext,"ps":0},{"n":"id","pt":$n[0].String,"ps":1}],"sn":"getShader","rt":$n[1].WebGLShader,"p":[$n[1].WebGLRenderingContext,$n[0].String]},{"a":2,"n":"HandleKeyDown","t":8,"pi":[{"n":"e","pt":Event,"ps":0}],"sn":"handleKeyDown","rt":$n[0].Void,"p":[Event]},{"a":2,"n":"HandleKeyUp","t":8,"pi":[{"n":"e","pt":Event,"ps":0}],"sn":"handleKeyUp","rt":$n[0].Void,"p":[Event]},{"a":2,"n":"HandleKeys","t":8,"sn":"handleKeys","rt":$n[0].Void},{"a":2,"n":"HandleLoadedTexture","t":8,"pi":[{"n":"image","pt":HTMLImageElement,"ps":0}],"sn":"handleLoadedTexture","rt":$n[0].Void,"p":[HTMLImageElement]},{"a":2,"n":"InitBuffers","t":8,"sn":"initBuffers","rt":$n[0].Void},{"a":2,"n":"InitShaders","t":8,"sn":"initShaders","rt":$n[0].Void},{"a":2,"n":"InitTexture","t":8,"sn":"initTexture","rt":$n[0].Void},{"a":2,"n":"SetMatrixUniforms","t":8,"sn":"setMatrixUniforms","rt":$n[0].Void},{"a":2,"n":"Tick","t":8,"sn":"tick","rt":$n[0].Void},{"a":2,"n":"alpha","t":4,"rt":$n[0].Double,"sn":"alpha"},{"a":2,"n":"alphaUniform","t":4,"rt":$n[1].WebGLUniformLocation,"sn":"alphaUniform"},{"a":2,"n":"ambientB","t":4,"rt":$n[0].Double,"sn":"ambientB"},{"a":2,"n":"ambientColorUniform","t":4,"rt":$n[1].WebGLUniformLocation,"sn":"ambientColorUniform"},{"a":2,"n":"ambientG","t":4,"rt":$n[0].Double,"sn":"ambientG"},{"a":2,"n":"ambientR","t":4,"rt":$n[0].Double,"sn":"ambientR"},{"a":2,"n":"canvas","t":4,"rt":HTMLCanvasElement,"sn":"canvas"},{"a":2,"n":"cubeVertexIndexBuffer","t":4,"rt":$n[1].WebGLBuffer,"sn":"cubeVertexIndexBuffer"},{"a":2,"n":"cubeVertexNormalBuffer","t":4,"rt":$n[1].WebGLBuffer,"sn":"cubeVertexNormalBuffer"},{"a":2,"n":"cubeVertexPositionBuffer","t":4,"rt":$n[1].WebGLBuffer,"sn":"cubeVertexPositionBuffer"},{"a":2,"n":"cubeVertexTextureCoordBuffer","t":4,"rt":$n[1].WebGLBuffer,"sn":"cubeVertexTextureCoordBuffer"},{"a":2,"n":"currentlyPressedKeys","t":4,"rt":$n[2].Dictionary$2(System.Int32,System.Boolean),"sn":"currentlyPressedKeys"},{"a":2,"n":"directionalB","t":4,"rt":$n[0].Double,"sn":"directionalB"},{"a":2,"n":"directionalColorUniform","t":4,"rt":$n[1].WebGLUniformLocation,"sn":"directionalColorUniform"},{"a":2,"n":"directionalG","t":4,"rt":$n[0].Double,"sn":"directionalG"},{"a":2,"n":"directionalR","t":4,"rt":$n[0].Double,"sn":"directionalR"},{"a":2,"n":"gl","t":4,"rt":$n[1].WebGLRenderingContext,"sn":"gl"},{"a":2,"n":"lastTime","t":4,"rt":$n[0].Double,"sn":"lastTime"},{"a":2,"n":"lightDirectionX","t":4,"rt":$n[0].Double,"sn":"lightDirectionX"},{"a":2,"n":"lightDirectionY","t":4,"rt":$n[0].Double,"sn":"lightDirectionY"},{"a":2,"n":"lightDirectionZ","t":4,"rt":$n[0].Double,"sn":"lightDirectionZ"},{"a":2,"n":"lightingDirectionUniform","t":4,"rt":$n[1].WebGLUniformLocation,"sn":"lightingDirectionUniform"},{"a":2,"n":"mvMatrix","t":4,"rt":$n[0].Array.type(System.Double),"sn":"mvMatrix"},{"a":2,"n":"mvMatrixStack","t":4,"rt":$n[0].Array.type(System.Array.type(System.Double)),"sn":"mvMatrixStack"},{"a":2,"n":"mvMatrixUniform","t":4,"rt":$n[1].WebGLUniformLocation,"sn":"mvMatrixUniform"},{"a":2,"n":"nMatrixUniform","t":4,"rt":$n[1].WebGLUniformLocation,"sn":"nMatrixUniform"},{"a":2,"n":"pMatrix","t":4,"rt":$n[0].Array.type(System.Double),"sn":"pMatrix"},{"a":2,"n":"pMatrixUniform","t":4,"rt":$n[1].WebGLUniformLocation,"sn":"pMatrixUniform"},{"a":2,"n":"program","t":4,"rt":$n[1].WebGLProgram,"sn":"program"},{"a":2,"n":"samplerUniform","t":4,"rt":$n[1].WebGLUniformLocation,"sn":"samplerUniform"},{"a":2,"n":"texture","t":4,"rt":$n[1].WebGLTexture,"sn":"texture"},{"a":2,"n":"textureCoordAttribute","t":4,"rt":$n[0].Int32,"sn":"textureCoordAttribute"},{"a":2,"n":"textureImageSrc","t":4,"rt":$n[0].String,"sn":"textureImageSrc"},{"a":2,"n":"useBlending","t":4,"rt":$n[0].Boolean,"sn":"useBlending"},{"a":2,"n":"useLighting","t":4,"rt":$n[0].Boolean,"sn":"useLighting"},{"a":2,"n":"useLightingUniform","t":4,"rt":$n[1].WebGLUniformLocation,"sn":"useLightingUniform"},{"a":2,"n":"vertexNormalAttribute","t":4,"rt":$n[0].Int32,"sn":"vertexNormalAttribute"},{"a":2,"n":"vertexPositionAttribute","t":4,"rt":$n[0].Int32,"sn":"vertexPositionAttribute"},{"a":2,"n":"xRotation","t":4,"rt":$n[0].Double,"sn":"xRotation"},{"a":2,"n":"xSpeed","t":4,"rt":$n[0].Int32,"sn":"xSpeed"},{"a":2,"n":"yRotation","t":4,"rt":$n[0].Double,"sn":"yRotation"},{"a":2,"n":"ySpeed","t":4,"rt":$n[0].Int32,"sn":"ySpeed"},{"a":2,"n":"z","t":4,"rt":$n[0].Double,"sn":"z"}]}; });
    $m($n[3].App, function () { return {"att":1048577,"a":2,"m":[{"a":2,"isSynthetic":true,"n":".ctor","t":1,"sn":"ctor"},{"a":2,"n":"Create3DContext","is":true,"t":8,"pi":[{"n":"canvas","pt":HTMLCanvasElement,"ps":0}],"sn":"create3DContext","rt":$n[1].WebGLRenderingContext,"p":[HTMLCanvasElement]},{"a":2,"n":"GetCanvasEl","is":true,"t":8,"pi":[{"n":"id","pt":$n[0].String,"ps":0}],"sn":"getCanvasEl","rt":HTMLCanvasElement,"p":[$n[0].String]},{"a":2,"n":"InitCube","is":true,"t":8,"pi":[{"n":"canvasId","pt":$n[0].String,"ps":0}],"sn":"initCube","rt":$n[0].Void,"p":[$n[0].String]},{"a":2,"n":"InitSettings","is":true,"t":8,"pi":[{"n":"cube","pt":$n[3].Cube,"ps":0}],"sn":"initSettings","rt":$n[0].Void,"p":[$n[3].Cube]},{"a":2,"n":"Main","is":true,"t":8,"sn":"main","rt":$n[0].Void},{"a":2,"n":"ShowError","is":true,"t":8,"pi":[{"n":"canvas","pt":HTMLCanvasElement,"ps":0},{"n":"message","pt":$n[0].String,"ps":1}],"sn":"showError","rt":$n[0].Void,"p":[HTMLCanvasElement,$n[0].String]}]}; });
});
