<template>
	<div class="layout-navbars-breadcrumb-user pr15" :style="{ flex: layoutUserFlexNum }">
		<el-dropdown :show-timeout="70" :hide-timeout="50" trigger="click" @command="onComponentSizeChange">
			<div class="layout-navbars-breadcrumb-user-icon">
				<i class="iconfont icon-ziti" title="Component size"></i>
			</div>
			<template #dropdown>
				<el-dropdown-menu>
					<el-dropdown-item command="large" :disabled="state.disabledSize === 'large'">Large</el-dropdown-item>
					<el-dropdown-item command="default" :disabled="state.disabledSize === 'default'">Default</el-dropdown-item>
					<el-dropdown-item command="small" :disabled="state.disabledSize === 'small'">Small</el-dropdown-item>
				</el-dropdown-menu>
			</template>
		</el-dropdown>
		<el-dropdown :show-timeout="70" :hide-timeout="50" trigger="click" @command="onLanguageChange">
			<div class="layout-navbars-breadcrumb-user-icon">
				<i class="iconfont" :class="state.disabledI18n === 'en' ? 'icon-fuhao-yingwen' : 'icon-fuhao-zhongwen'"
					title="language switch"></i>
			</div>
			<template #dropdown>
				<el-dropdown-menu>
					<el-dropdown-item v-for="lang in state.languages" :key="lang.value" :command="lang.value"
						:disabled="lang.value === state.disabledI18n">
						{{ lang.label }}
					</el-dropdown-item>
				</el-dropdown-menu>
			</template>
		</el-dropdown>
		<div class="layout-navbars-breadcrumb-user-icon" @click="onSearchClick">
			<el-icon title="Menu search">
				<ele-Search />
			</el-icon>
		</div>
		<div class="layout-navbars-breadcrumb-user-icon" @click="onLayoutSetingClick">
			<i class="icon-skin iconfont" title="layout configuration"></i>
		</div>
		<div class="layout-navbars-breadcrumb-user-icon">
			<el-popover placement="bottom" trigger="click" transition="el-zoom-in-top" :width="400" :persistent="false">
				<template #reference>
					<el-badge :is-dot="hasUnreadNotice">
						<el-icon title="information">
							<ele-Bell />
						</el-icon>
					</el-badge>
				</template>
				<UserNews :noticeList="state.noticeList" />
			</el-popover>
		</div>
		<div class="layout-navbars-breadcrumb-user-icon" @click="onScreenfullClick">
			<i class="iconfont" :title="state.isScreenfull ? 'Exit Fullscreen' : 'Enter Fullscreen'"
				:class="!state.isScreenfull ? 'icon-fullscreen' : 'icon-tuichuquanping'"></i>
		</div>
		<div class="layout-navbars-breadcrumb-user-icon mr10" @click="onOnlineUserClick">
			<el-icon title="Online users">
				<ele-User />
			</el-icon>
		</div>
		<el-dropdown :show-timeout="70" :hide-timeout="50" trigger="click" size="large" @command="onHandleCommandClick">
			<span class="layout-navbars-breadcrumb-user-link">
				<el-tooltip effect="dark" placement="left">
					<template #content>
						Account: {{ userInfos.account }}<br />
						Name: {{ userInfos.realName }}<br />
						Phone: {{ userInfos.phone }}<br />
						Email: {{ userInfos.email }}<br />
						Department: {{ userInfos.orgName }}<br />
						Position: {{ userInfos.posName }}<br />
					</template>
					<img :src="userInfos.avatar" class="layout-navbars-breadcrumb-user-link-photo mr5" />
				</el-tooltip>
				{{ userInfos.realName == '' ? userInfos.account : userInfos.realName }}
				<el-icon class="dropdown-icon">
					<ele-ArrowDown />
				</el-icon>
			</span>
			<template #dropdown>
				<el-dropdown-menu>
					<!-- <el-dropdown-item command="/dashboard/home">Home</el-dropdown-item> -->
					<el-dropdown-item :icon="Avatar" command="/system/userCenter">Personal Center</el-dropdown-item>
					<el-dropdown-item :icon="Loading" command="clearCache">clear cache</el-dropdown-item>
					<el-dropdown-item :icon="Switch" divided command="changeTenant"
						v-if="auth('sysTenant:changeTenant')">Switch Tenant</el-dropdown-item>
					<el-dropdown-item :icon="Lock" divided command="lockScreen">Turn on lock screen</el-dropdown-item>
					<el-dropdown-item :icon="CircleCloseFilled" divided command="logOut">Log out</el-dropdown-item>
				</el-dropdown-menu>
			</template>
		</el-dropdown>
		<Search ref="searchRef" />
		<OnlineUser ref="onlineUserRef" />
		<ChangeTenant ref="changeTenantRef" />
	</div>
</template>

<script setup lang="ts" name="layoutBreadcrumbUser">
import { defineAsyncComponent, ref, computed, reactive, onMounted } from 'vue';
import { useCssVar } from '@vueuse/core'
import { useRouter } from 'vue-router';
import { ElMessageBox, ElMessage, ElNotification } from 'element-plus';
import screenfull from 'screenfull';
import { storeToRefs } from 'pinia';
import { useUserInfo } from '/@/stores/userInfo';
import { useThemeConfig } from '/@/stores/themeConfig';
import other from '/@/utils/other';
import mittBus from '/@/utils/mitt';
import { Local, Session } from '/@/utils/storage';
import Push from 'push.js';
import { signalR } from '/@/views/system/onlineUser/signalR';
import { Avatar, CircleCloseFilled, Loading, Lock, Switch } from '@element-plus/icons-vue';
import { accessTokenKey, clearAccessAfterReload, getAPI } from '/@/utils/axios-utils';
import { SysAuthApi, SysNoticeApi, SysUserApi } from '/@/api-services/api';
import { auth } from '/@/utils/authFunction';
import { useLangStore } from '/@/stores/useLangStore';

const langStore = useLangStore();
// Introduce components
const UserNews = defineAsyncComponent(() => import('/@/layout/navBars/topBar/userNews.vue'));
const Search = defineAsyncComponent(() => import('/@/layout/navBars/topBar/search.vue'));
const OnlineUser = defineAsyncComponent(() => import('/@/views/system/onlineUser/index.vue'));
const ChangeTenant = defineAsyncComponent(() => import('./changeTenant.vue'));

// Define variable content
const router = useRouter();
const stores = useUserInfo();
const storesThemeConfig = useThemeConfig();
const { userInfos } = storeToRefs(stores);
const { themeConfig } = storeToRefs(storesThemeConfig);
const searchRef = ref();
const onlineUserRef = ref();
const changeTenantRef = ref();
const state = reactive({
	isScreenfull: false,
	disabledI18n: 'zh-cn',
	disabledSize: 'large',
	noticeList: [] as any, // Site message list
	languages: [] as any, // Language list
});
// Set split style
const layoutUserFlexNum = computed(() => {
	let num: string | number = '';
	const { layout, isClassicSplitMenu } = themeConfig.value;
	const layoutArr: string[] = ['defaults', 'columns'];
	if (layoutArr.includes(layout) || (layout === 'classic' && !isClassicSplitMenu)) num = '1';
	else num = '';
	return num;
});
// Are there any unread messages?
const hasUnreadNotice = computed(() => {
	return state.noticeList.some((r: any) => r.readStatus == undefined || r.readStatus == 0);
});
// Full screen click
const onScreenfullClick = () => {
	if (!screenfull.isEnabled) {
		ElMessage.warning('Full screen is not supported yet');
		return false;
	}
	screenfull.toggle();
	screenfull.on('change', () => {
		if (screenfull.isFullscreen) state.isScreenfull = true;
		else state.isScreenfull = false;
	});
};
// Layout configuration icon when clicked
const onLayoutSetingClick = () => {
	mittBus.emit('openSettingsDrawer');
};
// When the drop-down menu is clicked
const onHandleCommandClick = (path: string) => {
	if (path === 'clearCache') {
		Local.clear();
		Session.clear();
		window.location.reload();
	} else if (path === 'lockScreen') {
		Local.remove('themeConfig');
		themeConfig.value.isLockScreen = true;
		themeConfig.value.lockScreenTime = 1;
		Local.set('themeConfig', themeConfig.value);
	} else if (path === 'logOut') {
		ElMessageBox({
			closeOnClickModal: false,
			closeOnPressEscape: false,
			title: 'Prompt',
			message: 'thisOperationwillLog out, YesnoContinue?',
			type: 'warning',
			showCancelButton: true,
			confirmButtonText: 'Confirm',
			cancelButtonText: 'Cancel',
			buttonSize: 'default',
			beforeClose: async (action, instance, done) => {
				if (action === 'confirm') {
					instance.confirmButtonLoading = true;
					instance.confirmButtonText = 'Exiting';
					try {
						await getAPI(SysAuthApi).apiSysAuthLogoutPost();
					} catch (error) {
						console.error(error);
					}
					instance.confirmButtonLoading = false;
					done();
				} else {
					done();
				}
			},
		})
			.then(async () => {
				clearAccessAfterReload();
			})
			.catch(() => { });
	} else if (path === 'changeTenant') {
		changeTenantRef.value?.openDialog();
	} else {
		router.push(path);
	}
};
// Menu search click
const onSearchClick = () => {
	searchRef.value.openSearch();
};
// Online user list
const onOnlineUserClick = () => {
	onlineUserRef.value.openDrawer();
};
// Component size changes
const onComponentSizeChange = (size: string) => {
	Local.remove('themeConfig');
	themeConfig.value.globalComponentSize = size;
	Local.set('themeConfig', themeConfig.value);
	initI18nOrSize('globalComponentSize', 'disabledSize');
	//window.location.reload();
};
// language switch
const onLanguageChange = async (lang: string) => {
	const langItem = state.languages.find((item: { value: string }) => item.value === lang);
	if (langItem) {
		await getAPI(SysUserApi).apiSysUserSetLangCodeLangCodePost(langItem.code);
		const accessToken = Local.get(accessTokenKey);
		await getAPI(SysAuthApi).apiSysAuthRefreshTokenGet(`${accessToken}`);
		window.location.reload();
	}
	Local.remove('themeConfig');
	themeConfig.value.globalI18n = lang;
	Local.set('themeConfig', themeConfig.value);
	window.$changeLang(lang)
	other.useTitle();
	initI18nOrSize('globalI18n', 'disabledI18n');
};
// Initialize component size/i18n
const initI18nOrSize = (value: string, attr: string) => {
	(<any>state)[attr] = Local.get('themeConfig')[value];

    // Set menu height - landscape
    useCssVar('--el-menu-horizontal-height').value = 'var(--el-menu-item-height-' + themeConfig.value.globalComponentSize + ')';
    useCssVar('--el-menu-horizontal-sub-item-height').value = 'var(--el-menu-item-height-' + themeConfig.value.globalComponentSize + ')';
    // Set menu height - portrait
    useCssVar('--el-menu-item-height').value = 'var(--el-menu-item-height-' + themeConfig.value.globalComponentSize + ')';
    useCssVar('--el-menu-sub-item-height').value = 'var(--el-menu-item-height-' + themeConfig.value.globalComponentSize + ')';
};
// When the page loads
onMounted(async () => {
	state.languages = langStore.languages;
	if (Local.get('themeConfig')) {
		initI18nOrSize('globalComponentSize', 'disabledSize');
		const userLangCode = userInfos.value.langCode;
		const langItem = state.languages.find((item: { code: string }) => item.code === userLangCode);
		if (langItem) {
			const themeConfig = Local.get('themeConfig');
			themeConfig.globalI18n = langItem.value;
			Local.set('themeConfig', themeConfig);
			initI18nOrSize('globalI18n', 'disabledI18n');
		} else {
			initI18nOrSize('globalI18n', 'disabledI18n');
		}
	}
	// Manually obtain user desktop notification permissions
	if (Push.Permission.GRANTED) {
		// Determine whether you currently have permission, if not, obtain it manually
		Push.Permission.request(undefined, undefined);
	}
	// Monitor the browser to see if the current system is on the current page
	document.addEventListener('visibilitychange', () => {
		if (!document.hidden) {
			// Clear and close message notifications,
			Push.clear();
		}
	});
	// Load unread site messages
	var res = await getAPI(SysNoticeApi).apiSysNoticeUnReadListGet();
	state.noticeList = res.data.result ?? [];

	// Receive site messages
	signalR.on('PublicNotice', receiveNotice);

	// // Process message read
	// mittBus.on('noticeRead', (id) => {
	// 	const notice = state.noticeList.find((r: any) => r.id == id);
	// 	if (notice == undefined) return;

	// 	//Set read
	// 	notice.readStatus = 1;
	// });
});
// //When the page is unloaded
// onUnmounted(() => {
// 	mittBus.off('noticeRead', () => {});
// });

const receiveNotice = (msg: any) => {
	state.noticeList.unshift(msg);

	ElNotification({
		title: 'Prompt',
		message: 'You have a new message...',
		type: 'info',
		position: 'bottom-right',
	});
	Push.create('Prompt', {
		body: 'You have a new message',
		icon: 'logo.png', // in the public directory
		timeout: 4500, // Notification display time in milliseconds
	});
};
</script>

<style scoped lang="scss">
.layout-navbars-breadcrumb-user {
	display: flex;
	align-items: center;
	justify-content: flex-end;

	&-link {
		height: 100%;
		display: flex;
		align-items: center;
		white-space: nowrap;
        cursor: pointer;

		&-photo {
			width: 25px;
			height: 25px;
			border-radius: 100%;
		}

        .dropdown-icon {
            transition: transform 0.3s; /* Add toTransition effect */
        }
        &:has(.dropdown-icon)[aria-expanded=true] {
            .dropdown-icon {
                transform: rotate(180deg);
            }
        }
	}

	&-icon {
		padding: 0 10px;
		cursor: pointer;
		color: var(--next-bg-topBarColor);
		height: 50px;
		line-height: 50px;
		display: flex;
		align-items: center;
        font-size: var(--el-font-size-medium);

		&:hover {
			background: var(--next-color-user-hover);

			i {
				display: inline-block;
				animation: logoAnimation 0.3s ease-in-out;
			}
		}
	}

	:deep(.el-dropdown) {
		color: var(--next-bg-topBarColor);
	}

	:deep(.el-badge) {
		height: 40px;
		line-height: 40px;
		display: flex;
		align-items: center;
	}

	:deep(.el-badge__content.is-fixed) {
		top: 12px;
	}
}
</style>
