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
