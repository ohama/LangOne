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

let mulDivTests =
    testList "Parser multiplication/division tests" [
        test "parse 2 * 3" {
            // 2 * 3 => Binary(Literal 2, Multiply, Literal 3)
            let tokens = [Number 2.0; Star; Number 3.0; EOF]
            let result = parse tokens
            let expected = Binary(Literal 2.0, Multiply, Literal 3.0)
            Expect.equal result (Ok expected) "Should parse multiplication"
        }

        test "parse 6 / 2" {
            // 6 / 2 => Binary(Literal 6, Divide, Literal 2)
            let tokens = [Number 6.0; Slash; Number 2.0; EOF]
            let result = parse tokens
            let expected = Binary(Literal 6.0, Divide, Literal 2.0)
            Expect.equal result (Ok expected) "Should parse division"
        }

        test "precedence 1 + 2 * 3 = 1 + (2 * 3)" {
            // 1 + 2 * 3 => Binary(Literal 1, Add, Binary(Literal 2, Multiply, Literal 3))
            let tokens = [Number 1.0; Plus; Number 2.0; Star; Number 3.0; EOF]
            let result = parse tokens
            let expected = Binary(Literal 1.0, Add, Binary(Literal 2.0, Multiply, Literal 3.0))
            Expect.equal result (Ok expected) "Multiplication should have higher precedence than addition"
        }
    ]

let parenTests =
    testList "Parser parentheses tests" [
        test "parse (1 + 2)" {
            // (1 + 2) => Binary(Literal 1, Add, Literal 2)
            let tokens = [LParen; Number 1.0; Plus; Number 2.0; RParen; EOF]
            let result = parse tokens
            let expected = Binary(Literal 1.0, Add, Literal 2.0)
            Expect.equal result (Ok expected) "Should parse parenthesized expression"
        }

        test "parse (1 + 2) * 3 overrides precedence" {
            // (1 + 2) * 3 => Binary(Binary(1, Add, 2), Multiply, 3)
            let tokens = [LParen; Number 1.0; Plus; Number 2.0; RParen; Star; Number 3.0; EOF]
            let result = parse tokens
            let expected = Binary(Binary(Literal 1.0, Add, Literal 2.0), Multiply, Literal 3.0)
            Expect.equal result (Ok expected) "Parentheses should override precedence"
        }

        test "parse nested parentheses ((1 + 2) * 3)" {
            // ((1 + 2) * 3) => Binary(Binary(1, Add, 2), Multiply, 3)
            let tokens = [LParen; LParen; Number 1.0; Plus; Number 2.0; RParen; Star; Number 3.0; RParen; EOF]
            let result = parse tokens
            let expected = Binary(Binary(Literal 1.0, Add, Literal 2.0), Multiply, Literal 3.0)
            Expect.equal result (Ok expected) "Should handle nested parentheses"
        }

        test "mismatched parentheses error (1 + 2" {
            // Missing closing paren
            let tokens = [LParen; Number 1.0; Plus; Number 2.0; EOF]
            let result = parse tokens
            Expect.isError result "Should return error for missing closing parenthesis"
        }
    ]

let allParserTests =
    testList "Parser tests" [
        literalTests
        addSubTests
        mulDivTests
        parenTests
    ]
