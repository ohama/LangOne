module LangOne.Tests.ParserTests

open Expecto
open LangOne.Types
open LangOne.Parser

let literalTests =
    testList "Parser literal tests" [
        test "parse single number" {
            let tokens = [Number 42.0; EOF]
            let result = parse tokens
            Expect.equal result (Ok (Literal 42.0)) "Should parse single number to Literal"
        }

        test "parse decimal number" {
            let tokens = [Number 3.14; EOF]
            let result = parse tokens
            Expect.equal result (Ok (Literal 3.14)) "Should parse decimal number to Literal"
        }

        test "parse empty input returns error" {
            let tokens = [EOF]
            let result = parse tokens
            Expect.isError result "Should return error for empty input"
        }
    ]

let allParserTests =
    testList "Parser tests" [
        literalTests
    ]
