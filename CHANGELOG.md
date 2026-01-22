# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.3.0] - 2026-01-22

### Added
- **Phase 1: Lexer (토큰화) 완료**
  - Token, LexError 타입 정의 (Types.fs)
  - Result ROP 헬퍼 함수 (Result.fs)
  - 단일 문자 토큰 인식 (+, -, *, /, (, ))
  - 숫자 토큰 인식 (정수, 소수점)
  - 복합 표현식 lexing
  - Serilog 로깅 통합 (Logging.fs)
- **Phase 2: Parser (구문 분석) 완료**
  - AST 타입 정의 (Expr, BinaryOp)
  - Recursive Descent Parser 구현 (Parser.fs)
  - 숫자 리터럴 파싱
  - 덧셈/뺄셈 파싱 (좌결합)
  - 곱셈/나눗셈 파싱 (연산자 우선순위)
  - 괄호 파싱 (우선순위 재정의)
  - ParseError 에러 처리
- **How-To 튜토리얼 문서**
  - discriminated-unions.md - F# DU 타입 정의
  - result-rop.md - Result 타입과 ROP 패턴
  - lexer-basics.md - 기본 Lexer 구현
  - lexer-numbers.md - 숫자 토큰화
  - lexer-expressions.md - 복합 표현식과 로깅
  - ast-types.md - AST 타입 정의
  - parser-literals.md - Parser 기초
  - parser-precedence.md - 연산자 우선순위와 괄호

### Changed
- 47 tests passing (Expecto)

## [0.2.0] - 2026-01-22

### Added
- F# solution and project structure (Phase 0 completed)
  - `LangOne.sln` solution file
  - `src/LangOne/` main console project
  - `tests/LangOne.Tests/` test project
- Test framework setup
  - Expecto 10.2.3
  - Expecto.FsCheck 10.2.3
  - FsCheck 2.16.6
- Logging setup
  - Serilog 4.3.0
  - Serilog.Sinks.Console 6.1.1
  - Destructurama.FSharp 4.0.0
- Basic smoke tests with Expecto
- .gitignore for .NET projects

## [0.1.0] - 2026-01-22

### Added
- Project plan for F# arithmetic expression interpreter (PLAN.md)
  - 5 Phases, 17 Steps: Lexer → Parser → Evaluator pipeline
  - TDD workflow with Expecto + FsCheck
  - ROP (Railway Oriented Programming) pattern
- CLAUDE.md for Claude Code project context
- Session logging system (.claude/logs/)
- Claude Code workflow commands (startsession, nextstep, tdd, etc.)

### Changed
- README.md rewritten for LangOne project (previously template documentation)

### Removed
- README.ko.md (consolidated into single README)
