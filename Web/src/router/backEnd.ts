import { RouteRecordRaw } from 'vue-router';
import pinia from '/@/stores/index';
import { useUserInfo } from '/@/stores/userInfo';
import { useRequestOldRoutes } from '/@/stores/requestOldRoutes';
import { Session } from '/@/utils/storage';
import { NextLoading } from '/@/utils/loading';
import { dynamicRoutes, notFoundAndNoPower } from '/@/router/route';
import { formatTwoStageRoutes, formatFlatteningRoutes, router } from '/@/router/index';
import { useRoutesList } from '/@/stores/routesList';
import { useTagsViewRoutes } from '/@/stores/tagsViewRoutes';

import { getAPI } from '/@/utils/axios-utils';
import { SysMenuApi } from '/@/api-services/api';
// import { ElMessage } from 'element-plus';

// Backend control routing

/**
 * ObtainTable of Contentsunder .vue、.tsx All files
 * @method import.meta.glob
 * @link Reference：https://cn.vitejs.dev/guide/features.html#json
 */
const layouModules: any = import.meta.glob('../layout/routerView/*.{vue,tsx}');
const viewsModules: any = import.meta.glob('../views/**/*.{vue,tsx}');
const dynamicViewsModules: Record<string, Function> = Object.assign({}, { ...layouModules }, { ...viewsModules });

/**
 * Backend-controlled routing：BeginningInitialization method，PreventRefreshtimeRoute lost
 * @method NextLoading interface loading AnimationStartExecute
 * @method useUserInfo().setUserInfos() TriggerBeginningInitializationUserInformation pinia
 * @method useRequestOldRoutes().setRequestOldRoutes() Storage Interface Original Route（Not yetHandlecomponent），Choose to use according to needs
 * @method setAddRoute Add toDynamic routing
 * @method setFilterMenuAndCacheTagsViewRoutes Set route to pinia routesList in（Processed into multi-level nested routes）andcacheMulti-level nested numbersgroupprocessedoneDimensiongroup
 */
export async function initBackEndControlRoutes() {
	// Interface loading animation starts execution
	if (window.nextLoading === undefined) NextLoading.start();
	// No token, stop executing the next step
	if (!Session.get('token')) return false;
	// Trigger initialization of user information pinia
	// https://gitee.com/lyt-top/vue-next-admin/issues/I5F1HP
	await useUserInfo().setUserInfos();
	await useUserInfo().setConstList();
	await useUserInfo().setDictList();
	// Get routing menu data
	const res = await getBackEndControlRoutes();
	// When there is no login permission, add judgment
	// https://gitee.com/lyt-top/vue-next-admin/issues/I64HVO
	if (res == undefined || res.length <= 0) return Promise.resolve(true);
	// Store the original route of the interface (component is not processed), choose to use it according to your needs
	useRequestOldRoutes().setRequestOldRoutes(res as string[]);
	// Process routing (component) and replace the route of the first top-level children of dynamicRoutes (/@/router/route)
	dynamicRoutes[0].children = await backEndComponent(res);
	// Check user-defined homepage settings
	dynamicRoutes[0].redirect = Session.get('homepage') || dynamicRoutes[0].redirect;
	// Add dynamic routing
	await setAddRoute();
	// Set routes to pinia routesList (processed into multi-level nested routes) and cache the one-dimensional array processed by multi-level nested arrays
	setFilterMenuAndCacheTagsViewRoutes();
}

/**
 * Set route to pinia routesList in（Processed into multi-level nested routes）andcacheMulti-level nested numbersgroupprocessedoneDimensiongroup
 * @description For the left sidemenu、HorizontalmenuofDisplay
 * @description used for tagsView、Menu searchin：Not yetFilterhideof(isHide)
 */
export async function setFilterMenuAndCacheTagsViewRoutes() {
	const storesRoutesList = useRoutesList(pinia);
	storesRoutesList.setRoutesList(dynamicRoutes[0].children as any);
	setCacheTagsViewRoutes();
}

/**
 * cacheMulti-level nested numbersgroupprocessedoneDimensiongroup
 * @description used for tagsView、Menu searchin：Not yetFilterhideof(isHide)
 */
export function setCacheTagsViewRoutes() {
	const storesTagsView = useTagsViewRoutes(pinia);
	storesTagsView.setTagsViewRoutes(formatTwoStageRoutes(formatFlatteningRoutes(dynamicRoutes))[0].children);
}

/**
 * Handling routing format andAdd toCapture all routes or 404 Not found Router
 * @description Replace dynamicRoutes（/@/router/route）NumberonepieceTop children the route
 * @returns Return the number of routes replacedgroup
 */
export function setFilterRouteEnd() {
	let filterRouteEnd: any = formatTwoStageRoutes(formatFlatteningRoutes(dynamicRoutes));
	// notFoundAndNoPower prevents 404 and 401 from not being in the layout. If not set, the 404 and 401 interface will be displayed in full screen
	// Related issues No match found for location with path 'xxx'
	filterRouteEnd[0].children = [...filterRouteEnd[0].children, ...notFoundAndNoPower];
	return filterRouteEnd;
}

/**
 * Add toDynamic routing
 * @method router.addRoute
 * @description The loop here is dynamicRoutes（/@/router/route）NumberonepieceTop children the routeoneDimensiongroup，Non-multi-level nesting
 * @link Reference：https://next.router.vuejs.org/zh/api/#addroute
 */
export async function setAddRoute() {
	await setFilterRouteEnd().forEach((route: RouteRecordRaw) => {
		router.addRoute(route);
	});
}

/**
 * Request backend routemenuInterface
 * @description isRequestRoutes for true，Then enable backend controlled routing
 * @returns Return to backend routemenuData
 */
export async function getBackEndControlRoutes() {
	var res = await getAPI(SysMenuApi).apiSysMenuLoginMenuTreeGet();
	// if (res.data.result == undefined || res.data.result.length < 1) {
	// 	ElMessage.error('No menu permissions, please contact the administrator!');
	// 	setTimeout(() => {
	// 		Session.removeToken();
	// 		window.location.reload();
	// 	}, 3000);
	// }
	return res.data.result;
}

/**
 * Re-request backend routemenuInterface
 * @description used forMenu managementinterfaceRefreshmenu（Not yetConduct a test）
 * @description Path：/src/views/system/menu/component/addMenu.vue
 */
export async function setBackEndControlRefreshRoutes() {
	await getBackEndControlRoutes();
}

/**
 * Backend Routing component Convert
 * @param routes Number of route tables returned by the backendgroup
 * @returns Return the processed result as a function component
 */
export function backEndComponent(routes: any) {
	if (!routes) return;
	return routes.map((item: any) => {
		if (!item.path) item.path = ''; // Prevent the route returned by the backend from not having the path attribute, causing routing errors.
		if (item.component) item.component = dynamicImport(dynamicViewsModules, item.component as string);
		item.children && backEndComponent(item.children);
		return item;
	});
}

/**
 * Backend Routing component Conversion function
 * @param dynamicViewsModules ObtainTable of Contentsunder .vue、.tsx All files
 * @param component Current items to be processed component
 * @returns Return the processed result as a function component
 */
export function dynamicImport(dynamicViewsModules: Record<string, Function>, component: string) {
	const keys = Object.keys(dynamicViewsModules);
	const matchKeys = keys.filter((key) => {
		const k = key.replace(/..\/views|../, '');
		return k.startsWith(`${component}`) || k.startsWith(`/${component}`);
	});
	if (matchKeys?.length === 1) {
		const matchKey = matchKeys[0];
		return dynamicViewsModules[matchKey];
	}
	if (matchKeys?.length > 1) {
		return false;
	}
}
