using DX.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

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
                str = TCP_SourcePort + "--------->" + TCP_DestinationPort;
            }
            else
            {
                str = TCP_DestinationPort + "<---------" + TCP_SourcePort;
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
            string str = new DateTime(621355968000000000).AddMilliseconds(time / 1000).ToString("yyyy-MM-dd HH:mm:ss.fff");
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
                return Tools.BytesToShowBytes(byteArray);
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
}
