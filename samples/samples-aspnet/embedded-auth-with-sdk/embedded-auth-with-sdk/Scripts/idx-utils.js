// original source: https://github.com/okta/okta-signin-widget/blob/master/src/util/CryptoUtil.js
function binToStr(bin) {
    return btoa(new Uint8Array(bin).reduce((s, byte) => s + String.fromCharCode(byte), ''));
}

function strToBin(str) {
    return Uint8Array.from(atob(base64UrlSafeToBase64(str)), c => c.charCodeAt(0));
}

function base64UrlSafeToBase64(str) {
    return str.replace(new RegExp('_', 'g'), '/').replace(new RegExp('-', 'g'), '+');
}