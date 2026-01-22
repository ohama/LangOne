# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
