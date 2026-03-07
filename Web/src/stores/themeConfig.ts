import { defineStore } from 'pinia';

export const useThemeConfig = defineStore('themeConfig', {
	state: (): ThemeConfigState => ({
		themeConfig: {
			// Whether to open the layout configuration drawer
			isDrawer: false,

			/**
			 * global theme
			 */
			// Default primary theme color
			primary: '#0F59A4', // Carmine: #F03F24 // Delphinium blue: #0F59A4 // Mint green: #207F4C
			// Whether to enable dark mode
			isIsDark: false,

			/**
			 * Top bar settings
			 */
			// Default top bar navigation background color
			topBar: '#FFFFFF',
			// Default top bar navigation font color
			topBarColor: '#000000',
			// Whether to enable the top bar background color gradient
			isTopBarColorGradual: false,

			/**
			 * Menu Settings
			 */
			// Default menu navigation background color
			menuBar: '#FFFFFF',
			// Default menu navigation font color
			menuBarColor: '#000000',
			// Default menu highlight background color
			menuBarActiveColor: 'var(--el-color-primary-light-7)',
			// Whether to enable menu background color gradient
			isMenuBarColorGradual: false,

			/**
			 * Column Settings
			 */
			// Default column menu background color
			columnsMenuBar: '#2C3A49',
			// Default column menu font color
			columnsMenuBarColor: '#F0F0F0',
			// Whether to enable the background color gradient of the column menu
			isColumnsMenuBarColorGradual: false,
			// Whether to enable mouse-over preloading for column menus (preview menu)
			isColumnsMenuHoverPreload: false,
			// Column Logo height (px)
			columnsLogoHeight: 50,
			// Column menu width (px)
			columnsMenuWidth: 70,
			// Column menu height (px)
			columnsMenuHeight: 50,

			/**
			 * Interface Settings
			 */
			// Whether to enable the menu horizontal folding effect
			isCollapse: false,
			// Whether to enable the menu accordion effect
			isUniqueOpened: true,
			// Whether to enable fixed header
			isFixedHeader: true,
			// Initialization variable, used to update the height of the menu el-scrollbar, please do not delete it
			isFixedHeaderChange: false,
			// Whether to enable the classic layout split menu (only classic layout takes effect)
			isClassicSplitMenu: false,
			// Whether to turn on automatic screen lock
			isLockScreen: false,
			// Turn on automatic screen lock countdown (s/second)
			lockScreenTime: 300,

			/**
			 * Interface display
			 */
			// Whether to enable the sidebar logo
			isShowLogo: true,
			// Initialization variable, used for height update of el-scrollbar, please do not delete it
			isShowLogoChange: false,
			// Whether to turn on Breadcrumb, forcing the classic and horizontal layout not to be displayed
			isBreadcrumb: true,
			// Whether to enable Tagsview
			isTagsview: true,
			// Whether to enable the Breadcrumb icon
			isBreadcrumbIcon: true,
			// Whether to enable Tagsview icon
			isTagsviewIcon: true,
			// Whether to enable TagsView caching
			isCacheTagsView: true,
			// Whether to enable TagsView drag and drop
			isSortableTagsView: true,
			// Whether to enable TagsView sharing -- shared details interface: only one tagsView will appear; non-shared details interface: multiple tagsView will appear
			isShareTagsView: true,
			// Whether to turn on the copyright information at the bottom of the Footer
			isFooter: true,
			// Whether to enable gray mode
			isGrayscale: false,
			// Whether to enable color weakness mode
			isInvert: false,
			// Whether to turn on watermark
			isWatermark: true,
			// watermark copy
			watermarkText: 'Admin.NET',

			/**
			 * OtherSettings
			 */
			// Tagsview style: optional value "<tags-style-one|tags-style-four|tags-style-five>", default tags-style-five
			// The defined value has the same name as the class in `/src/layout/navBars/tagsView/tagsView.vue`
			tagsStyle: 'tags-style-one',
			// Main page switching animation: Animate.css
			animation: 'fadeLeft',
			// Column highlighting style: optional value "<columns-round|columns-card>", default columns-round
			columnsAsideStyle: 'columns-round',
			// Column layout style: optional value "<columns-horizontal|columns-vertical>", default columns-horizontal
			columnsAsideLayout: 'columns-vertical',

			/**
			 * Layout Switch
			 * Attention：For demonstration，Switch layouttime，The color will berestoreSucceedDefault，Code location：/@/layout/navBars/topBar/settings.vue
			 * inof `initSetLayoutChange (Set layout switch, reset theme style)` Method
			 */
			// Layout switching: optional value "<defaults|classic|transverse|columns>", default defaults
			layout: 'defaults',

			/**
			 * Backend-controlled routing
			 */
			// Whether to enable backend control routing
			isRequestRoutes: true,

			/**
			 * Global websitetitle / Subtitle
			 */
			// Website main title (menu navigation, browser current web page title)
			globalTitle: 'Admin.NET aaaaaaaaaaa',
			// Website subtitle (text at the top of the login page)
			globalViceTitle: 'Admin.NET',
			// Website subtitle (text at the top of the login page)
			globalViceTitleMsg: '.NET General Permission Development Framework Standing on the Shoulders of Giants',
			// Copyright and filing text
			copyright: 'Copyright © 2026. All rights reserved.',
			// Default initial language, optional value "<zh-cn|en|zh-tw>", default zh-cn
			globalI18n: 'zh-cn',
			// Default global component size, optional value "<large|'default'|small>", default 'large'
			globalComponentSize: 'small',
			// System logo address
			logoUrl: '',
			// ICP registration number
			icp: '',
			// ICP address
			icpUrl: '',

			// Whether to enable secondary verification
			secondVer: false,
			// Whether to enable registration function
			registration: false,
			// Hide tenant on login
			hideTenantForLogin: false,
			// Whether to enable verification code
			captcha: false,
			// Is loading completed?
			isLoaded: false,
		},
	}),
	actions: {
		setThemeConfig(data: ThemeConfigState) {
			this.themeConfig = data.themeConfig;
		},
	},
});
