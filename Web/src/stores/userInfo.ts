import { defineStore } from 'pinia';
import { Local, Session } from '/@/utils/storage';
import Watermark from '/@/utils/watermark';
import { useThemeConfig } from '/@/stores/themeConfig';

import { getAPI } from '/@/utils/axios-utils';
import { SysAuthApi, SysConstApi, SysDictTypeApi } from '/@/api-services/api';

/**
 * UserInformation
 * @methods setUserInfos SettingsUserInformation
 */
export const useUserInfo = defineStore('userInfo', {
	state: (): UserInfosState => ({
		userInfos: {} as any,
		constList: [] as any,
		dictList: {} as any,
	}),
	getters: {
		// // Get the system constant list
		// async getSysConstList(): Promise<any[]> {
		// 	var res = await getAPI(SysConstApi).apiSysConstListGet();
		// 	this.constList = res.data.result ?? [];
		// 	return this.constList;
		// },
	},
	actions: {
		// Store user information in browser cache
		async setUserInfos() {
			this.userInfos = Session.get('userInfo') ?? <UserInfos>await this.getApiUserInfo();
		},

		// Store constant information in browser cache
		async setConstList() {
			this.constList = Session.get('constList') ?? <any[]>await this.getSysConstList();
			if (!Session.get('constList')) Session.set('constList', this.constList);
		},

		// Store dictionary information in browser cache
		async setDictList() {
			var dictList = await getAPI(SysDictTypeApi).apiSysDictTypeAllDictListGet().then(res => res.data.result ?? {});
			var dictListTemp = JSON.parse(JSON.stringify(dictList));

			await Promise.all(Object.keys(dictList).map(async (key) => {
				// dictList[key].forEach((da: any, index: any) => {
				// 	setDictLangMessageAsync(dictListTemp[key][index]);
				// });
				// If key ends with "Enum", convert value to number
				if (key.endsWith("Enum")) {
					dictListTemp[key].forEach((e: any) => e.value = Number(e.value));
				}
			}))
			this.dictList = dictListTemp;
		},

		// Get current user information
		getApiUserInfo() {
			return new Promise((resolve) => {
				getAPI(SysAuthApi)
					.apiSysAuthUserInfoGet()
					.then(async (res: any) => {
						if (res.data.result == null) return;
						var d = res.data.result;
						const userInfos = {
							id: d.id,
							account: d.account,
							realName: d.realName,
							phone: d.phone,
							idCardNum: d.idCardNum,
							email: d.email,
							accountType: d.accountType,
							avatar: d.avatar ?? '/upload/logo.png',
							address: d.address,
							signature: d.signature,
							orgId: d.orgId,
							orgName: d.orgName,
							posName: d.posName,
							roles: d.roleIds,
							authBtnList: d.buttons,
							tenantId: d.tenantId,
							currentTenantId: d.currentTenantId,
							langCode: d.langCode,
							time: new Date().getTime(),
						};

						// vue-next-admin Submit Id: 225bce7 Submit message: admin-23.03.26: Release v2.4.32 version
						// The following code has been added, causing the user information of the current session to not be refreshed. For example: the resubmitted avatar is not updated, and a new page needs to be opened to display it correctly.
						// Session.set('userInfo', userInfos);

						// User watermark
						const storesThemeConfig = useThemeConfig();
						storesThemeConfig.themeConfig.watermarkText = d.watermarkText ?? '';
						if (storesThemeConfig.themeConfig.isWatermark) Watermark.set(storesThemeConfig.themeConfig.watermarkText);
						else Watermark.del();

						Local.remove('themeConfig');
						Local.set('themeConfig', storesThemeConfig.themeConfig);

						resolve(userInfos);
					});
			});
		},

		// Get a collection of constants
		getSysConstList() {
			return new Promise((resolve) => {
				getAPI(SysConstApi)
					.apiSysConstListGet()
					.then(async (res: any) => {
						resolve(res.data.result ?? []);
					});
			});
		},

		// Get constant data based on constant class name
		getConstDataByTypeCode(typeCode: string) {
			return this.constList.find((item: any) => item.code === typeCode)?.data?.result || [];
		},

		// Get constant value based on constant class name and encoding
		getConstItemNameByType(typeCode: string, itemCode: string) {
			const data = this.getConstDataByTypeCode(typeCode);
			return data.find((item: any) => item.code === itemCode)?.name;
		},

		// Get dictionary data based on dictionary type
		getDictDataByCode(dictTypeCode: string) {
			return this.dictList[dictTypeCode] || [];
		}
	},
});

// Process dictionary internationalization and display the label value in the dictionary by default
// const setDictLangMessageAsync = async (dict: any) => {
// 	dict.langMessage = `message.dictType.${dict.typeCode}_${dict.value}`;
// 	const text = dict.langMessage;
// 	dict.label = text !== dict.langMessage ? text : dict.label;
// }