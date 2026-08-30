# Currency Converter

<sub>Documentation drafted with claude.ai</sub>

A WPF currency converter that pulls live rates from the Open Exchange Rates API
and deserialises them with Newtonsoft.Json.

## Setup

You need your own Open Exchange Rates app ID - get a free one at
https://openexchangerates.org. The request is built in `MainWindow.xaml.cs`;
replace the `app_id` value with yours before running.

    dotnet run

Requires .NET on Windows.

## Notes

Rates are fetched once on load into a `Root`/`Rate` model covering the major
currencies (USD, EUR, GBP, ZAR, JPY, and others).
