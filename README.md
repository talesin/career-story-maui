# Career Story Evaluator

A cross-platform .NET MAUI application that uses AI to evaluate and score career stories using the STAR format rubric. This application helps professionals improve their interview stories by providing detailed feedback on structure, content, and impact.

## 🎯 What It Does

The Career Story Evaluator analyzes career stories and provides comprehensive scoring across six key dimensions:

- **Relevance**: Context clarity and relevance to the target role
- **Ownership**: Clear definition of individual goals and responsibilities  
- **Complexity**: Action specificity and complexity of challenges faced
- **Influence**: Ability to influence others and navigate ambiguous situations
- **Outcome**: Measurable, observable results tied to actions taken
- **Reflection**: Learning and growth from the experience

The application uses OpenAI's GPT-4 model to evaluate stories against the STAR (Situation, Task, Action, Result) format, providing:
- Detailed scores (1-5) for each dimension with explanations
- Overall percentage score out of 100
- Specific areas for improvement recommendations
- Professional-grade evaluation suitable for engineering leadership roles

## 🚀 Features

### Core Functionality
- **Story Input**: Clean, intuitive text editor for entering career stories
- **AI-Powered Evaluation**: Uses OpenAI GPT-4 for comprehensive story analysis
- **Detailed Scoring**: Six-dimensional rubric with explanations for each score
- **Improvement Recommendations**: Actionable feedback for story enhancement
- **Real-time Validation**: Input validation with helpful error messages

### User Experience
- **Cross-Platform**: Runs on Android, iOS, macOS, and Windows
- **Modern MVVM Architecture**: Clean separation of concerns with reactive UI
- **Loading Indicators**: Visual feedback during AI processing
- **Error Handling**: Graceful error management with user-friendly messages
- **Responsive Design**: Optimized for various screen sizes and orientations

## 🏗️ Technical Architecture

### Project Structure
```
CareerStory.sln
├── CareerStory/              # Main MAUI application
│   ├── ViewModels/          # MVVM ViewModels with CommunityToolkit.MVVM
│   ├── Services/            # Navigation and application services
│   ├── Views/               # XAML pages and user interface
│   └── Resources/           # Fonts, images, styles, and assets
├── StoryMaker/              # Core business logic library
│   ├── StoryScore.cs        # Domain models and evaluation logic
│   ├── ChatManager.cs       # OpenAI integration layer
│   └── Global.cs            # LanguageExt global imports
└── StoryMakerTests/         # Comprehensive unit test suite
```

### Technology Stack

**Frontend Framework**
- **.NET MAUI 9.0**: Microsoft's cross-platform UI framework
- **C# 12**: Modern language features with nullable reference types
- **XAML**: Declarative UI markup with data binding

**Architecture Patterns**
- **MVVM with CommunityToolkit.MVVM**: Modern MVVM implementation
- **Functional Programming**: LanguageExt for functional patterns
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **Clean Architecture**: Separation of concerns across layers

**AI Integration**
- **OpenAI SDK**: Official .NET SDK for GPT-4 integration
- **Structured JSON Responses**: Schema-validated AI output
- **Async/Await Patterns**: Non-blocking UI operations

**Quality Assurance**
- **xUnit**: Unit testing framework with comprehensive coverage
- **Moq**: Mocking framework for isolated testing
- **LanguageExt**: Functional programming for better error handling

## 📋 Prerequisites

### Development Environment
- **.NET 9.0 SDK** or later
- **Visual Studio 2022** (Windows/Mac) or **Visual Studio Code**
- **MAUI Workload**: `dotnet workload install maui`

### Platform-Specific Requirements
- **Windows**: Windows 10/11 for Windows development
- **macOS**: macOS 10.15+ for iOS/macOS development  
- **Xcode**: Required for iOS/macOS development
- **Android SDK**: Required for Android development

### API Access
- **OpenAI API Key**: Required for story evaluation functionality
  - Sign up at [OpenAI Platform](https://platform.openai.com/)
  - Generate an API key from the dashboard

## ⚙️ Setup and Configuration

### 1. Clone the Repository
```bash
git clone <repository-url>
cd career-story-maui
```

### 2. Install Dependencies
```bash
dotnet restore
```

### 3. Configure OpenAI API Key

Create or update the configuration files:

**For Development:**
```json
// CareerStory/appSettings.Development.json
{
  "openai_api_key": "your-openai-api-key-here"
}
```

**For Production:**
```json
// CareerStory/appSettings.Production.json  
{
  "openai_api_key": "your-production-api-key-here"
}
```

### 4. Build the Solution
```bash
dotnet build CareerStory.sln
```

### 5. Run Tests
```bash
dotnet test StoryMakerTests/StoryMakerTests.csproj
```

## 🚀 Running the Application

### Development
```bash
# Run on specific platform
dotnet build -f net9.0-android
dotnet build -f net9.0-ios  
dotnet build -f net9.0-maccatalyst
dotnet build -f net9.0-windows10.0.19041.0
```

### Using Visual Studio
1. Open `CareerStory.sln` in Visual Studio
2. Select your target platform (Android, iOS, Windows, etc.)
3. Press F5 to build and run

## 🎮 How to Use

1. **Launch the App**: Start the Career Story Evaluator
2. **Enter Your Story**: Type or paste your career story in the text editor
   - Minimum 50 characters required
   - Maximum 5000 characters supported
3. **Evaluate**: Click the "Score" button to analyze your story
4. **Review Results**: View detailed scoring in a popup with:
   - Individual dimension scores and explanations
   - Overall percentage score
   - Specific recommendations for improvement
5. **Iterate**: Use the feedback to refine your story and re-evaluate

### Story Writing Tips
The application evaluates stories based on the STAR format:
- **Situation**: Set the context and background
- **Task**: Describe your responsibility or goal
- **Action**: Explain the specific steps you took
- **Result**: Share the measurable outcomes

## 🧪 Testing

The application includes a comprehensive test suite covering:

### Unit Tests
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Coverage
- **Domain Models**: StoryScore, criteria, and validation logic
- **Business Logic**: Story evaluation and parsing functionality  
- **API Integration**: Mocked OpenAI service interactions
- **Error Handling**: Edge cases and failure scenarios

## 🔧 Development Commands

### Building
```bash
# Build entire solution
dotnet build CareerStory.sln

# Build specific project
dotnet build CareerStory/CareerStory.csproj
dotnet build StoryMaker/StoryMaker.csproj
```

### Testing  
```bash
# Run all tests
dotnet test StoryMakerTests/StoryMakerTests.csproj

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"
```

### Platform-Specific Development
```bash
# Android
dotnet build -f net9.0-android

# iOS (macOS only)  
dotnet build -f net9.0-ios

# macOS (macOS only)
dotnet build -f net9.0-maccatalyst

# Windows
dotnet build -f net9.0-windows10.0.19041.0
```

## 📁 Configuration Files

### Application Settings
- `appSettings.json`: Base configuration
- `appSettings.Development.json`: Development overrides
- `appSettings.Production.json`: Production overrides

### Key Configuration
```json
{
  "openai_api_key": "your-api-key-here"
}
```

## 🏛️ Architecture Highlights

### Functional Programming
The application leverages LanguageExt for functional programming patterns:
- **Option<T>**: Safe handling of nullable values
- **Try<T>**: Exception-safe operations  
- **Either<L,R>**: Validation and error handling
- **Immutable Data Structures**: Thread-safe domain models

### MVVM Implementation
Modern MVVM with CommunityToolkit.MVVM:
- **ObservableProperty**: Automatic property change notifications
- **RelayCommand**: Command pattern for UI interactions
- **Data Binding**: Reactive UI updates
- **Separation of Concerns**: Clean architecture layers

## 🐛 Troubleshooting

### Common Issues

**API Key Not Configured**
```
Error: OpenAI API key is not configured in the app settings.
```
Solution: Ensure your API key is properly set in `appSettings.Development.json`

**Build Errors on macOS/iOS**
```
Error: Xcode command line tools not found
```
Solution: Install Xcode and accept the license agreement

**Android Build Issues**
```
Error: Java SDK not found
```
Solution: Install Java 17 and set JAVA_HOME environment variable

### Debug Mode
Enable detailed logging in `appSettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "StoryMaker": "Trace"
    }
  }
}
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Run tests to ensure quality (`dotnet test`)
4. Commit your changes (`git commit -m 'Add amazing feature'`)
5. Push to the branch (`git push origin feature/amazing-feature`)
6. Open a Pull Request

### Code Standards
- Follow C# coding conventions and naming standards
- Maintain functional programming patterns with LanguageExt
- Ensure comprehensive test coverage for new features
- Use MVVM patterns for UI components

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙋‍♂️ Support

For questions, issues, or contributions:
- Open an issue on GitHub
- Review the troubleshooting section
- Check the CLAUDE.md file for development guidance

---

Built with ❤️ using .NET MAUI, LanguageExt, and OpenAI GPT-4