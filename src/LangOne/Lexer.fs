module LangOne.Lexer

open LangOne.Types

/// Tokenize a single character at given position
let private tokenizeChar (c: char) (pos: int) : Result<Token, LexError> =
    match c with
    | '+' -> Ok Plus
    | '-' -> Ok Minus
    | '*' -> Ok Star
    | '/' -> Ok Slash
    | '(' -> Ok LParen
    | ')' -> Ok RParen
    | _ -> Error (UnexpectedCharacter (c, pos))

/// Tokenize an input string into a list of tokens
let tokenize (input: string) : Result<Token list, LexError> =
    let rec loop (pos: int) (acc: Token list) =
        if pos >= input.Length then
            Ok (List.rev (EOF :: acc))
        else
            let c = input.[pos]
            match tokenizeChar c pos with
            | Ok token -> loop (pos + 1) (token :: acc)
            | Error e -> Error e
    loop 0 []
