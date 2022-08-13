# BlazorEssentials
## Introduction

Each version of this package includes the following features:

### Version 1.1.1
- Storage
  - LocalStorage
  - SessionStorage
- State Management
- Geolocation
- Basic Authentication and Authorization

### Version 1.2.1
- Loading Indicator
- Awaitable Prompt
- Modal Dialog
- Copy to Clipboard
- Client Device Info
  - Client OS
  - Client Architecture (32 or 64 bit)
  - Client Device Type (Mobile or Desktop/Laptop)


## Implementing Each Feature


## Version 1.1.1

First it should be mentioned that it is recommended that every page and component that will need to access these features should inherit the class `EssentialsBaseComponent` found in this package under the namespace `BlazorEssentials.Components`. This base component has provided members to access these services and to subscribe to their events to update your page when necessary. (ex. The `StateService` fires an event every time the state is updated, and the base component subscribes to the event and calls `StateHasChanged()` so that you don't have to call it yourself, you simply update your State and the components will refresh automatically). This base component looks like this:

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

The best way to utilize this and any of your own services is to create your own base component and have that inherit this class. Then in your own class you provide your services and they will all be available. Your own base class is where you would want to put your own implementation of the StateService (More info on this below).

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

## Location Service
This service provides an API to get the devices current location. This is dependent on the user granting permission to the app to access the location. This service has only a single method: `Task<GeoLocation> GetLocationAsync();`. It returns a `GeoLocation` object that contains the `Lat` and `Lon` values correlating to the user's geo coordinates. This service relies on the `Interop` class that registers and uses JSInterop functions. There is no need to include the JS file that it references, it is automatically referenced by the package and no further action is needed. This service will prompt the user for permission and await the response. You probably won't need to provide your own implementation of this service since it is so simple, but you can if you would like.

## State Service
This service will provide an interface for storing State variables and updating them.Behind the scenes this works as a Dictionary that holds all of the items and their keys. The best way to use this feature is to create your own class to handle the State and have it inherit from the provided class `StateService'. This class implements the `IStateService` interface and provides methods for updating the State values. In your own implementation you should provide each variable as a property and in each of the getters and setters you should call the `IStateService` methods `GetStateItem<T>(string key);` and `SetStateItem<T>(string key, T item);`. These methods will retrieve and update the data properly, and will fire the events to update the UI.

Here is an example:
```
public class StateManager : StateService
    {
        public StateManager(IJSRuntime js) : base(js)
        {
        }

        public GeoLocation Location
        {
            get { return GetStateItem<GeoLocation>("GeoLocation"); }
            set { SetStateItem("GeoLocation", value); }
        }

        public GeoLocation PersistentLocation
        {
            get { return GetPersistentStateItem<GeoLocation>("GeoLocation"); }
            set { SetPersistentStateItem("GeoLocation", value); }
        }
    }
```
Notice the class `StateManager` inherits `StateService` rather than implementing `IStateService`, this is because the logic to persist the State objects across browser sessions or across page reloads is already implemented in the class `StateService`. You can implement this yourself if you would like, but this class, `StateService` has done a pretty generic job of that which can easily be reused for almost any needs.

The State Service has methods that will persist the State variables to the LocalStorage, and to the SessionStorage, these help because they allow your users to navigate away from the page without losing their progress. Since the State Service is registered as a Scoped Service it is alive for the duration of the request, and is also the same instance across all components. So you can easily pass data from page to page without the need for route parameters. There are still times when you might want to use route parameters, but you don't need them because you could always just store the parameters in a State object, and then retrieve the State object when the next page loads.

To inject the default services into the Dependency Injection Container, call the method `builder.Services.AddBlazorEssentials("https://localhost:5000/api")` in the `Program.cs` file. This URL that is passed into the method sets the base URL on the `HttpClient`. This will provide the default services for `IStorageManager`, `ILocationService`, `Interop` (The JSInterop class for all things regarding Blazor Essentials), the `HttpClient` with the provided BaseURL, and the `IAuthService`. If you would like to manually implement any of these then don't call this method, and instead provide each of these manually in the `Program.cs` file. The StateService that you create should be added in the same place like this `builder.Services.AddScoped<StateManager>();` with your implementation in plcae of `StateManager`.

## Version 1.2.1
All of these features have been added into the JavaScript Interop class. This is available in the `EssentialsBaseComponent`. If your component inherits this class, then you can use these features. Simply call them on the `JS` object in the base component
### Loading Indicator

This is accessed via the `JS.ShowLoader()` method. It has 4 optional parameters:
- Message (string)
- Style (enum, spinning indicator or a growing ball)
- Size (enum, small or medium sized indicator)
- Color (enum, the color of the indicator)

### Awaitable Prompt

This is accessed via the `JS.ConfirmAsync()` method, it returns a `Task<bool>`. It takes a single parameter of the following type:
```
    public class PromptModel
    {
        public int MaxTimeout { get; set; } = 0;
        public string Title { get; set; }
        public string Prompt { get; set; }
        public string ConfirmButtonText { get; set; }
        public string DenyButtonText { get; set; }

    }

``` 
If the timeout is left at 0 then the prompt will wait until it receives a response, otherwise the method will return a false after the timeout period.

### Modal Dialog
This is an actual component that you can use in your `.razor` files. It needs to have both the `MaxWidth` and `MinWidth` attributes (each are an int that correlate to a pixel amount) set to show up. Here is an example:

```
<Dialog @ref="TestDialog" MaxWidth="800" MinWidth="200" Title="This is a test Dialog">
    <Body>
        <div class="row">
            <h5>This is the body of the Dialog</h5>
        </div>
    </Body>
    <Buttons>
        <button class="btn btn-block btn-outline-success" @onclick="() => TestDialog.CloseModal()">Close</button>
    </Buttons>
</Dialog>

@code{

private Dialog TestDialog { get; set; }

// To open the dialog, call TestDialog.ShowModal();

}

```

### Copy to Clipboard
This is accessed via the `JS.CopyToClipboard()` method. It takes a single string parameter containing the text to copy to the clipboard. There are more features to this that I intend to add in future updates, such as copying images, but at this time it can only copy plain text to the clipboard.

### Client Device Info
This is accessed via the `JS.GetDeviceType()` and `JS.GetUserAgent()` methods. Neither take any parameters and each return (respectively) an enum of type:
```
public enum ClientDeviceType
    {
        Mobile,
        Desktop
    }
```
and an object of type:
```
public class ClientUserAgent
{
    public string AppInfo { get; set; }
    public string PlatformInfo { get; set; }
    public string ProductInfo { get; set; }
    public string ApplicationNameInfo { get; set; }
    public OSType GetOSType() {...} // Returns an enum
    public ArchitectureType GetArchitectureType() {...} // Returns an enum
}
```

  
