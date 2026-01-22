module LangOne.Parser

open LangOne.Types

/// Parse a list of tokens into an AST expression
let parse (tokens: Token list) : Result<Expr, ParseError> =
    match tokens with
    | [EOF] -> Error UnexpectedEndOfInput
    | [Number n; EOF] -> Ok (Literal n)
    | Number n :: _ -> Ok (Literal n)
    | token :: _ -> Error (UnexpectedToken (token, "number"))
    | [] -> Error UnexpectedEndOfInput
