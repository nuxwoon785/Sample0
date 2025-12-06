using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ModelConfig.Models;

namespace ModelConfig.Services;

public class ConfigStorage
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<ProjectConfiguration> LoadAsync(string path)
    {
        if (!File.Exists(path))
        {
            return new ProjectConfiguration();
        }

        await using var stream = File.OpenRead(path);
        var config = await JsonSerializer.DeserializeAsync<ProjectConfiguration>(stream, _options);
        return config ?? new ProjectConfiguration();
    }

    public async Task SaveAsync(ProjectConfiguration configuration, string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, configuration, _options);
    }

    public async Task<string> BackupAsync(string sourcePath, string backupDirectory)
    {
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException("설정 파일을 찾을 수 없습니다.", sourcePath);
        }

        if (!Directory.Exists(backupDirectory))
        {
            Directory.CreateDirectory(backupDirectory);
        }

        var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        var backupPath = Path.Combine(backupDirectory, $"config-{timestamp}.json");
        await using var sourceStream = File.OpenRead(sourcePath);
        await using var destinationStream = File.Create(backupPath);
        await sourceStream.CopyToAsync(destinationStream);
        return backupPath;
    }
}
