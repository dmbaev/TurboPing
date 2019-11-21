using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TurboPing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer = new DispatcherTimer();
        private List<_stat> _StatusLog = new List<_stat>(); 
        private _Succes_stat_class _Succes_stat = new _Succes_stat_class();

        private class _stat
        {
            public string _status { get; set; }
            public int _count { get; set; }
        }

        private class _Succes_stat_class
        {
            public long max { get; set; }
            public long all_time { get; set; } //ms
        }

        public MainWindow()
        {
            InitializeComponent();
            _timer.Interval = new TimeSpan(0,0,1);
            _timer.Tick += _PingSend;
        }


        private void _Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            _StatusLog.Clear();
            _Succes_stat.max = 0;
            _Succes_stat.all_time = 0;
            if (_timer.IsEnabled)
            {
                _timer.Stop();
               
            }
            else
            {
                _TextBlock.Text = "";
                _timer.Interval = new TimeSpan(0, 0, int.Parse(_timespan.Text));
                _timer.Start();
                _TabControl.SelectedItem = _TabMain;
            }
            //_TabSettings.IsEnabled = !_TabSettings.IsEnabled;
            //_timespan.IsEnabled = !_timespan.IsEnabled;
            //_timeout.IsEnabled = !_timeout.IsEnabled;
            _host.IsEnabled = !_host.IsEnabled;
        }

        private async void _PingSend(object sender, object e)//, PingCompletedEventArgs e)
        {
            string _r = string.Empty;
            string _r_s = string.Empty;
            _timer.Stop();

            if (_timer.Interval != new TimeSpan(0, 0, int.Parse(_timespan.Text)))
            {
                _timer.Interval = new TimeSpan(0, 0, int.Parse(_timespan.Text));
            }

            try
            {
                Ping _pingSender = new Ping();

                var _pr = await _pingSender.SendPingAsync(_host.Text, int.Parse(_timeout.Text));

                if (_StatusLog.Where(w => w._status == _pr.Status.ToString()).Count() == 0)
                {
                    _StatusLog.Add(new _stat { _status = _pr.Status.ToString(), _count = 1 });
                }
                else
                {
                    var _my_st = _StatusLog.Where(w => w._status == _pr.Status.ToString()).First();
                    _my_st._count++;
                }

                if (_pr.Status == IPStatus.Success)
                {
                    _Succes_stat.all_time = _Succes_stat.all_time + _pr.RoundtripTime;

                    if (_Succes_stat.max < _pr.RoundtripTime)
                    {
                        _Succes_stat.max = _pr.RoundtripTime;
                    }

                  
                }

                foreach (var item in _StatusLog)
                {
                    _r = _r + item._status + ": " + item._count + "\n";
                }

                var _c = _StatusLog.Where(w => w._status == IPStatus.Success.ToString()).First()._count;
                if (_c>0)
                {
                    _r_s = IPStatus.Success.ToString() + " max: " + _Succes_stat.max + "\n" + IPStatus.Success.ToString() + " middle: " + (_Succes_stat.all_time / _c).ToString();
                }

            }
            catch (Exception ex)
            {
                _r_s = "";
                _r = ex.Message;
            }
            finally
            {
                _timer.Start();
            }
            _TextBlock.Text = "Host: " +_host.Text + "\n" + _r_s + "\n" + _r;
        }
    }
}
