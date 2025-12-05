namespace YoloLabeler.Services;

public class ClassCatalog
{
    private readonly List<string> _classes = new();
    public IReadOnlyList<string> Classes => _classes;

    public void Load(string folder)
    {
        _classes.Clear();
        var path = GetClassPath(folder);
        if (!File.Exists(path))
        {
            return;
        }

        _classes.AddRange(File.ReadAllLines(path).Where(l => !string.IsNullOrWhiteSpace(l)));
    }

    public void Save(string folder)
    {
        var path = GetClassPath(folder);
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

    private static string GetClassPath(string folder) => Path.Combine(folder, "classes.txt");
}
