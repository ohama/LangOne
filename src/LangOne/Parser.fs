module LangOne.Parser

open LangOne.Types

/// Parser state: current position in token list
type private ParserState = { Tokens: Token list; Position: int }

/// Get current token
let private current state =
    if state.Position < List.length state.Tokens then
        List.item state.Position state.Tokens
    else
        EOF

/// Advance to next token
let private advance state =
    { state with Position = state.Position + 1 }

/// Check if at end of input
let private isAtEnd state =
    current state = EOF

/// Parse a primary expression (number literal)
let private parsePrimary state =
    match current state with
    | Number n -> Ok (Literal n, advance state)
    | EOF -> Error UnexpectedEndOfInput
    | token -> Error (UnexpectedToken (token, "number"))

/// Parse addition and subtraction (left-associative)
let private parseAddSub state =
    // Parse left operand
    parsePrimary state
    |> Result.bind (fun (left, state) ->
        // Loop to handle left-associativity
        let rec loop left state =
            match current state with
            | Plus ->
                let state = advance state
                parsePrimary state
                |> Result.bind (fun (right, state) ->
                    loop (Binary(left, Add, right)) state)
            | Minus ->
                let state = advance state
                parsePrimary state
                |> Result.bind (fun (right, state) ->
                    loop (Binary(left, Subtract, right)) state)
            | _ -> Ok (left, state)
        loop left state)

/// Parse expression (entry point)
let private parseExpr state =
    parseAddSub state

/// Parse a list of tokens into an AST expression
let parse (tokens: Token list) : Result<Expr, ParseError> =
    let state = { Tokens = tokens; Position = 0 }
    match parseExpr state with
    | Ok (expr, _) -> Ok expr
    | Error err -> Error err
