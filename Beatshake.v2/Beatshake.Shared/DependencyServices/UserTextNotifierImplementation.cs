using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Beatshake.DependencyServices;

    class UserTextNotifierImplementation : IUserTextNotifier
    {

        public async Task Notify(string message)
        {

        //Source: http://stackoverflow.com/questions/17274865/messagedialog-needs-to-wait-for-user-input
        var tsc = new TaskCompletionSource<bool>();
            var dialogTask = tsc.Task;

        await
                Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        var dialog = new MessageDialog(message);
                        dialog.Commands.Add(new UICommand("OK"));
                        
                        var result = await dialog.ShowAsync();

                        tsc.SetResult(true);
                    });

            var res = await dialogTask;
            return;

        }

        public async Task Notify(Exception exception)
        {
            await Notify(exception.ToString());
        }
    }
