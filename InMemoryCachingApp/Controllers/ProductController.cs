using InMemoryCachingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryCachingApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IMemoryCache _memoryCache; //IMemomryCache üzerinden ilgili methodlara erişilerek inmemorycache yapılabilir.
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            //Not: Serialize yapıldıktan sonra png, pdf gibi bir çok veri memory'de tutulabilir. 

            //1.Yol
            //if(String.IsNullOrEmpty(_memoryCache.Get<string>("zaman"))) //zaman key'i memory'de var mı yok mu kontrol edilir. Yoksa memory'ye set edilir.
            //{
            //    _memoryCache.Set<string>("zaman", DateTime.Now.ToString()); //zaman key'ine DateTime.Now.ToString() değeri set edilir.Bu şekilde ilgili değer memory'ye set edilir.
            //}

            //2.Yol
            if (!_memoryCache.TryGetValue("zaman", out string zamancache)) //zaman key'i memory'de var mı yok mu kontrol edilir. Eğer varsa zamancache değişkenine atar. Bu değişken üzerinden veriyi Index üzerinde kullanabiliriz. Yoksa memory'ye set edilir.
            {
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions(); //MemoryCacheEntryOptions nesnesi oluşturulur.
                options.AbsoluteExpiration = DateTime.Now.AddSeconds(10); //AbsoluteExpiration'a 10 saniye eklenir. Bu şekilde 10 saniye sonra memory'den silinir. Bu şekilde cache ömrü belirlenebiliyor.

                //options.SlidingExpiration = TimeSpan.FromSeconds(10); //SlidingExpiration'a 10 saniye eklenir. Bu şekilde 10 saniye boyunca işlem yapılmazsa memory'den silinir. Bu şekilde cache ömrü belirlenebiliyor.

                options.Priority = CacheItemPriority.High; //Priority özelliği ile cache önceliği belirlenebilir. Low en düşük öncelik, High en yüksek öncelik. NeverRemove seçersek hiç silinmeyecek.

                options.RegisterPostEvictionCallback((key,value,reason, state)=> { 
                    _memoryCache.Set<string>("callback", $"{key} -> {value} => sebep:{reason}"); //Zaman key'ine sahip value yani tarih bilgimiz hangi sebepten silindiyse reason kısmında da o gösterilir.
                });//Bir datanın hangi sebepten memory'den düştüğünü bu method ile tespit edebiliyoruz. Görebilmek için de cache'e yazdık.

                _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), options); //zaman key'ine DateTime.Now.ToString() değeri set edilir.Bu şekilde ilgili değer memory'ye set edilir. 3.parametre options nesnesi ile cache ömrü belirlenir.

                Product p = new Product { Id = 1, Name = "Kalem", Price = 200 };

                _memoryCache.Set<Product>("product:1", p); //Bu şekilde bir product nesnesini de cache'leyebiliriz. Herhangi bir serialize yazpmaya gerek yok. Serialize işlemi in memory cache de otomatik olarak gerçekleşiyor.
                //_memoryCache.Set<double>("money", 100.99); //Bu şekilde istediğimiz tüm değerleri cache'leyebiliriz.


            }
            return View();
        }

        public IActionResult Show()
        {
            //_memoryCache.Remove("zaman"); //zaman key'ine ait değeri memory'den siler.

            //_memoryCache.GetOrCreate<string>("zaman", entry =>
            //{
            //    return DateTime.Now.ToString(); //zaman key'ine ait değeri varsa memory'den getirir. Eğer yoksa yaratır entry üzerinden değeri set eder.
            //});

            //ViewBag.zaman = _memoryCache.Get<string>("zaman"); //zaman key'ine ait değeri memory'den getirir.

            _memoryCache.TryGetValue("zaman", out string zamancache); //Eğer zaman key'i memory'de varsa zamancache değişkenine atar. Bu değişken üzerinden veriyi kullanabiliriz.
            _memoryCache.TryGetValue("callback", out string callback); //Eğer callback key'i memory'de varsa callback değişkenine atar. Bu değişken üzerinden veriyi kullanabiliriz.

            ViewBag.zaman = zamancache;
            ViewBag.callback = callback;
            ViewBag.product = _memoryCache.Get<Product>("product:1");
            return View();
        }
    }
}
