/**
 * pinia TypeDefinition
 */

// User information
declare interface UserInfos<T = any> {
	authBtnList: string[];
	photo: string;
	roles: string[];
	time: number;
	userName: string;
	
	id:string;
	account:string;
	realName:string;
	phone:string;
	idCardNum:string;
	email:string;
	accountType:number;
	avatar:string;
	address:string;
	signature:string;
	orgId:string;
	orgName:string;
	tenantId:string;
	langCode:string;
	currentTenantId:string;

	[key: string]: T;

}
declare interface UserInfosState {
	userInfos: UserInfos;
	constList: T[];
	dictList: T;
}

// Route cache list
declare interface KeepAliveNamesState {
	keepAliveNames: string[];
	cachedViews: string[];
}

// The backend returns the original route (when not processed)
declare interface RequestOldRoutesState {
	requestOldRoutes: string[];
}

// TagsView route list
declare interface TagsViewRoutesState<T = any> {
	tagsViewRoutes: T[];
	isTagsViewCurrenFull: Boolean;
}

// Route list
declare interface RoutesListState<T = any> {
	routesList: T[];
	isColumnsMenuHover: Boolean;
	isColumnsNavHover: Boolean;
}

// layout configuration
declare interface ThemeConfigState {
	themeConfig: {
		isDrawer: boolean; // Whether to enable drawer configuration
		primary: string; // theme color
		topBar: string; // top bar background
		topBarColor: string; // Top bar background color
		isTopBarColorGradual: boolean; // Whether the top bar background gradient
		menuBar: string; // Sidebar menu bar background
		menuBarColor: string; // Sidebar menu bar background color
		menuBarActiveColor: string; // Sidebar activation item background color
		isMenuBarColorGradual: boolean; // Whether the sidebar menu bar background gradient
		columnsMenuBar: string; // Sidebar menu bar background
		columnsMenuBarColor: string; // Sidebar menu bar background
		isColumnsMenuBarColorGradual: boolean; // Whether the sidebar menu bar background gradient
		isColumnsMenuHoverPreload: boolean; // Whether to preload routes on mouseover
		columnsLogoHeight: number; // Sidebar logo height
		columnsMenuWidth: number; // sidebar width
		columnsMenuHeight: number; // sidebar height
		isCollapse: boolean; // Whether to collapse the menu horizontally (supports mobile phones)
		isUniqueOpened: boolean; // Whether to keep only one menu expanded
		isFixedHeader: boolean; // Whether to fix the head
		isFixedHeaderChange: boolean; // Whether to fix the head
		isClassicSplitMenu: boolean; // Whether to split the menu
		isLockScreen: boolean; // Whether to enable lock screen
		lockScreenTime: number; // lock screen time
		isShowLogo: boolean; // Whether to display logo
		isShowLogoChange: boolean; // Whether to display logo animation
		isBreadcrumb: boolean; // Whether to display breadcrumbs
		isTagsview: boolean; // Whether to display multiple tab pages
		isBreadcrumbIcon: boolean; // Whether to display the breadcrumb icon
		isTagsviewIcon: boolean; // Whether to display multi-tab icons
		isCacheTagsView: boolean; // Whether to cache TagsView
		isSortableTagsView: boolean; // Whether to enable drag and drop sorting
		isShareTagsView: boolean; // Whether to enable multi-tab caching
		isFooter: boolean; // Whether to display footer
		isGrayscale: boolean; // Whether grayscale mode
		isInvert: boolean; // Whether color weak mode
		isIsDark: boolean; // Whether dark mode
		isWatermark: boolean; // Whether to turn on watermark
		watermarkText: string; // Watermark content
		tagsStyle: string; // Tab theme
		animation: string; // animation
		columnsAsideStyle: string; // Sidebar theme
		columnsAsideLayout: string; // Sidebar layout
		layout: string; // layout mode
		isRequestRoutes: boolean; // Whether to enable lazy loading of routes
		globalI18n: string; // Whether to enable internationalization
		globalComponentSize: string; // global component size
		globalTitle: string; // global title
		globalViceTitle: string; // global subtitle
		globalViceTitleMsg: string; // Global subtitle message
		copyright: string; // Copyright information
		logoUrl: string; // System logo address
		icp: string; // ICP registration number
		icpUrl: string; // ICP address
		secondVer: boolean; // Whether to enable secondary verification
		registration: boolean; // Whether to enable registration function
		hideTenantForLogin: boolean; // Hide tenants when logging in
		captcha: boolean; // Whether to enable verification code
		isLoaded: boolean; // Is loading completed?
	};
}
