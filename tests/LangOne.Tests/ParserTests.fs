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

let addSubTests =
    testList "Parser addition/subtraction tests" [
        test "parse 1 + 2" {
            // 1 + 2 => Binary(Literal 1, Add, Literal 2)
            let tokens = [Number 1.0; Plus; Number 2.0; EOF]
            let result = parse tokens
            let expected = Binary(Literal 1.0, Add, Literal 2.0)
            Expect.equal result (Ok expected) "Should parse addition"
        }

        test "parse 5 - 3" {
            // 5 - 3 => Binary(Literal 5, Subtract, Literal 3)
            let tokens = [Number 5.0; Minus; Number 3.0; EOF]
            let result = parse tokens
            let expected = Binary(Literal 5.0, Subtract, Literal 3.0)
            Expect.equal result (Ok expected) "Should parse subtraction"
        }

        test "parse left-associative 1 + 2 + 3" {
            // 1 + 2 + 3 => Binary(Binary(Literal 1, Add, Literal 2), Add, Literal 3)
            let tokens = [Number 1.0; Plus; Number 2.0; Plus; Number 3.0; EOF]
            let result = parse tokens
            let expected = Binary(Binary(Literal 1.0, Add, Literal 2.0), Add, Literal 3.0)
            Expect.equal result (Ok expected) "Should be left-associative"
        }
    ]

let allParserTests =
    testList "Parser tests" [
        literalTests
        addSubTests
    ]
