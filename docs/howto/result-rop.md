# Result 타입과 ROP (Railway Oriented Programming) 패턴

## 개요

- F# Result 타입으로 에러 처리
- ROP 패턴으로 파이프라인 구성
- bind, map, Kleisli 연산자 구현

## 사전 준비

- F# 기본 문법 이해
- Discriminated Union 개념
- `src/LangOne/Result.fs` 파일

## 구현

### 1. Result 타입 이해

F#의 내장 `Result<'T, 'E>` 타입:

```fsharp
// 내장 타입 (정의할 필요 없음)
type Result<'T, 'E> =
    | Ok of 'T
    | Error of 'E
```

### 2. bind 함수

Result를 반환하는 함수를 연결합니다.

```fsharp
/// Bind: Chain Result-returning functions
let bind f result =
    match result with
    | Ok x -> f x           // 성공이면 함수 적용
    | Error e -> Error e    // 실패면 그대로 전파
```

**사용 예**:

```fsharp
let validatePositive x =
    if x > 0 then Ok x else Error "Must be positive"

let double x = Ok (x * 2)

// bind로 연결
let result = validatePositive 5 |> bind double
// result = Ok 10

let result2 = validatePositive -1 |> bind double
// result2 = Error "Must be positive" (double은 호출되지 않음)
```

### 3. map 함수

일반 함수를 Result에 적용합니다.

```fsharp
/// Map: Apply a function to the success value
let map f result =
    match result with
    | Ok x -> Ok (f x)      // 성공이면 함수 적용 후 다시 Ok로 감싸기
    | Error e -> Error e    // 실패면 그대로 전파
```

**bind vs map**:
- `bind`: `'a -> Result<'b, 'e>` 함수에 사용
- `map`: `'a -> 'b` 함수에 사용

### 4. 연산자 정의

파이프라인 스타일 코딩을 위한 연산자:

```fsharp
/// Bind operator (>>=)
let (>>=) result f = bind f result

/// Map operator (<!>)
let (<!>) f result = map f result

/// Kleisli composition (>=>)
let (>=>) f g x = f x >>= g
```

### 5. ROP 패턴 적용

인터프리터 파이프라인 예시:

```fsharp
// 각 단계가 Result를 반환
let lex: string -> Result<Token list, LexError>
let parse: Token list -> Result<Expr, ParseError>
let eval: Expr -> Result<float, EvalError>

// Kleisli로 조합
let interpret = lex >=> parse >=> eval
```

## 테스트

```fsharp
open Expecto
open LangOne.Result

let resultTests =
    testList "Result tests" [
        test "Result bind works with Ok" {
            let result = Ok 5 >>= (fun x -> Ok (x * 2))
            Expect.equal result (Ok 10) "Bind should apply function"
        }

        test "Result bind short-circuits on Error" {
            let result = Error "fail" >>= (fun x -> Ok (x * 2))
            Expect.equal result (Error "fail") "Bind should propagate Error"
        }

        test "Kleisli composition works" {
            let double x = Ok (x * 2)
            let addOne x = Ok (x + 1)
            let composed = double >=> addOne
            Expect.equal (composed 5) (Ok 11) "5 * 2 + 1 = 11"
        }
    ]
```

```bash
dotnet run --project tests/LangOne.Tests
```

## 시각화: Railway 비유

```
Input ─────────────────────────────────────────────── Output
       │                                               │
       ▼                                               ▼
    ┌──────┐    ┌──────┐    ┌──────┐    ┌──────┐
 ───│ Lex  │───▶│Parse │───▶│ Eval │───▶│  Ok  │──▶ 결과
    └──────┘    └──────┘    └──────┘    └──────┘
       │           │           │
       ▼           ▼           ▼
    ┌──────────────────────────────────────────┐
 ───│              Error Track                 │──▶ 에러
    └──────────────────────────────────────────┘
```

에러가 발생하면 즉시 Error Track으로 이동하여 이후 단계를 건너뜁니다.

## 문제 해결

| 문제 | 해결 방법 |
|------|-----------|
| 타입 불일치 에러 | bind vs map 선택 확인 |
| Error 타입 통일 안됨 | mapError로 변환 |
| 연산자 우선순위 | 괄호로 명시적 그룹화 |

## 참고

- [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/)
- 관련 파일: `src/LangOne/Result.fs`
