using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using HookCrashPlugin.Windows;

namespace HookCrashPlugin
{
    using System;
    using Dalamud.Game;
    using Dalamud.Hooking;
    using HookCrashPlugin.Windows;

    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "HookCrashPlugin";
        private const string CommandName = "/testHookCrash";

        private DalamudPluginInterface PluginInterface { get; init; }
        
        private ICommandManager CommandManager { get; init; }
        
        internal IGameInteropProvider GameInteropProvider { get; init; }
        public readonly WindowSystem WindowSystem = new("HookCrashPlugin");

        private MainWindow MainWindow { get; init; }
        
        
        private nint MemoryAddress { get; set; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager,
            [RequiredVersion("1.0")] IGameInteropProvider gameInteropProvider,
            [RequiredVersion("1.0")] ISigScanner sigScanner)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.GameInteropProvider = gameInteropProvider;


            MainWindow = new MainWindow(this);
            
            WindowSystem.AddWindow(MainWindow);
            
            // address of HandleItemHover
            this.MemoryAddress = sigScanner.ScanText("E8 ?? ?? ?? ?? 48 8B 5C 24 ?? 48 89 AE ?? ?? ?? ?? 48 89 AE ?? ?? ?? ??");

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "A useful message to display in /xlhelp"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenMainUi += () => MainWindow.IsOpen ^= true;
#if DEBUG
            this.MainWindow.IsOpen = true;
#endif
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            
            MainWindow.Dispose();
            
            this.CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            MainWindow.IsOpen = true;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        internal IDisposable CreateHook()
        {
            return new HookClass(this.GameInteropProvider, this.MemoryAddress);
        }
    }
}
