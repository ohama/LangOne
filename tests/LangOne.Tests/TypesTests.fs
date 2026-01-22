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
        resultTests
    ]
