using System;
using System.Numerics;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace HookCrashPlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin Plugin;
    private int counter;
    private int iterations = 1;

    public MainWindow(Plugin plugin) : base(
        "HookCrashPlugin", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.Plugin = plugin;
    }

    public void Dispose()
    {
    }

    public override void Draw()
    {
        ImGui.TextUnformatted("Counter: ");
        ImGui.SameLine();
        ImGui.TextUnformatted(this.counter.ToString());
        
        ImGui.Separator();

        ImGui.InputInt("Iterations", ref this.iterations);
        if (this.iterations <= 0)
        {
            this.iterations = 1;
        }
        ImGui.Separator();
        
        if (ImGui.Button("Create Hook"))
        {
            for (int i = 0; i < this.iterations; i++)
            {
                using (this.Plugin.CreateHook())
                {
                    this.counter++;
                }
            }
        }
    }
}
