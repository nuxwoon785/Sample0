using System;
using System.IO;
using System.Collections.Generic;
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

    public async Task<ProjectConfiguration> LoadAsync(string rootPath)
    {
        var configuration = new ProjectConfiguration();

        if (string.IsNullOrWhiteSpace(rootPath) || !Directory.Exists(rootPath))
        {
            return configuration;
        }

        var directories = Directory.GetDirectories(rootPath);
        foreach (var directory in directories)
        {
            var masterPath = Path.Combine(directory, "master.json");
            MasterConfiguration master;

            if (File.Exists(masterPath))
            {
                await using var stream = File.OpenRead(masterPath);
                master = await JsonSerializer.DeserializeAsync<MasterConfiguration>(stream, _options) ?? new MasterConfiguration();
            }
            else
            {
                master = new MasterConfiguration { Name = Path.GetFileName(directory) };
            }

            master.FolderName = Path.GetFileName(directory);
            configuration.Masters.Add(master);
        }

        return configuration;
    }

    public async Task SaveAsync(ProjectConfiguration configuration, string rootPath)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("설정 루트 경로가 비어 있습니다.", nameof(rootPath));
        }

        Directory.CreateDirectory(rootPath);

        var desiredFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var master in configuration.Masters)
        {
            var folderName = !string.IsNullOrWhiteSpace(master.FolderName)
                ? master.FolderName
                : SanitizeFolderName(!string.IsNullOrWhiteSpace(master.ModelCode) ? master.ModelCode : master.Name);

            if (string.IsNullOrWhiteSpace(folderName))
            {
                folderName = Guid.NewGuid().ToString();
            }

            var masterDirectory = Path.Combine(rootPath, folderName);
            Directory.CreateDirectory(masterDirectory);
            master.FolderName = folderName;
            desiredFolders.Add(folderName);

            var masterPath = Path.Combine(masterDirectory, "master.json");
            await using var stream = File.Create(masterPath);
            await JsonSerializer.SerializeAsync(stream, master, _options);
        }

        foreach (var directory in Directory.GetDirectories(rootPath))
        {
            var folderName = Path.GetFileName(directory);
            if (!desiredFolders.Contains(folderName))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    public async Task<string> BackupAsync(string sourcePath, string backupDirectory)
    {
        if (!Directory.Exists(sourcePath))
        {
            throw new DirectoryNotFoundException($"설정 루트 폴더를 찾을 수 없습니다: {sourcePath}");
        }

        if (!Directory.Exists(backupDirectory))
        {
            Directory.CreateDirectory(backupDirectory);
        }

        var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        var backupPath = Path.Combine(backupDirectory, $"config-{timestamp}");
        CopyDirectory(sourcePath, backupPath);
        await Task.CompletedTask;
        return backupPath;
    }

    private static string SanitizeFolderName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var invalidChar in invalidChars)
        {
            name = name.Replace(invalidChar, '_');
        }

        return name.Trim();
    }

    private static void CopyDirectory(string sourceDir, string destinationDir)
    {
        Directory.CreateDirectory(destinationDir);

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var destFile = Path.Combine(destinationDir, Path.GetFileName(file));
            File.Copy(file, destFile, overwrite: true);
        }

        foreach (var directory in Directory.GetDirectories(sourceDir))
        {
            var destSubDir = Path.Combine(destinationDir, Path.GetFileName(directory));
            CopyDirectory(directory, destSubDir);
        }
    }
}
