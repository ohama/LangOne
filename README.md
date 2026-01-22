# LangOne

F#으로 작성된 간단한 4칙 연산 표현식 인터프리터입니다.

## 기능

- 4칙 연산: `+`, `-`, `*`, `/`
- 연산자 우선순위: `*`, `/`가 `+`, `-`보다 먼저 계산
- 괄호 지원: `(1 + 2) * 3`
- 소수점 지원: `3.14 * 2`

## 설치

```bash
# 저장소 클론
git clone https://github.com/your-username/LangOne.git
cd LangOne

# 빌드
dotnet build
```

## 사용법

### REPL 모드

```bash
dotnet run --project src/LangOne
```

```
LangOne - Simple Expression Interpreter
Type an expression or 'exit' to quit

> 1 + 2
= 3

> 1 + 2 * 3
= 7

> (1 + 2) * 3
= 9

> 10 / 0
Eval error: DivisionByZero

> exit
Goodbye!
```

### 라이브러리로 사용

```fsharp
open LangOne.Interpreter

let result = interpret "1 + 2 * 3"
// Ok 7.0

let error = interpret "5 / 0"
// Error (EvaluatorError DivisionByZero)
```

## 아키텍처

```
"1 + 2 * 3"
     │
     ▼
┌─────────┐
│  Lexer  │  → [Number 1; Plus; Number 2; Star; Number 3; EOF]
└─────────┘
     │
     ▼
┌─────────┐
│ Parser  │  → Binary(Literal 1, Add, Binary(Literal 2, Multiply, Literal 3))
└─────────┘
     │
     ▼
┌──────────┐
│Evaluator │  → Ok 7.0
└──────────┘
```

## 테스트

```bash
# 모든 테스트 실행
dotnet test

# 특정 테스트 필터
dotnet run --project tests/LangOne.Tests -- --filter "Lexer"
```

## 기술 스택

- **F#** - 함수형 프로그래밍 언어
- **.NET 8** - 런타임
- **Expecto** - 테스트 프레임워크
- **FsCheck** - Property-based 테스트
- **Serilog** - 구조화된 로깅

## 프로젝트 구조

```
LangOne/
├── src/LangOne/
│   ├── Types.fs           # 토큰, AST, 에러 타입
│   ├── Lexer.fs           # 토큰화
│   ├── Parser.fs          # 구문 분석
│   ├── Evaluator.fs       # 평가
│   ├── Interpreter.fs     # 통합 파이프라인
│   └── Program.fs         # REPL
├── tests/LangOne.Tests/
│   ├── LexerTests.fs
│   ├── ParserTests.fs
│   ├── EvaluatorTests.fs
│   └── IntegrationTests.fs
└── LangOne.sln
```

## 라이선스

MIT License
