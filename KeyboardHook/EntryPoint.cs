namespace HookSample
{
    using System.Windows.Input;

    public class EntryPoint
    {
        [System.STAThreadAttribute]
        static void Main()
        {
            using (var hook = new KeyboardHook { SelectedKey = Key.F1 })
            {
                var app = new App(hook);
                app.InitializeComponent();
                app.Run();
            }
        }

    }
}
