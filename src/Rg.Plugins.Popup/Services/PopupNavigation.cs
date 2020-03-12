using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;

namespace Rg.Plugins.Popup.Services
{
    public static class PopupNavigation
    {
        private static IPopupNavigation _popupNavigation;

        public static IPopupNavigation Instance
        {
            get
            {
                if(_popupNavigation == null)
                    _popupNavigation = new PopupNavigationImpl();

                return _popupNavigation;
            }
        }

       
    }
}
