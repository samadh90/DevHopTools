using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevHopTools.MVVM.Commands;

namespace DevHopTools.MVVM.ViewModels
{
    public abstract class ViewModelBase : ObservableObject
    {
        public ViewModelBase()
        {
            Type viewModelType = GetType();

            IEnumerable<PropertyInfo> propertyInfos = viewModelType.GetProperties().Where(p => p.PropertyType == typeof(ICommand) || p.PropertyType.GetInterfaces().Contains(typeof(ICommand)));

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                ICommand command = (ICommand)propertyInfo.GetMethod.Invoke(this, null);
                PropertyChanged += (s, e) => command.RaiseCanExecuteChanged();
            }
        }
    }
}