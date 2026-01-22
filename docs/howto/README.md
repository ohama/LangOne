# How-To 문서 목록

F# 인터프리터 구현 튜토리얼 문서입니다.

## 문서 목록

| 문서 | 설명 | 관련 Step |
|------|------|-----------|
| [discriminated-unions](./discriminated-unions.md) | F# Discriminated Union으로 타입 정의하기 | Step 1.1 |
| [result-rop](./result-rop.md) | Result 타입과 ROP 패턴 | Step 1.1 |
| [lexer-basics](./lexer-basics.md) | 기본 Lexer 구현하기 | Step 1.2 |
| [lexer-numbers](./lexer-numbers.md) | Lexer에서 숫자 토큰화하기 | Step 1.3 |
| [lexer-expressions](./lexer-expressions.md) | 복합 표현식 Lexing과 Serilog 로깅 | Step 1.4 |
| [ast-types](./ast-types.md) | AST 타입과 재귀적 구조 정의 | Step 2.1 |

## 학습 순서

### Phase 1: Lexer (토큰화)

1. **discriminated-unions** - F# 타입 시스템 기초
2. **result-rop** - 에러 처리 패턴
3. **lexer-basics** - 문자열을 토큰으로 변환
4. **lexer-numbers** - 숫자 파싱
5. **lexer-expressions** - 복합 표현식과 로깅

### Phase 2: Parser (구문 분석)

6. **ast-types** - AST 타입 정의와 재귀적 구조
