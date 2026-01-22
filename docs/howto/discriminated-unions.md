# F# Discriminated Union으로 타입 정의하기

## 개요

- F# Discriminated Union (DU)으로 Token, Error, AST 타입 정의
- 패턴 매칭을 통한 안전한 타입 처리
- 인터프리터의 핵심 데이터 구조 설계

## 사전 준비

- F# 기본 문법 이해
- .NET SDK 설치
- `src/LangOne/Types.fs` 파일

## 구현

### 1. Token 타입 정의

Lexer가 인식할 토큰 종류를 DU로 정의합니다.

```fsharp
type Token =
    | Number of float      // 숫자 (값 포함)
    | Plus                 // +
    | Minus                // -
    | Star                 // *
    | Slash                // /
    | LParen               // (
    | RParen               // )
    | EOF                  // 입력 끝
```

**포인트**:
- `Number of float`: 데이터를 포함하는 케이스
- `Plus`, `Minus` 등: 데이터 없는 단순 케이스
- 모든 가능한 토큰을 열거하여 컴파일 타임 안전성 확보

### 2. Error 타입 정의

각 처리 단계별 에러를 명확히 구분합니다.

```fsharp
/// Lexer 에러
type LexError =
    | UnexpectedCharacter of char * position: int
    | InvalidNumber of string * position: int

/// Parser 에러
type ParseError =
    | UnexpectedToken of Token * expected: string
    | UnexpectedEndOfInput
    | MismatchedParenthesis

/// Evaluator 에러
type EvalError =
    | DivisionByZero
```

**포인트**:
- Named tuple 필드 (`position: int`)로 가독성 향상
- 에러 종류별로 필요한 정보만 포함

### 3. AST 타입 정의

Parser가 생성할 추상 구문 트리입니다.

```fsharp
type Expr =
    | Literal of float
    | Binary of left: Expr * operator: BinaryOp * right: Expr

and BinaryOp =
    | Add
    | Subtract
    | Multiply
    | Divide
```

**포인트**:
- `and` 키워드: 상호 참조하는 타입 동시 정의
- 재귀적 구조: `Binary`가 `Expr`을 포함

### 4. 통합 에러 타입

모든 에러를 하나의 타입으로 래핑합니다.

```fsharp
type InterpreterError =
    | LexerError of LexError
    | ParserError of ParseError
    | EvaluatorError of EvalError
```

## 테스트

```fsharp
open Expecto
open LangOne.Types

let tokenTests =
    testList "Token tests" [
        test "Token equality works for Number" {
            Expect.equal (Number 42.0) (Number 42.0) "Same numbers should be equal"
        }

        test "Different tokens are not equal" {
            Expect.notEqual Plus Minus "Plus and Minus should not be equal"
        }
    ]
```

```bash
dotnet run --project tests/LangOne.Tests
```

## 문제 해결

| 문제 | 해결 방법 |
|------|-----------|
| "The type 'X' is not defined" | 파일 순서 확인 (fsproj에서 Types.fs가 먼저) |
| 상호 참조 타입 에러 | `and` 키워드로 동시 정의 |
| 패턴 매칭 경고 | 모든 케이스를 처리했는지 확인 |

## 참고

- [F# Discriminated Unions](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/discriminated-unions)
- 관련 파일: `src/LangOne/Types.fs`
