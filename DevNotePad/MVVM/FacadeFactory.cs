using Generic.MVVM.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DevNotePad.MVVM
{
    public static class FacadeFactory
    {
        /// <summary>
        /// Initialisation of the IoC container. 
        /// </summary>
        public static void InitFactory()
        {
            var container = GetContainer();

            // container instance is missing. Create it now!
            if (container == null)
            {
                var appWideResources = Application.Current.Resources;
                IContainer containerInstance = new Container();

                appWideResources.Add("Container", containerInstance);
            }
        }

        public static ContainerFacade Create()
        {
            ContainerFacade facade;

            var container = GetContainer();

            if (container == null)
            {
                throw new Exception("Could not found IoC Container!");
            }

            facade = new ContainerFacade(container);
            return facade;
        }

        private static IContainer? GetContainer()
        {
            IContainer? container = null;
            foreach (var currentInstance in Application.Current.Resources.Values)
            {
                if (currentInstance is IContainer)
                {
                    container = currentInstance as IContainer;
                    break;
                }
            }

            return container;
        }

    }
}
