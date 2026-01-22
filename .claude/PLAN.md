# PLAN

## 프로젝트 목표

F# 언어로 4칙 연산 표현식 인터프리터 구현
- "1 + 2 * 3" 같은 문자열을 파싱하여 계산
- 연산자 우선순위 및 괄호 지원
- TDD, ROP 패턴 적용

## 전체 진행률

| Phase | 상태 | 진행률 |
|-------|------|--------|
| Phase 0: 프로젝트 초기 설정 | **Completed** | 2/2 |
| Phase 1: Lexer (토큰화) | **Completed** | 4/4 |
| Phase 2: Parser (구문 분석) | Not Started | 0/5 |
| Phase 3: Evaluator (평가) | Not Started | 0/3 |
| Phase 4: 통합 및 마무리 | Not Started | 0/3 |

---

## 시스템 아키텍처

```
Input: "1 + 2 * (3 - 4)"
         │
         ▼
┌─────────────────┐
│     LEXER       │  문자열 → Token list
└─────────────────┘
         │
         ▼
┌─────────────────┐
│     PARSER      │  Token list → AST (Recursive Descent)
└─────────────────┘
         │
         ▼
┌─────────────────┐
│   EVALUATOR     │  AST → Result<float, Error>
└─────────────────┘
         │
         ▼
Output: Ok 14.0 | Error ...
```

---

## 프로젝트 디렉토리 구조

```
LangOne/
├── src/LangOne/
│   ├── LangOne.fsproj
│   ├── Types.fs           # 공통 타입 정의
│   ├── Result.fs          # ROP 헬퍼 함수
│   ├── Lexer.fs           # 토큰화
│   ├── Parser.fs          # 구문 분석
│   ├── Evaluator.fs       # 평가
│   ├── Interpreter.fs     # 통합 파이프라인
│   ├── Logging.fs         # Serilog 설정
│   └── Program.fs         # 진입점 (REPL)
├── tests/LangOne.Tests/
│   ├── LangOne.Tests.fsproj
│   ├── Program.fs         # 테스트 진입점
│   ├── LexerTests.fs
│   ├── ParserTests.fs
│   ├── EvaluatorTests.fs
│   └── IntegrationTests.fs
└── LangOne.sln
```

---

## Phase 0: 프로젝트 초기 설정

**목표**: F# 솔루션 및 프로젝트 구조 생성, 의존성 설치

**완료 조건**:
- [ ] 솔루션 및 프로젝트 생성
- [ ] 모든 의존성 설치 완료
- [ ] 기본 빌드 및 테스트 실행 성공

### Step 0.1: 솔루션 및 프로젝트 생성

**Goal**: .NET 솔루션과 F# 프로젝트 구조 생성

**Depends on**: 없음
**Blocks**: Step 0.2

**Tasks**:
- [x] `LangOne.sln` 솔루션 생성
- [x] `src/LangOne/LangOne.fsproj` 콘솔 프로젝트 생성
- [x] `tests/LangOne.Tests/LangOne.Tests.fsproj` 테스트 프로젝트 생성
- [x] 프로젝트 참조 연결

**DoD**:
- [x] `dotnet build` 성공

**Status**: **Completed**

---

### Step 0.2: 의존성 설치 및 기본 설정

**Goal**: Expecto, FsCheck, Serilog 패키지 설치

**Depends on**: Step 0.1
**Blocks**: Step 1.1

**Tasks**:
- [x] 테스트 프로젝트에 Expecto, FsCheck 설치
- [x] 메인 프로젝트에 Serilog 설치
- [x] 기본 테스트 파일 생성 및 실행 확인

**DoD**:
- [x] `dotnet build` 성공
- [x] `dotnet run --project tests/LangOne.Tests` 테스트 통과

**Status**: **Completed**

---

## Phase 1: Lexer (토큰화)

**목표**: 문자열을 토큰 리스트로 변환하는 Lexer 구현

**완료 조건**:
- [ ] 숫자 토큰 인식 (정수, 소수)
- [ ] 연산자 토큰 인식 (+, -, *, /)
- [ ] 괄호 토큰 인식
- [ ] 공백 처리
- [ ] 에러 처리 (Result 타입)

### Step 1.1: 기본 타입 정의

**Goal**: Token, Error 타입 및 ROP 헬퍼 함수 정의

**Depends on**: Step 0.2
**Blocks**: Step 1.2

**Tests**:
- [x] Test: Token equality works
- [x] Test: Result bind works
- [x] Test: Result bind short-circuits on Error

**DoD**:
- [x] 모든 타입 정의 완료
- [x] ROP 헬퍼 함수 테스트 통과

**Status**: **Completed**

---

### Step 1.2: 단일 토큰 Lexing

**Goal**: 단일 문자 토큰 (연산자, 괄호) 인식

**Depends on**: Step 1.1
**Blocks**: Step 1.3

**Tests**:
- [x] Test: tokenize plus (+)
- [x] Test: tokenize minus (-)
- [x] Test: tokenize star (*)
- [x] Test: tokenize slash (/)
- [x] Test: tokenize parentheses
- [x] Test: unknown character returns error

**DoD**:
- [x] 모든 연산자/괄호 토큰 테스트 통과

**Status**: **Completed**

---

### Step 1.3: 숫자 토큰 Lexing

**Goal**: 정수 및 소수점 숫자 인식

**Depends on**: Step 1.2
**Blocks**: Step 1.4

**Tests**:
- [x] Test: tokenize single digit
- [x] Test: tokenize multi-digit number
- [x] Test: tokenize decimal number

**DoD**:
- [x] 정수/소수 토큰화 테스트 통과

**Status**: **Completed**

---

### Step 1.4: 복합 표현식 Lexing

**Goal**: 여러 토큰으로 구성된 표현식 처리

**Depends on**: Step 1.3
**Blocks**: Step 2.1

**Tests**:
- [x] Test: tokenize "1 + 2"
- [x] Test: tokenize "1 + 2 * 3"
- [x] Test: tokenize "(1 + 2) * 3"
- [x] Test: tokenize without spaces

**DoD**:
- [x] 복합 표현식 테스트 통과
- [x] Serilog 로깅 동작 확인

**Status**: **Completed**

---

## Phase 2: Parser (구문 분석)

**목표**: 토큰 리스트를 AST로 변환하는 Parser 구현

**완료 조건**:
- [ ] 숫자 리터럴 파싱
- [ ] 이항 연산 파싱
- [ ] 연산자 우선순위 처리 (* / > + -)
- [ ] 괄호 처리
- [ ] 에러 처리 (Result 타입)

### Step 2.1: AST 타입 정의

**Goal**: Expression AST 타입 정의

**Depends on**: Step 1.4
**Blocks**: Step 2.2

**Tests**:
- [ ] Test: Literal creation
- [ ] Test: Binary expression creation

**DoD**:
- [ ] AST 타입 정의 완료
- [ ] ParseError 타입 정의 완료

**Status**: Not Started

---

### Step 2.2: 숫자 리터럴 파싱

**Goal**: 단일 숫자를 Literal AST로 변환

**Depends on**: Step 2.1
**Blocks**: Step 2.3

**Tests**:
- [ ] Test: parse single number
- [ ] Test: parse decimal number
- [ ] Test: parse empty input returns error

**DoD**:
- [ ] 숫자 리터럴 파싱 테스트 통과

**Status**: Not Started

---

### Step 2.3: 덧셈/뺄셈 파싱

**Goal**: + - 연산자 파싱 (좌결합)

**Depends on**: Step 2.2
**Blocks**: Step 2.4

**Tests**:
- [ ] Test: parse "1 + 2"
- [ ] Test: parse "5 - 3"
- [ ] Test: parse left-associative "1 + 2 + 3"

**DoD**:
- [ ] 덧셈/뺄셈 파싱 테스트 통과
- [ ] 좌결합 테스트 통과

**Status**: Not Started

---

### Step 2.4: 곱셈/나눗셈 파싱

**Goal**: * / 연산자 파싱 (우선순위 처리)

**Depends on**: Step 2.3
**Blocks**: Step 2.5

**Tests**:
- [ ] Test: parse "2 * 3"
- [ ] Test: parse "6 / 2"
- [ ] Test: precedence "1 + 2 * 3" = 1 + (2 * 3)

**DoD**:
- [ ] 곱셈/나눗셈 파싱 테스트 통과
- [ ] 우선순위 테스트 통과

**Status**: Not Started

---

### Step 2.5: 괄호 파싱

**Goal**: 괄호로 우선순위 재정의

**Depends on**: Step 2.4
**Blocks**: Step 3.1

**Tests**:
- [ ] Test: parse "(1 + 2)"
- [ ] Test: parse "(1 + 2) * 3" overrides precedence
- [ ] Test: nested parentheses
- [ ] Test: mismatched parentheses error

**DoD**:
- [ ] 괄호 파싱 테스트 통과
- [ ] 에러 처리 테스트 통과

**Status**: Not Started

---

## Phase 3: Evaluator (평가)

**목표**: AST를 순회하여 결과값 계산

**완료 조건**:
- [ ] 리터럴 평가
- [ ] 이항 연산 평가
- [ ] 0으로 나누기 에러 처리

### Step 3.1: 리터럴 및 기본 연산 평가

**Goal**: Literal과 Add, Subtract 평가

**Depends on**: Step 2.5
**Blocks**: Step 3.2

**Tests**:
- [ ] Test: evaluate Literal 42 = 42
- [ ] Test: evaluate 1 + 2 = 3
- [ ] Test: evaluate 5 - 3 = 2

**DoD**:
- [ ] 리터럴/덧셈/뺄셈 평가 테스트 통과

**Status**: Not Started

---

### Step 3.2: 곱셈/나눗셈 및 에러 처리

**Goal**: Multiply, Divide 평가 및 DivisionByZero 에러

**Depends on**: Step 3.1
**Blocks**: Step 3.3

**Tests**:
- [ ] Test: evaluate 2 * 3 = 6
- [ ] Test: evaluate 6 / 2 = 3
- [ ] Test: evaluate 5 / 0 = Error DivisionByZero

**DoD**:
- [ ] 곱셈/나눗셈 평가 테스트 통과
- [ ] 0으로 나누기 에러 처리 통과

**Status**: Not Started

---

### Step 3.3: Property Tests

**Goal**: FsCheck 속성 테스트

**Depends on**: Step 3.2
**Blocks**: Step 4.1

**Tests**:
- [ ] Test: literal evaluates to itself
- [ ] Test: addition is commutative
- [ ] Test: multiplication is commutative
- [ ] Test: a + 0 = a
- [ ] Test: a * 1 = a

**DoD**:
- [ ] 모든 Property 테스트 통과

**Status**: Not Started

---

## Phase 4: 통합 및 마무리

**목표**: 전체 파이프라인 통합 및 REPL 구현

**완료 조건**:
- [ ] Interpreter 모듈 완성
- [ ] E2E 테스트 통과
- [ ] REPL 동작

### Step 4.1: Interpreter 통합 모듈

**Goal**: Lexer → Parser → Evaluator ROP 파이프라인

**Depends on**: Step 3.3
**Blocks**: Step 4.2

**Tests**:
- [ ] Test: interpret "1 + 2" = Ok 3
- [ ] Test: interpret "1 + 2 * 3" = Ok 7
- [ ] Test: interpret "(1 + 2) * 3" = Ok 9
- [ ] Test: interpret "5 / 0" = Error DivisionByZero

**DoD**:
- [ ] 모든 통합 테스트 통과

**Status**: Not Started

---

### Step 4.2: E2E Property Tests

**Goal**: 전체 파이프라인 FsCheck 테스트

**Depends on**: Step 4.1
**Blocks**: Step 4.3

**Tests**:
- [ ] Test: number string interprets correctly
- [ ] Test: a + b computes correctly
- [ ] Test: spaces don't affect result

**DoD**:
- [ ] E2E Property 테스트 통과

**Status**: Not Started

---

### Step 4.3: REPL 및 문서화

**Goal**: REPL 구현 및 문서 완성

**Depends on**: Step 4.2
**Blocks**: 없음

**Tasks**:
- [ ] REPL 구현
- [ ] README.md 업데이트

**DoD**:
- [ ] REPL 동작 확인
- [ ] 모든 테스트 통과
- [ ] 빌드 성공

**Status**: Not Started

---

## 의존성 그래프

```
Step 0.1 → Step 0.2
              │
              ▼
Step 1.1 → Step 1.2 → Step 1.3 → Step 1.4
                                    │
                                    ▼
Step 2.1 → Step 2.2 → Step 2.3 → Step 2.4 → Step 2.5
                                               │
                                               ▼
                      Step 3.1 → Step 3.2 → Step 3.3
                                               │
                                               ▼
                      Step 4.1 → Step 4.2 → Step 4.3
```

---

## 핵심 데이터 타입

```fsharp
// Token
type Token =
    | Number of float
    | Plus | Minus | Star | Slash
    | LParen | RParen | EOF

// AST
type Expr =
    | Literal of float
    | Binary of left: Expr * operator: BinaryOp * right: Expr

and BinaryOp = Add | Subtract | Multiply | Divide

// Errors
type LexError = UnexpectedCharacter of char * int | InvalidNumber of string * int
type ParseError = UnexpectedToken of Token * string | UnexpectedEndOfInput | MismatchedParenthesis
type EvalError = DivisionByZero
type InterpreterError = LexerError of LexError | ParserError of ParseError | EvaluatorError of EvalError
```

---

## 빌드 및 실행 명령어

```bash
# 빌드
dotnet build

# 테스트
dotnet test

# REPL 실행
dotnet run --project src/LangOne
```
