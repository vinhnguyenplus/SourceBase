import Cookies from 'js-cookie';

/**
 * window.localStorage BrowserPermanentcache
 * @method set Set permanentlycache
 * @method get ObtainPermanentcache
 * @method remove Remove permanentlycache
 * @method clear Remove all permanentlycache
 */
export const Local = {
	// View v2.4.3 version update log
	setKey(key: string) {
		// @ts-ignore
		return `${__NEXT_NAME__}:${key}`;
	},
	// Set up permanent cache
	set<T>(key: string, val: T) {
		window.localStorage.setItem(Local.setKey(key), JSON.stringify(val));
	},
	// Get permanent cache
	get(key: string) {
		let json = <string>window.localStorage.getItem(Local.setKey(key));
		return JSON.parse(json);
	},
	// Remove persistent cache
	remove(key: string) {
		window.localStorage.removeItem(Local.setKey(key));
	},
	// Remove all permanent caches
	clear() {
		window.localStorage.clear();
	},
};

/**
 * window.sessionStorage Browserto face; to overlook; to arrive; about totimecache
 * @method set Set temporarytimecache
 * @method get Obtainto face; to overlook; to arrive; about totimecache
 * @method remove Remove temporarytimecache
 * @method clear Remove all temporarytimecache
 */
export const Session = {
	// Set up temporary cache
	set<T>(key: string, val: T) {
		if (key === 'token') return Cookies.set(key, val);
		window.sessionStorage.setItem(Local.setKey(key), JSON.stringify(val));
	},
	// Get temporary cache
	get(key: string) {
		if (key === 'token') return Cookies.get(key);
		let json = <string>window.sessionStorage.getItem(Local.setKey(key));
		return JSON.parse(json);
	},
	// Remove temporary cache
	remove(key: string) {
		if (key === 'token') return Cookies.remove(key);
		window.sessionStorage.removeItem(Local.setKey(key));
	},
	// Remove all temporary caches
	clear() {
		Cookies.remove('token');
		Cookies.remove('userInfo');
		Cookies.remove('constList');
		window.sessionStorage.clear();
	},
};
