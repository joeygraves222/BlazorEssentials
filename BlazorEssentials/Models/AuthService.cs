using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEssentials.Models
{
    public interface IAuthService
    {
        bool IsAuthenticated { get; }
        Task Authenticated(AuthResult auth);
        Task<bool> Authorized(string key);
    }

    public class AuthService : IAuthService
    {
        private static string AuthenticationKey = "{0512E7A4-6E89-4C0A-8856-24C0B47A3686}";
        private readonly IStorageManager Storage;
        private readonly HttpClient Http;
        private readonly IStateService State;
        private AuthResult AuthDetails { get; set; }

        public AuthService(IStorageManager storage, HttpClient http, IStateService state)
        {
            Storage = storage;
            Http = http;
            State = state;
        }

        private async Task InitializeService()
        {
            var existingAuth = await Storage.GetLocalAsync<AuthResult>(AuthenticationKey);

            AuthDetails = existingAuth;
        }

        public async Task Authenticated(AuthResult auth)
        {
            AuthDetails = auth;
        }

        public async Task<bool> Authorized(string key)
        {
            try
            {
                if (AuthDetails != null)
                {
                    return AuthDetails.VerifyPermission(key);
                }
                else
                {
                    await InitializeService();
                    
                    if (AuthDetails != null)
                    {
                        return AuthDetails.VerifyPermission(key);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                try
                {
                    if (AuthDetails != null)
                    {
                        return AuthDetails.Authenticated();
                    }
                    else
                    {
                        InitializeService().RunSynchronously();

                        if (AuthDetails != null)
                        {
                            return AuthDetails.Authenticated();
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }

    public class AuthResult
    {
        public DateTime TimeAuthenticated { get; set; }
        public DateTime? TimeExpires { get; set; }
        public TimeSpan? Duration { get; set; }

        public bool Authenticated()
        {
            if (TimeExpires.HasValue)
            {
                return DateTime.Now < TimeExpires.Value;
            }
            else if (Duration.HasValue)
            {
                return (TimeAuthenticated + Duration.Value) > DateTime.Now;
            }
            else
            {
                return false;
            }
        }

        public void SetPermissions(List<Permission> perms)
        {
            Permissions = perms;
        }

        public void AddPermission(Permission perm)
        {
            Permissions ??= new();
            Permissions.Add(perm);
        }

        public void AddPermission(string key, bool authorized)
        {
            Permissions ??= new();
            Permissions.Add(new Permission() { Authorized = authorized, Key = key });
        }

        public bool VerifyPermission(string key)
        {
            var perm = Permissions.FirstOrDefault(p => p.Key == key);

            if (perm != null)
            {
                return perm.Authorized;
            }
            else
            {
                return false;
            }
        }

        private List<Permission> Permissions { get; set; }
    }

    public class Permission
    {
        public string Key { get; set; }
        public bool Authorized { get; set; }
    }
}
