Bridge.define('Cube3D.App', {
    statics: {
        config: {
            init: function () {
                Bridge.ready(this.main);
            }
        },
        main: function () {
            var cube = new Cube3D.Cube();

            cube.canvas = Cube3D.App.getCanvasEl("canvas1");
            cube.gl = Cube3D.App.initGL(cube.canvas);
            cube.initShaders();
            cube.initBuffers();
            cube.initTexture();
            cube.tick();

            document.onkeydown = Bridge.fn.bind(cube, cube.handleKeyDown);
            document.onkeyup = Bridge.fn.bind(cube, cube.handleKeyUp);
        },
        getCanvasEl: function (id) {
            return document.getElementById(id);
        },
        initGL: function (canvas) {
            var gl = Cube3D.App.create3DContext(canvas);

            if (gl === null) {
                Bridge.global.alert("Could not initialise WebGL, sorry :-(");
            }

            return gl;
        },
        create3DContext: function (canvas) {
            var $t;
            var names = ["webgl", "experimental-webgl", "webkit-3d", "moz-webgl"];

            var context = null;

            $t = Bridge.getEnumerator(names);
            while ($t.moveNext()) {
                var name = $t.getCurrent();
                try {
                    context = canvas.getContext(name);
                }
                catch (ex) {
                }

                if (context !== null) {
                    break;
                }
            }

            return context;
        }
    }
});

Bridge.define('Cube3D.Cube', {
    canvas: null,
    gl: null,
    program: null,
    texture: null,
    vertexPositionAttribute: 0,
    vertexNormalAttribute: 0,
    textureCoordAttribute: 0,
    pMatrixUniform: null,
    mvMatrixUniform: null,
    nMatrixUniform: null,
    samplerUniform: null,
    alphaUniform: null,
    cubeVertexPositionBuffer: null,
    cubeVertexNormalBuffer: null,
    cubeVertexTextureCoordBuffer: null,
    cubeVertexIndexBuffer: null,
    xRotation: 0,
    xSpeed: 3,
    yRotation: 0,
    lastTime: 0,
    config: {
        init: function () {
            this.mvMatrix = mat4.create();
            this.mvMatrixStack = [];
            this.pMatrix = mat4.create();
            this.ySpeed = -3;
            this.z = -5.0;
            this.currentlyPressedKeys = [];
        }
    },
    getShader: function (gl, id) {
        var shaderScript = document.getElementById(id);

        if (shaderScript === null) {
            return null;
        }

        var str = "";
        var k = shaderScript.firstChild;

        while (k !== null) {
            if (k.nodeType === 3) {
                str += k.textContent;
            }

            k = k.nextSibling;
        }

        var shader;

        if (shaderScript.type === "x-shader/x-fragment") {
            shader = gl.createShader(gl.FRAGMENT_SHADER);
        }
        else 
            if (shaderScript.type === "x-shader/x-vertex") {
                shader = gl.createShader(gl.VERTEX_SHADER);
            }
            else  {
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

        if (Bridge.is(shaderProgram, Bridge.Int)) {
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
        this.textureCoordAttribute = this.gl.getAttribLocation(shaderProgram, "aTextureCoord");

        this.gl.enableVertexAttribArray(this.vertexPositionAttribute);
        this.gl.enableVertexAttribArray(this.textureCoordAttribute);

        this.pMatrixUniform = this.gl.getUniformLocation(shaderProgram, "uPMatrix");
        this.mvMatrixUniform = this.gl.getUniformLocation(shaderProgram, "uMVMatrix");
        this.nMatrixUniform = this.gl.getUniformLocation(shaderProgram, "uNMatrix");
        this.samplerUniform = this.gl.getUniformLocation(shaderProgram, "uSampler");
        this.alphaUniform = this.gl.getUniformLocation(shaderProgram, "uAlpha");

        this.program = shaderProgram;
    },
    handleLoadedTexture: function (image) {
        this.gl.pixelStorei(this.gl.UNPACK_FLIP_Y_WEBGL, this.gl.ONE);
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

        textureImageElement.src = "crate.gif";
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
        this.currentlyPressedKeys[e.keyCode] = true;
    },
    handleKeyUp: function (e) {
        this.currentlyPressedKeys[e.keyCode] = false;
    },
    handleKeys: function () {
        if (this.currentlyPressedKeys[33]) {
            this.z -= 0.05;
        }

        if (this.currentlyPressedKeys[34]) {
            this.z += 0.05;
        }

        if (this.currentlyPressedKeys[37]) {
            this.ySpeed -= 1;
        }

        if (this.currentlyPressedKeys[39]) {
            this.ySpeed += 1;
        }

        if (this.currentlyPressedKeys[38]) {
            this.xSpeed -= 1;
        }

        if (this.currentlyPressedKeys[40]) {
            this.xSpeed += 1;
        }
    },
    initBuffers: function () {
        this.cubeVertexPositionBuffer = this.gl.createBuffer();
        this.gl.bindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexPositionBuffer);

        var vertices = [-1.0, -1.0, 1.0, 1.0, -1.0, 1.0, 1.0, 1.0, 1.0, -1.0, 1.0, 1.0, -1.0, -1.0, -1.0, -1.0, 1.0, -1.0, 1.0, 1.0, -1.0, 1.0, -1.0, -1.0, -1.0, 1.0, -1.0, -1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, -1.0, -1.0, -1.0, -1.0, 1.0, -1.0, -1.0, 1.0, -1.0, 1.0, -1.0, -1.0, 1.0, 1.0, -1.0, -1.0, 1.0, 1.0, -1.0, 1.0, 1.0, 1.0, 1.0, -1.0, 1.0, -1.0, -1.0, -1.0, -1.0, -1.0, 1.0, -1.0, 1.0, 1.0, -1.0, 1.0, -1.0];

        this.gl.bufferData(this.gl.ARRAY_BUFFER, new Float32Array(vertices), this.gl.STATIC_DRAW);
        this.cubeVertexNormalBuffer = this.gl.createBuffer();

        this.gl.bindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexNormalBuffer);

        var vertexNormals = [0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0];

        this.gl.bufferData(this.gl.ARRAY_BUFFER, new Float32Array(vertexNormals), this.gl.STATIC_DRAW);
        this.cubeVertexTextureCoordBuffer = this.gl.createBuffer();

        this.gl.bindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexTextureCoordBuffer);

        var textureCoords = [0.0, 0.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 1.0, 1.0, 1.0, 1.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0];

        this.gl.bufferData(this.gl.ARRAY_BUFFER, new Float32Array(textureCoords), this.gl.STATIC_DRAW);

        this.cubeVertexIndexBuffer = this.gl.createBuffer();
        this.gl.bindBuffer(this.gl.ELEMENT_ARRAY_BUFFER, this.cubeVertexIndexBuffer);

        var cubeVertexIndices = [0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7, 8, 9, 10, 8, 10, 11, 12, 13, 14, 12, 14, 15, 16, 17, 18, 16, 18, 19, 20, 21, 22, 20, 22, 23];

        this.gl.bufferData(this.gl.ELEMENT_ARRAY_BUFFER, new Uint16Array(cubeVertexIndices), this.gl.STATIC_DRAW);
    },
    drawScene: function () {
        this.gl.viewport(0, 0, this.canvas.width, this.canvas.height);
        this.gl.clear(this.gl.COLOR_BUFFER_BIT | this.gl.DEPTH_BUFFER_BIT);

        mat4.perspective(45, Bridge.cast(this.canvas.width, Number) / this.canvas.height, 0.1, 100.0, this.pMatrix);
        mat4.identity(this.mvMatrix);
        mat4.translate(this.mvMatrix, [0.0, 0.0, this.z]);
        mat4.rotate(this.mvMatrix, this.degToRad(this.xRotation), [1, 0, 0]);
        mat4.rotate(this.mvMatrix, this.degToRad(this.yRotation), [0, 1, 0]);

        this.gl.bindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexPositionBuffer);
        this.gl.vertexAttribPointer(this.vertexPositionAttribute, 3, this.gl.FLOAT, false, 0, 0);

        this.gl.bindBuffer(this.gl.ARRAY_BUFFER, this.cubeVertexTextureCoordBuffer);
        this.gl.vertexAttribPointer(this.textureCoordAttribute, 2, this.gl.FLOAT, false, 0, 0);

        this.gl.activeTexture(this.gl.TEXTURE0);
        this.gl.bindTexture(this.gl.TEXTURE_2D, this.texture);

        this.gl.uniform1i(this.samplerUniform, 0);

        // Add Blending
        this.gl.blendFunc(this.gl.SRC_ALPHA, this.gl.ONE);
        this.gl.enable(this.gl.BLEND);
        this.gl.uniform1f(this.alphaUniform, 0.5);

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
        // Global.RequestAnimationFrame(Tick);
        requestAnimFrame(Bridge.fn.bind(this, this.tick));

        this.handleKeys();
        this.drawScene();
        this.animate();
    }
});