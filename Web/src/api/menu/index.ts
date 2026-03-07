import request from '/@/utils/request';

/**
 * The following is a simulated interfaceaddress，gitee not connected，Just change fromselfthe real interfaceaddress
 *
 * （It is not recommended to write it as request.post(xxx)，Because of this post time，NoneLaw params and data SametimePassing parameters）
 *
 * Backend controlmenuSimulatejson，The path is at https://gitee.com/lyt-top/vue-next-admin-images/tree/master/menu
 * Backend-controlled routing，isRequestRoutes for true，Then enable backend controlled routing
 * @method getAdminMenu ObtainBackend dynamic routingmenu(admin)
 * @method getTestMenu ObtainBackend dynamic routingmenu(test)
 */
export function useMenuApi() {
	return {
		getAdminMenu: (params?: object) => {
			return request({
				url: '/gitee/lyt-top/vue-next-admin-images/raw/master/menu/adminMenu.json',
				method: 'get',
				params,
			});
		},
		getTestMenu: (params?: object) => {
			return request({
				url: '/gitee/lyt-top/vue-next-admin-images/raw/master/menu/testMenu.json',
				method: 'get',
				params,
			});
		},
	};
}
