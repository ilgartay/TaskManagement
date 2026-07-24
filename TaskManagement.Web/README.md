# TaskManagement.Web

Görev yönetim sisteminin Angular 22 ile geliştirilen web arayüzüdür.

## Çalıştırma

Önce backend API'yi `http://localhost:5085` adresinde çalıştırın. Ardından:

```bash
npm install
npm start
```

Uygulama `http://localhost:4200` adresinde açılır.

## Proje yapısı

- `core`: HTTP servisleri, guard, interceptor ve token yönetimi
- `features/auth`: giriş ve kayıt ekranları
- `features/tasks`: görev paneli
- `shared/models`: backend DTO'larına karşılık gelen TypeScript modelleri
- `environments`: API adresleri

## Kontrol

```bash
npm run build
npm test -- --watch=false
```
