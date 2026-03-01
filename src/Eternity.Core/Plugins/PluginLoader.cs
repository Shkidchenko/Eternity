using System.Reflection;
using Eternity.Core.Errors;
using Eternity.Plugins.Abstractions;

namespace Eternity.Core.Plugins;

/// <summary>Dynamically loads plugins from assemblies.</summary>
public sealed class PluginLoader
{
    /// <summary>Loads plugin instances from folder.</summary>
    public Result<IReadOnlyList<IFlashingPlugin>> LoadPlugins(string folder)
    {
        if (!Directory.Exists(folder))
        {
            return Result<IReadOnlyList<IFlashingPlugin>>.Success(Array.Empty<IFlashingPlugin>());
        }

        var plugins = new List<IFlashingPlugin>();
        foreach (var file in Directory.GetFiles(folder, "*.dll"))
        {
            try
            {
                var asm = Assembly.LoadFrom(file);
                var types = asm.GetTypes().Where(t => typeof(IFlashingPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
                foreach (var type in types)
                {
                    if (Activator.CreateInstance(type) is IFlashingPlugin plugin)
                    {
                        plugins.Add(plugin);
                    }
                }
            }
            catch (Exception ex)
            {
                return Result<IReadOnlyList<IFlashingPlugin>>.Fail(new OperationError(ErrorCode.PluginLoadFailed, ex.Message, "plugin"));
            }
        }

        return Result<IReadOnlyList<IFlashingPlugin>>.Success(plugins);
    }
}
