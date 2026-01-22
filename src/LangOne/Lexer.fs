module LangOne.Lexer

open LangOne.Types
open System
open Serilog

let private log = Log.ForContext("Module", "Lexer")

/// Check if character is a digit
let private isDigit (c: char) = c >= '0' && c <= '9'

/// Check if character is whitespace
let private isWhitespace (c: char) = c = ' ' || c = '\t' || c = '\n' || c = '\r'

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
    log.Debug("Tokenizing input: {Input}", input)
    let rec loop (pos: int) (acc: Token list) =
        if pos >= input.Length then
            let tokens = List.rev (EOF :: acc)
            log.Debug("Tokenization complete: {TokenCount} tokens", List.length tokens)
            Ok tokens
        else
            let c = input.[pos]
            if isWhitespace c then
                loop (pos + 1) acc  // Skip whitespace
            elif isDigit c then
                let (numStr, newPos) = readNumber input pos
                match Double.TryParse(numStr) with
                | (true, value) ->
                    log.Verbose("Token: Number {Value} at position {Position}", value, pos)
                    loop newPos (Number value :: acc)
                | (false, _) ->
                    log.Warning("Invalid number: {NumStr} at position {Position}", numStr, pos)
                    Error (InvalidNumber (numStr, pos))
            else
                match tokenizeOperator c pos with
                | Some (Ok token) ->
                    log.Verbose("Token: {Token} at position {Position}", token, pos)
                    loop (pos + 1) (token :: acc)
                | Some (Error e) -> Error e
                | None ->
                    log.Warning("Unexpected character: {Char} at position {Position}", c, pos)
                    Error (UnexpectedCharacter (c, pos))
    loop 0 []
