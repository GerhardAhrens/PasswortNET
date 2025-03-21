﻿namespace PasswortNET.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using ModernBaseLibrary.Extension;

    using ModernUI.MVVM.Base;

    using PasswortNET.Views.ContentControls;

    public static class DialogFactory
    {
        private static Dictionary<Enum, Type> Views = null;

        static DialogFactory()
        {
            RegisterControls();
        }

        public static MenuWorkArea Get(MainButton mainButton)
        {
            MenuWorkArea menuWorkArea = null;
            using (LoadingWaitCursor wc = new LoadingWaitCursor())
            {
                using (LoadingViewTime lvt = new LoadingViewTime())
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    if (Views.ContainsKey(mainButton) == true)
                    {
                        menuWorkArea = new MenuWorkArea(CreateInstanceContent(mainButton));
                        menuWorkArea.WorkContent.Focusable = true;
                        menuWorkArea.WorkContent.Focus();
                    }

                    if (menuWorkArea != null)
                    {
                        menuWorkArea.UsedTime = lvt.Result();
                        menuWorkArea.ButtonDescription = mainButton.ToDescription();
                    }
                }
            }

            return menuWorkArea;
        }


        /// <summary>
        /// Registrieren der Ribbonmenü / Content Controls (Übersichtsdialoge)
        /// </summary>
        private static void RegisterControls()
        {
            try
            {
                if (Views == null)
                {
                    Views = new Dictionary<Enum, Type>();
                    Views.Add(MainButton.Home, typeof(HomeUC));
                    Views.Add(MainButton.Login, typeof(LoginUC));
                    Views.Add(MainButton.ChangePassword, typeof(ChangePasswordUC));
                    Views.Add(MainButton.AppSettings, typeof(AppSettingsUC));
                    Views.Add(MainButton.About, typeof(AboutUC));
                    Views.Add(MainButton.MainOverview, typeof(MainOverviewUC));
                    Views.Add(MainButton.PasswordDetail, typeof(PasswordDetailUC));
                    Views.Add(MainButton.Export, typeof(ExcelXMLExportUC));
                    Views.Add(MainButton.DataSync, typeof(DataSyncUC));
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private static UserControlBase CreateInstanceContent(Enum key)
        {
            Type viewObject = Views[key];

            if (viewObject != null)
            {
                if (viewObject.GetConstructors().Count() >= 1)
                {
                    if (viewObject.GetConstructors()[0].GetParameters().Count() == 1)
                    {
                        ParameterInfo param = viewObject.GetConstructors()[0].GetParameters()[0];
                        if (param.Name == "pageTyp")
                        {
                            return (UserControlBase)Activator.CreateInstance(viewObject, key);
                        }
                        else
                        {
                            return (UserControlBase)Activator.CreateInstance(viewObject, null);
                        }
                    }
                    else
                    {
                        return (UserControlBase)Activator.CreateInstance(viewObject);
                    }
                }
                else
                {
                    return (UserControlBase)Activator.CreateInstance(viewObject,1,null);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
