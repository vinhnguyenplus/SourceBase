import request from '/@/utils/request';

/**
 * （It is not recommended to write it as request.post(xxx)，Because of this post time，NoneLaw params and data SametimePassing parameters）
 *
 * LoginapiInterface Collection
 * @method signIn User login
 * @method signOut UserLog out
 */
export function useLoginApi() {
	return {
		signIn: (data: object) => {
			return request({
				url: '/user/signIn',
				method: 'post',
				data,
			});
		},
		signOut: (data: object) => {
			return request({
				url: '/user/signOut',
				method: 'post',
				data,
			});
		},
	};
}
