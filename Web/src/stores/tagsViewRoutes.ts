import { defineStore } from 'pinia';
import { Session } from '/@/utils/storage';

/**
 * TagsView Routing List
 * @methods setTagsViewRoutes Settings TagsView Routing List
 * @methods setCurrenFullscreen Turn on the setting/Exit full screentimeof boolean state
 */
export const useTagsViewRoutes = defineStore('tagsViewRoutes', {
	state: (): TagsViewRoutesState => ({
		tagsViewRoutes: [],
		isTagsViewCurrenFull: false,
	}),
	actions: {
		async setTagsViewRoutes(data: Array<string>) {
			this.tagsViewRoutes = data;
		},
		setCurrenFullscreen(bool: Boolean) {
			Session.set('isTagsViewCurrenFull', bool);
			this.isTagsViewCurrenFull = bool;
		},
	},
});
