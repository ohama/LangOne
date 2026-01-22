# Lexer에서 숫자 토큰화하기

## 개요

- 정수와 소수점 숫자를 토큰으로 변환
- 여러 자리 숫자 처리 (123, 3.14)
- F# 재귀 함수로 문자열 파싱

## 사전 준비

- 기본 Lexer 구현 완료
- Token 타입에 `Number of float` 케이스
- `src/LangOne/Lexer.fs` 파일

## 구현

### 1. 숫자 판별 함수

```fsharp
/// Check if character is a digit
let private isDigit (c: char) = c >= '0' && c <= '9'
```

### 2. 숫자 문자열 읽기

여러 자리 숫자와 소수점을 처리하는 재귀 함수:

```fsharp
/// Read a number starting at position, returns (number string, new position)
let private readNumber (input: string) (startPos: int) : string * int =
    let rec readDigits pos hasDecimal =
        if pos >= input.Length then
            (pos, hasDecimal)
        else
            let c = input.[pos]
            if isDigit c then
                readDigits (pos + 1) hasDecimal
            elif c = '.' && not hasDecimal && pos + 1 < input.Length && isDigit input.[pos + 1] then
                readDigits (pos + 1) true  // 소수점 발견, hasDecimal = true
            else
                (pos, hasDecimal)
    let (endPos, _) = readDigits startPos false
    (input.Substring(startPos, endPos - startPos), endPos)
```

**핵심 로직**:
- `hasDecimal`: 소수점을 이미 만났는지 추적
- 소수점 조건: `.` 뒤에 반드시 숫자가 와야 함
- 소수점은 한 번만 허용 (`not hasDecimal`)

### 3. 토큰화 통합

메인 루프에서 숫자 처리:

```fsharp
let tokenize (input: string) : Result<Token list, LexError> =
    let rec loop (pos: int) (acc: Token list) =
        if pos >= input.Length then
            Ok (List.rev (EOF :: acc))
        else
            let c = input.[pos]
            if isDigit c then
                let (numStr, newPos) = readNumber input pos
                match Double.TryParse(numStr) with
                | (true, value) -> loop newPos (Number value :: acc)
                | (false, _) -> Error (InvalidNumber (numStr, pos))
            else
                // ... 연산자 처리
    loop 0 []
```

**포인트**:
- `Double.TryParse`: 안전한 문자열 → float 변환
- 파싱 실패 시 `InvalidNumber` 에러
- `newPos`로 위치 점프 (이미 읽은 숫자 건너뛰기)

## 테스트

```fsharp
let numberTokenTests =
    testList "Number token lexing" [
        test "tokenize single digit" {
            let result = tokenize "5"
            Expect.equal result (Ok [Number 5.0; EOF]) "Should tokenize 5"
        }

        test "tokenize multi-digit number" {
            let result = tokenize "123"
            Expect.equal result (Ok [Number 123.0; EOF]) "Should tokenize 123"
        }

        test "tokenize decimal number" {
            let result = tokenize "3.14"
            Expect.equal result (Ok [Number 3.14; EOF]) "Should tokenize 3.14"
        }

        test "tokenize number with leading zero" {
            let result = tokenize "0.5"
            Expect.equal result (Ok [Number 0.5; EOF]) "Should tokenize 0.5"
        }
    ]
```

```bash
dotnet run --project tests/LangOne.Tests
```

## 엣지 케이스

| 입력 | 결과 | 설명 |
|------|------|------|
| `123` | `Number 123.0` | 정수 |
| `3.14` | `Number 3.14` | 소수 |
| `0.5` | `Number 0.5` | 0으로 시작 |
| `1.2.3` | `Number 1.2`, 에러 | 두 번째 `.`은 에러 |
| `.5` | 에러 | 소수점으로 시작 불가 |

## 문제 해결

| 문제 | 해결 방법 |
|------|-----------|
| 소수점만 있는 숫자 에러 | `.` 뒤에 숫자 필수 조건 추가 |
| 여러 소수점 | `hasDecimal` 플래그로 한 번만 허용 |
| 위치 점프 안됨 | `newPos` 반환값 사용 확인 |

## 참고

- 관련 파일: `src/LangOne/Lexer.fs`
- 이전 문서: [lexer-basics](./lexer-basics.md)
