# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Architecture

This is a .NET MAUI cross-platform application for evaluating career stories using AI. The solution consists of three projects:

- **CareerStory** - Main MAUI application with UI (targets iOS, Android, macOS Catalyst, Windows)
- **StoryMaker** - Core business logic library containing story evaluation functionality  
- **StoryMakerTests** - Unit tests using xUnit and Moq

### Key Components

- **StoryEvaluator**: Uses OpenAI GPT-4o to evaluate career stories against STAR format rubric
- **ChatManager/OpenAIChatManager**: Abstracts OpenAI API interactions with dependency injection
- **StoryScore**: Domain model with comprehensive rubric scoring (Relevance, Ownership, Complexity, Influence, Outcome, Reflection)
- **JSON Schema Validation**: Enforces structured responses from AI model

### Dependencies

- Uses **LanguageExt** functional programming library for error handling and monadic operations
- **OpenAI SDK** for AI integration
- **Microsoft.Extensions** for dependency injection, configuration, and logging
- **System.Text.Json** with schema generation for structured AI responses

## Development Commands

### Build and Test
```bash
# Build entire solution
dotnet build CareerStory.sln

# Run tests
dotnet test StoryMakerTests/StoryMakerTests.csproj

# Build specific project
dotnet build CareerStory/CareerStory.csproj
dotnet build StoryMaker/StoryMaker.csproj
```

### MAUI Specific
```bash
# Run on specific platform (from CareerStory directory)
dotnet build -f net9.0-android
dotnet build -f net9.0-ios
dotnet build -f net9.0-maccatalyst
dotnet build -f net9.0-windows10.0.19041.0
```

## Configuration

Application uses `appsettings.json` with environment-specific overrides:
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides  
- `appsettings.Production.json` - Production overrides

OpenAI API key must be configured in application settings for story evaluation functionality.

## Functional Programming Patterns

The codebase leverages LanguageExt for functional programming:
- Uses `Optional` and `Try` monads for safe operations
- Immutable record types for domain models
- Function composition and error handling without exceptions

When modifying code, maintain functional programming patterns and immutable data structures.

## C# and .NET MAUI Best Practices

### C# Coding Standards (2024-2025)
- **Naming Conventions**: PascalCase for public members, camelCase for locals, _camelCase for private fields
- **Modern Language Features**: Use latest C# features, async/await for I/O operations, LINQ for collections
- **Exception Handling**: Catch specific exceptions only when they can be handled, avoid generic catches
- **Language Keywords**: Use `string` instead of `System.String`, `int` instead of `System.Int32`

### .NET MAUI Architecture Best Practices
- **MVVM Pattern**: Use Model-View-ViewModel with CommunityToolkit.MVVM for clean separation
- **Performance**: 
  - Use compiled bindings for 8-20x faster data binding
  - Implement async/await to keep UI responsive
  - Use virtualized lists for large collections
  - Enable linker optimizations
- **Scalability**: Apply dependency injection, clean architecture, and single responsibility principle
- **Anti-patterns to Avoid**:
  - God ViewModels (break into focused units)
  - Heavy code-behind (use data binding and commands)

### LanguageExt Functional Patterns
- **Core Types**: `Option<T>`, `Either<L,R>`, `Try<T>` for safe operations without nulls/exceptions
- **Immutable Collections**: `Lst<T>`, `Map<K,V>`, `Set<T>` for thread-safe data structures
- **Pure Functions**: Same inputs always produce same outputs, no side effects
- **Function Composition**: Chain operations using monadic patterns
- **Global Usings**: Include `LanguageExt`, `LanguageExt.Common`, and `static LanguageExt.Prelude`
- **Architecture**: Use "Functional Core, Imperative Shell" pattern for testable, pure business logic