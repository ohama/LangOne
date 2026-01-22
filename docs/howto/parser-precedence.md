# Recursive Descent Parser: 연산자 우선순위와 괄호

## 개요

이 문서에서 배우는 것:
- Recursive Descent Parser 패턴
- 연산자 우선순위 구현 방법
- 좌결합(left-associative) 연산자 처리
- 괄호로 우선순위 재정의
- 상호 재귀 (`let rec ... and ...`)

## 사전 준비

- Parser 기초 ([parser-literals.md](./parser-literals.md) 참고)
- AST 타입 이해 ([ast-types.md](./ast-types.md) 참고)

## 연산자 우선순위란?

수학에서 `1 + 2 * 3`은 `1 + (2 * 3) = 7`입니다.
곱셈이 덧셈보다 **우선순위가 높기** 때문입니다.

| 우선순위 | 연산자 |
|----------|--------|
| 높음 | `*` `/` |
| 낮음 | `+` `-` |

## Recursive Descent 구조

우선순위가 낮은 것부터 높은 것 순서로 파싱합니다:

```
parseExpr → parseAddSub → parseMulDiv → parsePrimary
              (+ -)        (* /)         (numbers, parens)
```

각 레벨은 다음 레벨을 호출합니다.

## 구현

### 1. Primary (가장 높은 우선순위)

숫자 리터럴과 괄호 표현식:

```fsharp
let rec private parsePrimary state =
    match current state with
    | Number n -> Ok (Literal n, advance state)
    | LParen ->
        let state = advance state // consume '('
        parseExpr state
        |> Result.bind (fun (expr, state) ->
            match current state with
            | RParen -> Ok (expr, advance state) // consume ')'
            | _ -> Error MismatchedParenthesis)
    | EOF -> Error UnexpectedEndOfInput
    | token -> Error (UnexpectedToken (token, "number or '('"))
```

### 2. 곱셈/나눗셈 (높은 우선순위)

```fsharp
and private parseMulDiv state =
    parsePrimary state
    |> Result.bind (fun (left, state) ->
        let rec loop left state =
            match current state with
            | Star ->
                let state = advance state
                parsePrimary state
                |> Result.bind (fun (right, state) ->
                    loop (Binary(left, Multiply, right)) state)
            | Slash ->
                let state = advance state
                parsePrimary state
                |> Result.bind (fun (right, state) ->
                    loop (Binary(left, Divide, right)) state)
            | _ -> Ok (left, state)
        loop left state)
```

### 3. 덧셈/뺄셈 (낮은 우선순위)

```fsharp
and private parseAddSub state =
    parseMulDiv state  // 먼저 높은 우선순위 파싱!
    |> Result.bind (fun (left, state) ->
        let rec loop left state =
            match current state with
            | Plus ->
                let state = advance state
                parseMulDiv state  // 높은 우선순위 호출
                |> Result.bind (fun (right, state) ->
                    loop (Binary(left, Add, right)) state)
            | Minus ->
                let state = advance state
                parseMulDiv state
                |> Result.bind (fun (right, state) ->
                    loop (Binary(left, Subtract, right)) state)
            | _ -> Ok (left, state)
        loop left state)
```

### 4. 진입점

```fsharp
and private parseExpr state =
    parseAddSub state

let parse (tokens: Token list) : Result<Expr, ParseError> =
    let state = { Tokens = tokens; Position = 0 }
    match parseExpr state with
    | Ok (expr, _) -> Ok expr
    | Error err -> Error err
```

## 좌결합 (Left-Associative)

`1 + 2 + 3`은 `(1 + 2) + 3`으로 파싱되어야 합니다.

이것이 `loop` 패턴을 사용하는 이유입니다:

```fsharp
let rec loop left state =
    match current state with
    | Plus ->
        // ... parse right operand ...
        loop (Binary(left, Add, right)) state  // left가 누적됨
    | _ -> Ok (left, state)
```

**실행 흐름:**
```
1 + 2 + 3
loop(1)
  → + 발견, right = 2
  → loop(Binary(1, +, 2))
    → + 발견, right = 3
    → loop(Binary(Binary(1, +, 2), +, 3))
      → 더 이상 + 없음
      → 반환
```

## 상호 재귀 (Mutual Recursion)

괄호 안에서 다시 전체 표현식을 파싱해야 합니다:

```
parsePrimary → (parenthesized) → parseExpr → parseAddSub → parseMulDiv → parsePrimary
```

F#에서는 `let rec ... and ...` 패턴을 사용합니다:

```fsharp
let rec private parsePrimary state = ...
and private parseMulDiv state = ...
and private parseAddSub state = ...
and private parseExpr state = ...
```

**중요**: `and` 키워드로 연결된 함수들만 서로를 호출할 수 있습니다.

## 테스트

### ParserTests.fs

```fsharp
let mulDivTests =
    testList "Parser multiplication/division tests" [
        test "precedence 1 + 2 * 3 = 1 + (2 * 3)" {
            let tokens = [Number 1.0; Plus; Number 2.0; Star; Number 3.0; EOF]
            let result = parse tokens
            let expected = Binary(Literal 1.0, Add, Binary(Literal 2.0, Multiply, Literal 3.0))
            Expect.equal result (Ok expected) "Multiplication should have higher precedence"
        }
    ]

let parenTests =
    testList "Parser parentheses tests" [
        test "parse (1 + 2) * 3 overrides precedence" {
            let tokens = [LParen; Number 1.0; Plus; Number 2.0; RParen; Star; Number 3.0; EOF]
            let result = parse tokens
            let expected = Binary(Binary(Literal 1.0, Add, Literal 2.0), Multiply, Literal 3.0)
            Expect.equal result (Ok expected) "Parentheses should override precedence"
        }

        test "mismatched parentheses error" {
            let tokens = [LParen; Number 1.0; Plus; Number 2.0; EOF]
            let result = parse tokens
            Expect.isError result "Should return error for missing closing paren"
        }
    ]
```

### 테스트 실행

```bash
dotnet run --project tests/LangOne.Tests
# EXPECTO! 47 tests run – 47 passed
```

## AST 시각화

### `1 + 2 * 3`

```
      +
     / \
    1   *
       / \
      2   3
```

AST: `Binary(Literal 1, Add, Binary(Literal 2, Multiply, Literal 3))`

### `(1 + 2) * 3`

```
      *
     / \
    +   3
   / \
  1   2
```

AST: `Binary(Binary(Literal 1, Add, Literal 2), Multiply, Literal 3)`

## 문제 해결

| 문제 | 해결 방법 |
|------|-----------|
| "Value not defined" 에러 | 함수 순서 확인, 상호 재귀는 `let rec ... and ...` 사용 |
| 우선순위 잘못됨 | 호출 순서 확인: 낮은 우선순위 → 높은 우선순위 |
| 좌결합 안됨 | `loop` 패턴 확인, left가 누적되는지 확인 |
| 괄호 에러 | parsePrimary에서 RParen 체크 확인 |

## 다음 단계

- [evaluator-basics.md](./evaluator-basics.md) - AST 평가하기 (예정)
