using System.Diagnostics;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;

[ApiVersion(2, 1)]
public class References : TerrariaPlugin
{
	public override string Author => "Zoom L1";

	public override string Name => "References";

    private Dictionary<string, Assembly> loadedAssemblies = new ();

    private Dictionary<string, string> files = new ();

    public static readonly string ReferencesDirectoryPath = Path.Combine(AppContext.BaseDirectory, "References");

    public References(Main game)
		: base(game)
	{
		AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
		Directory.CreateDirectory(References.ReferencesDirectoryPath);
		files = Directory.GetFiles(References.ReferencesDirectoryPath, "*.dll", SearchOption.AllDirectories).
			ToDictionary((string i) => Path.GetFileName(i), (string i) => i);
		ServerApi.LogWriter.PluginWriteLine(this, "The references have been successfully uploaded.", TraceLevel.Info);
	}

	public override void Initialize()
	{
	}

	private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
	{
		var asembyName = args.Name.Split(',', StringSplitOptions.None)[0];
		if (files.TryGetValue(asembyName + ".dll", out string? path))
		{

			if (File.Exists(path))
			{
				if (!this.loadedAssemblies.TryGetValue(path, out Assembly? assembly))
				{
					try
					{
                        assembly = Assembly.LoadFrom(path);
                        loadedAssemblies[path] = assembly;
                    }
					catch (Exception ex)
					{
						ServerApi.LogWriter.PluginWriteLine(this, string.Format("Error on resolving assembly \"{0}.dll\":\n{1}", asembyName, ex), TraceLevel.Error);
                        return null;
                    }
				}
				return assembly;
			}
		}
		return null;
	}
}
