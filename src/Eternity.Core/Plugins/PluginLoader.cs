using System.Reflection;
using Eternity.Core.Errors;

namespace Eternity.Core.Plugins;

/// <summary>Dynamically loads plugins from assemblies.</summary>
public sealed class PluginLoader
{
    /// <summary>Loads plugin instances from folder.</summary>
    public Result<IReadOnlyList<object>> LoadPlugins(string folder)
    {
        if (!Directory.Exists(folder))
        {
            return Result<IReadOnlyList<object>>.Success(Array.Empty<object>());
        }

        var plugins = new List<object>();
        foreach (var file in Directory.GetFiles(folder, "*.dll"))
        {
            try
            {
                var asm = Assembly.LoadFrom(file);
                var types = asm.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && HasFlashingPluginContract(t));
                foreach (var type in types)
                {
                    if (Activator.CreateInstance(type) is { } plugin)
                    {
                        plugins.Add(plugin);
                    }
                }
            }
            catch (Exception ex)
            {
                return Result<IReadOnlyList<object>>.Fail(new OperationError(ErrorCode.PluginLoadFailed, ex.Message, "plugin"));
            }
        }

        return Result<IReadOnlyList<object>>.Success(plugins);
    }

    private static bool HasFlashingPluginContract(Type type)
        => type.GetInterfaces().Any(i => i.FullName is "Eternity.Plugins.Abstractions.IFlashingPlugin");
}
