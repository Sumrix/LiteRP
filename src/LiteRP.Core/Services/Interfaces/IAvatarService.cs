using System;
using System.IO;
using System.Threading.Tasks;

namespace LiteRP.Core.Services.Interfaces;

public interface IAvatarService
{
    /// <summary>
    /// Сохраняет основной (оригинальный) аватар для персонажа.
    /// </summary>
    /// <param name="characterId">ID персонажа.</param>
    /// <param name="imageStream">Поток данных изображения.</param>
    Task SavePermanentAvatarAsync(Guid characterId, Stream imageStream);

    /// <summary>
    /// Возвращает поток данных для аватара, измененного по размеру.
    /// Используется для отдачи через API.
    /// </summary>
    /// <param name="characterId">ID персонажа.</param>
    /// <param name="sizeKey">Ключ размера ('s', 'm', 'l', 'xl').</param>
    /// <param name="multiplier">Множитель (1, 2 или 3).</param>
    /// <returns>Поток с изображением или null, если аватар не найден.</returns>
    Task<Stream?> GetResizedAvatarStreamAsync(Guid characterId, string sizeKey, int multiplier);

    /// <summary>
    /// Генерирует превью аватара для отображения на форме.
    /// Обрабатывает изображение из потока и возвращает его в виде Data URL (base64).
    /// </summary>
    /// <param name="imageStream">Поток данных изображения.</param>
    /// <param name="sizeKey">Ключ размера для превью.</param>
    /// <returns>Строка Data URL для использования в <img> src.</returns>
    Task<string> GetPreviewAvatarAsDataUrlAsync(Stream imageStream, string sizeKey);

    /// <summary>
    /// Удаляет аватар персонажа.
    /// </summary>
    /// <param name="characterId">ID персонажа.</param>
    Task DeleteAvatarAsync(Guid characterId);
}