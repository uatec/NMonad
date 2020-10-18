using System.Threading.Tasks;
using NHotkey.WindowsForms;
using Keys = System.Windows.Forms.Keys;

namespace MaterialWindows.TaskBar
{
    public class SysTrayApp
    {
        private ThisApplicationContext applicationContext;
        private Actions _actions;

        public SysTrayApp(ThisApplicationContext applicationContext, Actions actions)
        {
            this.applicationContext = applicationContext;
            _actions = actions;
        }

        public void Init()
        {
            
            var superkey = Keys.Control | Keys.Alt;

            HotkeyManager.Current.AddOrReplace("CycleLayouts", superkey | Keys.Space, _actions.cycleLayouts);
            HotkeyManager.Current.AddOrReplace("ReverseCycleLayouts", superkey | Keys.Shift | Keys.Space, _actions.reverseCycleLayouts);
            
            HotkeyManager.Current.AddOrReplace("CycleMainWindow", superkey | Keys.Up, _actions.cycleMainPane);
            HotkeyManager.Current.AddOrReplace("ReverseCycleMainWindow", superkey | Keys.Down, _actions.reverseCycleMainPane);

            HotkeyManager.Current.AddOrReplace("IncreaseMainPane", superkey | Keys.H, _actions.increaseMainPane);
            HotkeyManager.Current.AddOrReplace("DecreaseMainPane", superkey | Keys.Shift | Keys.H, _actions.decreaseMainPane);
            
            HotkeyManager.Current.AddOrReplace("CycleAssignedScreen", superkey | Keys.Right, _actions.cycleAssignedScreen);
            HotkeyManager.Current.AddOrReplace("ReverseCycleAssignedScreen", superkey | Keys.Left, _actions.reverseCycleAssignedScreen);

            HotkeyManager.Current.AddOrReplace("DumpWindowList", superkey | Keys.K, _actions.dumpWindowList);
            // HotkeyManager.Current.AddOrReplace("Exit", superkey | Keys.Q, (s, e) => applicationContext.ExitThread());
        }
    }
}