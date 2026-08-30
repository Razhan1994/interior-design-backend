# InteriorMarketplace

بک‌اند MVP بازار طراحی داخلی با .NET 10 و معماری شش‌ضلعی است. برنامه یک **Modular Monolith** است؛ هر ماژول مرز دامنه و کاربردی مستقل دارد، اما همه در یک Host و یک PostgreSQL اجرا می‌شوند.

## معماری

جهت وابستگی همیشه به داخل است: `Domain <- Application <- Adapters <- Host`. لایه Domain هیچ وابستگی به HTTP، EF Core، JWT یا SDK خارجی ندارد. Application شامل use case و portهاست. آداپتر ورودی Minimal API، DTO و validation را نگه می‌دارد. آداپتر خروجی EF Core، repository، کاربر جاری، ذخیرهٔ محلی تصویر و notification را پیاده می‌کند. `IImageGenerationService` فقط یک port آینده است و در MVP ثبت یا فراخوانی نشده است.

ماژول‌های اصلی در `src/Modules/Projects` و `src/Modules/VendorOffers` قرار دارند. `src/Host/InteriorMarketplace.WebApi` تنها composition root است. تست‌های معماری وابسته نبودن Domain به فناوری‌های بیرونی را کنترل می‌کنند.

## اجرا

پیش‌نیازها: .NET SDK 10 و Docker.

```powershell
docker compose up -d
dotnet restore
dotnet run --project src/Host/InteriorMarketplace.WebApi
```

در Development مقدار `Password=CHANGE_ME` در `appsettings.Development.json` را با رمز Compose (`postgres`) یا secret محلی جایگزین کنید. Swagger در `/swagger` است. در startup migrationها اجرا و در دیتابیس خالی یک مالک، یک فروشنده و پروژهٔ منتشرشده با سه عنصر Sofa، Rug و FloorLamp seed می‌شوند. شناسه مالک `11111111-1111-1111-1111-111111111111` و فروشنده `22222222-2222-2222-2222-222222222222` است.

## Migration

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/Modules/Projects/Projects.Adapters.Outbound --startup-project src/Host/InteriorMarketplace.WebApi --output-dir Migrations
dotnet ef database update --project src/Modules/Projects/Projects.Adapters.Outbound --startup-project src/Host/InteriorMarketplace.WebApi
```

فرمان‌های Docker: `docker compose up -d` برای شروع، `docker compose logs -f postgres` برای log و `docker compose down` برای توقف. Volume داده را نگه می‌دارد؛ حذف volume فقط با `docker compose down -v` انجام می‌شود.

## جریان API

برای توسعه از `POST /api/auth/dev-token` با role برابر `Homeowner` یا `Vendor` توکن بگیرید. مالک پروژه را ایجاد می‌کند، عناصر مختصات‌دار را می‌افزاید و منتشر می‌کند. فروشنده فهرست منتشرشده‌ها را می‌بیند و برای هر عنصر یک پیشنهاد Pending می‌سازد. مالک پیشنهادها را می‌بیند و یکی را می‌پذیرد؛ سایر Pendingها رد می‌شوند. نمونه درخواست‌ها در `http/InteriorMarketplace.http` موجود است.

مختصات اعشاری و نرمال‌شده‌اند: همه مقادیر بین صفر و یک و `X + Width <= 1` و `Y + Height <= 1` هستند. زمان‌ها UTC ذخیره می‌شوند.

## فاز AI آینده

نقطه اتصال آینده `IImageGenerationService` در BuildingBlocks.Application است. در فاز بعد یک outbound adapter برای ارائه‌دهنده AI و یک use case صریح اضافه می‌شود؛ Domain و API فعلی نیازی به وابستگی مستقیم به SDK آن ارائه‌دهنده نخواهند داشت.

## تست

```powershell
dotnet build InteriorMarketplace.sln
dotnet test InteriorMarketplace.sln --no-build
```
