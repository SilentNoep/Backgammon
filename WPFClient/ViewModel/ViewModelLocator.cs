using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Windows.Navigation;
using WPFClient.Infra;
using WPFClient.Services;

namespace WPFClient.ViewModel
{

    public class ViewModelLocator
    {

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            //ViewModels
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SignInViewModel>();
            SimpleIoc.Default.Register<RegisterViewModel>();
            SimpleIoc.Default.Register<LobbyViewModel>();
            SimpleIoc.Default.Register<GameViewModel>();





            //Services
            SimpleIoc.Default.Register<IChatService, ChatService>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<IServerService, ServerService>();

            var navService = new Services.NavigationService();
            SimpleIoc.Default.Register<GalaSoft.MvvmLight.Views.INavigationService>(() => navService);




        }

        public MainViewModel MainVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        public SignInViewModel SignInVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SignInViewModel>();
            }
        }
        public RegisterViewModel RegisterVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RegisterViewModel>();
            }
        }
        public LobbyViewModel LobbyVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LobbyViewModel>();
            }
        }
        public GameViewModel GameVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<GameViewModel>(Guid.NewGuid().ToString());
            }
        }




        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}