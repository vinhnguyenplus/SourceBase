import { defineStore } from 'pinia';

/**
 * RouterCache List
 * @methods setCacheKeepAlive Set tocachethe route names（Open Tagsview）
 * @methods addCachedView Add towantcachethe route names（Close Tagsview）
 * @methods delCachedView Deletewantcachethe route names（Close Tagsview）
 * @methods delOthersCachedViews Right-clickmenu`Close others`，Deletewantcachethe route names（Close Tagsview）
 * @methods delAllCachedViews Right-clickmenu`Turn off all`，Deletewantcachethe route names（Close Tagsview）
 */
export const useKeepALiveNames = defineStore('keepALiveNames', {
	state: (): KeepAliveNamesState => ({
		keepAliveNames: [],
		cachedViews: [],
	}),
	actions: {
		async setCacheKeepAlive(data: Array<string>) {
			this.keepAliveNames = data;
		},
		async addCachedView(view: any) {
			if (view.meta.isKeepAlive) this.cachedViews?.push(view.name);
		},
		async delCachedView(view: any) {
			const index = this.cachedViews.indexOf(view.name);
			index > -1 && this.cachedViews.splice(index, 1);
		},
		async delOthersCachedViews(view: any) {
			if (view.meta.isKeepAlive) this.cachedViews = [view.name];
			else this.cachedViews = [];
		},
		async delAllCachedViews() {
			this.cachedViews = [];
		},
	},
});
