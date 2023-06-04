using System.Reflection;
using System.Diagnostics;

using Terraria;
using TerrariaApi.Server;

[ApiVersion(2, 1)]
public class ReferencesPlugin : TerrariaPlugin
{
    public override string Author => "Zoom L1";
    public override string Name => "References";
    public ReferencesPlugin(Main game) : base(game)
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        Directory.CreateDirectory(ReferencesDirectoryPath);

        files = Directory.GetFiles(ReferencesDirectoryPath, "*.dll", SearchOption.AllDirectories)
            .ToDictionary(i => Path.GetFileName(i), i => i);

        ServerApi.LogWriter.PluginWriteLine(this, "The references have been successfully uploaded.", TraceLevel.Info);
    }
     
    private Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();
    private Dictionary<string, string> files = new Dictionary<string, string>();

    public static readonly string ReferencesDirectoryPath 
        = Path.Combine(AppContext.BaseDirectory, "References");

    public override void Initialize()
    {
    }

    private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        string text = args.Name.Split(',')[0];
        string name = text + ".dll";

        if (files.TryGetValue(name, out string path))
        {
            try
            {
                if (File.Exists(path))
                {
                    if (!loadedAssemblies.TryGetValue(path, out Assembly? assembly))
                    {
                        assembly = Assembly.Load(File.ReadAllBytes(path));
                        loadedAssemblies.Add(path, assembly);
                    }
                    return assembly;
                }
            }
            catch (Exception ex)
            {
                ServerApi.LogWriter.PluginWriteLine(this, string.Format("Error on resolving assembly \"{0}.dll\":\n{1}", text, ex), TraceLevel.Error);
            }
        }
        
        return null;
    }
}