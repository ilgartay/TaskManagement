# TaskManagement.API

## Yerel JWT anahtarı

JWT imza anahtarı güvenlik nedeniyle `appsettings.json` içinde tutulmaz. Projeyi
ilk kez çalıştırmadan önce makineye özel bir anahtar oluşturun:

```bash
dotnet user-secrets set "Jwt:Key" "$(openssl rand -hex 32)"
```

Alternatif olarak en az 32 byte uzunluğunda bir anahtarı `Jwt__Key` ortam
değişkeniyle sağlayabilirsiniz.
