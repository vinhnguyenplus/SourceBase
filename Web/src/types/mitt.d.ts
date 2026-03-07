/**
 * mitt EventTypeDefinition
 *
 * @method openSettingsDrawer OpenLayout Settings Popup
 * @method restoreDefault ColumnLayout，ratMouse Over、RemoveDataDisplay
 * @method setSendColumnsChildren ColumnLayout，ratMouse Over、RemovemenuDatapassed into navMenu undermenuin
 * @method setSendClassicChildren classicLayout，Start Cuttingmenutime，menuDatapassed into navMenu undermenuin
 * @method getBreadcrumbIndexSetFilterRoutes Layout Settings Popup，Start Cuttingmenutime，menuDatapassed into navMenu undermenuin
 * @method layoutMobileResize BrowserWindow changetime，Used for adapting to mobile devicesInterface display
 * @method openOrCloseSortable Layout Settings Popup，Enable TagsView Dragging
 * @method openShareTagsView Layout Settings Popup，Enable TagsView sharing
 * @method onTagsViewRefreshRouterView tagsview Refreshinterface
 * @method onCurrentContextmenuClick tagsview Right-clickmenuEach clicktime
 */
declare type MittType<T = any> = {
	openSettingsDrawer?: string;
	restoreDefault?: string;
	setSendColumnsChildren: T;
	setSendClassicChildren: T;
	getBreadcrumbIndexSetFilterRoutes?: string;
	layoutMobileResize: T;
	openOrCloseSortable?: string;
	openShareTagsView?: string;
	onTagsViewRefreshRouterView?: T;
	onCurrentContextmenuClick?: T;
};

// mitt parameter type definition
declare type LayoutMobileResize = {
	layout: string;
	clientWidth: number;
};

// mitt parameter menu type
declare type MittMenu = {
	children: RouteRecordRaw[];
	item?: RouteItem;
};
