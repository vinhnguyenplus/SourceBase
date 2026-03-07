import axios, { AxiosInstance, AxiosRequestConfig } from 'axios';
import { ElMessage } from 'element-plus';
import { Local, } from '/@/utils/storage';
import {clearAccessAfterReload} from "/@/utils/axios-utils";

// Define request abort controller mapping table
const abortControllerMap: Map<string, AbortController> = new Map();

// Configure a new axios instance
export const service = axios.create({
	baseURL: window.__env__.VITE_API_URL as any,
	timeout: 50000,
	//headers: { 'Content-Type': 'application/json' }, this will cause the file uploaded to generate the code to be empty.
});

// token key definition
export const accessTokenKey = 'access-token';
export const refreshAccessTokenKey = `x-${accessTokenKey}`;

// Get token
export const getToken = () => {
	return Local.get(accessTokenKey);
};

// axios default instance
export const axiosInstance: AxiosInstance = axios;

// Add request interceptor
service.interceptors.request.use(
	(config) => {
		// //What to do before sending the request token
		// if (Session.get('token')) {
		// 	(<any>config.headers).common['Authorization'] = `${Session.get('token')}`;
		// }

		// Record abort control information
		const controller = new AbortController();
		config.signal = controller.signal;
		const url = config.url || '';
		abortControllerMap.set(url, controller);

		// Get local token
		const accessToken = Local.get(accessTokenKey);
		if (accessToken) {
			// Add token to request header
			config.headers!['Authorization'] = `Bearer ${accessToken}`;

			// Determine whether the accessToken has expired
			const jwt: any = decryptJWT(accessToken);
			const exp = getJWTDate(jwt.exp as number);

			// token has expired
			if (new Date() >= exp) {
				// Get refresh token
				const refreshAccessToken = Local.get(refreshAccessTokenKey);

				// Carrying refresh token
				if (refreshAccessToken) {
					config.headers!['X-Authorization'] = `Bearer ${refreshAccessToken}`;
				}
			}
			// debugger
			// get request mapping params parameters
			if (config.method?.toLowerCase() === 'get' && config.data) {
				let url = config.url + '?' + tansParams(config.data);
				url = url.slice(0, -1);
				config.data = {};
				config.url = url;
			}
		}
		return config;
	},
	(error) => {
		// What to do about request errors
		return Promise.reject(error);
	}
);

// Add response interceptor
service.interceptors.response.use(
	(res) => {

		// Clear abort controls after request ends
		const url = res.config.url || '';
		abortControllerMap.delete(url);

		// Get status code and return data
		var status = res.status;
		var serve = res.data;

		// Handling 401
		if (status === 401) {
			clearAccessAfterReload();
		}

		// Processing that has not been standardized
		if (status >= 400) {
			throw new Error(res.statusText || 'Request Error.');
		}

		// Handling normalization result errors
		if (serve && serve.hasOwnProperty('errors') && serve.errors) {
			throw new Error(JSON.stringify(serve.errors || 'Request Error.'));
		}

		// Read response message header token information
		var accessToken = res.headers[accessTokenKey];
		var refreshAccessToken = res.headers[refreshAccessTokenKey];

		// Determine whether it is an invalid token
		if (accessToken === 'invalid_token') {
			clearAccessAfterReload();
		}
		// Determine whether there is a refresh token, if it exists, store it locally, and reload the page
		else if (refreshAccessToken && accessToken) {
			Local.set(accessTokenKey, accessToken);
			Local.set(refreshAccessTokenKey, refreshAccessToken);
		}

		// Response interception and custom processing
		if (serve.code === 401) {
			clearAccessAfterReload();
		} else if (serve.code === undefined) {
			return Promise.resolve(res);
		} else if (serve.code !== 200) {
			var message;
			// Determine whether serve.message is an object
			if (serve.message && typeof serve.message == 'object') {
				message = JSON.stringify(serve.message);
			} else {
				message = serve.message;
			}
			ElMessage({
				dangerouslyUseHTMLString: true,
				message: message,
				type: 'error',
			});
			throw new Error(message);
		}

		return res;
	},
	(error) => {
		// Handling response errors
		if (error.response) {
			if (error.response.status === 401) {
				clearAccessAfterReload();
			}
		}

		// Do something about the response error
		if (error.message.indexOf('timeout') != -1) {
			ElMessage.error('Network timeout');
		} else if (error.message == 'Network Error') {
			ElMessage.error('Network connection error');
		} else {
			if (error.response.data) ElMessage.error(error.response.statusText);
			else ElMessage.error('Interface path not found');
		}

		return Promise.reject(error);
	}
);

// Cancel assignment request
export const cancelRequest = (url: string | string[]) => {
	const urlList = Array.isArray(url) ? url : [url];
	for (const _url of urlList) {
		abortControllerMap.get(_url)?.abort();
		abortControllerMap.delete(_url);
	}
}

// Cancel all requests
export const cancelAllRequest = () => {
	for (const [_, controller] of abortControllerMap) {
		controller.abort();
	}
	abortControllerMap.clear();
}

/**
 *  ParameterHandle
 * @param {*} params  Parameter
 */
export function tansParams(params: any) {
	let result = '';
	for (const propName of Object.keys(params)) {
		const value = params[propName];
		var part = encodeURIComponent(propName) + '=';
		if (value !== null && value !== '' && typeof value !== 'undefined') {
			if (typeof value === 'object') {
				for (const key of Object.keys(value)) {
					if (value[key] !== null && value[key] !== '' && typeof value[key] !== 'undefined') {
						let params = propName + '[' + key + ']';
						var subPart = encodeURIComponent(params) + '=';
						result += subPart + encodeURIComponent(value[key]) + '&';
					}
				}
			} else {
				result += part + encodeURIComponent(value) + '&';
			}
		}
	}
	return result;
}

/**
 * Decrypt JWT token information
 * @param token jwt token String
 * @returns <any>object
 */
export function decryptJWT(token: string): any {
	token = token.replace(/_/g, '/').replace(/-/g, '+');
	var json = decodeURIComponent(escape(window.atob(token.split('.')[1])));
	return JSON.parse(json);
}

/**
 * will JWT Timestampconvert to Date
 * @description Mainly aimed at `exp`，`iat`，`nbf`
 * @param timestamp Timestamp
 * @returns Date Object
 */
export function getJWTDate(timestamp: number): Date {
	return new Date(timestamp * 1000);
}

/**
 * AjaxRequest，IfsuccessReturnresultField，If notsuccessPromptmistakeInformation
 * @description AjaxRequest
 * @config AxiosRequestConfig Request parameters
 * @returns Return Object
 */
export function request2(config: AxiosRequestConfig<any>): any {
	return new Promise((resolve, reject) => {
		service(config)
			.then((res) => {
				if (res.data.type == 'success') {
					resolve(res.data.result);
				} else {
					console.log('res', res);
					ElMessage.success(res.data.message);
				}
			})
			.catch((res) => {
				console.log('res', res);
				ElMessage.error(res);
				reject(res);
			});
	});
}

/**
 * Use a new tokenLogin
 * @param accessInfo
 */
export function reLoadLoginAccessToken(accessInfo: any) {
	if (accessInfo?.accessToken && accessInfo?.refreshToken) {
		Local.set(accessTokenKey, accessInfo.accessToken);
		Local.set(refreshAccessTokenKey, accessInfo.refreshToken);
		setTimeout(() => location.href = "/", 300);
	}
}

// Export axios instance
export default service;
