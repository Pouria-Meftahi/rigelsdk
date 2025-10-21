# RigelSdk

[![Build Status](https://github.com/Pouria-Meftahi/rigelsdk/workflows/CI/badge.svg)](https://github.com/Pouria-Meftahi/rigelsdk/actions)
[![Test Status](https://github.com/Pouria-Meftahi/rigelsdk/workflows/Tests/badge.svg)](https://github.com/Pouria-Meftahi/rigelsdk/actions)
[![codecov](https://codecov.io/gh/Pouria-Meftahi/rigelsdk/branch/main/graph/badge.svg)](https://codecov.io/gh/Pouria-Meftahi/rigelsdk)
[![NuGet](https://img.shields.io/nuget/v/RigelSdk.svg)](https://www.nuget.org/packages/RigelSdk/)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

C# SDK for [Rigel](https://github.com/sdqri/rigel) - A small & fast image proxy server written in Go!

## 📦 Installation

### Via NuGet Package Manager
```bash
dotnet add package RigelSdk
```

### Via Package Manager Console
```powershell
Install-Package RigelSdk
```

### Via .csproj
```xml
<PackageReference Include="RigelSdk" Version="1.0.0" />
```

## 🚀 Quick Start

```csharp
using RigelSdk.Sdk;
using RigelSdk.Models;
using RigelSdk.Constants;

// Initialize SDK
const string KEY = "secretkey";
const string SALT = "secretsalt";
const string BASE_URL = "http://localhost:8080/rigel";

var sdk = new RigelSdk(BASE_URL, KEY, SALT);
```

## 📖 Usage Examples

### Proxy Image
Generate a proxy URL for an image with optional transformations:

```csharp
var proxyUrl = await sdk.ProxyImageAsync(
    "https://example.com/image.jpg",
    new Options 
    { 
        Width = 100, 
        Height = 100, 
        Type = ImageType.WEBP 
    },
    DateTimeOffset.Now.AddDays(1).ToUnixTimeMilliseconds()
);

Console.WriteLine($"Proxy URL: {proxyUrl}");
```

### Cache Image
Cache an image and get a short URL:

```csharp
var result = await sdk.CacheImageAsync(
    "https://example.com/image.jpg",
    new Options 
    { 
        Width = 300, 
        Height = 300, 
        Type = ImageType.WEBP,
        Quality = 90
    },
    DateTimeOffset.Now.AddDays(7).ToUnixTimeMilliseconds()
);

if (result is string shortUrl)
{
    Console.WriteLine($"Short URL: {shortUrl}");
}
else if (result is int statusCode)
{
    Console.WriteLine($"Failed with status: {statusCode}");
}
```

### Try Short URL
Attempts to cache, falls back to proxy if caching fails:

```csharp
var url = await sdk.TryShortUrlAsync(
    "https://example.com/image.jpg",
    new Options 
    { 
        Width = 100, 
        Height = 100, 
        Type = ImageType.WEBP 
    },
    DateTimeOffset.Now.AddDays(1).ToUnixTimeMilliseconds()
);

Console.WriteLine($"URL: {url}");
```

### Batched Cache
Cache multiple images in a single request:

```csharp
var batchParams = new List<ProxyParams>
{
    new ProxyParams(
        "https://example.com/image1.jpg", 
        new Options { Width = 100, Height = 100, Type = ImageType.WEBP }
    ),
    new ProxyParams(
        "https://example.com/image2.jpg", 
        new Options { Width = 200, Height = 200, Type = ImageType.PNG }
    ),
    new ProxyParams(
        "https://example.com/image3.jpg", 
        new Options { Width = 150, Height = 150, Type = ImageType.JPEG }
    )
};

var results = await sdk.BatchedCacheImageAsync(batchParams, -1);

foreach (var result in results)
{
    Console.WriteLine($"Image: {result.Img}");
    Console.WriteLine($"Short URL: {result.ShortUrl}");
    Console.WriteLine($"Signature: {result.Signature}");
    Console.WriteLine("---");
}
```

## ⚙️ Available Options

The `Options` class supports all Rigel image transformation parameters:

```csharp
var options = new Options
{
    // Dimensions
    Width = 800,
    Height = 600,
    AreaWidth = 1024,
    AreaHeight = 768,
    
    // Position
    Top = 10,
    Left = 20,
    Gravity = Gravity.Centre,
    
    // Quality
    Quality = 85,
    Compression = 6,
    
    // Transformations
    Crop = true,
    Enlarge = false,
    Embed = false,
    Flip = false,
    Flop = false,
    Rotate = "90",
    Zoom = 2,
    
    // Effects
    GaussianBlur = "5",
    Sharpen = "1.5",
    Brightness = 1.2f,
    Contrast = 0.8f,
    Gamma = 2.2f,
    
    // Format
    Type = ImageType.WEBP,
    Lossless = false,
    Interlace = true,
    
    // Metadata
    StripMetadata = true,
    NoAutoRotate = false,
    NoProfile = true,
    
    // Watermark
    Watermark = "Copyright 2024",
    WatermarkImage = "https://example.com/watermark.png",
    
    // Advanced
    Background = "#FFFFFF",
    Extend = "white",
    Interpolator = "bicubic",
    Palette = false
};
```

### Image Types

```csharp
public enum ImageType
{
    JPEG = 1,
    WEBP = 2,
    PNG = 3,
    TIFF = 4,
    GIF = 5,
    PDF = 6,
    SVG = 7,
    MAGICK = 8,
    HEIF = 9,
    AVIF = 10
}
```

### Gravity Options

```csharp
public enum Gravity
{
    Centre = 0,
    North = 1,
    East = 2,
    South = 3,
    West = 4,
    Smart = 5
}
```

## 🧪 Testing

### Running Tests

The SDK includes comprehensive unit and integration tests.

```bash
# Run all tests
dotnet test

# Run only unit tests
dotnet test --filter "Category!=Integration"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverageReportFormat=opencover
```

### Test Structure

```
RigelSdk.Tests/
├── Unit/
│   ├── SdkTests.cs           # SDK method tests
│   ├── UtilsTests.cs         # Utility function tests
│   └── ModelsTests.cs        # Model serialization tests
├── Integration/
│   └── SdkIntegrationTests.cs # End-to-end tests
└── Helpers/
    ├── TestConstants.cs       # Test configuration
    └── MockHttpMessageHandler.cs # HTTP mocking
```

### Writing Tests

Example test:

```csharp
[Fact]
public async Task ProxyImageAsync_WithOptions_ReturnsCorrectUrl()
{
    // Arrange
    var sdk = new RigelSdk("http://localhost:8080/rigel", "key", "salt");
    var options = new Options { Width = 100, Height = 100 };

    // Act
    var result = await sdk.ProxyImageAsync("https://example.com/image.jpg", options, -1);

    // Assert
    result.Should().Contain("width=100");
    result.Should().Contain("height=100");
}
```

### Test Coverage

The test suite covers:
- ✅ All SDK methods (ProxyImage, CacheImage, BatchedCacheImage, TryShortURL)
- ✅ Signature generation and validation
- ✅ Query string serialization
- ✅ Error handling and edge cases
- ✅ HTTP client mocking
- ✅ Model serialization/deserialization
- ✅ Integration scenarios

Current coverage: **>90%**

## 🔧 Development

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 / VS Code / Rider

### Building from Source

```bash
# Clone the repository
git clone https://github.com/Pouria-Meftahi/rigelsdk.git
cd rigelsdk

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Pack NuGet package
dotnet pack -c Release
```

### Project Structure

```
rigelsdk/
├── src/
│   ├── Sdk/
│   │   └── RigelSdk.cs
│   ├── Models/
│   │   ├── Options.cs
│   │   ├── ProxyParams.cs
│   │   └── CacheImageResponse.cs
│   ├── Utils/
│   │   ├── SignatureUtils.cs
│   │   └── QueryStringUtils.cs
│   ├── Constants/
│   │   ├── ImageType.cs
│   │   └── Gravity.cs
│   └── Interfaces/
│       └── IRigelSdk.cs
├── tests/
│   └── RigelSdk.Tests/
└── rigelsdk.sln
```

## 🔐 Security

### Signature Generation

All requests are signed using HMAC-SHA1:

```csharp
var signature = SignatureUtils.Sign(key, salt, signableString);
```

### Best Practices

- ⚠️ Never commit your API keys
- ✅ Use environment variables for configuration
- ✅ Set appropriate expiry times
- ✅ Validate image URLs before processing

## 🚀 CI/CD

This project uses GitHub Actions for continuous integration and deployment:

- **Build & Test**: Runs on every push and PR
- **Code Coverage**: Automatic coverage reporting
- **NuGet Publish**: Automatic package publishing on release

See [`.github/workflows/`](.github/workflows/) for configuration details.

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines

- Follow C# coding conventions
- Write unit tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting PR
- Maintain code coverage above 90%

## 📄 License

This SDK is distributed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0). See [LICENSE](LICENSE) for more information.

## 🙏 Acknowledgments

- [Rigel](https://github.com/sdqri/rigel) - The amazing image proxy server
- [sdqri/rigelsdk](https://github.com/sdqri/rigelsdk) - Original TypeScript SDK

## 📞 Support

- 🐛 [Report Bug](https://github.com/Pouria-Meftahi/rigelsdk/issues)
- 💡 [Request Feature](https://github.com/Pouria-Meftahi/rigelsdk/issues)
- 📧 [Contact Author](mailto:your-email@example.com)

## 📊 Stats

![GitHub stars](https://img.shields.io/github/stars/Pouria-Meftahi/rigelsdk?style=social)
![GitHub forks](https://img.shields.io/github/forks/Pouria-Meftahi/rigelsdk?style=social)
![GitHub issues](https://img.shields.io/github/issues/Pouria-Meftahi/rigelsdk)
![GitHub pull requests](https://img.shields.io/github/issues-pr/Pouria-Meftahi/rigelsdk)

---

Made with ❤️ by [Pouria Meftahi](https://github.com/Pouria-Meftahi)