import { defineStore } from 'pinia';

/**
 * The backend returns the original route(Not yetHandletime)
 * @methods setCacheKeepAlive Set interface original routeData
 */
export const useRequestOldRoutes = defineStore('requestOldRoutes', {
	state: (): RequestOldRoutesState => ({
		requestOldRoutes: [],
	}),
	actions: {
		async setRequestOldRoutes(routes: Array<string>) {
			this.requestOldRoutes = routes;
		},
	},
});
