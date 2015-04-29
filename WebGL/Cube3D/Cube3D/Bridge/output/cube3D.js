Bridge.define('Cube3D.Draw', {
    statics: {
        canvas: null,
        gL: null,
        program: null,
        texture: null,
        vertexPositionAttribute: 0,
        vertexNormalAttribute: 0,
        textureCoordAttribute: 0,
        pMatrixUniform: null,
        mvMatrixUniform: null,
        nMatrixUniform: null,
        samplerUniform: null,
        cubeVertexPositionBuffer: null,
        cubeVertexNormalBuffer: null,
        cubeVertexTextureCoordBuffer: null,
        cubeVertexIndexBuffer: null,
        xRot: 0,
        xSpeed: 3,
        yRot: 0,
        lastTime: 0,
        config: {
            init: function () {
                this.mvMatrix = mat4.create();
                this.mvMatrixStack = [];
                this.pMatrix = mat4.create();
                this.ySpeed = -3;
                this.z = -5.0;
                this.currentlyPressedKeys = [];
                Bridge.ready(this.webGLStart);
            }
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
                catch (e) {
                }

                if (context !== null) {
                    break;
                }
            }

            return context;
        }        ,
        initGL: function (canvas) {
            var gl = Cube3D.Draw.create3DContext(canvas);

            if (gl === null) {
                window.alert("Could not initialise WebGL, sorry :-(");
            }

            Cube3D.Draw.gL = gl;
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
                window.alert(gl.getShaderInfoLog(shader));
                return null;
            }

            return shader;
        },
        initShaders: function () {
            var gl = Cube3D.Draw.gL;
            var fragmentShader = Cube3D.Draw.getShader(gl, "shader-fs");
            var vertexShader = Cube3D.Draw.getShader(gl, "shader-vs");

            var shaderProgram = gl.createProgram();

            if (Bridge.is(shaderProgram, Bridge.Int)) {
                window.alert("Could not initialise program");
            }

            gl.attachShader(shaderProgram, vertexShader);
            gl.attachShader(shaderProgram, fragmentShader);
            gl.linkProgram(shaderProgram);

            if (!gl.getProgramParameter(shaderProgram, gl.LINK_STATUS)) {
                window.alert("Could not initialise shaders");
            }

            gl.useProgram(shaderProgram);

            Cube3D.Draw.vertexPositionAttribute = gl.getAttribLocation(shaderProgram, "aVertexPosition");
            gl.enableVertexAttribArray(Cube3D.Draw.vertexPositionAttribute);

            Cube3D.Draw.vertexNormalAttribute = gl.getAttribLocation(shaderProgram, "aVertexNormal");
            gl.enableVertexAttribArray(Cube3D.Draw.vertexNormalAttribute);

            Cube3D.Draw.textureCoordAttribute = gl.getAttribLocation(shaderProgram, "aTextureCoord");
            gl.enableVertexAttribArray(Cube3D.Draw.textureCoordAttribute);

            Cube3D.Draw.pMatrixUniform = gl.getUniformLocation(shaderProgram, "uPMatrix");
            Cube3D.Draw.mvMatrixUniform = gl.getUniformLocation(shaderProgram, "uMVMatrix");
            Cube3D.Draw.nMatrixUniform = gl.getUniformLocation(shaderProgram, "uNMatrix");
            Cube3D.Draw.samplerUniform = gl.getUniformLocation(shaderProgram, "uSampler");

            Cube3D.Draw.program = shaderProgram;
        },
        handleLoadedTexture: function (image) {
            var gl = Cube3D.Draw.gL;

            //gl.PixelStorei(gl.UNPACK_FLIP_Y_WEBGL, gl.ONE);
            gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, true);
            gl.bindTexture(gl.TEXTURE_2D, Cube3D.Draw.texture);
            gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, image);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR_MIPMAP_NEAREST);
            gl.generateMipmap(gl.TEXTURE_2D);
            gl.bindTexture(gl.TEXTURE_2D, null);
        },
        initTexture: function () {
            var gl = Cube3D.Draw.gL;

            Cube3D.Draw.texture = gl.createTexture();

            var textureImageElement = new Image();

            textureImageElement.onload = function (e) {
                Cube3D.Draw.handleLoadedTexture(textureImageElement);
            };

            textureImageElement.src = "crate.gif";
        },
        mVPushMatrix: function () {
            var copy = mat4.create();
            mat4.set(Cube3D.Draw.mvMatrix, copy);
            Cube3D.Draw.mvMatrixStack.push(copy);
        },
        mVPopMatrix: function () {
            if (Cube3D.Draw.mvMatrixStack.length === 0) {
                throw new Bridge.Exception("Invalid popMatrix!");
            }

            Cube3D.Draw.mvMatrix = Cube3D.Draw.mvMatrixStack.pop();
        },
        setMatrixUniforms: function () {
            var gl = Cube3D.Draw.gL;

            gl.uniformMatrix4fv(Cube3D.Draw.pMatrixUniform, false, Cube3D.Draw.pMatrix);
            gl.uniformMatrix4fv(Cube3D.Draw.mvMatrixUniform, false, Cube3D.Draw.mvMatrix);

            var normalMatrix = mat3.create();

            mat4.toInverseMat3(Cube3D.Draw.mvMatrix, normalMatrix);
            mat3.transpose(normalMatrix);
            gl.uniformMatrix3fv(Cube3D.Draw.nMatrixUniform, false, normalMatrix);
        },
        degToRad: function (degrees) {
            return degrees * Math.PI / 180;
        },
        handleKeyDown: function (e) {
            Cube3D.Draw.currentlyPressedKeys[e.keyCode] = true;
        },
        handleKeyUp: function (e) {
            Cube3D.Draw.currentlyPressedKeys[e.keyCode] = false;
        },
        handleKeys: function () {
            if (Cube3D.Draw.currentlyPressedKeys[33]) {
                // Page Up
                Cube3D.Draw.z -= 0.05;
            }

            if (Cube3D.Draw.currentlyPressedKeys[34]) {
                // Page Down
                Cube3D.Draw.z += 0.05;
            }

            if (Cube3D.Draw.currentlyPressedKeys[37]) {
                // Left cursor key
                Cube3D.Draw.ySpeed -= 1;
            }

            if (Cube3D.Draw.currentlyPressedKeys[39]) {
                // Right cursor key
                Cube3D.Draw.ySpeed += 1;
            }

            if (Cube3D.Draw.currentlyPressedKeys[38]) {
                // Up cursor key
                Cube3D.Draw.xSpeed -= 1;
            }

            if (Cube3D.Draw.currentlyPressedKeys[40]) {
                // Down cursor key
                Cube3D.Draw.xSpeed += 1;
            }
        },
        initBuffers: function () {
            var gl = Cube3D.Draw.gL;

            Cube3D.Draw.cubeVertexPositionBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, Cube3D.Draw.cubeVertexPositionBuffer);

            var vertices = [-1.0, -1.0, 1.0, 1.0, -1.0, 1.0, 1.0, 1.0, 1.0, -1.0, 1.0, 1.0, -1.0, -1.0, -1.0, -1.0, 1.0, -1.0, 1.0, 1.0, -1.0, 1.0, -1.0, -1.0, -1.0, 1.0, -1.0, -1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, -1.0, -1.0, -1.0, -1.0, 1.0, -1.0, -1.0, 1.0, -1.0, 1.0, -1.0, -1.0, 1.0, 1.0, -1.0, -1.0, 1.0, 1.0, -1.0, 1.0, 1.0, 1.0, 1.0, -1.0, 1.0, -1.0, -1.0, -1.0, -1.0, -1.0, 1.0, -1.0, 1.0, 1.0, -1.0, 1.0, -1.0];

            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertices), gl.STATIC_DRAW);
            Cube3D.Draw.cubeVertexNormalBuffer = gl.createBuffer();

            gl.bindBuffer(gl.ARRAY_BUFFER, Cube3D.Draw.cubeVertexNormalBuffer);

            var vertexNormals = [0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0, -1.0, 0.0, 0.0];

            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertexNormals), gl.STATIC_DRAW);
            Cube3D.Draw.cubeVertexTextureCoordBuffer = gl.createBuffer();

            gl.bindBuffer(gl.ARRAY_BUFFER, Cube3D.Draw.cubeVertexTextureCoordBuffer);

            var textureCoords = [0.0, 0.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 1.0, 1.0, 1.0, 1.0, 0.0, 1.0, 0.0, 0.0, 1.0, 0.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 1.0, 1.0, 0.0, 1.0];

            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(textureCoords), gl.STATIC_DRAW);

            Cube3D.Draw.cubeVertexIndexBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, Cube3D.Draw.cubeVertexIndexBuffer);
            var cubeVertexIndices = [0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7, 8, 9, 10, 8, 10, 11, 12, 13, 14, 12, 14, 15, 16, 17, 18, 16, 18, 19, 20, 21, 22, 20, 22, 23];

            gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, new Uint16Array(cubeVertexIndices), gl.STATIC_DRAW);
        },
        drawScene: function () {
            var gl = Cube3D.Draw.gL;

            gl.viewport(0, 0, Cube3D.Draw.canvas.width, Cube3D.Draw.canvas.height);

            gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);

            mat4.perspective(45, Bridge.cast(Cube3D.Draw.canvas.width, Number) / Cube3D.Draw.canvas.height, 0.1, 100.0, Cube3D.Draw.pMatrix);
            mat4.identity(Cube3D.Draw.mvMatrix);

            mat4.translate(Cube3D.Draw.mvMatrix, [0.0, 0.0, Cube3D.Draw.z]);
            mat4.rotate(Cube3D.Draw.mvMatrix, Cube3D.Draw.degToRad(Cube3D.Draw.xRot), [1, 0, 0]);
            mat4.rotate(Cube3D.Draw.mvMatrix, Cube3D.Draw.degToRad(Cube3D.Draw.yRot), [0, 1, 0]);

            gl.bindBuffer(gl.ARRAY_BUFFER, Cube3D.Draw.cubeVertexPositionBuffer);
            gl.vertexAttribPointer(Cube3D.Draw.vertexPositionAttribute, 3, gl.FLOAT, false, 0, 0);

            gl.bindBuffer(gl.ARRAY_BUFFER, Cube3D.Draw.cubeVertexNormalBuffer);
            gl.vertexAttribPointer(Cube3D.Draw.vertexNormalAttribute, 3, gl.FLOAT, false, 0, 0);

            gl.bindBuffer(gl.ARRAY_BUFFER, Cube3D.Draw.cubeVertexTextureCoordBuffer);
            gl.vertexAttribPointer(Cube3D.Draw.textureCoordAttribute, 2, gl.FLOAT, false, 0, 0);

            gl.activeTexture(gl.TEXTURE0);
            gl.bindTexture(gl.TEXTURE_2D, Cube3D.Draw.texture);

            gl.uniform1i(Cube3D.Draw.samplerUniform, 0);

            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, Cube3D.Draw.cubeVertexIndexBuffer);

            Cube3D.Draw.setMatrixUniforms();

            gl.drawElements(gl.TRIANGLES, 36, gl.UNSIGNED_SHORT, 0);
        },
        animate: function () {
            var timeNow = new Date().getTime();

            if (Cube3D.Draw.lastTime !== 0) {
                var elapsed = timeNow - Cube3D.Draw.lastTime;

                Cube3D.Draw.xRot += (Cube3D.Draw.xSpeed * elapsed) / 1000.0;
                Cube3D.Draw.yRot += (Cube3D.Draw.ySpeed * elapsed) / 1000.0;
            }

            Cube3D.Draw.lastTime = timeNow;
        },
        tick: function () {
            requestAnimFrame(Cube3D.Draw.tick);
            Cube3D.Draw.handleKeys();
            Cube3D.Draw.drawScene();
            Cube3D.Draw.animate();
        },
        webGLStart: function () {
            Cube3D.Draw.canvas = document.getElementById("lesson07-canvas");

            Cube3D.Draw.initGL(Cube3D.Draw.canvas);
            Cube3D.Draw.initShaders();
            Cube3D.Draw.initBuffers();
            Cube3D.Draw.initTexture();

            var gl = Cube3D.Draw.gL;

            gl.clearColor(1, 1, 1, 1);
            gl.enable(gl.DEPTH_TEST);

            document.onkeydown = Cube3D.Draw.handleKeyDown;
            document.onkeyup = Cube3D.Draw.handleKeyUp;

            Cube3D.Draw.tick();
        }
    }
});