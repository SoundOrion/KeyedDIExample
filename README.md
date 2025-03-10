以下に **完全な .NET 8 のコンソールアプリケーション** として動作する `AddKeyedSingleton` のサンプルを作成しました！ 🚀

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace KeyedDIExample
{
    public interface IHolidaysProvider
    {
        string GetHolidayName();
    }

    // 日本向けの実装
    public class JapanHolidaysProvider : IHolidaysProvider
    {
        public string GetHolidayName() => "天皇誕生日";
    }

    // アメリカ向けの実装
    public class USHolidaysProvider : IHolidaysProvider
    {
        public string GetHolidayName() => "Independence Day";
    }

    // 祝日サービス（コンストラクタインジェクション）
    public class HolidayService
    {
        private readonly IHolidaysProvider _holidaysProvider;

        public HolidayService([FromKeyedServices("US")] IHolidaysProvider holidaysProvider)
        {
            _holidaysProvider = holidaysProvider;
        }

        public void PrintHoliday()
        {
            Console.WriteLine($"Holiday: {_holidaysProvider.GetHolidayName()}");
        }
    }

    class Program
    {
        static void Main()
        {
            using var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    // デフォルトのプロバイダ（日本用）
                    services.AddSingleton<IHolidaysProvider, JapanHolidaysProvider>();

                    // キー付き登録（日本、アメリカ）
                    services.AddKeyedSingleton<IHolidaysProvider>("JP", new JapanHolidaysProvider());
                    services.AddKeyedSingleton<IHolidaysProvider>("US", new USHolidaysProvider());

                    // HolidayService を DI に追加
                    services.AddSingleton<HolidayService>();
                })
                .Build();

            // DIコンテナからサービスを取得
            var provider = host.Services;

            // デフォルトの祝日プロバイダ
            var defaultProvider = provider.GetRequiredService<IHolidaysProvider>();
            Console.WriteLine($"Default Provider: {defaultProvider.GetHolidayName()}"); // 天皇誕生日

            // キー付きの祝日プロバイダ取得
            var japanProvider = provider.GetRequiredKeyedService<IHolidaysProvider>("JP");
            var usProvider = provider.GetRequiredKeyedService<IHolidaysProvider>("US");

            Console.WriteLine($"Japan Provider: {japanProvider.GetHolidayName()}");     // 天皇誕生日
            Console.WriteLine($"US Provider: {usProvider.GetHolidayName()}");           // Independence Day

            // HolidayService を取得して実行
            var holidayService = provider.GetRequiredService<HolidayService>();
            holidayService.PrintHoliday(); // Holiday: Independence Day
        }
    }
}
```

---

### **📝 説明**
1. **`IHolidaysProvider`** インターフェースを作成。
2. **`JapanHolidaysProvider`**（日本の祝日）と **`USHolidaysProvider`**（アメリカの祝日）を実装。
3. **`HolidayService`** を作成し、`FromKeyedServices("US")` を使って **アメリカの祝日プロバイダを自動で注入**。
4. **`AddKeyedSingleton` を使い、キー付きで異なる `IHolidaysProvider` を登録**。
5. **`GetRequiredKeyedService<T>(key)` を使って、キーごとにサービスを取得**。
6. **ホストを使って DI コンテナを構築し、サービスを実行**。

---

### **📌 実行結果**
```
Default Provider: 天皇誕生日
Japan Provider: 天皇誕生日
US Provider: Independence Day
Holiday: Independence Day
```
🔹 **デフォルトの `IHolidaysProvider` は日本の祝日を返す。**  
🔹 **キー `"JP"` は日本の祝日、キー `"US"` はアメリカの祝日を返す。**  
🔹 **`HolidayService` は `"US"` の `IHolidaysProvider` を使い、アメリカの祝日を出力する。**

---

### **✨ まとめ**
✅ **`AddKeyedSingleton` を使うことで、同じインターフェースの異なる実装をキーで識別可能！**  
✅ **`GetRequiredKeyedService<T>(key)` で特定のキーのサービスを取得できる！**  
✅ **`FromKeyedServices("key")` を使えば、コンストラクタインジェクションでもキー付きのサービスを利用可能！**  

**これを活用すれば、環境や設定によって異なるサービスの実装を簡単に切り替えられます！** 🚀