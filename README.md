# Dreamine.Hybrid

[![CI](https://github.com/CodeMaru-Dreamine/Dreamine.Hybrid/actions/workflows/ci.yml/badge.svg)](https://github.com/CodeMaru-Dreamine/Dreamine.Hybrid/actions/workflows/ci.yml)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=CodeMaru-Dreamine_Dreamine.Hybrid&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CodeMaru-Dreamine_Dreamine.Hybrid)
[![Security](https://sonarcloud.io/api/project_badges/measure?project=CodeMaru-Dreamine_Dreamine.Hybrid&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=CodeMaru-Dreamine_Dreamine.Hybrid)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=CodeMaru-Dreamine_Dreamine.Hybrid&metric=coverage)](https://sonarcloud.io/summary/new_code?id=CodeMaru-Dreamine_Dreamine.Hybrid)
[![License](https://img.shields.io/github/license/CodeMaru-Dreamine/Dreamine.Hybrid)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8-512BD4)](https://dotnet.microsoft.com/)
[![NuGet](https://img.shields.io/nuget/v/Dreamine.Hybrid)](https://www.nuget.org/packages/Dreamine.Hybrid)
[![Downloads](https://img.shields.io/nuget/dt/Dreamine.Hybrid)](https://www.nuget.org/packages/Dreamine.Hybrid)
[![Docs](https://img.shields.io/badge/Docs-dreamine.kr-2496ED)](https://dreamine.kr)
[![Guide](https://img.shields.io/badge/Guide-dreamine.kr-2496ED)](https://dreamine.kr)
[![Playground](https://img.shields.io/badge/Playground-dreamine.kr-7B2CBF)](https://dreamine.kr)
[![Book](https://img.shields.io/badge/Book-Practical%20MVVM%20Architecture-111111)](https://dreamine.kr)

Core runtime and abstraction layer for Dreamine hybrid applications.

[➡️ 한국어 문서 보기](README_ko.md)

## Purpose

`Dreamine.Hybrid` contains platform-neutral contracts and in-memory implementations used to share messages and state between a host application and embedded UI layers.

It does not host WebView2 or Blazor directly. WPF-specific hosting is provided by `Dreamine.Hybrid.Wpf`.

## Main Types

- `IHybridMessage`
- `IHybridMessageBus`
- `IHybridStateStore`
- `HybridMessageBase`
- `InMemoryHybridMessageBus`
- `HybridStateStore`

App-specific messages should live in the application or sample project, not in this library package. Derive them from `HybridMessageBase` or implement `IHybridMessage`.

## Package Boundary

Use this package when you need shared hybrid contracts or an in-process message bus.

Use `Dreamine.Hybrid.Wpf` when you need a WPF `HybridHostControl` for BlazorWebView/WebView2 hosting.

## License

MIT License
