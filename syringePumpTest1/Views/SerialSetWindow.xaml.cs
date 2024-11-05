using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using syringePumpTest1;
using syringePumpTest1.ViewModels;

using SyringePumpTest1.ViewModels;

namespace SyringePumpTest1.Views
{
    /// <summary>
    /// SerialSetWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SerialSetWindow : Window
    {
        public SerialSetWindow()
        {
            InitializeComponent();
            this.DataContext = App.Current.Services.GetService(typeof(SerialSetViewModel));
            (DataContext as SerialSetViewModel).CloseRequested += () => this.Close();
        }
    }
}
