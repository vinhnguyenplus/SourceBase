import { defineStore } from 'pinia';

/**
 * Routing List
 * @methods setRoutesList Set up routingData
 * @methods setColumnsMenuHover SettingsColumnLayoutmenuratMouse Over boolean
 * @methods setColumnsNavHover SettingsColumnLayout leftmost navigationratMouse Over boolean
 */
export const useRoutesList = defineStore('routesList', {
	state: (): RoutesListState => ({
		routesList: [],
		isColumnsMenuHover: false,
		isColumnsNavHover: false,
	}),
	actions: {
		async setRoutesList(data: Array<string>) {
			this.routesList = data;
		},
		async setColumnsMenuHover(bool: Boolean) {
			this.isColumnsMenuHover = bool;
		},
		async setColumnsNavHover(bool: Boolean) {
			this.isColumnsNavHover = bool;
		},
	},
});
