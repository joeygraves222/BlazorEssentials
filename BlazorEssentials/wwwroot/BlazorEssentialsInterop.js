let GeolocationResult = null;
let StateManagerRef = null;

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
        //console.log("The <dialog> API is not supported by this browser");
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