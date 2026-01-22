# Parser 기초: 숫자 리터럴 파싱

## 개요

이 문서에서 배우는 것:
- Parser의 역할과 구조
- 토큰 리스트를 AST로 변환하는 방법
- 패턴 매칭으로 토큰 처리하기
- Result 타입으로 파싱 에러 처리

## 사전 준비

- Lexer 구현 완료 ([lexer-basics.md](./lexer-basics.md) 참고)
- AST 타입 정의 ([ast-types.md](./ast-types.md) 참고)
- Result 타입 이해 ([result-rop.md](./result-rop.md) 참고)

## Parser란?

Parser는 토큰 리스트를 AST(추상 구문 트리)로 변환합니다.

```
Lexer 출력: [Number 42.0; EOF]
              ↓
           Parser
              ↓
AST:        Literal 42.0
```

## 구현

### 1. Parser 모듈 생성

`src/LangOne/Parser.fs`:

```fsharp
module LangOne.Parser

open LangOne.Types

/// Parse a list of tokens into an AST expression
let parse (tokens: Token list) : Result<Expr, ParseError> =
    match tokens with
    | [EOF] -> Error UnexpectedEndOfInput
    | [Number n; EOF] -> Ok (Literal n)
    | Number n :: _ -> Ok (Literal n)
    | token :: _ -> Error (UnexpectedToken (token, "number"))
    | [] -> Error UnexpectedEndOfInput
```

### 2. 패턴 설명

| 패턴 | 의미 | 결과 |
|------|------|------|
| `[EOF]` | EOF만 있음 (빈 입력) | Error |
| `[Number n; EOF]` | 숫자 하나와 EOF | Ok (Literal n) |
| `Number n :: _` | 숫자로 시작 | Ok (Literal n) |
| `token :: _` | 숫자가 아닌 토큰 | Error |
| `[]` | 빈 리스트 | Error |

### 3. 프로젝트 파일 업데이트

`src/LangOne/LangOne.fsproj`:

```xml
<ItemGroup>
  <Compile Include="Types.fs" />
  <Compile Include="Result.fs" />
  <Compile Include="Logging.fs" />
  <Compile Include="Lexer.fs" />
  <Compile Include="Parser.fs" />    <!-- 추가 -->
  <Compile Include="Program.fs" />
</ItemGroup>
```

## 테스트

### ParserTests.fs

```fsharp
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
```

### 테스트 프로젝트 등록

`tests/LangOne.Tests/LangOne.Tests.fsproj`:

```xml
<ItemGroup>
  <Compile Include="TypesTests.fs" />
  <Compile Include="LexerTests.fs" />
  <Compile Include="ParserTests.fs" />    <!-- 추가 -->
  <Compile Include="Program.fs" />
</ItemGroup>
```

`tests/LangOne.Tests/Program.fs`:

```fsharp
let allTests =
    testList "All tests" [
        smokeTests
        TypesTests.allTypesTests
        LexerTests.allLexerTests
        ParserTests.allParserTests    // 추가
    ]
```

### 테스트 실행

```bash
dotnet run --project tests/LangOne.Tests
# EXPECTO! 37 tests run – 37 passed
```

## 패턴 매칭 순서

패턴 매칭은 위에서 아래로 순서대로 검사합니다. 순서가 중요합니다:

```fsharp
// 잘못된 순서 (Number n :: _ 가 먼저면 [Number n; EOF]는 매칭 안됨)
match tokens with
| Number n :: _ -> Ok (Literal n)        // 모든 숫자 시작 케이스 매칭
| [Number n; EOF] -> Ok (Literal n)      // 절대 도달 안함!

// 올바른 순서 (구체적인 것 먼저)
match tokens with
| [Number n; EOF] -> Ok (Literal n)      // 정확히 숫자 하나
| Number n :: _ -> Ok (Literal n)        // 숫자로 시작하는 나머지
```

## 리스트 패턴

F# 리스트 패턴 문법:

| 패턴 | 의미 |
|------|------|
| `[]` | 빈 리스트 |
| `[x]` | 요소 하나 |
| `[x; y]` | 요소 두 개 |
| `x :: xs` | 첫 요소 `x`, 나머지 `xs` |
| `x :: y :: xs` | 첫 두 요소와 나머지 |
| `_` | 아무 값 (무시) |

## Lexer + Parser 통합 예시

```fsharp
open LangOne.Lexer
open LangOne.Parser
open LangOne.Result

let interpret input =
    tokenize input
    |> Result.mapError LexerError
    >>= (parse >> Result.mapError ParserError)

// 사용
interpret "42"       // Ok (Literal 42.0)
interpret ""         // Error (ParserError UnexpectedEndOfInput)
interpret "@"        // Error (LexerError (UnexpectedCharacter ('@', 0)))
```

## 문제 해결

| 문제 | 해결 방법 |
|------|-----------|
| "The value 'parse' is not defined" | Parser.fs가 fsproj에 추가되었는지 확인 |
| 패턴 매칭 경고 | 모든 케이스 처리했는지 확인 |
| 테스트에서 Parser 못 찾음 | `open LangOne.Parser` 추가 |

## 다음 단계

- [parser-binary.md](./parser-binary.md) - 이항 연산자 파싱 (예정)
