using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Security.Claims;

namespace FilmsCatalog.Helpers
{
    internal class CommonHelper
    {
        /// <summary>
        /// Конвертировать файл в массив байтов
        /// </summary>
        internal static byte[] ConvertFileToBinary(IFormFile file)
        {
            byte[] imageData = null;
            // считываем переданный файл в массив байтов
            using (var binaryReader = new BinaryReader(file.OpenReadStream()))
            {
                imageData = binaryReader.ReadBytes((int)file.Length);
            }

            return imageData;
        }

        /// <summary>
        /// Получить ИД текущего пользователя
        /// </summary>
        internal static Guid GetUserId(ClaimsPrincipal claimsPrincipal)
        {
            Guid userId = Guid.Empty;
            if (claimsPrincipal.Identity.IsAuthenticated)
                Guid.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

            return userId;
        }
    }
}
