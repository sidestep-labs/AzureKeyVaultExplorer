# Azure Key Vault Explorer
# This is very much a work in progress.

## Overview
The **Key Vault Explorer** is a lightweight tool is designed to simplify the process of accessing secrets (and certitficates and keys) stored in Azure Key Vault, providing a user-friendly interface for aggregating, copying, and quickly viewing secret values.



## Screenshots
<img src="https://github.com/cricketthomas/kvexplorer/assets/15821271/80abd5d5-a399-4d05-bb29-07fd765af805" width="1000">
<img src="https://github.com/cricketthomas/kvexplorer/assets/15821271/29addc92-fc44-4981-93ab-e1eb0a2ca7de" width="800">
<img src="https://github.com/cricketthomas/kvexplorer/assets/15821271/01b3b175-70d8-4ec3-bdbb-49069350efd1" width="800">

## Windows only:

![image](https://github.com/cricketthomas/kvexplorer/assets/15821271/14fc932e-650c-4ab6-9f4e-1f10ae18e94c)


## Running the application:
This should be a simple clone, and set the start up project to be "kvexplorer.Desktop".
The code is very much in the learning phase of things at the moment, with lots of small commits of stepping stones to learning AvaloniaUI, and navigating MVVM.


### Dependencies
- **Azure.ResourceManager.KeyVault** 
- **Azure.Security.KeyVault.Certificates** 
- **Azure.Security.KeyVault.Keys** 
- **Azure.Security.KeyVault.Secrets** 
- **CommunityToolkit.Mvvm** 
- **Microsoft.Data.Sqlite**
- **Microsoft.Extensions.Caching.Memory** 
- **Microsoft.Identity.Client.Extensions.Msal**
- **FluentAvaloniaUI** (Version: 2.0.4)
- **Avalonia.Diagnostics** (Version: 11.0.6) 
- **Microsoft.Extensions.DependencyInjection** 
