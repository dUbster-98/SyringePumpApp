using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using syringePumpTest1.ViewModels;
using SyringePumpTest1.Service;
using SyringePumpTest1.Services;
using SyringePumpTest1.ViewModels;
using static SyringePumpTest1.Service.ISerialService;
using static SyringePumpTest1.Services.ITextBoxService;

namespace syringePumpTest1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
            this.InitializeComponent();
        }

        public new static App Current => (App)Application.Current;
        
        public IServiceProvider Services { get; private set; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddTransient(typeof(MainViewModel));
            services.AddTransient(typeof(SerialSetViewModel));

            services.AddTransient<IIniSetService, IniSetService>();
            services.AddSingleton<ISerialService, SerialService>();
            services.AddTransient<ITextBoxService, TextBoxService>();

            return services.BuildServiceProvider();
        }
    }

}
