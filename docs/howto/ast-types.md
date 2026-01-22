# AST (추상 구문 트리) 타입 정의하기

## 개요

이 문서에서 배우는 것:
- AST(Abstract Syntax Tree)란 무엇인가
- F#에서 재귀적 Discriminated Union 정의
- 표현식 트리 구조 설계

## 사전 준비

- F# Discriminated Union 이해 ([discriminated-unions.md](./discriminated-unions.md) 참고)
- Token 타입 정의 완료

## AST란?

AST는 소스 코드의 구조를 트리 형태로 표현한 것입니다.

```
Input: "1 + 2 * 3"

         +
        / \
       1   *
          / \
         2   3
```

이 트리를 F# 타입으로 표현하면:

```fsharp
Binary(
    Literal 1.0,
    Add,
    Binary(Literal 2.0, Multiply, Literal 3.0)
)
```

## 구현

### 1. 기본 표현식 타입

```fsharp
// Types.fs

/// AST expression types
type Expr =
    | Literal of float                              // 숫자 리터럴
    | Binary of left: Expr * operator: BinaryOp * right: Expr  // 이항 연산

and BinaryOp =
    | Add       // +
    | Subtract  // -
    | Multiply  // *
    | Divide    // /
```

### 2. 재귀적 타입 정의

`and` 키워드로 상호 참조하는 타입 정의:

```fsharp
type Expr =
    | Literal of float
    | Binary of left: Expr * operator: BinaryOp * right: Expr
                      ^^^^^
                      자기 자신을 참조 (재귀적)

and BinaryOp =  // 'and'로 Expr와 함께 정의
    | Add | Subtract | Multiply | Divide
```

### 3. Named Fields 사용

튜플 대신 이름 있는 필드를 사용하면 가독성이 높아집니다:

```fsharp
// 이름 없는 필드 (덜 명확)
| Binary of Expr * BinaryOp * Expr

// 이름 있는 필드 (권장)
| Binary of left: Expr * operator: BinaryOp * right: Expr
```

### 4. Parser 에러 타입

```fsharp
/// Parser error types
type ParseError =
    | UnexpectedToken of Token * expected: string
    | UnexpectedEndOfInput
    | MismatchedParenthesis
```

## 테스트

### TypesTests.fs

```fsharp
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
            Expect.equal expr (Binary(Literal 1.0, Add, Literal 2.0)) "Binary should be created"
        }

        test "Nested binary expression works" {
            // (1 + 2) * 3
            let inner = Binary(Literal 1.0, Add, Literal 2.0)
            let outer = Binary(inner, Multiply, Literal 3.0)
            Expect.equal outer
                (Binary(Binary(Literal 1.0, Add, Literal 2.0), Multiply, Literal 3.0))
                "Nested binary should work"
        }
    ]
```

### 테스트 실행

```bash
dotnet test
# 또는
dotnet run --project tests/LangOne.Tests
```

## AST 활용 예시

### 1. 표현식 출력 (Pretty Print)

```fsharp
let rec prettyPrint expr =
    match expr with
    | Literal n -> sprintf "%g" n
    | Binary(left, op, right) ->
        let opStr =
            match op with
            | Add -> "+" | Subtract -> "-"
            | Multiply -> "*" | Divide -> "/"
        sprintf "(%s %s %s)" (prettyPrint left) opStr (prettyPrint right)

// 사용
let ast = Binary(Literal 1.0, Add, Binary(Literal 2.0, Multiply, Literal 3.0))
prettyPrint ast  // "(1 + (2 * 3))"
```

### 2. 표현식 평가

```fsharp
let rec evaluate expr =
    match expr with
    | Literal n -> Ok n
    | Binary(left, op, right) ->
        evaluate left >>= fun l ->
        evaluate right >>= fun r ->
        match op with
        | Add -> Ok (l + r)
        | Subtract -> Ok (l - r)
        | Multiply -> Ok (l * r)
        | Divide ->
            if r = 0.0 then Error DivisionByZero
            else Ok (l / r)
```

## 패턴 매칭 팁

### 중첩된 패턴 매칭

```fsharp
match expr with
| Literal 0.0 -> "zero"
| Literal n when n < 0.0 -> "negative"
| Binary(Literal a, Add, Literal b) -> sprintf "%g + %g" a b
| Binary(_, Multiply, Literal 0.0) -> "anything times zero"
| _ -> "other"
```

## 문제 해결

| 문제 | 해결 방법 |
|------|-----------|
| "The type 'BinaryOp' is not defined" | `and` 키워드로 함께 정의 |
| 재귀 함수 에러 | `let rec` 사용 |
| 패턴 불완전 경고 | 모든 케이스 처리 또는 `_` 추가 |

## 다음 단계

- [parser-basics.md](./parser-basics.md) - Parser 구현하기 (예정)
