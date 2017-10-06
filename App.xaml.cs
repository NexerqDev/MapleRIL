using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MapleRIL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex singleInstanceMutex = null;
        private string appMutexId = @"NexerqDev-MapleRIL";

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
#if !DEBUG
            e.Handled = true;
            MessageBox.Show("A fatal error has occured in MapleRIL. Please screenshot and report this error to Nicholas Tay <nexerq@gmail.com>!\n\n" + e.Exception.ToString(), "MapleRIL - Fatal Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
#endif
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            bool mutexCreated;
            singleInstanceMutex = new Mutex(true, appMutexId, out mutexCreated);
            if (!mutexCreated)
            {
                MessageBox.Show("MapleRIL is already running. Only one instance is allowed at a time.");
                Environment.Exit(1);
                return;
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (singleInstanceMutex != null)
                singleInstanceMutex.ReleaseMutex();

            base.OnExit(e);
        }
    }
}
