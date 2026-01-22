module LangOne.Types

/// Token types for the lexer
type Token =
    | Number of float
    | Plus
    | Minus
    | Star
    | Slash
    | LParen
    | RParen
    | EOF

/// Lexer error types
type LexError =
    | UnexpectedCharacter of char * position: int
    | InvalidNumber of string * position: int

/// AST expression types
type Expr =
    | Literal of float
    | Binary of left: Expr * operator: BinaryOp * right: Expr

and BinaryOp =
    | Add
    | Subtract
    | Multiply
    | Divide

/// Parser error types
type ParseError =
    | UnexpectedToken of Token * expected: string
    | UnexpectedEndOfInput
    | MismatchedParenthesis

/// Evaluator error types
type EvalError =
    | DivisionByZero

/// Unified interpreter error type
type InterpreterError =
    | LexerError of LexError
    | ParserError of ParseError
    | EvaluatorError of EvalError
