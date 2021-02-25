using DX.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DX.Models
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListViewItem item = (ListViewItem)value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item) + 1;
            return index.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class StringToStringLengthConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string)value;
            return str.Length;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PortToCMDRESConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int TCP_DestinationPort = (int)value;
            string str = "";
            if (TCP_DestinationPort == 80)
            {
                str = "CMD";
            }
            else
            {
                str = "RES";
            }
            return str;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PortToDestinationConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int TCP_SourcePort = (int)values[0];
            int TCP_DestinationPort = (int)values[1];
            string str = "";

            if (TCP_DestinationPort == 80)
            {
                str = TCP_SourcePort + "------>" + TCP_DestinationPort;
            }
            else
            {
                str = TCP_DestinationPort + "<------" + TCP_SourcePort;
            }
            return str;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class UlongToDateTimeConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ulong time = (ulong)value;
            string str = new DateTime(621355968000000000).AddMilliseconds(time / 1000).ToString(ConfigurationManager.AppSettings["TimeFormat"]);
            return str;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BytesFormatConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value!=null)
            {
                HttpModel http = value as HttpModel;
                byte[] byteArray = System.Text.Encoding.Default.GetBytes(http.Content);
                string str = Tools.BytesToShowBytes(byteArray);

                if (str.Length > 2000)
                {
                    str = str.Remove(2000, str.Length - 2000);
                }
                return str;
            }
            else
            {
                return "";
            }
            
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IPConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                HttpModel http = value as HttpModel;
                string str = "IP: FROM " + http.IP_SourceAddress + ":" + http.TCP_SourcePort
                                    + "     ------->     TO " + http.IP_DestinationAddress + ":" + http.TCP_DestinationPort;
                return str.Replace(',','.');
            }
            else
            {
                return "";
            }

        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ClourConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListViewItem item = (ListViewItem)value;
            ListView listView =ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            if (listView!=null)
            {
                var obj = listView.ItemContainerGenerator.ItemFromContainer(item);

                ErrorCode errorCode = (obj as HttpModel).ErrorCode;
                switch (errorCode)
                {
                    case ErrorCode.NORMAL:
                        return Brushes.White;

                    case ErrorCode.NET_TIMEOUT:
                        return Brushes.Yellow;

                    case ErrorCode.NET_DELAY_RESPONSE:
                        return Brushes.Red;

                    case ErrorCode.NET_NO_RESPONSE:
                        return Brushes.Purple;

                    case ErrorCode.HTTP_ERROR:
                        return Brushes.Yellow;

                    case ErrorCode.RESPONSE_ERROR:
                        return Brushes.Red;

                    default:
                        return Brushes.Red;
                }
            }
            else
            {
                return Brushes.White;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StateConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((StateCode)value)
            {
                case StateCode.NORMAL:
                    return Brushes.Green;

                case StateCode.WARNING:
                    return Brushes.Yellow;

                case StateCode.ERROR:
                    return Brushes.Red;

                default:
                    return Brushes.Green;
            }

        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringLimitConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int stringLimit = int.Parse(ConfigurationManager.AppSettings["StringLimit"]);
            int displayLimit = int.Parse(ConfigurationManager.AppSettings["DisplayLimit"]);

            string str = (string)value;
            if (str.Length > stringLimit)
            {
                str = str.Remove(displayLimit, str.Length - displayLimit) +"......";
            }
            return str;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IE2CountConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<HttpModel> ie = (IEnumerable<HttpModel>)value;
            return ie.Count();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringLimitForFliterWindowConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string)value;
            if (str.Length > 20)
            {
                str = str.Remove(20, str.Length - 20) + "......";
            }
            
            return str;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
