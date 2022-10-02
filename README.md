# My Wishly

An open-source wishlist app built on Azure.

## Features

* Add any items to your wishlist from any online store
* Hide items from public view (like a "draft")
* Reorder / reprioritize your list using drag & drop
* Vanity URLs (`/l/myName`)
* Purchase tracking (allows someone to mark an item as "bought")
  * Purchases are hidden from view and can only be accessed behind a warning screen, to avoid spoilers
* Reset accidental buys

## Technology

* Built on .NET 6 / ASP.NET Core.
* Uses the following Azure services:
  * Azure WebApps
  * Azure App Configuration
  * Azure Storage (Blobs, Tables)
  * SendGrid (SaaS via Azure Marketplace)
  * Azure DNS

## Building / Running

Built using Visual Studio 2022. Requires no other dependencies besides the .NET 6 SDK.

Add a User Secrets file to your project with the connection string to Azure App Configuration, like this:

```json
{
  "ConnectionStrings": {
    "AppConfig": "Endpoint=https://YOUR-APPCFG-URL-appconfig.azconfig.io;Id=XXXXXXXXXXXXXXXXX;Secret=YYYYYYYYYYYYYYYYYYYYYYYYY"
  }
}
```

All other settings are retrieved from Azure App Config at startup time.

## Contributing

Fork, make changes, submit a PR!