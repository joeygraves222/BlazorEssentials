let GeolocationResult = null;
let StateManagerRef = null;
let LoaderId = 'AB68B71D93414BF3BB3BE8C9FC191B4A';
let LoaderCSSId = '6353788E7A2C48AA990AA71610992F09';
let loaderCSS = "#" + LoaderId + "::backdrop { background: rgba(104, 104, 104, .75);} #" + LoaderId + "{background-color: white;border: none;border-radius: 10px;padding: 20px;} .dark-mode #" + LoaderId + " {background-color: #454d55;color: white;} #" + LoaderId + " h3{width: 100 %;text-align: center;} .dialog-container{display: flex;flex-direction: column;justify-content: space-between;align-items: center;} .dialog-buttons-container {display: flex;flex-direction: column;justify-content: space-evenly;align-items: center;width: 100 %;margin-top: 10px;} ";

export async function GetGeoLocation() {
    GeolocationResult = null;
    var resultData = await GetTheLocation();
    return JSON.stringify({ Lat: resultData.coords.latitude, Lon: resultData.coords.longitude });
}

window.addEventListener('storage', event =>
{
    if (StateManagerRef != null) {
        StateManagerRef.invokeMethodAsync("StorageChanged");
    }
});


async function GetTheLocation() {
    getLocation();
    while (GeolocationResult == null) {
        await delay(10);
    }
    return GeolocationResult;
}


function getLocation() {
    if (navigator.geolocation) {
        var loc = navigator.geolocation.getCurrentPosition(setPosition);
    } else {
        x.innerHTML = "Geolocation is not supported by this browser.";
    }
}

function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

function setPosition(position) {
    GeolocationResult = position;
}

export function SetLocalStorage(key, item) {
    localStorage.setItem(key, item);
}

export function GetLocalStorage(key) {
    return localStorage.getItem(key);
}

export function DeleteLocalStorage(key) {
    localStorage.removeItem(key);
}

export function DeleteAllLocalStorage() {
    localStorage.clear();
}

export function SetSessionStorage(key, item) {
    sessionStorage.setItem(key, item);
}

export function GetSessionStorage(key) {
    return sessionStorage.getItem(key);
}

export function DeleteSessionStorage(key) {
    sessionStorage.removeItem(key);
}

export function DeleteAllSessionStorage() {
    sessionStorage.clear();
}

export function SubscribeToStorageEvent(objRef) {
    StateManagerRef = objRef;
}

export function showDialogModal(modalId) {
    var dialog = document.getElementById(modalId);

    if (typeof dialog.showModal === "function") {
        dialog.showModal();
    } else {
        dialogPolyfill.registerDialog(dialog);
        dialog.showModal();
    }
}

export function closeDialogModal(modalId) {
    var dialog = document.getElementById(modalId);

    dialog.close();
}

export function BlazorFocusElement(element) {
    
    if (element instanceof HTMLElement) {
        element.focus();
    }
}
}

export function showLoader(message, cssClasses) {

    var dialog = document.createElement('dialog');
    dialog.id = LoaderId;

    var dialogCSS = document.createElement('style');
    dialogCSS.id = LoaderCSSId;
    dialogCSS.innerHTML = loaderCSS;

    var dialogContainer = document.createElement('div');
    dialogContainer.setAttribute('class', "dialog-container");

    var loaderContent = document.createElement('div');
    loaderContent.setAttribute('style', "width: 200px;");

    var loaderContainer = document.createElement('div');
    loaderContainer.setAttribute('class', "d-flex justify-content-center flex-column align-items-center");

    var loader = document.createElement('div');
    loader.setAttribute('class', cssClasses);

    var messageContainer = document.createElement('div');
    messageContainer.setAttribute('class', "d-flex justify-content-center mt-4");
    messageContainer.textContent = message;

    loaderContainer.appendChild(loader);
    loaderContent.appendChild(loaderContainer).appendChild(messageContainer);
    dialogContainer.appendChild(loaderContent);
    dialog.appendChild(dialogContainer);

    document.body.appendChild(dialogCSS);
    document.body.appendChild(dialog);

    if (typeof dialog.showModal === "function") {
        dialog.showModal();
    } else {
        dialogPolyfill.registerDialog(dialog);
        dialog.showModal();
    }
}

export function closeLoader() {
    var dialog = document.getElementById(LoaderId);
    var dialogCSS = document.getElementById(LoaderCSSId);

    dialog.close();

    document.body.removeChild(dialogCSS);
    document.body.removeChild(dialog);
}