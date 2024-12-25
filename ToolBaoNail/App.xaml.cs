using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;
using ToolBaoNail.Data;
using ToolBaoNail.ViewController;
using ToolBaoNail.ViewModel;

namespace ToolBaoNail
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        //public App()
        //{
        //    // Đăng ký các dịch vụ và ViewModels
        //    var services = new ServiceCollection();

        //    // Đăng ký DbContext với chuỗi kết nối từ appsettings.json
        //    services.AddDbContext<ApplicationDbContext>(options =>
        //        options.UseSqlServer("Server=localhost;Database=BaoNail_BackEnd_API_v2;Trusted_Connection=True;TrustServerCertificate=True"));

        //    services.AddScoped<IStateScraperService, StateScraperService>();  // Đăng ký dịch vụ lấy dữ liệu
        //    //services.AddScoped<ApplicationDBContext>();  // Đăng ký ApplicationDBContext
        //    services.AddScoped<CrawlControlsViewModel>();  // Đăng ký ViewModel
        //    services.AddScoped<CrawlControls>();  // Đăng ký cửa sổ chính

        //    _serviceProvider = services.BuildServiceProvider();
        //}

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Đăng ký ApplicationDBContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer("Server=localhost;Database=BaoNail_BackEnd_API_v2;Trusted_Connection=True;TrustServerCertificate=True"));

            // Đăng ký HttpClient
            services.AddHttpClient();

            // Đăng ký StateScraperService
            services.AddTransient<IStateScraperService, StateScraperService>();

            // Đăng ký CrawlControls
            services.AddTransient<CrawlControls>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Đặt mức độ ưu tiên CPU và RAM cho ứng dụng

            SetHighPriority();

            var crawlControls = _serviceProvider.GetService<CrawlControls>();
            crawlControls.Show();

        }

        /// <summary>
        /// Đặt mức độ ưu tiên CPU và RAM cho ứng dụng
        /// </summary>
        private void SetHighPriority()
        {
            try
            {
                // Lấy tiến trình hiện tại
                var currentProcess = Process.GetCurrentProcess();

                // Đặt mức độ ưu tiên cao
                currentProcess.PriorityClass = ProcessPriorityClass.AboveNormal; // Hoặc ProcessPriorityClass.RealTime   or .High nếu cần
            }
            catch (Exception ex)
            {
                // Ghi log nếu có lỗi
                Debug.WriteLine($"Error setting process priority: {ex.Message}");
            }
        }

    }

}
