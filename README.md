# 💳 Wallet Management Microservice Architecture
Bu proje; modern finansal sistemlerin ihtiyaç duyduğu ölçeklenebilirlik, yüksek erişilebilirlik ve veri tutarlılığı prensipleri gözetilerek tasarlanmış, 
ASP.NET Core tabanlı bir **Mikroservis Mimarisi** örneğidir. Proje, bir kullanıcının kayıt aşamasından başlayarak **cüzdan yönetimi, döviz alım-satımı ve servisler arası 
güvenli iletişimi** kapsayan uçtan uca bir finansal akışı simüle eder.

-----

## 🏗️ Mimari Yapı ve Tasarım Desenleri (Patterns)
Proje sadece kod yazımından ibaret olmayıp, kurumsal yazılım dünyasında kabul görmüş mimari desenleri içerir:

**Clean Architecture (Onion Architecture)**: Bağımlılıkların içe doğru olduğu, iş mantığının (Domain) dış etkenlerden (DB, UI) izole edildiği katmanlı yapı.

**API Gateway Pattern**: Mikroservislerin dış dünyaya tek bir kapıdan açılması ve trafiğin merkezi yönetimi.

**Aggregation Pattern**: Birden fazla servisten gelen verinin Gateway seviyesinde birleştirilerek frontend'e optimize edilmiş şekilde sunulması.

**M2M (Machine-to-Machine) Auth**: Servislerin birbirleriyle "Internal Secret Key" mekanizması üzerinden güvenli konuşması.

-----

## 🛠️ Kullanılan Teknolojiler ve Görevleri
Projede kullanılan her bir araç, belirli bir mühendislik problemini çözmek amacıyla seçilmiştir:

| Teknoloji | Kullanım Amacı | 
| :--- | :--- |
| `ASP.NET Core 8.0` | `Yüksek performanslı ve cross-platform mikroservislerin geliştirilmesi.` |
| `React (Vite)` | `Kullanıcı deneyimini maksimize eden, hızlı ve bileşen tabanlı arayüz yönetimi.` |
| `Ocelot API Gateway` | `Tüm mikroservis trafiğinin yönlendirilmesi, güvenliği ve rotalanması.` |
| `Redis (Distributed Caching)` | `Yüksek Performans: Sık değişmeyen verilerin (Dil çevirileri, döviz kurları) bellek üzerinde tutularak DB yükünün %90 azaltılması.` |
| `Stored Procedures (SP)` | `Veri Tutarlılığı & Performans: Para transferi ve bakiye güncellemeleri gibi kritik işlemlerin DB seviyesinde, atomik ve hızlı yönetilmesi.` |
| `Entity Framework Core` | `Veritabanı işlemlerinin ORM (Object-Relational Mapping) ile yönetilmesi.` |
| `MS SQL Server` | `Finansal verilerin ilişkisel ve ACID kurallarına uygun şekilde saklanması.` |
| `Polly` | `Resilience (Dayanıklılık) stratejileri; Circuit Breaker ve Timeout yönetimi.` |
| `JWT Bearer Auth` | `Kullanıcı kimlik doğrulama ve yetkilendirme süreçlerinin yönetimi.` |
| `Serilog` | `Sistemdeki tüm olayların (Error, Info, Warning) merkezi olarak loglanması.` |
| `IHttpClientFactory` | `Servisler arası HTTP çağrılarının performanslı ve yönetilebilir şekilde yapılması.` |
| `Delegating Handlers` | `Gateway üzerinde özelleştirilmiş Localization ve Resilience süreçlerinin yürütülmesi.` |

-----

## 🚀 Öne Çıkan Özellikler

#### 💻 Dinamik ve Reaktif Arayüz (React & Tailwind CSS)

- **State Management**: Kullanıcı bakiyeleri ve işlem geçmişi gibi dinamik veriler gerçek zamanlı olarak yönetilir.

- **Gateway Integration**: Frontend, arkadaki mikroservislerin portlarını bilmez; sadece Gateway (Port 5000) ile konuşur. Bu, güvenlik ve mimari gizleme sağlar.

- **Localization Support**: Gateway'den gelen tercüme edilmiş hata mesajları, arayüzde anlık olarak kullanıcıya sunulur.

#### 🌍 Akıllı Yerelleştirme ve Gateway Entegrasyonu (Centralized Localization)

- Sistemdeki tüm hata mesajları mikroservislerde ham kodlar (ERR_...) olarak üretilir.
Gateway üzerindeki LocalizationHandler, bu kodları yakalar ve kullanıcının dil tercihine göre **(TR/EN)** merkezi bir servisten tercüme ederek son kullanıcıya sunar.

#### 🔀 Akıllı Havale ve Döviz Takas Motoru

- Veritabanı seviyesinde kurgulanan SP'ler sayesinde; para transferleri, döviz alım-satım (Trade) işlemleri **Transaction** blokları içinde yapılır.
Para bir hesaptan çıkmadan diğerine girmez, veri tutarlılığı %100 garantilenir.

#### ⚡ Redis ile Dağıtık Önbellekleme (Distributed Caching)

- Sistemde özellikle **MultiLanguage servisi ve Döviz Kurları** için Redis entegrasyonu yapılmıştır. Bu sayede her çeviri veya kur bilgisi için
ana veritabanına gitmek yerine, milisaniyeler içinde RAM üzerinden veri sunularak sistem tepki süresi (latency) minimize edilmiştir.

#### 🛡️ Veritabanı Seviyesinde İşlem Yönetimi (Stored Procedures)

Finansal işlemlerin güvenliği için iş mantığı sadece kod seviyesinde bırakılmamış, SQL Server tarafında Stored Procedure'ler kullanılmıştır.

- **Atomicity**: BEGIN TRANSACTION yapısı ile para transferi sırasında oluşabilecek bir hata anında tüm işlem geri alınır (Rollback).

- **Performance**: Karmaşık hesaplamalar uygulama katmanında değil, verinin olduğu yerde (DB) yapılarak network trafiği azaltılmıştır.

-----

## ⚙️ Proje Bileşenleri (Services)

- **Frontend (Web UI)**: Kullanıcıların cüzdanlarını yönettiği, döviz alım-satımı yaptığı React tabanlı arayüz.

- **Identity/Customer Service**: Kullanıcı kayıt, giriş ve profil yönetimi.

- **Wallet Service**: Cüzdan oluşturma, bakiye yönetimi ve işlem geçmişi.

- **Investment Service**: Güncel döviz kurlarının dış API olan TCMB'den çekilmesi ve gösterimi, alım-satım ve portföy yönetimi.

- **MultiLanguage Service**: Dinamik dil desteği ve Redis üzerinden hızlı çeviri sunumu.

- **API Gateway (Ocelot)**: Güvenlik kontrolü, Aggregation ve Resilience yönetimi.


-----


## 🖼️ Proje İle İlgili Ekran Görüntüleri


### 🏠 Giriş: 


<img src="https://github.com/user-attachments/assets/a120ab55-29de-4498-96cd-22ca13c0928b" width:600 />


### 🪪 Kayıt: 


<img src="https://github.com/user-attachments/assets/50288c5f-049d-41e1-9a97-20d240c5bd6a" width:600/>


## 📋 Ana Sayfa:


<img src="https://github.com/user-attachments/assets/5b5324b8-4574-490f-9a25-c612bfeaf727" width:600/>




<img src="https://github.com/user-attachments/assets/4df05416-952e-424c-afad-5720ad7b3505" width:600/>


#### 👛 Yeni Cüzdan Oluşturma İşlemi:


<img src="https://github.com/user-attachments/assets/4c160657-bc2b-46e9-9307-4b5a416b0704" width:600/>


<img src="https://github.com/user-attachments/assets/c034ba17-e03a-480e-bd37-1340bd681eeb" width:600/>


## ➡️ İşlem Sayfası


<img src="https://github.com/user-attachments/assets/a7474d7e-695b-4d70-bdc9-5a858a78a14c" width:600/>


<img src="https://github.com/user-attachments/assets/29266ef3-7250-49f6-972c-8d288e91d717" width:600/>


## 📈 İşlem Geçmişi Sayfası (Tarih Filtreleme & Sayfalama)


<img src="https://github.com/user-attachments/assets/6fabd2d5-6c3b-4aec-ae0a-87ff4607d812" width:600/>


## 📈 Yatırım İşlemleri Sayfası (Güncel Döviz Kurlarının Çekilmesi ve Alım/Satım İşlemleri)


<img src="https://github.com/user-attachments/assets/97d81ed6-7acd-4d16-9f4e-615ce4857f86" width:600/>


<img src="https://github.com/user-attachments/assets/7b0c714b-4958-4372-9e0b-57d484d5d297" width:600/>


<img src="https://github.com/user-attachments/assets/3e2482c5-d840-4662-bef4-d8c8a6ad0cdf" width:600/>








