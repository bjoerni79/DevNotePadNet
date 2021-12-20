using DevNotePad.MVVM;
using DevNotePad.Service;
using Generic.MVVM;
using Generic.MVVM.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevNotePad.ViewModel
{
    public abstract class AbstractViewModel : GenericViewModel
    {
        public AbstractViewModel()
        {

        }

        protected IDialogService GetDialogService()
        {
            var facade = GetFacade();
            var dialogService = facade.Get<IDialogService>(Bootstrap.DialogServiceId);
            if (dialogService == null)
            {
                throw new Exception("Cannot access DialogService");
            }

            return dialogService;
        }

        protected IIoService GetIoService()
        {
            var facade = GetFacade();
            var ioService = facade.Get<IIoService>(Bootstrap.IoServiceId);
            if (ioService == null)
            {
                throw new Exception("Cannot access I/O Service");
            }

            return ioService;
        }

        protected ContainerFacade GetFacade()
        {
            var facade = FacadeFactory.Create();
            if (facade == null)
            {
                throw new Exception("Cannot access MVVM facade");
            }

            return FacadeFactory.Create();
        }

        protected void ShowError(Exception exception, string component)
        {
            var dialogService = GetDialogService();
            dialogService.ShowErrorDialog(exception, component);
        }
    }
}
