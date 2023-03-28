# Moedim.AspNetCore.Demo

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/kdcllc/Moedim.Microservices/master/LICENSE)
![master workflow](https://github.com/kdcllc/Moedim.Microservices/actions/workflows/master.yml/badge.svg)[![NuGet](https://img.shields.io/nuget/v/Moedim.Microservices.svg)](https://www.nuget.org/packages?q=Moedim.Microservices)
![Nuget](https://img.shields.io/nuget/dt/Moedim.Microservices)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https://f.feedz.io/kdcllc/moedim/shield/Moedim.Microservices/latest)](https://f.feedz.io/kdcllc/moedim/packages/Moedim.Microservices/latest/download)

> This is a Hebrew word that translates "feast" or "appointed time."
> "Appointed times" refers to HaSham's festivals in Vayikra/Leviticus 23rd.
> The feasts are "signals and signs" to help us know what is on the heart of HaShem.

_Note: Pre-release packages are distributed via [feedz.io](https://f.feedz.io/kdcllc/moedim/nuget/index.json)._

This goal of this repo is to provide with a reusable libraries for developing with DotNetCore platform for Microservices.

## Hire me

Please send [email](mailto:kingdavidconsulting@gmail.com) if you consider to **hire me**.

[![buymeacoffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/vyve0og)

## Give a Star! :star:

If you like or are using this project to learn or start your solution, please give it a star. Thanks!

## Configuration of the application

### Azure Key Vault

Enable Azure Key Vault to be used with Health Check enabled.

```json
  "Microservice": {

    "AzureVaultEnabled": true,

    "AzureVault": {
      "BaseUrl": "https://moedim.vault.azure.net/",
      "HealthCheckSecret": "Microservice--HealthCheckSecret"
    }
  }
```

Create a secret for azure vault health checks

```azurecli

    az keyvault secret set --vault-name "moedim" --name "Microservice--HealthCheckSecret" --value "HealthCheckSecret"
```

### Data protection wth Azure Blob Storage

```json
    "Microservice": {
        "DataProtection": {
          "AzureBlobStorageUrl": "https://moedim.blob.core.windows.net",
          "ContainerName": "prod-dataprotection-keys",
          "FileName": "moedimdemoappkey.xml"
        }
    }
```

Create Azure Blob Storage for Data protection file:

```azurecli
    az storage container create -n prod-dataprotection-keys --account-name moedim
```

Make sure that identity that accessing this Azure Blob Storage has `Storage Blob Data Contributor`.
