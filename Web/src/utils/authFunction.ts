import { useUserInfo } from '/@/stores/userInfo';
import { judgementSameArr } from '/@/utils/arrayOperation';
import { resolveDirective, withDirectives, VNode } from 'vue';

/**
 * Single permission verification
 * @param value Permissionvalue
 * @returns Has permission，Return `true`，The opposite applies
 */
export function auth(value: string): boolean {
	const stores = useUserInfo();
	return stores.userInfos.authBtnList.some((v: string) => v === value);
}

/**
 * Multiple permission verifications，satisfyoneeach is true
 * @param value Permissionvalue
 * @returns Has permission，Return `true`，The opposite applies
 */
export function auths(value: Array<string>): boolean {
	let flag = false;
	const stores = useUserInfo();
	stores.userInfos.authBtnList.map((val: string) => {
		value.map((v: string) => {
			if (val === v) flag = true;
		});
	});
	return flag;
}

/**
 * Multiple permission verifications，All conditions met true
 * @param value Permissionvalue
 * @returns Has permission，Return `true`，The opposite applies
 */
export function authAll(value: Array<string>): boolean {
	const stores = useUserInfo();
	return judgementSameArr(value, stores.userInfos.authBtnList);
}
/**
 * Single permission verification，Yesnosatisfy，ReturnVNode
 * @param VNode Yuanplain
 * @param value Permissionvalue
 * @returns VNode
 */
export function hAuth<T extends VNode>(el: T, value: string): T {
	return withDirectives(el, [[resolveDirective('auth'), value]]);
}
/**
 * Multiple permission verifications，JudgmentYesnosatisfyonepiece，ReturnVNode
 * @param VNode Yuanplain
 * @param value Permissionvalue
 * @returns VNode
 */
export function hAuths<T extends VNode>(el: T, value: Array<string>): T {
	return withDirectives(el, [[resolveDirective('auths'), value]]);
}
/**
 * Multiple permission verifications，JudgmentYesnoAll satisfied，ReturnVNode
 * @param VNode Yuanplain
 * @param value Permissionvalue
 * @returns VNode
 */
export function hAuthAll<T extends VNode>(el: T, value: Array<string>): T {
	return withDirectives(el, [[resolveDirective('auth-all'), value]]);
}
