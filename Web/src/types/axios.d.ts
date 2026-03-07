/* eslint-disable */
import * as axios from 'axios';

// Extended axios data return type, can be extended by yourself
declare module 'axios' {
	export interface AxiosResponse<T = any> {
		code: number;
		data: T;
		message: string;
		type?: string;
		[key: string]: T;
	}
	export interface AxiosRequestConfig {
		customCatch?: boolean;
	}
}
