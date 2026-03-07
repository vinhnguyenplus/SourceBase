import { createRouter, createWebHashHistory } from 'vue-router';
import NProgress from 'nprogress';
import 'nprogress/nprogress.css';
import pinia from '/@/stores/index';
import { storeToRefs } from 'pinia';
import { useKeepALiveNames } from '/@/stores/keepAliveNames';
import { useRoutesList } from '/@/stores/routesList';
import { useThemeConfig } from '/@/stores/themeConfig';
import {Local, Session} from '/@/utils/storage';
import { staticRoutes, notFoundAndNoPower } from '/@/router/route';
import { initFrontEndControlRoutes } from '/@/router/frontEnd';
import { initBackEndControlRoutes } from '/@/router/backEnd';

/**
 * 1、Frontend routing controltime：isRequestRoutes for false，Need to write roles，Need to go setFilterRoute Method。
 * 2、Backend-controlled routingtime：isRequestRoutes for true，unnecessarywrite roles，unnecessaryGo setFilterRoute Method），
 * The relevant methods have been broken down to the corresponding `backEnd.ts` and `frontEnd.ts`（They do not affect each other.，unnecessarySametimeChange 2 a file）。
 * Specialillustrate：
 * 1、Front-end control：RoutermenuWritten by the front end（NoneMenu managementinterface，haverole managementinterface），role managementinhave roles Attribute，need to return to userInfo in。
 * 2、Backend control：RoutermenuReturned by the backend（haveMenu managementinterface、haverole managementinterface）
 */

// Read `/src/stores/themeConfig.ts` to check whether to enable backend control routing configuration
const storesThemeConfig = useThemeConfig(pinia);
const { themeConfig } = storeToRefs(storesThemeConfig);
const { isRequestRoutes } = themeConfig.value;

/**
 * Createonecan be Vue The routing instance used by the application
 * @method createRouter(options: RouterOptions): Router
 * @link Reference：https://next.router.vuejs.org/zh/api/#createrouter
 */
export const router = createRouter({
	history: createWebHashHistory(),
	/**
	 * illustrate：
	 * 1、notFoundAndNoPower DefaultAdd to 404、401 interface，PreventoneStraightPrompt No match found for location with path 'xxx'
	 * 2、backEnd.ts(Backend-controlled routing)、frontEnd.ts(Frontend routing control) inAlso needs to be added notFoundAndNoPower 404、401 interface。
	 *    Prevent 404、401 Not here layout Layoutin，If not set，404、401 The interface will go full screenDisplay
	 */
	routes: [...notFoundAndNoPower, ...staticRoutes],
});

/**
 * Number of nested route levelsgroupprocessed intooneDimensiongroup
 * @param arr Incoming routemenuDatanumbergroup
 * @returns Return processedoneVeriRoutermenunumbergroup
 */
export function formatFlatteningRoutes(arr: any) {
	if (arr.length <= 0) return false;
	for (let i = 0; i < arr.length; i++) {
		if (arr[i].children) {
			arr = arr.slice(0, i + 1).concat(arr[i].children, arr.slice(i + 1));
		}
	}
	return arr;
}

/**
 * oneDimensiongroupProcess into multi-level nested numbersgroup（Keep onlyTwolevel：justYesTwoAll levels above processed into onlyTwolevel，keep-alive SupportTwolevelcache）
 * @description isKeepAlive Handle `name` value，carry outcache。TopClose，All notcache
 * @link Reference：https://v3.cn.vuejs.org/api/built-in-components.html#keep-alive
 * @param arr processedoneVeriRoutermenunumbergroup
 * @returns Return willoneDimensiongroupReprocess into `Define dynamic routes (dynamicRoutes)` format
 */
export function formatTwoStageRoutes(arr: any) {
	if (arr.length <= 0) return false;
	const newArr: any = [];
	const cacheList: Array<string> = [];
	arr.forEach((v: any) => {
		if (v.path == null || v.path == undefined) return;

		if (v.path === '/') {
			newArr.push({ component: v.component, name: v.name, path: v.path, redirect: v.redirect, meta: v.meta, children: [] });
		} else {
			// Determine whether it is a dynamic route (xx/:id/:name), used in tagsView, etc.
			// Fix: https://gitee.com/lyt-top/vue-next-admin/issues/I3YX6G
			if (v.path.indexOf('/:') > -1) {
				v.meta['isDynamic'] = true;
				v.meta['isDynamicPath'] = v.path;
			}
			newArr[0].children.push({ ...v });
			// Store the name value and use it in include in keep-alive to implement route caching.
			// Path:/@/layout/routerView/parent.vue
			if (newArr[0].meta.isKeepAlive && v.meta.isKeepAlive) {
				cacheList.push(v.name);
			}
		}
	});
	const stores = useKeepALiveNames(pinia);
	stores.setCacheKeepAlive(cacheList);
	return newArr;
}

// Before routing is loaded
router.beforeEach(async (to, from, next) => {
	NProgress.configure({ showSpinner: false });
	if (to.meta.title) NProgress.start();
	const token = Session.get('token');
	if (to.meta.isPublic && !token) {
		next();
		NProgress.done();
	} else {
		if (!token) {
			next(`/login?redirect=${to.path}&params=${JSON.stringify(to.query ? to.query : to.params)}`);
			Session.clear();
			NProgress.done();
		} else if (token && to.path === '/login') {
			next('/dashboard/home');
			NProgress.done();
		} else {
			const storesRoutesList = useRoutesList(pinia);
			const { routesList } = storeToRefs(storesRoutesList);
			if (routesList.value.length === 0) {
				if (isRequestRoutes) {
					// Backend control routing: routing data initialization to prevent loss during refresh
					await initBackEndControlRoutes();
					// Solve the problem of always jumping to the 404 page when refreshing, and the related problem No match found for location with path 'xxx'
					// to.query prevents the parameters from being lost when the page is refreshed and ordinary routes have parameters. Dynamic routing (xxx/:id/:name") isDynamic No processing required
					next({ path: to.path, query: to.query });
				} else {
					// https://gitee.com/lyt-top/vue-next-admin/issues/I5F1HP
					await initFrontEndControlRoutes();
					next({ path: to.path, query: to.query });
				}
			} else {
				next();
			}
		}
	}
});

// After routing is loaded
router.afterEach(() => {
	NProgress.done();
});

// Export route
export default router;
