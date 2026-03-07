import { RouteRecordRaw } from 'vue-router';
import { storeToRefs } from 'pinia';
import { formatTwoStageRoutes, formatFlatteningRoutes, router } from '/@/router/index';
import { dynamicRoutes, notFoundAndNoPower } from '/@/router/route';
import pinia from '/@/stores/index';
import { Session } from '/@/utils/storage';
import { useUserInfo } from '/@/stores/userInfo';
import { useTagsViewRoutes } from '/@/stores/tagsViewRoutes';
import { useRoutesList } from '/@/stores/routesList';
import { NextLoading } from '/@/utils/loading';

// Front-end control routing

/**
 * Frontend routing control：BeginningInitialization method，PreventRefreshtimeRoute lost
 * @method  NextLoading interface loading AnimationStartExecute
 * @method useUserInfo(pinia).setUserInfos() TriggerBeginningInitializationUserInformation pinia
 * @method setAddRoute Add toDynamic routing
 * @method setFilterMenuAndCacheTagsViewRoutes Set recursive filtering for routes with permissions to pinia routesList in（Processed into multi-level nested routes）andcacheMulti-level nested numbersgroupprocessedoneDimensiongroup
 */
export async function initFrontEndControlRoutes() {
	// Interface loading animation starts execution
	if (window.nextLoading === undefined) NextLoading.start();
	// No token, stop executing the next step
	if (!Session.get('token')) return false;
	// Trigger initialization of user information pinia
	// https://gitee.com/lyt-top/vue-next-admin/issues/I5F1HP
	await useUserInfo(pinia).setUserInfos();
	// Add dynamic routing
	await setAddRoute();
	// Set recursive filtering of authorized routes to pinia routesList (processed into multi-level nested routes) and cache the one-dimensional array processed by multi-level nested arrays
	setFilterMenuAndCacheTagsViewRoutes();
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
 * Delete/resetRouter
 * @method router.removeRoute
 * @description The loop here is dynamicRoutes（/@/router/route）NumberonepieceTop children the routeoneDimensiongroup，Non-multi-level nesting
 * @link Reference：https://next.router.vuejs.org/zh/api/#push
 */
export async function frontEndsResetRoute() {
	await setFilterRouteEnd().forEach((route: RouteRecordRaw) => {
		const routeName: any = route.name;
		router.hasRoute(routeName) && router.removeRoute(routeName);
	});
}

/**
 * Obtainhave currentUserPermission Identifiernumber of routesgroup，Carry out the replacement of the original route
 * @description Replace dynamicRoutes（/@/router/route）NumberonepieceTop children the route
 * @returns Return the number of routes replacedgroup
 */
export function setFilterRouteEnd() {
	let filterRouteEnd: any = formatTwoStageRoutes(formatFlatteningRoutes(dynamicRoutes));
	// notFoundAndNoPower prevents 404 and 401 from not being in the layout. If not set, the 404 and 401 interface will be displayed in full screen
	// Related issues No match found for location with path 'xxx'
	filterRouteEnd[0].children = [...setFilterRoute(filterRouteEnd[0].children), ...notFoundAndNoPower];
	return filterRouteEnd;
}

/**
 * ObtainCurrentUserPermission IdentifierGo compare the routing table（Not yetProcess into multi-level nested routes）
 * @description This is mainly used for dynamic routingAdd to，router.addRoute
 * @link Reference：https://next.router.vuejs.org/zh/api/#addroute
 * @param chil dynamicRoutes（/@/router/route）NumberonepieceTop children the set of lower routes
 * @returns Return has currentUserPermission Identifiernumber of routesgroup
 */
export function setFilterRoute(chil: any) {
	const stores = useUserInfo(pinia);
	const { userInfos } = storeToRefs(stores);
	let filterRoute: any = [];
	chil.forEach((route: any) => {
		if (route.meta.roles) {
			route.meta.roles.forEach((metaRoles: any) => {
				userInfos.value.roles.forEach((roles: any) => {
					if (metaRoles === roles) filterRoute.push({ ...route });
				});
			});
		}
	});
	return filterRoute;
}

/**
 * cacheMulti-level nested numbersgroupprocessedoneDimensiongroup
 * @description used for tagsView、Menu searchin：Not yetFilterhideof(isHide)
 */
export function setCacheTagsViewRoutes() {
	// Obtain routes with permissions, otherwise routes without permissions in tagsView and menu search will also be displayed.
	const stores = useUserInfo(pinia);
	const storesTagsView = useTagsViewRoutes(pinia);
	const { userInfos } = storeToRefs(stores);
	let rolesRoutes = setFilterHasRolesMenu(dynamicRoutes, userInfos.value.roles);
	// Add to pinia setTagsViewRoutes
	storesTagsView.setTagsViewRoutes(formatTwoStageRoutes(formatFlatteningRoutes(rolesRoutes))[0].children);
}

/**
 * Set recursive filtering for routes with permissions to pinia routesList in（Processed into multi-level nested routes）andcacheMulti-level nested numbersgroupprocessedoneDimensiongroup
 * @description For the left sidemenu、HorizontalmenuofDisplay
 * @description used for tagsView、Menu searchin：Not yetFilterhideof(isHide)
 */
export function setFilterMenuAndCacheTagsViewRoutes() {
	const stores = useUserInfo(pinia);
	const storesRoutesList = useRoutesList(pinia);
	const { userInfos } = storeToRefs(stores);
	storesRoutesList.setRoutesList(setFilterHasRolesMenu(dynamicRoutes[0].children, userInfos.value.roles));
	setCacheTagsViewRoutes();
}

/**
 * Determine routing `meta.roles` inYesnoincludeCurrentLoginUserPermissionField
 * @param roles UserPermission Identifier，at/in/on userInfos（UserInformation）of roles（LoginPageLogintimecachetoBrowser）numbergroup
 * @param route Current looptimerouting item
 * @returns Return the route items that have permissions after comparison
 */
export function hasRoles(roles: any, route: any) {
	if (route.meta && route.meta.roles) return roles.some((role: any) => route.meta.roles.includes(role));
	else return true;
}

/**
 * ObtainCurrentUserPermission IdentifierGo compare the routing table，Set recursive filtering for authorized routes
 * @param routes Current Route children
 * @param roles UserPermission Identifier，at/in/on userInfos（UserInformation）of roles（LoginPageLogintimecachetoBrowser）numbergroup
 * @returns Return the number of routes with permissionsgroup `meta.roles` inControl
 */
export function setFilterHasRolesMenu(routes: any, roles: any) {
	const menu: any = [];
	routes.forEach((route: any) => {
		const item = { ...route };
		if (hasRoles(roles, item)) {
			if (item.children) item.children = setFilterHasRolesMenu(item.children, roles);
			menu.push(item);
		}
	});
	return menu;
}
