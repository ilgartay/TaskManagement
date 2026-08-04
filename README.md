# Task Management

ASP.NET Core 8 API ve Angular 22 arayüzünden oluşan görev yönetim uygulamasıdır. Veritabanı sağlayıcısı PostgreSQL veya Oracle olarak seçilebilir.

## Yerel geliştirme

Gizli değerler repoda tutulmaz. Backend'i ilk kez çalıştırmadan önce JWT anahtarını ve bağlantı bilgisini user-secrets ile tanımlayın:

```bash
dotnet user-secrets set "Jwt:Key" "$(openssl rand -hex 32)"
dotnet user-secrets set "ConnectionStrings:PostgreSQLConnection" "Host=localhost;Port=5432;Database=taskmanagementdb;Username=postgres;Password=your-password"
dotnet run --launch-profile http
```

Oracle kullanmak için:

```bash
dotnet user-secrets set "DatabaseProvider" "Oracle"
dotnet user-secrets set "ConnectionStrings:OracleConnection" "User Id=your-user;Password=your-password;Data Source=localhost:1521/XEPDB1"
```

Frontend ayrı bir terminalde çalıştırılır:

```bash
cd TaskManagement.Web
npm install
npm start
```

Geliştirme adresleri API için `http://localhost:5085`, frontend için `http://localhost:4200` şeklindedir. Demo hesabı `demo` / `Demo123!` bilgileriyle kullanılabilir.

## Testler

Tüm kontrolleri tek komutla çalıştırmak için:

```bash
./scripts/test-all.sh
```

Backend testleri auth akışı, CORS, kullanıcı izolasyonu, rol kontrolü, görev CRUD işlemleri, filtreleme, pagination, dosya yükleme, hata durumları, performans ve PostgreSQL/Oracle migration SQL'ini kapsar. Frontend testleri HTTP servislerini, token interceptor'ını, hata mesajlarını, form validasyonunu, arama, filtreleme, sıralama, pagination ve dosya seçimini kontrol eder.

Gerçek PostgreSQL testi sadece test amacıyla ayrılmış ve adı `_test` ile biten bir veritabanında çalışır. Test benzersiz bir şema açar ve transaction sonunda rollback yapar:

```bash
TEST_POSTGRES_CONNECTION="Host=localhost;Port=5432;Database=taskmanagement_test;Username=postgres;Password=postgres" \
dotnet test TaskManagement.API.Tests/TaskManagement.API.Tests.csproj
```

Bu bağlantı tanımlanmazsa canlı PostgreSQL testi atlanır. GitHub Actions bu testi geçici PostgreSQL servisiyle otomatik çalıştırır.

## Production yapılandırması

Aşağıdaki değerler environment variable olarak verilmelidir:

- `DatabaseProvider`: `PostgreSQL` veya `Oracle`
- `ConnectionStrings__PostgreSQLConnection` veya `ConnectionStrings__OracleConnection`
- `Jwt__Key`: en az 32 byte uzunluğunda imza anahtarı
- `Cors__AllowedOrigins__0`: gerekiyorsa izin verilen frontend adresi
- `Database__ApplyMigrationsOnStartup`: kontrollü deployment için `true`
- `Storage__UploadPath`: kalıcı dosya dizini

Uygulama ters proxy arkasında `X-Forwarded-For` ve `X-Forwarded-Proto` başlıklarını kullanır. `/health` endpoint'i deployment sağlık kontrolü için anonim erişime açıktır.

## Docker ve SSL deployment

Örnek Docker kurulumu PostgreSQL, API ve Angular/NGINX servislerini içerir. NGINX HTTP isteklerini HTTPS'e yönlendirir ve API isteklerini backend'e iletir.

```bash
cp .env.example .env
```

`.env` içindeki örnek değerleri değiştirin. SSL dosyalarını şu konumlara yerleştirin:

```text
deploy/certs/fullchain.pem
deploy/certs/privkey.pem
```

Ardından:

```bash
./scripts/deploy.sh
```

`.env`, sertifikalar, veritabanı verileri ve yüklenen dosyalar Git'e eklenmez. Oracle deployment'ında `compose.yaml` içindeki PostgreSQL servisi yerine yönetilen Oracle bağlantısı ve `DatabaseProvider=Oracle` değeri kullanılmalıdır.
