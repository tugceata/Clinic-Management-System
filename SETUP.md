# ClinicMvc — Faz 1-2 Kurulum

Bu klasördeki `Models/` ve `Data/` klasörlerini, birazdan oluşturacağın MVC projesinin
**kök dizinine** kopyalayacaksın. Adımları sırayla uygula.

---

## 1. Projeyi oluştur

```bash
dotnet new mvc -o ClinicMvc
cd ClinicMvc
code .
```

> Not: `dotnet --version` ile SDK sürümünü kontrol et. Komutlar 8 / 9 / 10 fark etmeksizin çalışır.

## 2. Paketleri ekle

```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
```

EF aracı kurulu değilse (bir kez):

```bash
dotnet tool install --global dotnet-ef
# zaten varsa:  dotnet tool update --global dotnet-ef
```

## 3. Dosyaları kopyala

Bu zip'ten gelen `Models/` ve `Data/` klasörlerini `ClinicMvc/` kök dizinine bırak.
(Üzerine sor derse "değiştir" de — `Models` zaten var ama bizimkiler eklenecek.)

## 4. appsettings.json

`appsettings.json` içine, en dıştaki `{ }` bloğunun içine şu satırı ekle:

```json
"ConnectionStrings": {
  "Default": "Data Source=clinic.db"
}
```

(Mevcut `"Logging"` bloğunun yanına, virgülle ayırarak.)

## 5. Program.cs — 2 yer ekle

**a)** En üste using ekle:

```csharp
using ClinicMvc.Data;
using Microsoft.EntityFrameworkCore;
```

**b)** `builder.Services.AddControllersWithViews();` satırının HEMEN ALTINA:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
```

**c)** `var app = builder.Build();` satırının HEMEN ALTINA (DB'yi kurup seed eder):

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
}
```

## 6. Migration oluştur ve çalıştır

```bash
dotnet ef migrations add Init
dotnet run
```

`dotnet run` ilk açılışta migration'ı uygular ve seed verisini ekler.

---

## Başarı kontrolü

- Proje kök dizininde **`clinic.db`** dosyası oluştuysa ✅
- DBeaver / "SQLite Viewer" eklentisiyle açıp `Departments`, `Doctors`,
  `Patients`, `Appointments`, `Prescriptions`, `Reviews` tablolarını ve
  içlerindeki seed verisini gördüysen ✅

Buraya kadar geldiyse Faz 3'e (rol seçimli login) geçiyoruz.
