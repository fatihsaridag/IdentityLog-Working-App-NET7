# Loglar #
Loglar uygulamada runtime yaşanan problemleri yönetebilmemizi kolaylaştırmak için kullandığımız bir yapılanmadır.

## Log mekanizması ne gibi ayrıntı içermelidir ? ##

1) Sistemi kim kullanıyordu ? 
2) Arıza kodun neresinde gerçekleşmelidir ? 
3) Hata Kodu ? 
4) Ne zaman oldu ?  
5) Uygulama neden başarısız oldu ? 

## Log mekanizmasında neler kaydedilmelidir ? ##
1. Exceptionlar 
2. Veri Değişkenleri 
3. Şüpheli Etkinlikler : Başarısız kimlik doğrulaması , Kısıtlı verilere erişme girişimleri , gecersiz parametreler
4. Requestler : Tarih ve saat olarak kayıt edilmelidir
5. Kullanıcı Bilgileri
6. Kısa açıklama

## Ekstralar ## 

Hata mesajlarını özelleştirebilirsiniz..
Log kayıtları görselleştirilmelidir.

## Loglar nerede ve nasıl tutulmalıdır ? ##
1. Loglar console üzerinde tutulabilir
2. Loglar analiz edilmesi için harici dosyalarda tutulabilir. 
3. Loglar veritabanlarında fiziksel olarak saklanması gerekebilir.

## Serilog nedir ? ##
Uygulamadaki logları Console , File, Veritabanlarına , Seq vs kolayca aktarmamızı sağlayan bir kütüphanedir.

## Serilog Kütüphanesi bazı parametreleri ##
1. Logger log = new LoggerConfiguration() => Log ayarlarımızı yapmamızı sağlar ve serilog.AspNetCore nugget paketine ihtiyaç duyar
2. WriteTo.Console , WriteTo.File , WriteTo.MSSqlServer(..) => WriteTo bizim loglamayı nereye yapacağımızı gösterir.
3. WriteTo.MSSqlServer(connectionString : builder.Configuration.GetConnectionString("ConnectionDatabasemiz")) , => Burada artık MSSql ayarlarına giriyoruz connectionstring de hangi database oldugunu belirttik.
4. MSSqlServer ayarlarından sinkOptions : new MSSqlServerSinkOptions{AutoCreateTable = true , TableName = "logs"} , => Bu tablonun otomatik oluşturulmasını istedik ve tablonun adının logs olduğunu belirttik. 
5.  Daha sonra User propertysini eklemek için custom işlemleri gerçekleştiriyoruz. Githuba proje olarak atıyor olacağım . 
6.  Peki bu userName ' i nasıl yakalayacağız ? 


## UserName yakalamak için ##

Kullanıcıdan gelen her isteğin tetiklendiği isteğin geldiği noktada biz araya bir middleware oluşturup gireceğiz. Bu middleware de gelen istekte Authentication olmuş kullanıcı bilgisi varsa  ve bu kullanıcının name bilgisi var ise bunu yakalayıp username propertysine karşılık logun contextine atıyoruz. Buraya attığımızda da custom oluşturduğumuz CustomUserNameColumn da varsa yakalanacak yoksa yakalanmayacak. Eğer bizim middleware miz tetiklenirse request sürecinde logEvent.Properties de username olacak ve biz ona göre o logu yapan kullanıcıya dair bilgiyi rahatlıkla tutuyoruz.  

# Projede Loglamaya dair yapılan İşlemler #
## 1. Program.cs içerisinde loglama ayarlarımızı gerçekleştiriyoruz bir tablo ve sütun oluşturuyoruz. Nerede bu logları göstereceksek bu ayarları yapıyoruz. MsSQL log ayarlarını yapıyoruz ve ekstra bir sütun oluşturacağımız için User Sütununu ekliyoruz. ##
![7](https://user-images.githubusercontent.com/68101192/210206045-dd8014a9-8c80-432a-9f67-6a6124ddebf9.png)

## 2. Loglama sürecindeki property değerlerindne UserName'i manuel olarak oluşturmak için Abstract sınıfından türeyen bir sınıf oluşturuyoruz ve bu sınıf içerisinde de logu veritabanına kaydediyoruz.  ##
![8](https://user-images.githubusercontent.com/68101192/210206466-8abb05a6-2514-45f1-8f34-f25a43242c3b.png)

## 3. Cookie üzerinden yukarıda belirtmiş olduğumuz sınıfa bir UserName değeri getirmek için bir middleware yazmamız gerekiyor bu middleware ile User.Identity.UserName değerini elde ediyor ve sonra bu sınıfta kullanıyoruz. Aşağıdaki gibi bir middleware..  ##
![9](https://user-images.githubusercontent.com/68101192/210206612-d4181bfb-5309-4cf0-b34b-d083d5c032ce.png)

## 4. Home/Index  Action içerisine bir loglama işlemi yaptım artık bu Authenticate kontrolü yapılan bu sayfaya login olduğumuzda loglama işlemlerini görebiliriz. ##
![2](https://user-images.githubusercontent.com/68101192/210206826-daea69a1-73a8-41ac-9e27-1c577bff9d08.png)
![6](https://user-images.githubusercontent.com/68101192/210206755-0d3aaf66-3f46-4575-aeec-95e23bada294.png)

## 5. MsSQL de yapılan loglama işlemini görelim .. ##
![1](https://user-images.githubusercontent.com/68101192/210206804-13b5a56a-cd7b-454d-8cc9-6a8b17a20961.png)

## 6. Docker ile Seq'i ayağa kaldırarak Seq Arayüzünde loglamala işlemlerini görelim : Komut => docker run --name Seq -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest ##
![3](https://user-images.githubusercontent.com/68101192/210206888-4baccc06-2185-4607-8c4f-28830136322e.png)

## 7. Seq Üzerinde yaptığımız loglama işlemini ve ayrıntılarını rahatlıkla görebilir ve ihtiyaç doğrultusunda fitreleyebiliriz. ##
![4](https://user-images.githubusercontent.com/68101192/210207151-a42b2829-808b-4347-a501-91408cb141ca.png)
![5](https://user-images.githubusercontent.com/68101192/210207154-319f9750-00b1-4622-bdbd-1d22b067cb6a.png)


