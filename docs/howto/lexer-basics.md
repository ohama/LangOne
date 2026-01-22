# 기본 Lexer 구현하기

## 개요

- 문자열을 토큰 리스트로 변환하는 Lexer 구현
- 단일 문자 토큰 처리 (+, -, *, /, (, ))
- Result 타입으로 에러 처리

## 사전 준비

- Token 타입 정의 완료
- Result 헬퍼 함수 이해
- `src/LangOne/Lexer.fs` 파일

## 구현

### 1. 기본 구조

```fsharp
module LangOne.Lexer

open LangOne.Types

/// Tokenize an input string into a list of tokens
let tokenize (input: string) : Result<Token list, LexError> =
    // 구현
```

### 2. 단일 문자 토큰 처리

각 문자를 해당 토큰으로 변환:

```fsharp
/// Tokenize a single character at given position
let private tokenizeChar (c: char) (pos: int) : Result<Token, LexError> =
    match c with
    | '+' -> Ok Plus
    | '-' -> Ok Minus
    | '*' -> Ok Star
    | '/' -> Ok Slash
    | '(' -> Ok LParen
    | ')' -> Ok RParen
    | _ -> Error (UnexpectedCharacter (c, pos))
```

**포인트**:
- 패턴 매칭으로 각 문자 처리
- 알 수 없는 문자는 에러 반환
- 위치 정보 포함으로 디버깅 용이

### 3. 재귀적 토큰화

문자열 전체를 순회하며 토큰 리스트 생성:

```fsharp
let tokenize (input: string) : Result<Token list, LexError> =
    let rec loop (pos: int) (acc: Token list) =
        if pos >= input.Length then
            Ok (List.rev (EOF :: acc))  // 끝에 EOF 추가
        else
            let c = input.[pos]
            match tokenizeChar c pos with
            | Ok token -> loop (pos + 1) (token :: acc)
            | Error e -> Error e        // 에러 시 즉시 반환
    loop 0 []
```

**포인트**:
- `acc`에 토큰을 역순으로 쌓고 마지막에 `List.rev`
- EOF 토큰으로 입력 끝 표시
- 에러 발생 시 즉시 중단 (fail-fast)

### 4. 전체 코드

```fsharp
module LangOne.Lexer

open LangOne.Types

let private tokenizeChar (c: char) (pos: int) : Result<Token, LexError> =
    match c with
    | '+' -> Ok Plus
    | '-' -> Ok Minus
    | '*' -> Ok Star
    | '/' -> Ok Slash
    | '(' -> Ok LParen
    | ')' -> Ok RParen
    | _ -> Error (UnexpectedCharacter (c, pos))

let tokenize (input: string) : Result<Token list, LexError> =
    let rec loop (pos: int) (acc: Token list) =
        if pos >= input.Length then
            Ok (List.rev (EOF :: acc))
        else
            let c = input.[pos]
            match tokenizeChar c pos with
            | Ok token -> loop (pos + 1) (token :: acc)
            | Error e -> Error e
    loop 0 []
```

## 테스트

```fsharp
open Expecto
open LangOne.Types
open LangOne.Lexer

let lexerTests =
    testList "Lexer tests" [
        test "tokenize plus" {
            let result = tokenize "+"
            Expect.equal result (Ok [Plus; EOF]) "Should tokenize +"
        }

        test "tokenize operators" {
            let result = tokenize "+-*/"
            Expect.equal result (Ok [Plus; Minus; Star; Slash; EOF])
                "Should tokenize all operators"
        }

        test "unknown character returns error" {
            let result = tokenize "@"
            match result with
            | Error (UnexpectedCharacter ('@', 0)) -> ()
            | _ -> failtest "Should return error for @"
        }
    ]
```

```bash
dotnet run --project tests/LangOne.Tests
```

## 다음 단계

이 Lexer는 아직 불완전합니다:

1. **숫자 처리** (Step 1.3): `123`, `3.14` 같은 숫자 토큰
2. **공백 처리** (Step 1.4): 공백 문자 무시
3. **복합 표현식** (Step 1.4): `1 + 2 * 3`

## 문제 해결

| 문제 | 해결 방법 |
|------|-----------|
| 토큰 순서가 뒤집힘 | `List.rev` 확인 |
| EOF 누락 | loop 종료 시 EOF 추가 확인 |
| 위치 정보 부정확 | pos 증가 로직 확인 |

## 참고

- 관련 파일: `src/LangOne/Lexer.fs`, `tests/LangOne.Tests/LexerTests.fs`
