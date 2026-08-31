# Currency Converter

<sub>Documentation drafted with claude.ai</sub>

A WPF currency converter that pulls live rates from the Open Exchange Rates API
and deserialises them with Newtonsoft.Json.

## Setup

You need your own Open Exchange Rates app ID - get a free one at
https://openexchangerates.org. It is read from an environment variable rather
than being stored in the app, so set it before running:

    setx OPENEXCHANGERATES_APP_ID "your-app-id"

## Running it

This targets .NET Framework 4.7.2, so open `CurrencyConverter_Static.sln` in
Visual Studio and run it from there (or build with MSBuild). It is not a
`dotnet run` project.

## Notes

Rates are fetched once on load into a `Root`/`Rate` model covering the major
currencies (USD, EUR, GBP, ZAR, JPY, and others).

Currency records are also kept in a local SQL Server LocalDB database - a
single `Currency_Master` table, with add, edit and delete wired up from the UI.
The path in `App.config` points at the machine it was written on, so update
`AttachDbFilename` before running it elsewhere.
