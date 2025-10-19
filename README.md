# Text Filter Application

A C# .NET 8 console application that applies multiple configurable text filters to process text files.

## Assignment Requirements

**Text Filter Application** - A C# application with unit tests that applies multiple text filters to strings.

**Core Requirements:**
- ✅ Read from a txt file
- ✅ Apply 3 specific filters:
    - **Filter1**: Remove words with vowels in the middle (center 1-2 letters)
        - Examples: "clean" (middle: 'e') → filtered, "the" (middle: 'h') → kept
    - **Filter2**: Remove words with length less than 3
    - **Filter3**: Remove words containing the letter 't'
- ✅ Display filtered text in console
- ✅ Comprehensive unit tests
- ✅ Clean, extensible architecture
## Quick Start

### Prerequisites
- .NET 8 SDK
- Git (for cloning)

### Installation & Running

```bash
# Clone the repository
git clone [repository-url]
cd TextFilterConsoleApp

# Restore dependencies
dotnet restore

# Run the application
dotnet run --project TextFilterConsoleApp

# Run all tests
dotnet test
```

### Expected Output
```
==== FILTERED TEXT OUTPUT ====
beginning and once she reading, 'and use she considering own she and and picking daisies, remarkable she she all and and hurried flashed she never and burning she and large under hedge.In never once considering world she and dipped herself she herself falling she she she and wonder happen she and she she sides and filled and shelves; and she and She one shelves she passed; she killing one she
==============================
```

## Configuration

The application uses `appsettings.json` for configuration:

```json
{
  "TextFilters": {
    "VowelMiddleFilter": {
      "Enabled": true,
      "Description": "Filters out words with vowels in the middle (center 1-2 letters)"
    },
    "MinimumLengthFilter": {
      "Enabled": true,
      "MinimumLength": 3,
      "Description": "Filters out words with length less than specified minimum"
    },
    "ContainsLetterFilter": {
      "Enabled": true,
      "Letter": "t",
      "Description": "Filters out words containing the specified letter"
    }
  },
  "Processing": {
    "ChunkSize": 1000,
    "InputFilePath": "input.txt"
  }
}
```

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific test category
dotnet test --filter "VowelMiddleFilterTests"

# Run with detailed output
dotnet test --verbosity normal
```