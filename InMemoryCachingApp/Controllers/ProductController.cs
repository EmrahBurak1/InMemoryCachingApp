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

                options.SlidingExpiration = TimeSpan.FromSeconds(10); //SlidingExpiration'a 10 saniye eklenir. Bu şekilde 10 saniye boyunca işlem yapılmazsa memory'den silinir. Bu şekilde cache ömrü belirlenebiliyor.

                options.Priority = CacheItemPriority.High; //Priority özelliği ile cache önceliği belirlenebilir. Low en düşük öncelik, High en yüksek öncelik. NeverRemove seçersek hiç silinmeyecek.

                _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), options); //zaman key'ine DateTime.Now.ToString() değeri set edilir.Bu şekilde ilgili değer memory'ye set edilir. 3.parametre options nesnesi ile cache ömrü belirlenir.
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
            ViewBag.zaman = zamancache;

            return View();
        }
    }
}
