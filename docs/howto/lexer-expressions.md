# 복합 표현식 Lexing과 Serilog 로깅

## 개요

- 여러 토큰으로 구성된 표현식 처리
- 공백 문자 건너뛰기
- Serilog로 구조화된 로깅 추가

## 사전 준비

- 숫자 토큰화 구현 완료
- Serilog 패키지 설치됨
- `src/LangOne/Lexer.fs`, `src/LangOne/Logging.fs` 파일

## 구현

### 1. 공백 처리

```fsharp
/// Check if character is whitespace
let private isWhitespace (c: char) = c = ' ' || c = '\t' || c = '\n' || c = '\r'
```

메인 루프에서 공백 건너뛰기:

```fsharp
let tokenize (input: string) : Result<Token list, LexError> =
    let rec loop (pos: int) (acc: Token list) =
        if pos >= input.Length then
            Ok (List.rev (EOF :: acc))
        else
            let c = input.[pos]
            if isWhitespace c then
                loop (pos + 1) acc  // 공백 건너뛰기, 토큰 추가 안함
            elif isDigit c then
                // 숫자 처리...
            else
                // 연산자 처리...
    loop 0 []
```

**포인트**:
- 공백은 `acc`에 추가하지 않고 위치만 증가
- 탭, 줄바꿈도 처리

### 2. Serilog 설정

`Logging.fs` 모듈:

```fsharp
module LangOne.Logging

open Serilog

/// Initialize Serilog logger
let initialize () =
    Log.Logger <- LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .CreateLogger()

/// Get logger for a specific context
let forContext<'T> () = Log.ForContext<'T>()
```

### 3. Lexer에 로깅 추가

```fsharp
module LangOne.Lexer

open Serilog

let private log = Log.ForContext("Module", "Lexer")

let tokenize (input: string) : Result<Token list, LexError> =
    log.Debug("Tokenizing input: {Input}", input)
    let rec loop (pos: int) (acc: Token list) =
        if pos >= input.Length then
            let tokens = List.rev (EOF :: acc)
            log.Debug("Tokenization complete: {TokenCount} tokens", List.length tokens)
            Ok tokens
        else
            // ...
            match Double.TryParse(numStr) with
            | (true, value) ->
                log.Verbose("Token: Number {Value} at position {Position}", value, pos)
                loop newPos (Number value :: acc)
            | (false, _) ->
                log.Warning("Invalid number: {NumStr} at position {Position}", numStr, pos)
                Error (InvalidNumber (numStr, pos))
    loop 0 []
```

**로그 레벨**:
- `Debug`: 토큰화 시작/완료
- `Verbose`: 각 토큰 생성 (상세)
- `Warning`: 파싱 에러

### 4. 프로젝트 파일 순서

F#은 파일 순서가 중요합니다:

```xml
<ItemGroup>
  <Compile Include="Types.fs" />
  <Compile Include="Result.fs" />
  <Compile Include="Logging.fs" />   <!-- Lexer 전에 -->
  <Compile Include="Lexer.fs" />
  <Compile Include="Program.fs" />
</ItemGroup>
```

## 테스트

```fsharp
let expressionTests =
    testList "Expression lexing" [
        test "tokenize 1 + 2" {
            let result = tokenize "1 + 2"
            Expect.equal result (Ok [Number 1.0; Plus; Number 2.0; EOF])
                "Should tokenize 1 + 2"
        }

        test "tokenize 1 + 2 * 3" {
            let result = tokenize "1 + 2 * 3"
            Expect.equal result
                (Ok [Number 1.0; Plus; Number 2.0; Star; Number 3.0; EOF])
                "Should tokenize 1 + 2 * 3"
        }

        test "tokenize (1 + 2) * 3" {
            let result = tokenize "(1 + 2) * 3"
            Expect.equal result
                (Ok [LParen; Number 1.0; Plus; Number 2.0; RParen; Star; Number 3.0; EOF])
                "Should tokenize (1 + 2) * 3"
        }

        test "tokenize without spaces" {
            let result = tokenize "1+2*3"
            Expect.equal result
                (Ok [Number 1.0; Plus; Number 2.0; Star; Number 3.0; EOF])
                "Should work without spaces"
        }

        test "tokenize with extra spaces" {
            let result = tokenize "  1  +  2  "
            Expect.equal result (Ok [Number 1.0; Plus; Number 2.0; EOF])
                "Should ignore extra spaces"
        }
    ]
```

```bash
dotnet run --project tests/LangOne.Tests
```

## 로그 출력 예시

```
[11:19:30 DBG] Tokenizing input: "1 + 2 * 3"
[11:19:30 VRB] Token: Number 1 at position 0
[11:19:30 VRB] Token: Plus at position 2
[11:19:30 VRB] Token: Number 2 at position 4
[11:19:30 VRB] Token: Star at position 6
[11:19:30 VRB] Token: Number 3 at position 8
[11:19:30 DBG] Tokenization complete: 6 tokens
```

## 문제 해결

| 문제 | 해결 방법 |
|------|-----------|
| Serilog 네임스페이스 에러 | `open Serilog` 추가 |
| 로그가 안 나옴 | `Logging.initialize()` 호출 확인 |
| 파일 순서 에러 | fsproj에서 Logging.fs를 Lexer.fs 앞에 |

## 참고

- 관련 파일: `src/LangOne/Lexer.fs`, `src/LangOne/Logging.fs`
- Serilog 문서: https://serilog.net/
- 이전 문서: [lexer-numbers](./lexer-numbers.md)
