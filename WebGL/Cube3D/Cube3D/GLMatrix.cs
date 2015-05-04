// That is a partial implementation of glMatrix: http://glmatrix.net
namespace Bridge.GLMatrix
{
    /// <summary>
    /// 3x3 matrix API
    /// </summary>
    [Ignore]
    [Name("mat3")]
    public static class Mat3
    {
        /// <summary>
        /// Returns a 9 element array. If WebGL is enabled, array type will be WebGLFloatArray, otherwise it will be a standard JavaScript Array. 
        /// </summary>
        /// <returns>A matrix</returns>
        public static double[] Create()
        {
            return null;
        }

        /// <summary>
        ///  Transposes the matrix
        /// </summary>
        /// <param name="matrix">The matrix</param>
        public static void Transpose(double[] matrix) { }
    }

    /// <summary>
    /// 4x4 matrix API
    /// </summary>
    [Ignore]
    [Name("mat4")]
    public static class Mat4
    {
        /// <summary>
        /// Returns a 16 element array. If WebGL is enabled, array type will be WebGLFloatArray, otherwise it will be a standard JavaScript Array. 
        /// </summary>
        /// <returns>A matrix</returns>
        public static double[] Create()
        {
            return null;
        }

        /// <summary>
        /// Set the matrix to an identity matrix. 
        /// </summary>
        /// <param name="matrix">The result matrix</param>
        public static void Identity(double[] matrix) { }

        /// <summary>
        /// Calculates a perspective matrix with the given parameters and writes it into dest. 
        /// </summary>
        /// <param name="fovy">The fovy value</param>
        /// <param name="aspect">The aspect value</param>
        /// <param name="near">The near value</param>
        /// <param name="far">The far value</param>
        /// <param name="dest">The result matrix</param>
        public static void Perspective(double fovy, double aspect, double near, double far, double[] dest) { }

        /// <summary>
        /// Rotates the matrix by angle (given in radians) around the axis given by the vector axis. 
        /// </summary>
        /// <param name="matrix">The matrix</param>
        /// <param name="angle">The angle value</param>
        /// <param name="axisVector">The axis vector</param>
        public static void Rotate(double[] matrix, double angle, double[] axisVector) { }

        /// <summary>
        /// Calculates the inverse of the top 3x3 elements of mat. (ie: calculates the normal matrix). dest is expected to be a sequence of at least 9 elements. If dest is not specified a new 9 element array is created and returned. 
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="destination">The destionation</param>
        public static void ToInverseMat3(double[] source, double[] destination) { }

        /// <summary>
        /// Translates the matrix by the vector. 
        /// </summary>
        /// <param name="matrix">The matrix</param>
        /// <param name="vector">The vestor to translate by</param>
        public static void Translate(double[] matrix, double[] vector) { }
    }

    /// <summary>
    /// 3-dimensional vector API
    /// </summary>
    [Ignore]
    [Name("vec3")]
    public static class Vec3
    {
        /// <summary>
        /// Creates a 3-dimensional vector
        /// </summary>
        /// <returns>A 3-dimensional vector</returns>
        public static double[][][] Create()
        {
            return null;
        }

        /// <summary>
        /// Normalizes a 3-dimensional vector
        /// </summary>
        /// <param name="normalizeVector">The normalize vector</param>
        /// <param name="resultVector">The result vector</param>
        public static void Normalize(double[] normalizeVector, double[][][] resultVector) { }

        /// <summary>
        /// Scales a 3-dimensional vector by a scalar number 
        /// </summary>
        /// <param name="vector">The 3-dimensional vector</param>
        /// <param name="scalar">The scalar value</param>
        public static void Scale(double[][][] vector, double scalar) { }
    }
}