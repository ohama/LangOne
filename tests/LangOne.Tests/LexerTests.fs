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

let numberTokenTests =
    testList "Number token lexing" [
        test "tokenize single digit" {
            let result = tokenize "5"
            Expect.equal result (Ok [Number 5.0; EOF]) "Should tokenize 5 as Number 5.0"
        }

        test "tokenize multi-digit number" {
            let result = tokenize "123"
            Expect.equal result (Ok [Number 123.0; EOF]) "Should tokenize 123 as Number 123.0"
        }

        test "tokenize decimal number" {
            let result = tokenize "3.14"
            Expect.equal result (Ok [Number 3.14; EOF]) "Should tokenize 3.14 as Number 3.14"
        }

        test "tokenize number with leading zero" {
            let result = tokenize "0.5"
            Expect.equal result (Ok [Number 0.5; EOF]) "Should tokenize 0.5 as Number 0.5"
        }
    ]

let allLexerTests =
    testList "Lexer tests" [
        singleTokenTests
        numberTokenTests
    ]
