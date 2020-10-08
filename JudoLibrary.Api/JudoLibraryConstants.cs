using System;
using IdentityServer4;

namespace JudoLibrary.Api
{
    public struct JudoLibraryConstants
    {
        public struct Policies
        {
            // Pointing to "IdentityServerAccessToken" for User => you should be a User to access "this"
            public const string User = IdentityServerConstants.LocalApi.PolicyName;
            public const string Mod = nameof(Mod);
        }
        
        public struct IdentityResources
        {
            public const string RoleScope = "role";
        }
        
        public struct Claims
        {
            public const string Role = "role";
        }
        
        public struct Roles
        {
            public const string Mod = nameof(Mod);
        }
        
        public struct Files
        {
            public struct Providers
            {
                public const string Local = nameof(Local);
                public const string S3 = nameof(S3);
            }
            
            public const string TempPrefix = "temp_";
            public const string ConvertedPrefix = "conv_";
            public const string ThumbnailPrefix = "thumb_";
            public const string ProfilePrefix = "profile_";
        
        
            // Returns string -> converted video file name => e.g. .mp4
            public static string GenerateConvertedFileName() => $"{ConvertedPrefix}{DateTime.Now.Ticks}.mp4";

            // Returns string -> thumbnail for video file name => e.g. .png
            public static string GenerateThumbnailFileName() => $"{ThumbnailPrefix}{DateTime.Now.Ticks}.jpg";
        
            // Returns string -> thumbnail for profile picture file name => e.g. .png
            public static string GenerateProfileFileName() => $"{ProfilePrefix}{DateTime.Now.Ticks}.jpg";
        }
    }
}