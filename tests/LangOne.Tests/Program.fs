module LangOne.Tests.Program

open Expecto

let smokeTests =
    testList "Smoke tests" [
        test "basic math works" {
            Expect.equal (1 + 1) 2 "1 + 1 should equal 2"
        }

        test "string comparison works" {
            Expect.equal "hello" "hello" "Strings should match"
        }
    ]

let allTests =
    testList "All tests" [
        smokeTests
        TypesTests.allTypesTests
        LexerTests.allLexerTests
    ]

[<EntryPoint>]
let main args =
    runTestsWithCLIArgs [] args allTests
