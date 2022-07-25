# BlazorEssentials
## Introduction

Tha latest version of this package includes the following features:

- Storage
  - LocalStorage
  - SessionStorage
- State Management
- Geolocation
- Basic Authentication and Authorization

Each of these features is defined in an interface and can be implemented however you would like, however a simple implementation is also provided that takes care of many basic functions. This base component looks like this:

```
@using BlazorEssentials.Models

@code{

    [Inject]
    protected IStorageManager Storage { get; set; }
    [Inject]
    protected ILocationService GeoLocationService { get; set; }
    [Inject]
    protected HttpClient Http { get; set; }
    [Inject]
    protected IAuthService Auth { get; set; }
    [Inject]
    protected NavigationManager Nav { get; set; }
}
```

## Implementing Each Feature

First it should be mentioned that it is recommended that every page and component that will need to access these features should inherit the class `EssentialsBaseComponent` found in this package under the namespace `BlazorEssentials.Components`. This base component has provided members to access these services and to subscribe to their events to update your page when necessary. (ex. The `StateManager` fires an event every time the state is updated, and the base component subscribes to the event and calls `StateHasChanged()` so that you don't have to call it yourself, you simply update your State and the components will refresh automatically).

### Storage Manager
This feature is very simple and you probably won't need to make your own implementation since LocalStorage and SessionStorage only have a few basic APIs.
The interface includes the CRUD operations. Each operation will need a `Key` and in the case of Creating and Updating, you will need to provide a string representing the data to be be stored. It's recommended to serialize any objects to a JSON string. This is because Local and Session Storagedon't support storing complex objects, if you write a JSON object to Local or Session Storage, it will write the data, but it will return `[object Object]`. So always serialize your data before storing it. The IStorageManager accepts a `<T>` template type and in the default implementation it will serialize all items to be stored, and attempt to deserialize to the provided type upon retrieval.

```
public interface IStorageManager
    {
        Task<T> GetLocalAsync<T>(string key);
        Task<T> GetSessionAsync<T>(string key);
        Task DeleteLocalAsync(string key);
        Task DeleteSessionAsync(string key);
        Task SetLocalAsync<T>(string key, T item);
        Task SetSessionAsync<T>(string key, T item);
        Task DeleteAllLocalAsync();
        Task DeleteAllSessionAsync();
    }
```

## Auth Service
This feature provides easy management of authorization and authentication. It has a 3 methods to aid in these processes:
- `Task Authenticated(object authResult);`
  - This method takes some object that holds any authentication tokens, or other details regarding permissions and auth details. IT is recommended to store this data in your implementation to determine whether a user is allowed to access certain features or sections of your app.
- `bool Authorized(string key);`
  - This method takes a string as a key and compares it against the permissions stored in your `authResult` to determine if a user is Authorized for the given action.
- `Task<object> GetAuthResult();`
  - This method returns the value of your `authResult` object.
  
The default implementation of this service stores the `authResult` in the classes defined below:

```
public class AuthResult
    {
        public DateTime TimeAuthenticated { get; set; }
        public DateTime? TimeExpires { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Token { get; set; }

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

        public void SetPermission(Permission perm)
        {
            Permissions ??= new();
            var existingPermission = Permissions.FirstOrDefault(p => p.Key == perm.Key);

            if (existingPermission == null)
            {
                Permissions.Add(perm);
            }
            else
            {
                existingPermission.Authorized = perm.Authorized;
            }
        }

        public void SetPermission(string key, bool authorized)
        {
            Permissions ??= new();
            var existingPermission = Permissions.FirstOrDefault(p => p.Key == key);

            if (existingPermission == null)
            {
                Permissions.Add(new Permission() { Authorized = authorized, Key = key});
            }
            else
            {
                existingPermission.Authorized = authorized;
            }
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

```

The `AuthResult` class stores the time that the user was authenticated, as well as the duration of the authentication, or the time that it will expire and any token/cookie that may be associated with the authentication. The class also has a `Permissions` object that stores basically a Key Value pair of known actions, and whether the user may perform that action. This would ideally be provided when you authenticate your user with your backend service. This is not required, but can simplify your authorization checks in your markup. For example, if you are inheriting the `EssentialsBaseComponent` in your components and would like to hide or show different sections of the page or component based on user permissions, you can simply surround that section in an `if` statement like this:

```
@if (Auth.Authorized("Admin"))
{
  <a href="Accounts/create-new-user">Create New User</a>
}
```

This will only render this link to create a new user if the current user has the permission key "Admin" on their AuthResult. This will not prevent the user from manually typing the route into the address bar and accessing the page that way, so it is always recommended that your backend always check for permissions before allowing users to perform sensitive actions, the front end is not secure enough to rely solely on for Authentication and Authorization.
