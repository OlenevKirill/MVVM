using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;


namespace mvvm
{
    public partial class MainWindow : Window
    {
        private static readonly object _lock = new object();
        private static Random _random = new Random();
        private static string[] _components = { "Табак", "Бумага", "Спички" };
        private static string _tableComponent1;
        private static string _tableComponent2;

        public MainWindow()
        {
            InitializeComponent();
            Agent2.Text = "Посредник кладёт на стол";
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Thread agentThread = new Thread(GoAgent);
            Thread smoker1Thread = new Thread(() => Smoker("Табак"));
            Thread smoker2Thread = new Thread(() => Smoker("Бумага"));
            Thread smoker3Thread = new Thread(() => Smoker("Спички"));

            agentThread.Start();
            smoker1Thread.Start();
            smoker2Thread.Start();
            smoker3Thread.Start();
        }

        private void GoAgent()
        {
            while (true)
            {
                lock (_lock)
                {
                    _tableComponent1 = _components[_random.Next(0, 3)];
                    do
                    {
                        _tableComponent2 = _components[_random.Next(0, 3)];
                    } while (_tableComponent1 == _tableComponent2);

                    Dispatcher.Invoke(() =>
                    {
                        Agent.Text = $"Посредник кладёт на стол: {_tableComponent1} и {_tableComponent2}";
                    });

                    Monitor.PulseAll(_lock);
                    Monitor.Wait(_lock);
                }
            }
        }

        private void Smoker(string component)
        {
            while (true)
            {
                lock (_lock)
                {
                    while (_tableComponent1 == component || _tableComponent2 == component)
                    {
                        Monitor.Wait(_lock);
                    }

                    if ((_tableComponent1 != component && _tableComponent2 != component) && (_tableComponent1 != null && _tableComponent2 != null))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            TextBox.Clear(); 
                            TextBox.Text += $"Курильщик с {component} скручивает сигарету и курит.\n";
                        });

                        _tableComponent1 = null;
                        _tableComponent2 = null;

                        Monitor.PulseAll(_lock);
                    }
                }

                Thread.Sleep(1000); 
            }
        }
    }
}

