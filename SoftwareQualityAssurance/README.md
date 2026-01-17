# Real Estate Management REST API

.NET 8 ile geliştirilmiş Real Estate Management REST API projesi. Software Quality Assurance ve Testing dersi final projesi.

## 📋 Proje Hakkında

Bu proje, emlak yönetimi için geliştirilmiş bir REST API'dir. 5 farklı kaynak (Customers, Properties, Categories, Locations, Invoices) üzerinde CRUD operasyonları sağlar. Proje, kapsamlı test stratejisi (Unit, Integration, E2E) ve modern yazılım geliştirme prensipleri ile geliştirilmiştir.

## 🚀 Özellikler

- **Layered Architecture**: Controllers, Services, Repositories, DTOs katmanları
- **Entity Framework Core**: SQL Server veritabanı entegrasyonu
- **SOLID Principles**: Temiz kod prensipleri
- **Comprehensive Testing**: Unit, Integration ve E2E testler (32+ test)
- **Swagger/OpenAPI**: API dokümantasyonu (OpenAPI 3.0)
- **Soft Delete**: Kayıtların silinmesi yerine işaretlenmesi
- **Global Exception Handling**: 500 Internal Server Error yönetimi
- **HTTP Status Codes**: 200, 201, 400, 404, 500 durum kodları

## 📋 Teknoloji Stack

- **Backend**: .NET 8 Web API
- **ORM**: Entity Framework Core 8.0
- **Database**: Microsoft SQL Server
- **Testing**: 
  - xUnit (test framework)
  - FluentAssertions (assertion library)
  - Moq (mocking framework)
  - Microsoft.AspNetCore.Mvc.Testing (integration testing)
- **API Documentation**: Swagger/OpenAPI 3.0

## 🗄️ Veritabanı Yapısı

### Entity'ler

- **BaseEntity**: Tüm entity'ler için ortak alanlar (Id, CreatedDate, ModifiedDate, IsDeleted)
- **Customer**: Müşteri bilgileri (FirstName, LastName, Email, IdentityNumber, Balance, PhoneNumber)
- **Property**: Emlak bilgileri (Title, BlockNumber, ParcelNumber, SquareMeters, Price, CategoryId, LocationId, IsAvailable)
- **Category**: Emlak kategorileri (Name, Description)
- **Location**: Lokasyon bilgileri (CityName, PlateCode)
- **Invoice**: Fatura bilgileri (SerialNumber, TotalAmount, InvoiceDate, CustomerId, Status)

### İlişkiler

- **Invoice ↔ Customer**: Bir fatura bir müşteriye aittir (Many-to-One)
- **Property ↔ Category**: Bir emlak bir kategoriye aittir (Many-to-One)
- **Property ↔ Location**: Bir emlak bir lokasyona aittir (Many-to-One)

## 🛠️ Kurulum

### Gereksinimler

- .NET 8 SDK
- SQL Server (Express veya üzeri)
- Visual Studio 2022 veya Visual Studio Code

### Adım 1: Projeyi Klonlayın

```bash
git clone <repository-url>
cd SoftwareQualityAssurance
```

### Adım 2: Veritabanını Oluşturun

1. SQL Server Management Studio'yu açın
2. `Database/CreateDatabase.sql` dosyasını çalıştırın
3. Veya Package Manager Console'da:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Adım 3: Connection String'i Yapılandırın

`appsettings.json` dosyasında connection string'i düzenleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=RealEstateManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Adım 4: Projeyi Çalıştırın

```bash
# Bağımlılıkları yükle
dotnet restore

# Projeyi derle
dotnet build

# Projeyi çalıştır
dotnet run
```

API: `https://localhost:5001` veya `http://localhost:5000`  
Swagger UI: `https://localhost:5001/swagger` veya `http://localhost:5000/swagger`

## 📡 API Endpoints

### Swagger/OpenAPI Dokümantasyonu

API dokümantasyonuna erişmek için projeyi çalıştırdıktan sonra tarayıcınızda şu adresi açın:

```
http://localhost:5000/swagger
```

veya

```
https://localhost:5001/swagger
```

Swagger UI üzerinden tüm endpoint'leri test edebilir, request/response şemalarını görebilirsiniz.

**Not:** API'de güncelleme işlemleri için PUT metodu kullanılmaktadır. Gereksinimlerde belirtilen "PATCH/PUT" ifadesi gereği PUT metodu tercih edilmiştir. PATCH metodu da eklenebilir, ancak mevcut PUT implementasyonu tüm güncelleme ihtiyaçlarını karşılamaktadır.

### 1. Customers (Müşteriler)

#### Tüm müşterileri listele
```http
GET /api/customers
```

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "firstName": "Ahmet",
    "lastName": "Yılmaz",
    "email": "ahmet@example.com",
    "identityNumber": "12345678901",
    "balance": 50000,
    "phoneNumber": "05551234567"
  }
]
```

#### Müşteri detayı
```http
GET /api/customers/{id}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "firstName": "Ahmet",
  "lastName": "Yılmaz",
  "email": "ahmet@example.com",
  "identityNumber": "12345678901",
  "balance": 50000,
  "phoneNumber": "05551234567"
}
```

**Response (404 Not Found):**
```json
{
  "error": "Customer not found"
}
```

#### Yeni müşteri oluştur
```http
POST /api/customers
Content-Type: application/json

{
  "firstName": "Mehmet",
  "lastName": "Kaya",
  "email": "mehmet@example.com",
  "identityNumber": "98765432109",
  "balance": 75000,
  "phoneNumber": "05559876543"
}
```

**Response (201 Created):**
```json
{
  "id": 2,
  "firstName": "Mehmet",
  "lastName": "Kaya",
  "email": "mehmet@example.com",
  "identityNumber": "98765432109",
  "balance": 75000,
  "phoneNumber": "05559876543"
}
```

**Response (400 Bad Request):**
```json
{
  "error": "Email already exists"
}
```

#### Müşteri güncelle
```http
PUT /api/customers/{id}
Content-Type: application/json

{
  "firstName": "Mehmet Updated",
  "lastName": "Kaya",
  "email": "mehmet.updated@example.com",
  "identityNumber": "98765432109",
  "balance": 80000,
  "phoneNumber": "05559876543"
}
```

**Response (200 OK):** Güncellenmiş müşteri bilgileri  
**Response (404 Not Found):** Müşteri bulunamadı  
**Response (400 Bad Request):** Email zaten kullanılıyor

#### Müşteri sil (Soft Delete)
```http
DELETE /api/customers/{id}
```

**Response (204 No Content):** Başarılı  
**Response (404 Not Found):** Müşteri bulunamadı

### 2. Properties (Emlaklar)

#### Tüm emlakları listele
```http
GET /api/properties
```

#### Emlak detayı
```http
GET /api/properties/{id}
```

#### Yeni emlak oluştur
```http
POST /api/properties
Content-Type: application/json

{
  "title": "Lüks Villa",
  "blockNumber": "A-123",
  "parcelNumber": "P-456",
  "squareMeters": 250.50,
  "price": 1500000.00,
  "categoryId": 1,
  "locationId": 1,
  "isAvailable": true
}
```

**Response (201 Created):** Oluşturulan emlak bilgileri  
**Response (400 Bad Request):** Category veya Location bulunamadı

#### Emlak güncelle
```http
PUT /api/properties/{id}
```

#### Emlak sil
```http
DELETE /api/properties/{id}
```

### 3. Categories (Kategoriler)

#### Tüm kategorileri listele
```http
GET /api/categories
```

#### Kategori detayı
```http
GET /api/categories/{id}
```

#### Yeni kategori oluştur
```http
POST /api/categories
Content-Type: application/json

{
  "name": "Villa",
  "description": "Lüks villa kategorisi"
}
```

#### Kategori güncelle
```http
PUT /api/categories/{id}
```

#### Kategori sil
```http
DELETE /api/categories/{id}
```

### 4. Locations (Lokasyonlar)

#### Tüm lokasyonları listele
```http
GET /api/locations
```

#### Lokasyon detayı
```http
GET /api/locations/{id}
```

#### Yeni lokasyon oluştur
```http
POST /api/locations
Content-Type: application/json

{
  "cityName": "İstanbul",
  "plateCode": "34"
}
```

#### Lokasyon güncelle
```http
PUT /api/locations/{id}
```

#### Lokasyon sil
```http
DELETE /api/locations/{id}
```

### 5. Invoices (Faturalar)

#### Tüm faturaları listele
```http
GET /api/invoices
```

#### Fatura detayı
```http
GET /api/invoices/{id}
```

#### Yeni fatura oluştur
```http
POST /api/invoices
Content-Type: application/json

{
  "serialNumber": "INV-2024-001",
  "totalAmount": 5000.00,
  "invoiceDate": "2024-01-15T00:00:00Z",
  "customerId": 1,
  "status": "Pending"
}
```

**Response (201 Created):** Oluşturulan fatura bilgileri  
**Response (400 Bad Request):** Fatura tutarı sıfırdan büyük olmalıdır veya Customer bulunamadı

#### Fatura güncelle
```http
PUT /api/invoices/{id}
```

#### Fatura sil
```http
DELETE /api/invoices/{id}
```

## 📝 Business Rules

1. **Email Uniqueness**: Müşteri email adresi benzersiz olmalıdır
2. **Invoice Amount**: Fatura tutarı sıfırdan büyük olmalıdır
3. **Soft Delete**: Silinen kayıtlar veritabanından silinmez, IsDeleted flag'i ile işaretlenir
4. **Foreign Key Validation**: Property oluştururken Category ve Location mevcut olmalıdır
5. **Invoice-Customer Relationship**: Fatura oluştururken Customer mevcut olmalıdır

## 🧪 Testler

### Tüm Testleri Çalıştırma

```bash
dotnet test
```

### Test Coverage Raporu Oluşturma

```bash
# HTML formatında coverage raporu
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=html

# Rapor coverage/index.html dosyasında oluşturulur
```

### Test Kategorileri

#### 1. Unit Tests (17 test)

**Konum:** `SoftwareQualityAssurance.UnitTests`

- Service layer business logic testleri
- Mock kullanarak izole testler
- Pozitif ve negatif senaryolar

**Test Edilenler:**
- CustomerService: 10 test
  - GetAllCustomersAsync
  - GetCustomerByIdAsync (exists/not exists)
  - CreateCustomerAsync (valid/duplicate email)
  - UpdateCustomerAsync (exists/not exists/duplicate email)
  - DeleteCustomerAsync (exists/not exists)
- InvoiceService: 5 test
  - CreateInvoiceAsync (valid/zero amount/negative amount/non-existent customer)
  - UpdateInvoiceAsync (zero amount)
- CategoryService: 2 test
  - GetAllCategoriesAsync
  - CreateCategoryAsync

**Örnek Test Senaryoları:**
- Email uniqueness kontrolü
- Invoice amount > 0 kontrolü
- Customer CRUD işlemleri
- Business rule validasyonları

#### 2. Integration Tests (10 test)

**Konum:** `SoftwareQualityAssurance.IntegrationTests`

- Controller ve database entegrasyon testleri
- In-memory database kullanımı
- HTTP request/response testleri

**Test Edilenler:**
- CustomersController: 6 test
  - GetAllCustomers (empty list)
  - CreateCustomer (valid data/duplicate email)
  - GetCustomerById (exists)
  - UpdateCustomer (valid data)
  - DeleteCustomer (exists)
- InvoicesController: 4 test
  - CreateInvoice (zero amount/negative amount/valid data)
  - GetInvoiceById (exists)

**Test Edilen HTTP Metodları:**
- GET (liste + tekil)
- POST
- PUT
- DELETE

**Test Edilen HTTP Status Kodları:**
- 200 OK
- 201 Created
- 400 Bad Request
- 404 Not Found

#### 3. E2E Tests (5 test)

**Konum:** `SoftwareQualityAssurance.E2ETests`

- Gerçek dünya senaryoları
- Tam akış testleri
- Birden fazla kaynakla kompleks iş akışları

**Test Senaryoları:**

1. **CompleteCustomerLifecycle_ShouldWorkEndToEnd**
   - Senaryo: Müşteri oluştur → Güncelle → Getir → Sil
   - Adımlar:
     1. Yeni müşteri oluştur (POST /api/customers)
     2. Oluşturulan müşteriyi getir (GET /api/customers/{id})
     3. Müşteriyi güncelle (PUT /api/customers/{id})
     4. Güncellemeyi doğrula (GET /api/customers/{id})
     5. Müşteriyi sil (DELETE /api/customers/{id})
     6. Silme işlemini doğrula (GET /api/customers/{id} → 404)

2. **CustomerWithInvoiceFlow_ShouldWorkEndToEnd**
   - Senaryo: Müşteri oluştur → Fatura oluştur → İlişkiyi doğrula
   - Adımlar:
     1. Yeni müşteri oluştur
     2. Müşteri için fatura oluştur (POST /api/invoices)
     3. Fatura-müşteri ilişkisini doğrula
     4. Faturayı getir ve doğrula

3. **PropertyWithCategoryAndLocation_ShouldWorkEndToEnd**
   - Senaryo: Kategori oluştur → Lokasyon oluştur → Emlak oluştur
   - Adımlar:
     1. Yeni kategori oluştur (POST /api/categories)
     2. Yeni lokasyon oluştur (POST /api/locations)
     3. Kategori ve lokasyon ile emlak oluştur (POST /api/properties)
     4. İlişkileri doğrula

4. **MultipleInvoicesForCustomer_ShouldWorkEndToEnd**
   - Senaryo: Müşteri oluştur → Birden fazla fatura oluştur → Tümünü doğrula
   - Adımlar:
     1. Yeni müşteri oluştur
     2. İlk faturayı oluştur
     3. İkinci faturayı oluştur
     4. Her iki faturanın aynı müşteriye ait olduğunu doğrula
     5. Tüm faturaları listele ve sayıyı doğrula

5. **UpdateAndDeleteProperty_ShouldWorkEndToEnd**
   - Senaryo: Emlak oluştur → Güncelle → Sil
   - Adımlar:
     1. Kategori ve lokasyon oluştur
     2. Emlak oluştur
     3. Emlak bilgilerini güncelle (PUT /api/properties/{id})
     4. Güncellemeyi doğrula
     5. Emlak sil (DELETE /api/properties/{id})
     6. Silme işlemini doğrula (GET /api/properties/{id} → 404)

**Test Özellikleri:**
- Her test kendi verilerini oluşturur (bağımsız testler)
- Gerçek kullanım durumlarını simüle eder
- Birden fazla kaynakla kompleks iş akışları test edilir

### Test Coverage

Hedef: **%60+ kod kapsama oranı**

Coverage raporunu oluşturmak için:

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=html
```

Rapor `coverage/index.html` dosyasında oluşturulur.

## 🔧 Configuration

### Connection String

`appsettings.json` dosyasında connection string ayarlanmıştır:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=RealEstateManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### HTTP Status Codes

API aşağıdaki HTTP durum kodlarını kullanır:

- **200 OK**: Başarılı GET, PUT işlemleri
- **201 Created**: Başarılı POST işlemleri
- **204 No Content**: Başarılı DELETE işlemleri
- **400 Bad Request**: Geçersiz istek (validation hataları, business rule ihlalleri)
- **404 Not Found**: Kaynak bulunamadı
- **500 Internal Server Error**: Beklenmeyen sunucu hataları (Global Exception Handler ile yönetilir)

## 📊 Proje Yapısı

```
SoftwareQualityAssurance/
├── Controllers/              # API Controllers
│   ├── CustomersController.cs
│   ├── PropertiesController.cs
│   ├── CategoriesController.cs
│   ├── LocationsController.cs
│   └── InvoicesController.cs
├── Services/                 # Business Logic Layer
│   ├── CustomerService.cs
│   ├── PropertyService.cs
│   ├── CategoryService.cs
│   ├── LocationService.cs
│   └── InvoiceService.cs
├── Repositories/              # Data Access Layer
│   ├── Repository.cs
│   └── CustomerRepository.cs
├── Models/                   # Entity Models
│   ├── BaseEntity.cs
│   ├── Customer.cs
│   ├── Property.cs
│   ├── Category.cs
│   ├── Location.cs
│   └── Invoice.cs
├── DTOs/                     # Data Transfer Objects
│   ├── CustomerDto.cs
│   ├── PropertyDto.cs
│   ├── CategoryDto.cs
│   ├── LocationDto.cs
│   └── InvoiceDto.cs
├── Data/                     # DbContext
│   └── ApplicationDbContext.cs
├── Database/                 # SQL Scripts
│   ├── CreateDatabase.sql
│   ├── CreateTables.sql
│   └── InsertData.sql
├── Middleware/               # Middleware
│   └── GlobalExceptionHandler.cs
├── SoftwareQualityAssurance.UnitTests/
│   └── Services/
│       ├── CustomerServiceTests.cs
│       ├── InvoiceServiceTests.cs
│       └── CategoryServiceTests.cs
├── SoftwareQualityAssurance.IntegrationTests/
│   └── Controllers/
│       ├── CustomersControllerIntegrationTests.cs
│       └── InvoicesControllerIntegrationTests.cs
└── SoftwareQualityAssurance.E2ETests/
    └── Scenarios/
        └── CustomerE2ETests.cs
```

## 📚 Ek Kaynaklar

- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/)
- [xUnit Documentation](https://xunit.net/)
- [Swagger/OpenAPI Specification](https://swagger.io/specification/)

## 👤 Yazar

- **Ad Soyad**: [FATMA NUR GÜLKAN]
- **Öğrenci Numarası**: [4010930216]

## 📄 Lisans

Bu proje eğitim amaçlıdır.

---

**Not:** Proje formunu doldurmayı unutmayın: https://forms.gle/PZhigBPLfAhe874c9
