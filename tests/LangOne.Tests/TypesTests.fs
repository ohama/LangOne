module LangOne.Tests.TypesTests

open Expecto
open LangOne.Types
open LangOne.Result

let tokenTests =
    testList "Token tests" [
        test "Token equality works for Number" {
            Expect.equal (Number 42.0) (Number 42.0) "Same numbers should be equal"
        }

        test "Token equality works for operators" {
            Expect.equal Plus Plus "Plus should equal Plus"
            Expect.equal Minus Minus "Minus should equal Minus"
            Expect.equal Star Star "Star should equal Star"
            Expect.equal Slash Slash "Slash should equal Slash"
        }

        test "Different tokens are not equal" {
            Expect.notEqual (Number 1.0) (Number 2.0) "Different numbers should not be equal"
            Expect.notEqual Plus Minus "Plus and Minus should not be equal"
        }

        test "Parentheses tokens work" {
            Expect.equal LParen LParen "LParen should equal LParen"
            Expect.equal RParen RParen "RParen should equal RParen"
            Expect.notEqual LParen RParen "LParen and RParen should not be equal"
        }
    ]

let exprTests =
    testList "Expr tests" [
        test "Literal creation works" {
            let expr = Literal 42.0
            Expect.equal expr (Literal 42.0) "Literal should be created correctly"
        }

        test "Binary expression creation works" {
            let left = Literal 1.0
            let right = Literal 2.0
            let expr = Binary(left, Add, right)
            Expect.equal expr (Binary(Literal 1.0, Add, Literal 2.0)) "Binary should be created correctly"
        }

        test "Nested binary expression works" {
            // (1 + 2) * 3
            let inner = Binary(Literal 1.0, Add, Literal 2.0)
            let outer = Binary(inner, Multiply, Literal 3.0)
            Expect.equal outer (Binary(Binary(Literal 1.0, Add, Literal 2.0), Multiply, Literal 3.0)) "Nested binary should work"
        }

        test "BinaryOp types work" {
            Expect.equal Add Add "Add should equal Add"
            Expect.equal Subtract Subtract "Subtract should equal Subtract"
            Expect.equal Multiply Multiply "Multiply should equal Multiply"
            Expect.equal Divide Divide "Divide should equal Divide"
            Expect.notEqual Add Subtract "Different ops should not be equal"
        }
    ]

let parseErrorTests =
    testList "ParseError tests" [
        test "UnexpectedToken creation works" {
            let err = UnexpectedToken(Plus, "number")
            Expect.equal err (UnexpectedToken(Plus, "number")) "UnexpectedToken should be created correctly"
        }

        test "UnexpectedEndOfInput works" {
            let err = UnexpectedEndOfInput
            Expect.equal err UnexpectedEndOfInput "UnexpectedEndOfInput should work"
        }

        test "MismatchedParenthesis works" {
            let err = MismatchedParenthesis
            Expect.equal err MismatchedParenthesis "MismatchedParenthesis should work"
        }
    ]

let resultTests =
    testList "Result tests" [
        test "Result bind works with Ok" {
            let result = Ok 5 >>= (fun x -> Ok (x * 2))
            Expect.equal result (Ok 10) "Bind should apply function to Ok value"
        }

        test "Result bind short-circuits on Error" {
            let result = Error "fail" >>= (fun x -> Ok (x * 2))
            Expect.equal result (Error "fail") "Bind should propagate Error"
        }

        test "Result map works with Ok" {
            let result = Ok 5 |> map (fun x -> x * 2)
            Expect.equal result (Ok 10) "Map should apply function to Ok value"
        }

        test "Result map preserves Error" {
            let result = Error "fail" |> map (fun x -> x * 2)
            Expect.equal result (Error "fail") "Map should preserve Error"
        }

        test "Kleisli composition works" {
            let double x = Ok (x * 2)
            let addOne x = Ok (x + 1)
            let composed = double >=> addOne
            let result = composed 5
            Expect.equal result (Ok 11) "Kleisli composition should chain functions"
        }
    ]

let allTypesTests =
    testList "Types and Result tests" [
        tokenTests
        exprTests
        parseErrorTests
        resultTests
    ]
