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
        private RapidLinkWindow instance;

        public ProxyObject(RapidLinkWindow window)
        {
            instance = window;
        }

        public void SetInstance(RapidLinkWindow window)
        {
            instance = window;
        }
        public void showMessage(string msg)
        {//Read Note

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                instance.setResultList(msg);
            }));

        }
    }
}
