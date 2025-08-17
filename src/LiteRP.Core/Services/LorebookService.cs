using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LiteRP.Core.Helpers;
using LiteRP.Core.Models;
using LiteRP.Core.Services.Interfaces;

namespace LiteRP.Core.Services;

public class LorebookService : ILorebookService
{
    public async Task<List<Lorebook>> GetLorebooksAsync()
    {
        var lorebookFiles = Directory.GetFiles(PathManager.LorebooksDataPath, "*.json");
        var lorebooks = new List<Lorebook>();

        foreach (var file in lorebookFiles)
        {
            var json = await File.ReadAllTextAsync(file);
            var lorebook = JsonSerializer.Deserialize<Lorebook>(json, JsonHelper.Context.Lorebook);
            if (lorebook != null)
            {
                lorebooks.Add(lorebook);
            }
        }

        return lorebooks;
    }

    public async Task<Lorebook?> GetLorebookAsync(Guid id)
    {
        var filePath = PathManager.GetLorebookFilePath(id);
        if (!File.Exists(filePath))
        {
            return null;
        }

        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<Lorebook>(json, JsonHelper.Context.Lorebook);
    }

    public async Task SaveLorebookAsync(Lorebook lorebook)
    {
        var filePath = PathManager.GetLorebookFilePath(lorebook.Id);
        var json = JsonSerializer.Serialize(lorebook, JsonHelper.Context.Lorebook);
        await File.WriteAllTextAsync(filePath, json);
    }

    public Task DeleteLorebookAsync(Guid id)
    {
        var filePath = PathManager.GetLorebookFilePath(id);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}