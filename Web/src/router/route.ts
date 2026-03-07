import { RouteRecordRaw } from 'vue-router';

/**
 * Suggestion：Router path Paths and FoldersnameSame，Can look for the fileBrowseraddresslook for，Convenient for locating the file
 *
 * RoutermetaObjectParameterillustrate
 * meta: {
 *      title:          menuRailings and tagsView rail、Menu searchname（Internationalization）
 *      isLink：        YesnoHyperlinkmenu，Conditions for enabling external links，`1. isLink: The link address is not empty 2. isIframe: false`
 *      isHide：        Whether to hideThis route
 *      isKeepAlive：   Whether to cachegrouppiecestate
 *      isAffix：       Is it fixed?at/in/on tagsView On the railing
 *      isIframe：      YesnoEmbeddedWindow，Activation Conditions，`1. isIframe: true 2. isLink: the link address is not empty`
 *      roles：         Current RoutePermission Identifier，takerole management。Control routingDisplay、hide。super administrator：admin OrdinaryRole：common
 *      icon：          menu、tagsView icon，Ali：Add `iconfont xxx`，fontawesome：Add `fa xxx`
 * }
 */

// Extend RouteMeta interface
declare module 'vue-router' {
	interface RouteMeta {
		title?: string;
		isLink?: string;
		isHide?: boolean;
		isPublic?: boolean;
		isKeepAlive?: boolean;
		isAffix?: boolean;
		isIframe?: boolean;
		roles?: string[];
		icon?: string;
	}
}

/**
 * Define dynamic routing
 * FrontendAdd toRouter，Please inTopof the node `children array` insideAdd to
 * @description Not yetTurn on isRequestRoutes for true timeUse（Frontend routing control），Turn ontimeNumberonepieceTop children The route will be replaced with the route returned by the interface requestData
 * @description eachFieldPleaseView `ruleForm under /@/views/system/menu/component/addMenu.vue`
 * @returns Return routemenuData
 */
export const dynamicRoutes: Array<RouteRecordRaw> = [
	{
		path: '/',
		name: '/',
		component: () => import('/@/layout/index.vue'),
		redirect: '/dashboard/home',
		meta: {
			isKeepAlive: true,
		},
		children: [],
	},
	{
		path: '/platform/job/dashboard',
		name: 'jobDashboard',
		component: () => import('/@/views/system/job/dashboard.vue'),
		meta: {
			title: 'task board',
			isLink: window.__env__.VITE_API_URL + '/schedule',
			isHide: true,
			isKeepAlive: true,
			isAffix: false,
			isIframe: true,
			icon: 'ele-Clock',
		},
	},
	{
		path: '/develop/database/visual',
		name: 'databaseVisual',
		component: () => import('/@/views/system/database/component/visualTable.vue'),
		meta: {
			title: 'Database table visualization',
			isHide: true,
			isKeepAlive: true,
			isAffix: false,
			// isIframe: true,
			icon: 'ele-View',
		},
	},
];

/**
 * Definition404、401interface
 * @link Reference：https://next.router.vuejs.org/zh/guide/essentials/history-mode.html#netlify
 */
export const notFoundAndNoPower = [
	{
		path: '/:path(.*)*',
		name: 'notFound',
		component: () => import('/@/views/error/404.vue'),
		meta: {
			title: 'This page cannot be found',
			isHide: true,
		},
	},
	{
		path: '/401',
		name: 'noPower',
		component: () => import('/@/views/error/401.vue'),
		meta: {
			title: 'No permission',
			isHide: true,
		},
	},
];

/**
 * Define static route（DefaultRouter）
 * Do not touch this router，FrontendAdd toAs for the router，Please in `dynamicRoutes array` inAdd to
 * @description Directly modify through front-end control dynamicRoutes inthe route，Backend controlunnecessaryModify，Request interface routeDatatime，will overwrite dynamicRoutes NumberonepieceTop children ofcontent（Full screen，Noinclude layout inthe routing exit）
 * @returns Return routemenuData
 */
export const staticRoutes: Array<RouteRecordRaw> = [
	{
		path: '/login',
		name: 'login',
		component: () => import('/@/views/login/index.vue'),
		meta: {
			title: 'Login',
			isPublic: true,
		},
	},{
		path: '/$callTel',
		name: '$callTel',
		component: () => import('/@/components/callTel/index.vue'),
		meta: {
			title: 'Dial',
			isPublic: true,
		},
	},
	/**
	 * Prompt：What is written here is for full-screen mode，It is not recommended to write here
	 * Please write on `dynamicRoutes` Number of routesgroupin
	 */
	// {
	// 	path: '/visualizingDemo1',
	// 	name: 'visualizingDemo1',
	// 	component: () => import('/@/views/visualizing/demo1.vue'),
	// 	meta: {
	// 		title: 'message.router.visualizingLinkDemo1',
	// 	},
	// },
	// {
	// 	path: '/visualizingDemo2',
	// 	name: 'visualizingDemo2',
	// 	component: () => import('/@/views/visualizing/demo2.vue'),
	// 	meta: {
	// 		title: 'message.router.visualizingLinkDemo2',
	// 	},
	// },
];
