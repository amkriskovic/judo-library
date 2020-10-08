using System;
using JudoLibrary.Api;
using JudoLibrary.Api.BackgroundServices.VideoEditing;
using JudoLibrary.Api.Settings;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class RegisterService
    {
        // Extension Method -> Goal is to be injectable in other services
        public static IServiceCollection AddFileManager(this IServiceCollection services, IConfiguration config)
        {
            // Specifying settings section -> resembles FileSetting in appsettings.dev.json which is proxied via class FileSettings 
            var settingsSection = config.GetSection(nameof(FileSettings));

            // Casting json object to FileSettings type
            var settings = settingsSection.Get<FileSettings>();

            // Registers a configuration instance which T Options will bind against.
            services.Configure<FileSettings>(settingsSection);
            
            // If it's Local provider
            if (settings.Provider.Equals(JudoLibraryConstants.Files.Providers.Local))
            {
                // Register IFileManager as concrete implementation of FileManagerLocal
                services.AddSingleton<IFileManager, FileManagerLocal>(); 
            }
            else if (settings.Provider.Equals(JudoLibraryConstants.Files.Providers.S3))
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new Exception($"Invalid File Manager Provider: ${settings.Provider}");
            }
            
            // Return service collection
            return services;
        }
    }
}