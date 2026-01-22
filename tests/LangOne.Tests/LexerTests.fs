module LangOne.Tests.LexerTests

open Expecto
open LangOne.Types
open LangOne.Lexer

let singleTokenTests =
    testList "Single token lexing" [
        test "tokenize plus" {
            let result = tokenize "+"
            Expect.equal result (Ok [Plus; EOF]) "Should tokenize + as Plus"
        }

        test "tokenize minus" {
            let result = tokenize "-"
            Expect.equal result (Ok [Minus; EOF]) "Should tokenize - as Minus"
        }

        test "tokenize star" {
            let result = tokenize "*"
            Expect.equal result (Ok [Star; EOF]) "Should tokenize * as Star"
        }

        test "tokenize slash" {
            let result = tokenize "/"
            Expect.equal result (Ok [Slash; EOF]) "Should tokenize / as Slash"
        }

        test "tokenize left paren" {
            let result = tokenize "("
            Expect.equal result (Ok [LParen; EOF]) "Should tokenize ( as LParen"
        }

        test "tokenize right paren" {
            let result = tokenize ")"
            Expect.equal result (Ok [RParen; EOF]) "Should tokenize ) as RParen"
        }

        test "unknown character returns error" {
            let result = tokenize "@"
            match result with
            | Error (UnexpectedCharacter ('@', 0)) -> ()
            | _ -> failtest "Should return UnexpectedCharacter error for @"
        }
    ]

let allLexerTests =
    testList "Lexer tests" [
        singleTokenTests
    ]
