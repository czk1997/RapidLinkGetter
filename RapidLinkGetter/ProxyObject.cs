using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RapidLinkGetter
{
    class ProxyObject
    {

        public void showMessage(string msg)
        {//Read Note

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                Window2 windows = new Window2();
                windows.setResultList(msg);
                windows.ShowDialog();
            }));

        }
    }
}
