
// Import internationalization JSON file (merge mode)
import langJSON from './index.json'
(function () {
    // Define a translation function
    let $t = function (key, val, nameSpace) {
        // Get the language pack under the specified namespace
        const langPackage = $t[nameSpace];
        // Return the translation result; if it does not exist, return the default value
        return (langPackage || {})[key] || val;
    };
    // Define a simple translation function that directly returns the input value.
    let $$t = function (val) {
        return val;
    };
    globalThis.$deepScan = function (val) {
        return val;
    };
    globalThis.$iS = function (val, args) {
        // If the parameter is not a string or an array, return the original value directly
        if (typeof val !== 'string' || !Array.isArray(args)) {
            return val;
        }
        try {
            // Use a safer method for replacing with regular expressions
            return val.replace(/\$(?:\{|\｛)(\d+)(?:\}|\｝)/g, (match, index) => {
                // Convert index to number
                const position = parseInt(index, 10);
                // If args[position] exists, replace it; otherwise, keep the original placeholder
                return args[position] !== undefined ? String(args[position]) : match;
            });
        } catch (error) {
            console.warn('An exception occurred during string replacement:', error);
            return val;
        }
    }
    // Method to define and set the language pack
    $t.locale = function (locale, nameSpace) {
        // Set the language pack under the specified namespace to the passed-in locale
        $t[nameSpace] = locale || {};
    };
    // Mount the translation function onto the globalThis object, and use the existing one if it already exists.
    globalThis.$t = globalThis.$t || $t;
    // Mount the simple translation function onto the globalThis object
    globalThis.$$t = $$t;
    // Define a method to get the language object for a specified key from a JSON file (merge mode)
    globalThis._getJSONKey = function (key, insertJSONObj = undefined) {
        // Get JSON object
        const JSONObj = insertJSONObj;
        // Initialize language object
        const langObj = {};
        // Iterate over all keys of a JSON object
        Object.keys(JSONObj).forEach((value) => {
            // Add the corresponding key-value of each language to the language object
            langObj[value] = JSONObj[value][key];
        });
        // Return language object
        return langObj;
    };
})();
// Define a language mapping object
const langMap = {
    'zhhk': (globalThis && globalThis.lang && globalThis.lang.zhhk) ? globalThis.lang.zhhk : globalThis._getJSONKey('zh-hk', langJSON),
    'zhtw': (globalThis && globalThis.lang && globalThis.lang.zhtw) ? globalThis.lang.zhtw : globalThis._getJSONKey('zh-tw', langJSON),
    'en': (globalThis && globalThis.lang && globalThis.lang.en) ? globalThis.lang.en : globalThis._getJSONKey('en', langJSON),
    'it': (globalThis && globalThis.lang && globalThis.lang.it) ? globalThis.lang.it : globalThis._getJSONKey('it', langJSON),
    'vi': (globalThis && globalThis.lang && globalThis.lang.vi) ? globalThis.lang.vi : globalThis._getJSONKey('vi', langJSON),
    'vi': (globalThis && globalThis.lang && globalThis.lang.vi) ? globalThis.lang.vi : globalThis._getJSONKey('vi', langJSON)
};
globalThis.langMap = langMap;
// Does the storage language exist?
// Determine whether globalThis.localStorage.getItem is a function
const isFunction = (fn) => {
    return typeof fn === 'function';
};

const withStorageLang = isFunction && globalThis && globalThis.localStorage &&
    isFunction(globalThis.localStorage.getItem) && globalThis.localStorage.getItem('lang');
const withStorageCommonLang = isFunction && globalThis && globalThis.localStorage &&
    isFunction(globalThis.localStorage.getItem) && globalThis.localStorage.getItem('');
// Get the universal language from local storage, or use an empty string if it does not exist
const commonLang = withStorageCommonLang ? globalThis.localStorage.getItem('') : '';
// Get the current language from local storage, and use the source language if it doesn't exist
const baseLang = withStorageLang ? globalThis.localStorage.getItem('lang') : 'vi';
const lang = commonLang ? commonLang : baseLang;
// Translate the function's language pack according to the current language setting
globalThis.$t.locale(globalThis.langMap[lang], 'lang');
globalThis.$changeLang = (lang) => {
    globalThis.$t.locale(globalThis.langMap[lang], 'lang');
};
