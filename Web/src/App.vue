<template>
	<el-config-provider :size="getGlobalComponentSize" :locale="locale">
		<router-view v-show="setLockScreen" />
		<LockScreen v-if="themeConfig.isLockScreen" />
		<Settings ref="settingsRef" v-show="setLockScreen" />
		<CloseFull v-if="!themeConfig.isLockScreen" />
		<!-- <Upgrade v-if="needUpdate" /> -->
		<!-- <Sponsors /> -->
	</el-config-provider>
</template>

<script setup lang="ts" name="app">
import { defineAsyncComponent, computed, ref, onBeforeMount, onMounted, onUnmounted, nextTick, watch } from 'vue';
import { useRoute } from 'vue-router';
import { storeToRefs } from 'pinia';
import { useTagsViewRoutes } from '/@/stores/tagsViewRoutes';
import { useThemeConfig } from '/@/stores/themeConfig';
import other from '/@/utils/other';
import { Local, Session } from '/@/utils/storage';
import mittBus from '/@/utils/mitt';
import setIntroduction from '/@/utils/setIconfont';
// import Watermark from '/@/utils/watermark';
import { SysConfigApi } from '/@/api-services';
import { getAPI } from '/@/utils/axios-utils';
import { useLangStore } from '/@/stores/useLangStore';
import localeMap from '../lang/elementLocales'

const currentLang = Local.get('themeConfig')?.globalI18n || 'zh-cn'
const locale = computed(() => localeMap[currentLang] || localeMap['zh-cn'])

// Introduce components
const LockScreen = defineAsyncComponent(() => import('/@/layout/lockScreen/index.vue'));
const Settings = defineAsyncComponent(() => import('/@/layout/navBars/topBar/settings.vue'));
const CloseFull = defineAsyncComponent(() => import('/@/layout/navBars/topBar/closeFull.vue'));
// const Upgrade = defineAsyncComponent(() => import('/@/layout/upgrade/index.vue'));
// const Sponsors = defineAsyncComponent(() => import('/@/layout/sponsors/index.vue'));

// Define variable content
const settingsRef = ref();
const route = useRoute();
const stores = useTagsViewRoutes();
const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);
const needUpdate = ref(false);

// Set the component to be displayed and hidden when the lock screen is set
const setLockScreen = computed(() => {
	// Prevent irrelevant interfaces from appearing after refreshing after locking the screen
	// https://gitee.com/lyt-top/vue-next-admin/issues/I6AF8P
	return themeConfig.value.isLockScreen ? themeConfig.value.lockScreenTime > 1 : themeConfig.value.lockScreenTime >= 0;
});

// // Get the version number
// const getVersion = computed(() => {
// 	let isVersion = false;
// 	if (route.path !== '/login') {
// 		// @ts-ignore
// 		if ((Local.get('version') && Local.get('version') !== __NEXT_VERSION__) || !Local.get('version')) isVersion = true;
// 	}
// 	return isVersion;
// });

// checkUpdate(() => {
// 	needUpdate.value = true;
// }, 60000);

// Get the global component size and directly respond to themeConfig
const getGlobalComponentSize = computed(() => {
	return themeConfig.value.globalComponentSize;
});
// Get global i18n
// const getGlobalI18n = computed(() => {
//return messages.value[locale.value];
// });
// Set initialization to prevent restoration to default when refreshing
onBeforeMount(() => {
	// Set batch third-party icon icons
	setIntroduction.cssCdn();
	// Set up batch third-party js
	setIntroduction.jsCdn();
});
// When the page loads
onMounted(() => {
	nextTick(() => {
		// Click to open the listening layout configuration pop-up window.
		mittBus.on('openSettingsDrawer', () => {
			settingsRef.value.openDrawer();
		});
		// Get layout configuration from cache
		if (Local.get('themeConfig')) {
			storesThemeConfig.setThemeConfig({ themeConfig: Local.get('themeConfig') });
			document.documentElement.style.cssText = Local.get('themeConfigStyle');
		}
		// Get full-screen configuration from cache
		if (Session.get('isTagsViewCurrenFull')) {
			stores.setCurrenFullscreen(Session.get('isTagsViewCurrenFull'));
		}
	});
});
// When the page is destroyed, turn off the listening layout configuration/i18n listening
onUnmounted(() => {
	mittBus.off('openSettingsDrawer', () => { });
});
// Monitor routing changes and set website titles
watch(
	() => route.path,
	() => {
		other.useTitle();
	},
	{
		deep: true,
	}
);

// Load system information
const loadSysInfo = () => {
	getAPI(SysConfigApi)
		.apiSysConfigSysInfoGet()
		.then((res) => {
			if (res.data.type != 'success') return;

			const data = res.data.result;
			// System logo
			themeConfig.value.logoUrl = data.logo;
			// main title
			themeConfig.value.globalTitle = data.title;
			// subtitle
			themeConfig.value.globalViceTitle = data.viceTitle;
			// System description
			themeConfig.value.globalViceTitleMsg = data.viceDesc;
			// ICP filing information
			themeConfig.value.icp = data.icp;
			themeConfig.value.icpUrl = data.icpUrl;
			// watermark
			themeConfig.value.isWatermark = data.watermark != null;
			themeConfig.value.watermarkText = data.watermark;
			// Copyright statement
			themeConfig.value.copyright = data.copyright;
			// Login verification
			themeConfig.value.secondVer = data.secondVer == 1;
			themeConfig.value.captcha = data.captcha == 1;
			// Hide tenants when logging in
			themeConfig.value.hideTenantForLogin = data.hideTenantForLogin;
			// Registration function
			themeConfig.value.registration = data.enableReg == 1;
			// Update configuration loading status
			themeConfig.value.isLoaded = true;

			// Update favicon
			updateFavicon(data.logo);

			// Save configuration
			Local.remove('themeConfig');
			Local.set('themeConfig', storesThemeConfig.themeConfig);
		})
		.catch(() => {
			// Leave the logo address blank
			themeConfig.value.logoUrl = '';
			// Save configuration
			Local.remove('themeConfig');
			Local.set('themeConfig', storesThemeConfig.themeConfig);
			return;
		});
};

// Update favicon
const updateFavicon = (url: string): void => {
	const favicon = document.getElementById('favicon') as HTMLAnchorElement;
	favicon!.href = url ? url : 'data:;base64,=';
};

// Load system information
loadSysInfo();
const langStore = useLangStore();
langStore.loadLanguages();
// Prevent Firefox from opening new windows while dragging
document.body.ondrop = function (event) {
	event.preventDefault();
	event.stopPropagation();
};
</script>
