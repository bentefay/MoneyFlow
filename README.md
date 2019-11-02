# MoneyFlow

[MoneyFlow](https://moneyflow.azurewebsites.net/) tracks your money and how it flows in and out of your various banks and accounts.

[![Build Status](https://dev.azure.com/btefay/MoneyFlow/_apis/build/status/MoneyFlow)](https://dev.azure.com/btefay/MoneyFlow/_build/latest?definitionId=1)

## :page_facing_up: Single view of all your finances

View all your transactions in a single view, consolidating many banks and accounts.
Tag and filter to your hearts content.
Get the benefits of a modern, beautiful spend tracking app without being tied to a particular bank.

## :ledger: Know what you're spending money on

Categorise and visualize your spending, and MoneyFlow will learn over time to do this for you automatically.

## :lock: Safe and secure by design

You own your data. Your data is encrypted, end-to-end, so nobody can access it but you.

## :couple: Manage shared finances

Track ownership of spending if you share your accounts with someone you :heart:

## :inbox_tray: Simple data import

Data import is designed to be painless without comprimising your security. Visit your bank's online banking website
as you normally would, then either:
1. Copy and paste straight out of the web page.
2. Download CSV files of your transactions and upload them to MoneyFlow.
3. In the future, a desktop app will do this for you automatically
   (it will all be open source, so you can be sure nothing :fish: is going on).

## Getting started

Clone this repository then:

- `dotnet user-secrets -p src/Web set "StorageConnectionString" ""`
- `m run server` (watch run server)
- `m run client` (watch run client)
- `m test server` (watch test server)
- `m build` (same build that CI runs to prepare for deployment)

## Resources

- [App](https://moneyflow.azurewebsites.net/)
- [Build Server Management](https://dev.azure.com/btefay/MoneyFlow/_build)
- [Prod Server Management](https://portal.azure.com)
- [Project Management](https://github.com/bentefay/MoneyFlow/projects/1)

## Features

- [ ] Coming soon... :wink:
