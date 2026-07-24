# TaskManagement.API

## Yerel JWT anahtarı

JWT imza anahtarı `appsettings.json` içinde tutulmaz. Projeyi
ilk kez çalıştırırken önce makineye özel bir anahtar oluşturun:

```bash
dotnet user-secrets set "Jwt:Key" "$(openssl rand -hex 32)"
```

Alternatif olarak en az 32 byte uzunluğunda bir anahtarı `Jwt__Key` ortam
değişkeniyle sağlayabilirsiniz.

## Demo kullanıcı

Geliştirme ortamında arayüzü denemek için aşağıdaki hesap kullanın:

- Kullanıcı adı: `demo`
- Şifre: `Demo123!`

Demo kullanıcı veritabanına EF Core migration ile eklenir.
