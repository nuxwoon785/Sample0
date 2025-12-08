namespace YoloLabeler.Services;

public class ClassCatalog
{
    private readonly List<string> _classes = new();
    private string? _classFilePath;
    public IReadOnlyList<string> Classes => _classes;

    public void Load(string folder)
    {
        _classes.Clear();
        _classFilePath = ResolveClassPath(folder);
        if (_classFilePath != null && File.Exists(_classFilePath))
        {
            _classes.AddRange(File.ReadAllLines(_classFilePath).Where(l => !string.IsNullOrWhiteSpace(l)));
        }
    }

    public void Save(string folder)
    {
        var path = _classFilePath ?? ResolveClassPath(folder) ?? GetClassPath(folder);
        File.WriteAllLines(path, _classes);
    }

    public int EnsureClass(string folder, string className)
    {
        var existing = _classes.IndexOf(className);
        if (existing >= 0)
        {
            return existing;
        }

        _classes.Add(className);
        Save(folder);
        return _classes.Count - 1;
    }

    public string GetClass(int id) => id >= 0 && id < _classes.Count ? _classes[id] : string.Empty;

    private static string? ResolveClassPath(string folder)
    {
        var folderClassPath = GetClassPath(folder);
        if (File.Exists(folderClassPath))
        {
            return folderClassPath;
        }

        var parent = Directory.GetParent(folder);
        if (parent == null)
        {
            return folderClassPath;
        }

        var parentClassPath = GetClassPath(parent.FullName);
        return File.Exists(parentClassPath) ? parentClassPath : folderClassPath;
    }

    private static string GetClassPath(string folder) => Path.Combine(folder, "classes.txt");
}
