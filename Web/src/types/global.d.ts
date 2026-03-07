// Declare external npm plugin modules
declare module 'vue-grid-layout';
declare module 'vue-signature-pad';
declare module 'vform3-builds';
declare module 'qrcodejs2-fixes';
declare module 'splitpanes';
declare module 'js-cookie';
//declare module '@wangeditor/editor-for-vue';
//declare module 'js-table2excel';
//declare module 'qs';
//declare module 'sortablejs';
declare module 'vue-plugin-hiprint';
declare module 'vcrontab-3';

// Declare a module to prevent errors when importing files
declare module '*.json';
declare module '*.png';
declare module '*.jpg';
declare module '*.scss';
declare module '*.ts';
declare module '*.js';

// Declaration files, files with *.vue suffix are handed over to the vue module for processing
declare module '*.vue' {
	import type { DefineComponent } from 'vue';
	const component: DefineComponent<{}, {}, any>;
	export default component;
}

// Declaration file, define global variables
/* eslint-disable */
declare interface Window {
	nextLoading: boolean;
	BMAP_SATELLITE_MAP: any;
	BMap: any;
	__env__: any;
}

// Declare the current item type of the route
declare type RouteItem<T = any> = {
	path: string;
	name?: string | symbol | undefined | null;
	redirect?: string;
	k?: T;
	meta?: {
		title?: string;
		isLink?: string;
		isHide?: boolean;
		isKeepAlive?: boolean;
		isAffix?: boolean;
		isIframe?: boolean;
		roles?: string[];
		icon?: string;
		isDynamic?: boolean;
		isDynamicPath?: string;
		isIframeOpen?: string;
		loading?: boolean;
	};
	children: T[];
	query?: { [key: string]: T };
	params?: { [key: string]: T };
	contextMenuClickId?: string | number;
	commonUrl?: string;
	isFnClick?: boolean;
	url?: string;
	transUrl?: string;
	title?: string;
	id?: string | number;
};

// Declare route to from
declare interface RouteToFrom<T = any> extends RouteItem {
	path?: string;
	children?: T[];
}

// Declare route current item type collection
declare type RouteItems<T extends RouteItem = any> = T[];

// declare ref
declare type RefType<T = any> = T | null;

// Declare HTMLElement
declare type HtmlType = HTMLElement | string | undefined | null;

// Declare children optional
declare type ChilType<T = any> = {
	children?: T[];
};

// declare array
declare type EmptyArrayType<T = any> = T[];

// declare object
declare type EmptyObjectType<T = any> = {
	[key: string]: T;
};

// Declare select option
declare type SelectOptionType = {
	value: string | number;
	label: string | number;
};

// Mouse wheel scroll type
declare interface WheelEventType extends WheelEvent {
	wheelDelta: number;
}

// table data format public type
declare interface TableType<T = any> {
	total: number;
	loading: boolean;
	param: {
		pageNum: number;
		pageSize: number;
		[key: string]: T;
	};
}

// dictionary data structure
declare interface DictItem {
	typeCode: string;
	label: string;
	value: string;
	name: string;
	status: string;
	orderNo: number;
	remark?: string;
	tagType?: string;
	extData?: string;
	styleSetting?: string;
	classSetting?: string;
	[key: string]: any;
}