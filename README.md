# Key Vault Explorer

<img src="KeyVaultExplorer\Assets\kv-icon.png" width="300" >

## Overview

**Key Vault Explorer** is a lightweight tool with the idea to simplify finding and accessing secrets (and certitficates and keys) stored in Azure Key Vault, providing a interface for aggregating, filtering, and quickly getting secret values. The app was inspired by the original [AzureKeyVaultExplorer](https://github.com/microsoft/AzureKeyVaultExplorer) with the goal to eventually bring some more feature parity but first brining the application to macOS.

### Key features

- Signing in with a Microsoft Account [See how credentials are secured](#security)
- Support to selectively include/exclude subscriptions to show resource groups and key vaults in the tree
- Ability to filter subscriptions, resrouce groups, and key vaults by name
- Saving vaults to "pinned" section in quick access menu and saving selected subscriptions in SQLite
- Copy secrets to the clipboard using Control+C
- Automatic clearing of clipboard values after a set amount of time (configurable up to 60 seconds)
- Viewing details and tags about values
- Filtering and sorting of values
- Viewing last updates and next to expire values
- Downloading and saving .pfx and .cer files
- **No telemetry or logs collected**

# Security

The authentication and credentials storage uses [Microsoft.Identity.Client.Extensions.Msal](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) library are encrypted and stored with DPAPI on windows, and the keychain on macOS (you may be prompted multiple times to grant rights).
The security is pulled directly from this document: https://github.com/AzureAD/microsoft-authentication-extensions-for-dotnet/wiki/Cross-platform-Token-Cache#configuring-the-token-cache

## Screenshots
<img width="1419" alt="Dark" src="https://github.com/cricketthomas/KeyVaultExplorer/assets/15821271/365cea71-2a68-4cab-997c-2631922e7bc6">
<img width="1426" alt="Light" src="https://github.com/cricketthomas/KeyVaultExplorer/assets/15821271/41793cfa-eb01-4954-b062-56072d19d5ea">


## Running the application:

This should be a simple clone, and set the start up project to be "KeyVaultExplorer.Desktop".
The code is very much in the learning phase of things at the moment, with lots of small commits of stepping stones to learning AvaloniaUI, and navigating MVVM.

## Building from source

- ## WindowsOS

  Download from the Microsoft Store:
  
  Run the following scripts check the publish directory for a folder.
  run `.\KeyVaultExplorer\build.ps1 -RunBuild -Platform net8.0 -Runtime win-x64`
  run `.\KeyVaultExplorer\build.ps1 -RunBuild -Platform net8.0 -Runtime win-arm64`

- ## macOS
  Download from the release section:
  
  Run the following scripts and a 'Key Vault Explorer.app' mac os package will be generated in the publish directory. Move this to "Applications".
  run `.\KeyVaultExplorer\build.ps1 -RunBuild -Platform net8.0 -Runtime osx-x64`
  run `.\KeyVaultExplorer\build.ps1 -RunBuild -Platform net8.0 -Runtime osx-arm64`

### Dependencies

- **[AvaloniaUI](https://github.com/AvaloniaUI/Avalonia/)** (Version: 11.0.10-preview2)
- **[FluentAvaloniaUI](https://github.com/amwx/FluentAvalonia/)** (Version: 2.1.0-preview4)
- **Azure.ResourceManager.KeyVault**
- **Azure.Security.KeyVault.Certificates**
- **Azure.Security.KeyVault.Keys**
- **Azure.Security.KeyVault.Secrets**
- **CommunityToolkit.Mvvm**
- **Microsoft.Data.Sqlite**
- **Microsoft.Extensions.Caching.Memory**
- **Microsoft.Identity.Client.Extensions.Msal**
- **Microsoft.Extensions.DependencyInjection**
