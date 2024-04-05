namespace HookCrashPlugin;

using System;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;

public class HookClass : IDisposable
{
    private readonly Hook<HandleItemHoverDelegate> hook;
    
    public HookClass(IGameInteropProvider gameInteropProvider, IntPtr address)
    {
        HandleItemHoverDelegate delegateFunc = this.Detour;
        this.hook = gameInteropProvider.HookFromAddress(address, delegateFunc);
    }

    private unsafe nint Detour(IntPtr hoverState, IntPtr a2, IntPtr a3, ulong a4)
    {
        return this.hook.Original(hoverState, a2, a3, a4);
    }
    
    private delegate IntPtr HandleItemHoverDelegate(IntPtr hoverState, IntPtr a2, IntPtr a3, ulong a4);

    public void Dispose() => this.hook.Dispose();
}
