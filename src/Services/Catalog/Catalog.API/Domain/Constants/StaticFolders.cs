
namespace Catalog.API.Domain.Constants
{
    public class StaticFolders
    {
        public const string Base = "wwwroot";
        public const string ProductImages = "ProductImages";

        public static string GetStaticFolderPath(string folder)
        {
            return Path.Combine(Base, folder);
        }
    }
}