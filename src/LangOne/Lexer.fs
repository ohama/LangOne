module LangOne.Lexer

open LangOne.Types
open System

/// Check if character is a digit
let private isDigit (c: char) = c >= '0' && c <= '9'

/// Tokenize a single character operator at given position
let private tokenizeOperator (c: char) (pos: int) : Result<Token, LexError> option =
    match c with
    | '+' -> Some (Ok Plus)
    | '-' -> Some (Ok Minus)
    | '*' -> Some (Ok Star)
    | '/' -> Some (Ok Slash)
    | '(' -> Some (Ok LParen)
    | ')' -> Some (Ok RParen)
    | _ -> None

/// Read a number starting at position, returns (number string, new position)
let private readNumber (input: string) (startPos: int) : string * int =
    let rec readDigits pos hasDecimal =
        if pos >= input.Length then
            (pos, hasDecimal)
        else
            let c = input.[pos]
            if isDigit c then
                readDigits (pos + 1) hasDecimal
            elif c = '.' && not hasDecimal && pos + 1 < input.Length && isDigit input.[pos + 1] then
                readDigits (pos + 1) true
            else
                (pos, hasDecimal)
    let (endPos, _) = readDigits startPos false
    (input.Substring(startPos, endPos - startPos), endPos)

/// Tokenize an input string into a list of tokens
let tokenize (input: string) : Result<Token list, LexError> =
    let rec loop (pos: int) (acc: Token list) =
        if pos >= input.Length then
            Ok (List.rev (EOF :: acc))
        else
            let c = input.[pos]
            if isDigit c then
                let (numStr, newPos) = readNumber input pos
                match Double.TryParse(numStr) with
                | (true, value) -> loop newPos (Number value :: acc)
                | (false, _) -> Error (InvalidNumber (numStr, pos))
            else
                match tokenizeOperator c pos with
                | Some (Ok token) -> loop (pos + 1) (token :: acc)
                | Some (Error e) -> Error e
                | None -> Error (UnexpectedCharacter (c, pos))
    loop 0 []
