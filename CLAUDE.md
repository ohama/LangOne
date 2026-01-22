# CLAUDE.md

이 파일은 Claude Code가 LangOne 프로젝트를 이해하기 위한 컨텍스트를 제공합니다.

## 프로젝트 개요

**LangOne**은 F#으로 작성된 4칙 연산 표현식 인터프리터입니다.

- 문자열 수식 (예: `"1 + 2 * 3"`)을 파싱하여 계산
- 연산자 우선순위 및 괄호 지원
- Railway Oriented Programming (ROP) 패턴 적용

## 기술 스택

- **언어**: F# (.NET 8)
- **테스트**: Expecto + FsCheck
- **로깅**: Serilog

## 프로젝트 구조

```
LangOne/
├── src/LangOne/           # 메인 프로젝트
│   ├── Types.fs           # Token, AST, Error 타입
│   ├── Result.fs          # ROP 헬퍼 함수
│   ├── Lexer.fs           # 토큰화
│   ├── Parser.fs          # 구문 분석 (Recursive Descent)
│   ├── Evaluator.fs       # AST 평가
│   ├── Interpreter.fs     # 통합 파이프라인
│   └── Program.fs         # REPL 진입점
├── tests/LangOne.Tests/   # 테스트 프로젝트
└── LangOne.sln            # 솔루션 파일
```

## 빌드 및 실행

```bash
# 빌드
dotnet build

# 테스트
dotnet test

# REPL 실행
dotnet run --project src/LangOne
```

## 아키텍처

```
Input: "1 + 2 * 3"
    │
    ▼
┌─────────┐
│  Lexer  │  문자열 → Token list
└─────────┘
    │
    ▼
┌─────────┐
│ Parser  │  Token list → AST
└─────────┘
    │
    ▼
┌──────────┐
│Evaluator │  AST → Result<float, Error>
└──────────┘
    │
    ▼
Output: Ok 7.0
```

## 핵심 타입

```fsharp
// Token
type Token = Number of float | Plus | Minus | Star | Slash | LParen | RParen | EOF

// AST
type Expr = Literal of float | Binary of Expr * BinaryOp * Expr
type BinaryOp = Add | Subtract | Multiply | Divide

// Errors
type LexError = UnexpectedCharacter of char * int | InvalidNumber of string * int
type ParseError = UnexpectedToken of Token * string | UnexpectedEndOfInput | MismatchedParenthesis
type EvalError = DivisionByZero
```

## 개발 규칙

1. **TDD**: 테스트 먼저 작성 (Red → Green → Refactor)
2. **ROP**: 모든 에러 처리는 Result 타입 사용, 예외 금지
3. **함수형**: 불변성 우선, 순수 함수 선호
4. **파이프라인**: `|>` 연산자 적극 활용

## Claude Code 워크플로우

```
/startsession → /nextstep → /tdd (반복) → /endstep → /endsession
```

## 참고 문서

- `.claude/PLAN.md` - 프로젝트 계획 및 진행 상태
- `.claude/STATE.md` - 현재 작업 상태
- `.claude/skills/fsharp.md` - F# 코딩 가이드라인
