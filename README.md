# InteriorMarketplace

اسکلت backend بازار طراحی داخلی با .NET 10 است. ساختار solution از معماری پروژه Niam پیروی می‌کند و در این مرحله عمداً هیچ مدل دامنه، use case، endpoint، persistence یا integration پیاده‌سازی نشده است.

## ساختار

```text
src/
├── Application/
│   ├── Application/
│   └── Application.Contracts/
├── Domain/
│   └── Domain/
├── Infrastructure/
│   ├── Adapter.Http/
│   ├── Adapter.LocalImageStorage/
│   ├── Adapter.Notifications/
│   └── Adapter.PostgreSql/
├── Host/
└── client/

test/
├── Application.UnitTests/
└── Host.IntegrationTests/
```

## مسئولیت پروژه‌ها

- `Domain`: مدل دامنه، aggregateها، entityها، value objectها و portهای کاملاً دامنه‌ای.
- `Application.Contracts`: command، query، result و قراردادهایی که adapterها باید پیاده‌سازی کنند.
- `Application`: handlerهای use case و orchestration برنامه.
- `Adapter.Http`: ورودی HTTP، endpointها، DTOها، validation و mapping.
- `Adapter.PostgreSql`: EF Core، DbContext، migration و repositoryها.
- `Adapter.LocalImageStorage`: پیاده‌سازی محلی ذخیره تصویر.
- `Adapter.Notifications`: پیاده‌سازی notification.
- `Host`: composition root و نقطه اجرای برنامه.

## جهت وابستگی

```text
Domain <- Application.Contracts <- Application <- Adapter.Http <- Host
                 ^                     ^
                 └──── Outbound Adapters ────┘
```

Host همه adapterها را کنار هم قرار می‌دهد. Domain نباید به ASP.NET Core، EF Core، JWT، پایگاه‌داده یا SDK خارجی وابسته شود.

## وضعیت فعلی

این repository فقط اسکلت معماری را در خود دارد. قابلیت‌های Projects، Vendor Offers، Identity و Notifications در مرحله بعد و به شکل use caseهای مستقل اضافه خواهند شد.

## فرانت‌اند

مسیر `src/client` برای اضافه‌شدن repository مستقل فرانت‌اند در مرحله بعد رزرو شده است. روش پیشنهادی برای اتصال آن Git submodule یا Git subtree است تا تاریخچه و چرخه انتشار frontend مستقل باقی بماند.

## Build

```powershell
dotnet restore InteriorMarketplace.sln
dotnet build InteriorMarketplace.sln --no-restore
```
