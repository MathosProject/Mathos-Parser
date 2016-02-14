namespace Mathos.Parser
{
    /// <summary>
    /// Represents a token in the parser.
    /// </summary>
    public class MathToken
    {
        /// <summary>
        /// The types of tokens.
        /// </summary>
        public enum TokenType
        {
            /// <summary>
            /// An identifier (a, b, etc).
            /// </summary>
            Identifier,
            /// <summary>
            /// A function (sin, cos, etc).
            /// </summary>
            Function,
            /// <summary>
            /// An operator (+, -, etc).
            /// </summary>
            Operator,
            /// <summary>
            /// A number (1, pi, etc).
            /// </summary>
            Number
        }

        /// <summary>
        /// The token's value.
        /// </summary>
        public object Value;
        /// <summary>
        /// The token's type.
        /// </summary>
        public TokenType Type;

        /// <summary>
        /// Create a new token of <paramref name="type"/> equal to <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public MathToken(object value, TokenType type)
        {
            Value = value;
            Type = type;
        }

        /// <summary>
        /// Convert a string, <paramref name="op"/>, to a MathToken of type identifier.
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static MathToken ToIdentifier(string op)
        {
            return new MathToken(op, TokenType.Identifier);
        }

        /// <summary>
        /// Convert a string, <paramref name="op"/>, to a MathToken of type function.
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static MathToken ToFunction(string op)
        {
            return new MathToken(op, TokenType.Function);
        }
        
        /// <summary>
        /// Convert a string, <paramref name="op"/>, to a MathToken of type operator.
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static MathToken ToOperator(string op)
        {
            return new MathToken(op, TokenType.Operator);
        }

        /// <summary>
        /// Convert a string, <paramref name="op"/>, to a MathToken of type number.
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static MathToken ToNumber(string op)
        {
            return new MathToken(op, TokenType.Number);
        }
    }
}
