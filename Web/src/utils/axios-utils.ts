/**
 * CurrentVersion:v1.4.0
 * UseDescription:https:// editor.swagger.io code generation typescript-axios auxiliary tool library
 * Dependenceillustrate：Adapt axios Version:v0.21.4
 * Video tutorial：https://www.bilibili.com/video/BV1EW4y1C71D
 */

import globalAxios, { AxiosInstance } from 'axios';
import { Configuration } from '../api-services';
import { BASE_PATH } from '../api-services/base';

import { ElMessage } from 'element-plus';
import { Local, Session } from '../utils/storage';
import { useUserInfo } from "/@/stores/userInfo";
import {useRoute, useRouter} from "vue-router";

// Interface server configuration
export const serveConfig = new Configuration({
	basePath: window.__env__.VITE_API_URL,
});

// token key definition
export const accessTokenKey = 'access-token';
export const refreshAccessTokenKey = `x-${accessTokenKey}`;

// Get token
export const getToken = () => {
	return Local.get(accessTokenKey);
};

// Get request header token
export const getHeader = () => {
	return { authorization: 'Bearer ' + getToken() };
};

// clear token
export const clearAccessAfterReload = () => {
	clearTokens();
	// Refresh browser
	window.location.reload();
};

// clear token
export const clearTokens = () => {
	Local.remove(accessTokenKey);
	Local.remove(refreshAccessTokenKey);
	Session.clear();
};

// axios default instance
export const axiosInstance: AxiosInstance = globalAxios;

// Here you can configure more options for axios ===========================================
axiosInstance.defaults.timeout = 1000 * 60 * 10; // Set timeout, default 10 minutes

// axios request interception
axiosInstance.interceptors.request.use(
	(conf) => {
		// Get the local token or the token in the session
		const accessToken = Local.get(accessTokenKey) ? Local.get(accessTokenKey) : Session.get('token');
		if (accessToken) {
			// Add token to request header
			conf.headers!['Authorization'] = `Bearer ${accessToken}`;

			// Determine whether the accessToken has expired
			const jwt: any = decryptJWT(accessToken);
			const exp = getJWTDate(jwt.exp as number);

			// token has expired
			if (new Date() >= exp) {
				// Get refresh token
				const refreshAccessToken = Local.get(refreshAccessTokenKey);

				// Carrying refresh token
				if (refreshAccessToken) {
					conf.headers!['X-Authorization'] = `Bearer ${refreshAccessToken}`;
				}
			}
		}

		// Write the request interception code here =========================================

		// Get the language set by the front-end
		const globalI18n = Local.get('themeConfig')?.globalI18n;
		if (globalI18n) {
			// Add to request header
			conf.headers!['Accept-Language'] = globalI18n;
		}
		return conf;
	},
	(error) => {
		// Handling request errors
		if (error.request) {
			ElMessage.error(error);
		}

		// Request error codes and custom processing
		ElMessage.error(error);

		return Promise.reject(error);
	}
);

// axios response interception
axiosInstance.interceptors.response.use(
	(res) => {
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
		// Determine whether there is a refresh token, and if it exists, store it locally.
		else if (refreshAccessToken && accessToken && accessToken !== 'invalid_token') {
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
			// User-defined exception handling
			if (!res.config?.customCatch) {
				ElMessage.error(message);
			}
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

		// User-defined exception handling
		if (!error.config?.customCatch) {
			// Response error code and custom processing
			ElMessage.error(error);
		}

		return Promise.reject(error);
	}
);

/**
 * Packaging Promise and return [Error, any]
 * @param promise Promise Method
 * @param errorExt CustomizemistakeInformation（Expand）
 * @returns [Error, any]
 */
export function feature<T, U = Error>(promise: Promise<T>, errorExt?: object): Promise<[U, undefined] | [null, T]> {
	return promise
		.then<[null, T]>((data: T) => [null, data])
		.catch<[U, undefined]>((err: U) => {
			if (errorExt) {
				const parsedError = Object.assign({}, err, errorExt);
				return [parsedError, undefined];
			}

			return [err, undefined];
		});
}

/**
 * Obtain/Create Service API Example
 * @param apiType BaseAPI DerivativeType
 * @param configuration ServerConfigurationObject
 * @param basePath Serveraddress
 * @param axiosObject axios Example
 * @returns ServiceAPI Example
 */
export function getAPI<T>(
	// eslint-disable-next-line no-unused-vars
	apiType: new (configuration?: Configuration, basePath?: string, axiosInstance?: AxiosInstance) => T,
	configuration: Configuration = serveConfig,
	basePath: string = BASE_PATH,
	axiosObject: AxiosInstance = axiosInstance
) {
	return new apiType(configuration, basePath, axiosObject);
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
 * Implement asynchronous delay
 * @param delay Delaytimespace（millimetersecond）
 * @returns
 */
export function sleep(delay: number) {
	return new Promise((resolve) => setTimeout(resolve, delay));
}
