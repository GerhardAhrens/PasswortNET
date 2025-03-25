﻿namespace PasswortNET.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using ModernBaseLibrary.Extension;

    using ModernUI.MVVM.Base;

    using PasswortNET.DialogNavigation;
    using PasswortNET.Views.ContentControls;

    public class DialogFactory : IDialogFactory
    {
        private static Dictionary<Enum, Type> Views = null;

        static DialogFactory()
        {
            RegisterControls();
        }

        public static FactoryResult Get(MainButton mainButton)
        {
            FactoryResult resultContent = null;
            using (LoadingWaitCursor wc = new LoadingWaitCursor())
            {
                using (LoadingViewTime lvt = new LoadingViewTime())
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    if (Views.ContainsKey(mainButton) == true)
                    {
                        UserControlBase resultInstance = CreateInstanceContent(mainButton, null);
                        resultContent = new FactoryResult(resultInstance);
                        resultContent.WorkContent.Focusable = true;
                        resultContent.WorkContent.Focus();
                        resultContent.UsedTime = lvt.Result();
                        resultContent.ButtonDescription = mainButton.ToDescription();
                    }
                }
            }

            return resultContent;
        }

        public static FactoryResult Get(MainButton mainButton, IFactoryArgs changeViewArgs)
        {
            FactoryResult resultContent = null;
            using (LoadingWaitCursor wc = new LoadingWaitCursor())
            {
                using (LoadingViewTime lvt = new LoadingViewTime())
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    if (Views.ContainsKey(mainButton) == true)
                    {
                        UserControlBase resultInstance = CreateInstanceContent(mainButton, changeViewArgs);
                        resultContent = new FactoryResult(resultInstance);
                        resultContent.WorkContent.Focusable = true;
                        resultContent.WorkContent.Focus();
                        resultContent.UsedTime = lvt.Result();
                        resultContent.ButtonDescription = mainButton.ToDescription();
                    }
                }
            }

            return resultContent;
        }


        /// <summary>
        /// Registrieren der Content Controls
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

        private static UserControlBase CreateInstanceContent(Enum key, IFactoryArgs changeViewArgs)
        {
            Type viewObject = Views[key];

            if (viewObject != null && viewObject.IsAssignableTo(typeof(UserControlBase)) == true)
            {
                if (viewObject.GetConstructors().Count() >= 1)
                {
                    if (viewObject.GetConstructors()[0].GetParameters().Count() == 1)
                    {
                        ParameterInfo param = viewObject.GetConstructors()[0].GetParameters()[0];
                        Type typParam = Type.GetType($"{param.ParameterType.Namespace}.{param.ParameterType.Name}");
                        if (param != null && typParam.GetInterfaces().Contains(typeof(IFactoryArgs)) == true)
                        {
                            return (UserControlBase)Activator.CreateInstance(viewObject, changeViewArgs);
                        }
                        else
                        {
                            throw new NotSupportedException($"Es wurde kein Konstruktor angegeben. Es muß ein Kontruktor der 'IFactoryArgs' implementiert vorhanden sein. Control: {key.ToDescription()}; Object: {viewObject.GetFriendlyTypeName()}");
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
                throw new NotSupportedException($"Das UserControl implementiert kein 'UserControlBase'. Control: {key.ToDescription()}; Object: {viewObject.GetFriendlyTypeName()}");
            }
        }
    }
}
