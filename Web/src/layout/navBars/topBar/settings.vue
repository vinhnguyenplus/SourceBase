<template>
	<div class="layout-breadcrumb-seting">
		<el-drawer title="layout configuration" v-model="getThemeConfig.isDrawer" direction="rtl" destroy-on-close size="280px" @close="onDrawerClose">
			<el-scrollbar class="layout-breadcrumb-seting-bar">
				<!-- global theme -->
				<el-divider content-position="center">global theme</el-divider>
				<div class="layout-breadcrumb-seting-bar-flex">
					<div class="layout-breadcrumb-seting-bar-flex-label">theme color</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-color-picker v-model="getThemeConfig.primary" size="default" @change="onColorPickerChange"> </el-color-picker>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15">
					<div class="layout-breadcrumb-seting-bar-flex-label">dark mode</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isIsDark" size="small" @change="onAddDarkChange"></el-switch>
					</div>
				</div>

				<!-- Top bar settings -->
				<el-divider content-position="center">Top bar settings</el-divider>
				<div class="layout-breadcrumb-seting-bar-flex">
					<div class="layout-breadcrumb-seting-bar-flex-label">top bar background</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-color-picker v-model="getThemeConfig.topBar" size="default" @change="onBgColorPickerChange('topBar')"> </el-color-picker>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex">
					<div class="layout-breadcrumb-seting-bar-flex-label">Default font color of the top bar</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-color-picker v-model="getThemeConfig.topBarColor" size="default" @change="onBgColorPickerChange('topBarColor')"> </el-color-picker>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt10">
					<div class="layout-breadcrumb-seting-bar-flex-label">Top bar background gradient</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isTopBarColorGradual" size="small" @change="onTopBarGradualChange"></el-switch>
					</div>
				</div>

				<!-- Menu settings -->
				<el-divider content-position="center">Menu Settings</el-divider>
				<div class="layout-breadcrumb-seting-bar-flex">
					<div class="layout-breadcrumb-seting-bar-flex-label">Menu Background</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-color-picker v-model="getThemeConfig.menuBar" size="default" @change="onBgColorPickerChange('menuBar')"> </el-color-picker>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex">
					<div class="layout-breadcrumb-seting-bar-flex-label">Default menu font color</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-color-picker v-model="getThemeConfig.menuBarColor" size="default" @change="onBgColorPickerChange('menuBarColor')"> </el-color-picker>
					</div>
				</div>
				<!-- <div class="layout-breadcrumb-seting-bar-flex">
					<div class="layout-breadcrumb-seting-bar-flex-label">Menu highlight background color</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-color-picker v-model="getThemeConfig.menuBarActiveColor" size="default" show-alpha @change="onBgColorPickerChange('menuBarActiveColor')" />
					</div>
				</div> -->
				<div class="layout-breadcrumb-seting-bar-flex mt14">
					<div class="layout-breadcrumb-seting-bar-flex-label">Menu background gradient</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isMenuBarColorGradual" size="small" @change="onMenuBarGradualChange"></el-switch>
					</div>
				</div>

				<!-- Column settings -->
				<el-divider content-position="center" :style="{ opacity: getThemeConfig.layout !== 'columns' ? 0.5 : 1 }">Column Settings</el-divider>
				<div class="layout-breadcrumb-seting-bar-flex" :style="{ opacity: getThemeConfig.layout !== 'columns' ? 0.5 : 1 }">
					<div class="layout-breadcrumb-seting-bar-flex-label">ColumnMenu Background</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-color-picker v-model="getThemeConfig.columnsMenuBar" size="default" @change="onBgColorPickerChange('columnsMenuBar')" :disabled="getThemeConfig.layout !== 'columns'">
						</el-color-picker>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex" :style="{ opacity: getThemeConfig.layout !== 'columns' ? 0.5 : 1 }">
					<div class="layout-breadcrumb-seting-bar-flex-label">Column menu default font color</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-color-picker v-model="getThemeConfig.columnsMenuBarColor" size="default" @change="onBgColorPickerChange('columnsMenuBarColor')" :disabled="getThemeConfig.layout !== 'columns'">
						</el-color-picker>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt14" :style="{ opacity: getThemeConfig.layout !== 'columns' ? 0.5 : 1 }">
					<div class="layout-breadcrumb-seting-bar-flex-label">Column menu background gradient</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isColumnsMenuBarColorGradual" size="small" @change="onColumnsMenuBarGradualChange" :disabled="getThemeConfig.layout !== 'columns'"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt14" :style="{ opacity: getThemeConfig.layout !== 'columns' ? 0.5 : 1 }">
					<div class="layout-breadcrumb-seting-bar-flex-label">Column menu mouseover preloading</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isColumnsMenuHoverPreload" size="small" @change="onColumnsMenuHoverPreloadChange" :disabled="getThemeConfig.layout !== 'columns'"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt11">
					<div class="layout-breadcrumb-seting-bar-flex-label">Column Logo height (px)</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-input-number
							v-model="getThemeConfig.columnsLogoHeight"
							controls-position="right"
							:min="1"
							:max="9999"
							@change="onColumnsLogoHeightChange"
							size="small"
							style="width: 90px; margin-right: 1px"
						>
						</el-input-number>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt11">
					<div class="layout-breadcrumb-seting-bar-flex-label">Column menu width (px)</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-input-number
							v-model="getThemeConfig.columnsMenuWidth"
							controls-position="right"
							:min="1"
							:max="9999"
							@change="onColumnsMenuWidthChange"
							size="small"
							style="width: 90px; margin-right: 1px"
						>
						</el-input-number>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt11">
					<div class="layout-breadcrumb-seting-bar-flex-label">Column menu height (px)</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-input-number
							v-model="getThemeConfig.columnsMenuHeight"
							controls-position="right"
							:min="1"
							:max="9999"
							@change="onColumnsMenuHeightChange"
							size="small"
							style="width: 90px; margin-right: 1px"
						>
						</el-input-number>
					</div>
				</div>

				<!-- Interface settings -->
				<el-divider content-position="center">Interface Settings</el-divider>
				<div class="layout-breadcrumb-seting-bar-flex" :style="{ opacity: getThemeConfig.layout === 'transverse' ? 0.5 : 1 }">
					<div class="layout-breadcrumb-seting-bar-flex-label">Menu collapses horizontally</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isCollapse" :disabled="getThemeConfig.layout === 'transverse'" size="small" @change="onThemeConfigChange"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15" :style="{ opacity: getThemeConfig.layout === 'transverse' ? 0.5 : 1 }">
					<div class="layout-breadcrumb-seting-bar-flex-label">menu accordion</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isUniqueOpened" :disabled="getThemeConfig.layout === 'transverse'" size="small" @change="setLocalThemeConfig"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15">
					<div class="layout-breadcrumb-seting-bar-flex-label">Fixed Header</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isFixedHeader" size="small" @change="onIsFixedHeaderChange"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15" :style="{ opacity: getThemeConfig.layout !== 'classic' ? 0.5 : 1 }">
					<div class="layout-breadcrumb-seting-bar-flex-label">Classic layout split menu</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isClassicSplitMenu" :disabled="getThemeConfig.layout !== 'classic'" size="small" @change="onClassicSplitMenuChange"> </el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15">
					<div class="layout-breadcrumb-seting-bar-flex-label">Turn on lock screen</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isLockScreen" size="small" @change="setLocalThemeConfig"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt11">
					<div class="layout-breadcrumb-seting-bar-flex-label">Auto Lock Screen (s/seconds)</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-input-number v-model="getThemeConfig.lockScreenTime" controls-position="right" :min="1" :max="9999" @change="setLocalThemeConfig" size="small" style="width: 90px; margin-right: 1px">
						</el-input-number>
					</div>
				</div>

				<!-- Interface display -->
				<el-divider content-position="center">Interface display</el-divider>
				<div class="layout-breadcrumb-seting-bar-flex mt15">
					<div class="layout-breadcrumb-seting-bar-flex-label">Sidebar Logo</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isShowLogo" size="small" @change="onIsShowLogoChange"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15" :style="{ opacity: getThemeConfig.layout === 'classic' || getThemeConfig.layout === 'transverse' ? 0.5 : 1 }">
					<div class="layout-breadcrumb-seting-bar-flex-label">Enable Breadcrumb</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch
							v-model="getThemeConfig.isBreadcrumb"
							:disabled="getThemeConfig.layout === 'classic' || getThemeConfig.layout === 'transverse'"
							size="small"
							@change="onIsBreadcrumbChange"
						></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15">
					<div class="layout-breadcrumb-seting-bar-flex-label">Turn on the Breadcrumb icon</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isBreadcrumbIcon" size="small" @change="setLocalThemeConfig"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15">
					<div class="layout-breadcrumb-seting-bar-flex-label">Open Tagsview</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isTagsview" size="small" @change="setLocalThemeConfig"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15">
					<div class="layout-breadcrumb-seting-bar-flex-label">Turn on Tagsview icon</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isTagsviewIcon" size="small" @change="setLocalThemeConfig"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15">
					<div class="layout-breadcrumb-seting-bar-flex-label">Enable TagsView Cache</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isCacheTagsView" size="small" @change="setLocalThemeConfig"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15" :style="{ opacity: state.isMobile ? 0.5 : 1 }">
					<div class="layout-breadcrumb-seting-bar-flex-label">Enable TagsView Dragging</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isSortableTagsView" :disabled="state.isMobile ? true : false" size="small" @change="onSortableTagsViewChange"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15">
					<div class="layout-breadcrumb-seting-bar-flex-label">Enable TagsView sharing</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isShareTagsView" size="small" @change="onShareTagsViewChange"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15">
					<div class="layout-breadcrumb-seting-bar-flex-label">Open Footer</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isFooter" size="small" @change="setLocalThemeConfig"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15">
					<div class="layout-breadcrumb-seting-bar-flex-label">Gray mode</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isGrayscale" size="small" @change="onAddFilterChange('grayscale')"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15">
					<div class="layout-breadcrumb-seting-bar-flex-label">Color Weakness Mode</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-switch v-model="getThemeConfig.isInvert" size="small" @change="onAddFilterChange('invert')"></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15">
					<div class="layout-breadcrumb-seting-bar-flex-label">Enable watermark</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<!-- Controlled by parameter configuration sys_watermark -->
						<el-switch v-model="getThemeConfig.isWatermark" size="small" @change="onWatermarkChange" disabled></el-switch>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt14">
					<div class="layout-breadcrumb-seting-bar-flex-label">watermark copy</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-input v-model="getThemeConfig.watermarkText" size="small" style="width: 90px; margin-right: 1px" @input="onWatermarkTextInput" disabled></el-input>
					</div>
				</div>

				<!-- Other settings -->
				<el-divider content-position="center">OtherSettings</el-divider>
				<div class="layout-breadcrumb-seting-bar-flex mt15">
					<div class="layout-breadcrumb-seting-bar-flex-label">Tagsview style</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-select v-model="getThemeConfig.tagsStyle" placeholder="Please select" size="small" style="width: 90px; margin-right: 1px" @change="setLocalThemeConfig">
							<el-option label="Style 1" value="tags-style-one"></el-option>
							<el-option label="Style 4" value="tags-style-four"></el-option>
							<el-option label="Style 5" value="tags-style-five"></el-option>
						</el-select>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15">
					<div class="layout-breadcrumb-seting-bar-flex-label">Home page switching animation</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-select v-model="getThemeConfig.animation" placeholder="Please select" size="small" style="width: 90px; margin-right: 1px" @change="setLocalThemeConfig">
							<el-option label="slide-right" value="slide-right"></el-option>
							<el-option label="slide-left" value="slide-left"></el-option>
							<el-option label="opacitys" value="opacitys"></el-option>
							<el-option label="fade" value="fade"></el-option>
							<el-option label="fadeUp" value="fadeUp"></el-option>
							<el-option label="fadeDown" value="fadeDown"></el-option>
							<el-option label="fadeLeft" value="fadeLeft"></el-option>
							<el-option label="fadeRight" value="fadeRight"></el-option>
							<el-option label="lightSpeedLeft" value="lightSpeedLeft"></el-option>
							<el-option label="lightSpeedRight" value="lightSpeedRight"></el-option>
							<el-option label="zoom" value="zoom"></el-option>
							<el-option label="zoomUp" value="zoomUp"></el-option>
							<el-option label="zoomDown" value="zoomDown"></el-option>
							<el-option label="zoomLeft" value="zoomLeft"></el-option>
							<el-option label="zoomRight" value="zoomRight"></el-option>
							<el-option label="flip" value="flip"></el-option>
							<el-option label="flipX" value="flipX"></el-option>
							<el-option label="flipY" value="flipY"></el-option>
							<el-option label="backUp" value="backUp"></el-option>
							<el-option label="backDown" value="backDown"></el-option>
							<el-option label="backLeft" value="backLeft"></el-option>
							<el-option label="backRight" value="backRight"></el-option>
							<el-option label="bounce" value="bounce"></el-option>
							<el-option label="bounceUp" value="bounceUp"></el-option>
							<el-option label="bounceDown" value="bounceDown"></el-option>
							<el-option label="bounceLeft" value="bounceLeft"></el-option>
							<el-option label="bounceRight" value="bounceRight"></el-option>
						</el-select>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15" :style="{ opacity: getThemeConfig.layout !== 'columns' ? 0.5 : 1 }">
					<div class="layout-breadcrumb-seting-bar-flex-label">ColumnHighlight Style</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-select
							v-model="getThemeConfig.columnsAsideStyle"
							placeholder="Please select"
							size="small"
							style="width: 90px; margin-right: 1px"
							:disabled="getThemeConfig.layout !== 'columns' ? true : false"
							@change="setLocalThemeConfig"
						>
							<el-option label="Rounded corner" value="columns-round"></el-option>
							<el-option label="card" value="columns-card"></el-option>
						</el-select>
					</div>
				</div>
				<div class="layout-breadcrumb-seting-bar-flex mt15 mb27" :style="{ opacity: getThemeConfig.layout !== 'columns' ? 0.5 : 1 }">
					<div class="layout-breadcrumb-seting-bar-flex-label">Column layout style</div>
					<div class="layout-breadcrumb-seting-bar-flex-value">
						<el-select
							v-model="getThemeConfig.columnsAsideLayout"
							placeholder="Please select"
							size="small"
							style="width: 90px; margin-right: 1px"
							:disabled="getThemeConfig.layout !== 'columns' ? true : false"
							@change="setLocalThemeConfig"
						>
							<el-option label="level" value="columns-horizontal"></el-option>
							<el-option label="vertical" value="columns-vertical"></el-option>
						</el-select>
					</div>
				</div>

				<!-- Layout switching -->
				<el-divider content-position="center">Layout Switch</el-divider>
				<div class="layout-drawer-content-flex">
					<!-- defaults layout -->
					<div class="layout-drawer-content-item" @click="onSetLayout('defaults')">
						<section class="el-container el-circular" :class="{ 'drawer-layout-active': getThemeConfig.layout === 'defaults' }">
							<aside class="el-aside" style="width: 20px"></aside>
							<section class="el-container is-vertical">
								<header class="el-header" style="height: 10px"></header>
								<main class="el-main"></main>
							</section>
						</section>
						<div class="layout-tips-warp" :class="{ 'layout-tips-warp-active': getThemeConfig.layout === 'defaults' }">
							<div class="layout-tips-box">
								<p class="layout-tips-txt">Default</p>
							</div>
						</div>
					</div>
					<!-- classic layout -->
					<div class="layout-drawer-content-item" @click="onSetLayout('classic')">
						<section class="el-container is-vertical el-circular" :class="{ 'drawer-layout-active': getThemeConfig.layout === 'classic' }">
							<header class="el-header" style="height: 10px"></header>
							<section class="el-container">
								<aside class="el-aside" style="width: 20px"></aside>
								<section class="el-container is-vertical">
									<main class="el-main"></main>
								</section>
							</section>
						</section>
						<div class="layout-tips-warp" :class="{ 'layout-tips-warp-active': getThemeConfig.layout === 'classic' }">
							<div class="layout-tips-box">
								<p class="layout-tips-txt">classic</p>
							</div>
						</div>
					</div>
					<!-- transverse layout -->
					<div class="layout-drawer-content-item" @click="onSetLayout('transverse')">
						<section class="el-container is-vertical el-circular" :class="{ 'drawer-layout-active': getThemeConfig.layout === 'transverse' }">
							<header class="el-header" style="height: 10px"></header>
							<section class="el-container">
								<section class="el-container is-vertical">
									<main class="el-main"></main>
								</section>
							</section>
						</section>
						<div class="layout-tips-warp" :class="{ 'layout-tips-warp-active': getThemeConfig.layout === 'transverse' }">
							<div class="layout-tips-box">
								<p class="layout-tips-txt">Horizontal</p>
							</div>
						</div>
					</div>
					<!-- columns layout -->
					<div class="layout-drawer-content-item" @click="onSetLayout('columns')">
						<section class="el-container el-circular" :class="{ 'drawer-layout-active': getThemeConfig.layout === 'columns' }">
							<aside class="el-aside-dark" style="width: 10px"></aside>
							<aside class="el-aside" style="width: 20px"></aside>
							<section class="el-container is-vertical">
								<header class="el-header" style="height: 10px"></header>
								<main class="el-main"></main>
							</section>
						</section>
						<div class="layout-tips-warp" :class="{ 'layout-tips-warp-active': getThemeConfig.layout === 'columns' }">
							<div class="layout-tips-box">
								<p class="layout-tips-txt">Column</p>
							</div>
						</div>
					</div>
				</div>
				<div class="copy-config">
					<el-alert title="Click the button below to copy the layout configuration and modify it in `src/stores/themeConfig.ts`." type="warning" :closable="false"> </el-alert>
					<el-button size="default" class="copy-config-btn" type="primary" ref="copyConfigBtnRef" @click="onCopyConfigClick">
						<el-icon class="mr5">
							<ele-CopyDocument />
						</el-icon>
						onekeyCopyConfiguration
					</el-button>
					<el-button size="default" class="copy-config-btn-reset" type="info" @click="onResetConfigClick">
						<el-icon class="mr5">
							<ele-RefreshRight />
						</el-icon>
						Restore default with one click
					</el-button>
				</div>
			</el-scrollbar>
		</el-drawer>
	</div>
</template>

<script setup lang="ts" name="layoutBreadcrumbSeting">
import { nextTick, onUnmounted, onMounted, computed, reactive } from 'vue';
import { ElMessage } from 'element-plus';
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';
import { useChangeColor } from '/@/utils/theme';
import { verifyAndSpace } from '/@/utils/toolsValidate';
import { Local, Session } from '/@/utils/storage';
import Watermark from '/@/utils/watermark';
import commonFunction from '/@/utils/commonFunction';
import other from '/@/utils/other';
import mittBus from '/@/utils/mitt';

// Define variable content
const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);
const { copyText } = commonFunction();
const { getLightColor, getDarkColor } = useChangeColor();
const state = reactive({
	isMobile: false,
});

// Get layout configuration information
const updateThemeConfig = () => {
	if (!themeConfig.value.columnsMenuWidth) {
		themeConfig.value.columnsMenuWidth = 70;
	}
	if (!themeConfig.value.columnsMenuHeight) {
		themeConfig.value.columnsMenuHeight = 50;
	}
	if (!themeConfig.value.columnsLogoHeight) {
		themeConfig.value.columnsLogoHeight = 50;
	}
};
const getThemeConfig = computed(() => {
	updateThemeConfig();
	return themeConfig.value;
});
// 1. Global theme
const onColorPickerChange = () => {
	if (!getThemeConfig.value.primary) return ElMessage.warning('Global theme primary color value cannot be empty');
	// Color deepens
	document.documentElement.style.setProperty('--el-color-primary-dark-2', `${getDarkColor(getThemeConfig.value.primary, 0.1)}`);
	document.documentElement.style.setProperty('--el-color-primary', getThemeConfig.value.primary);
	// lighten color
	for (let i = 1; i <= 9; i++) {
		document.documentElement.style.setProperty(`--el-color-primary-light-${i}`, `${getLightColor(getThemeConfig.value.primary, i / 10)}`);
	}
	setDispatchThemeConfig();
};
// 2. Menu/top bar
const onBgColorPickerChange = (bg: string) => {
	document.documentElement.style.setProperty(`--next-bg-${bg}`, themeConfig.value[bg]);
	if (bg === 'menuBar') {
		document.documentElement.style.setProperty(`--next-bg-menuBar-light-1`, getLightColor(getThemeConfig.value.menuBar, 0.05));
	}
	onTopBarGradualChange();
	onMenuBarGradualChange();
	onColumnsMenuBarGradualChange();
	setDispatchThemeConfig();
};
// 2. Menu/Top Bar --> Top Bar Background Gradient
const onTopBarGradualChange = () => {
	setGraduaFun('.layout-navbars-breadcrumb-index', getThemeConfig.value.isTopBarColorGradual, getThemeConfig.value.topBar);
};
// 2. Menu/Top Bar --> Menu background gradient
const onMenuBarGradualChange = () => {
	setGraduaFun('.layout-container .el-aside', getThemeConfig.value.isMenuBarColorGradual, getThemeConfig.value.menuBar);
};
// 2. Menu/top bar --> Column menu background gradient
const onColumnsMenuBarGradualChange = () => {
	setGraduaFun('.layout-container .layout-columns-aside', getThemeConfig.value.isColumnsMenuBarColorGradual, getThemeConfig.value.columnsMenuBar);
};
// 2. Menu/Top Bar --> Background Gradient Function
const setGraduaFun = (el: string, bool: boolean, color: string) => {
	nextTick(() => {
		setTimeout(() => {
			let els = document.querySelector(el);
			if (!els) return false;
			document.documentElement.style.setProperty('--el-menu-bg-color', document.documentElement.style.getPropertyValue('--next-bg-menuBar'));
			if (bool) els.setAttribute('style', `background:linear-gradient(to bottom , ${color}, ${getLightColor(color, 0.5)})`);
			else els.setAttribute('style', ``);
			setLocalThemeConfig();
		}, 300);
	});
};
// 2. Column settings -> Column menu mouse hover preloading
const onColumnsMenuHoverPreloadChange = () => {
	setLocalThemeConfig();
};
// 2. Column settings -> Column Logo height
const onColumnsLogoHeightChange = () => {
	document.documentElement.style.setProperty('--el-columnsLogoHeight', `${themeConfig.value.columnsLogoHeight}px`);
	setLocalThemeConfig();
};
// 2. Column settings -> Column menu width
const onColumnsMenuWidthChange = () => {
	document.documentElement.style.setProperty('--el-columnsMenuWidth', `${themeConfig.value.columnsMenuWidth}px`);
	document.documentElement.style.setProperty('--el-columnsMenuRoundWidth', `${themeConfig.value.columnsMenuWidth - 5}px`);
	setLocalThemeConfig();
};
// 2. Column settings -> Column menu height
const onColumnsMenuHeightChange = () => {
	document.documentElement.style.setProperty('--el-columnsMenuHeight', `${themeConfig.value.columnsMenuHeight}px`);
	document.documentElement.style.setProperty('--el-columnsMenuRoundHeight', `${themeConfig.value.columnsMenuHeight - 5}px`);
	setLocalThemeConfig();
};
// 3. Interface settings --> Menu horizontal folding
const onThemeConfigChange = () => {
	setDispatchThemeConfig();
};
// 3. Interface settings --> Fixed Header
const onIsFixedHeaderChange = () => {
	getThemeConfig.value.isFixedHeaderChange = getThemeConfig.value.isFixedHeader ? false : true;
	setLocalThemeConfig();
};
// 3. Interface settings --> Classic layout split menu
const onClassicSplitMenuChange = () => {
	getThemeConfig.value.isBreadcrumb = false;
	setLocalThemeConfig();
	mittBus.emit('getBreadcrumbIndexSetFilterRoutes');
};
// 4. Interface display --> Sidebar Logo
const onIsShowLogoChange = () => {
	getThemeConfig.value.isShowLogoChange = getThemeConfig.value.isShowLogo ? false : true;
	setLocalThemeConfig();
};
// 4. Interface display --> Breadcrumb
const onIsBreadcrumbChange = () => {
	if (getThemeConfig.value.layout === 'classic') {
		getThemeConfig.value.isClassicSplitMenu = false;
	}
	setLocalThemeConfig();
};
// 4. Interface display --> Enable TagsView drag and drop
const onSortableTagsViewChange = () => {
	mittBus.emit('openOrCloseSortable');
	setLocalThemeConfig();
};
// 4. Interface display --> Enable TagsView sharing
const onShareTagsViewChange = () => {
	mittBus.emit('openShareTagsView');
	setLocalThemeConfig();
};
// 4. Interface display --> gray mode/color weak mode
const onAddFilterChange = (attr: string) => {
	if (attr === 'grayscale') {
		if (getThemeConfig.value.isGrayscale) getThemeConfig.value.isInvert = false;
	} else {
		if (getThemeConfig.value.isInvert) getThemeConfig.value.isGrayscale = false;
	}
	const cssAttr = attr === 'grayscale' ? `grayscale(${getThemeConfig.value.isGrayscale ? 1 : 0})` : `invert(${getThemeConfig.value.isInvert ? '80%' : '0%'})`;
	const appEle = document.body;
	appEle.setAttribute('style', `filter: ${cssAttr}`);
	setLocalThemeConfig();
};
// 4. Interface display --> dark mode
const onAddDarkChange = () => {
	const body = document.documentElement as HTMLElement;
	if (getThemeConfig.value.isIsDark) body.setAttribute('data-theme', 'dark');
	else body.setAttribute('data-theme', '');
};
// 4. Interface display --> turn on watermark
const onWatermarkChange = () => {
	getThemeConfig.value.isWatermark ? Watermark.set(getThemeConfig.value.watermarkText) : Watermark.del();
	setLocalThemeConfig();
};
// 4. Interface display --> watermark copywriting
const onWatermarkTextInput = (val: string) => {
	getThemeConfig.value.watermarkText = verifyAndSpace(val);
	if (getThemeConfig.value.watermarkText === '') return false;
	if (getThemeConfig.value.isWatermark) Watermark.set(getThemeConfig.value.watermarkText);
	setLocalThemeConfig();
};
// 5. Layout switching
const onSetLayout = (layout: string) => {
	Local.set('oldLayout', layout);
	if (getThemeConfig.value.layout === layout) return false;
	if (layout === 'transverse') getThemeConfig.value.isCollapse = false;
	getThemeConfig.value.layout = layout;
	getThemeConfig.value.isDrawer = false;
	initLayoutChangeFun();
};
// Set layout switching function
const initLayoutChangeFun = () => {
	onBgColorPickerChange('menuBar');
	onBgColorPickerChange('menuBarColor');
	onBgColorPickerChange('menuBarActiveColor');
	onBgColorPickerChange('topBar');
	onBgColorPickerChange('topBarColor');
	onBgColorPickerChange('columnsMenuBar');
	onBgColorPickerChange('columnsMenuBarColor');
};
// When closing the pop-up window, initialize the variables. Variables are used to handle layoutScrollbarRef.value.update() to update the scroll bar height
const onDrawerClose = () => {
	getThemeConfig.value.isFixedHeaderChange = false;
	getThemeConfig.value.isShowLogoChange = false;
	getThemeConfig.value.isDrawer = false;
	setLocalThemeConfig();
};
// The layout configuration pop-up window opens.
const openDrawer = () => {
	getThemeConfig.value.isDrawer = true;
};
// Trigger store layout configuration update
const setDispatchThemeConfig = () => {
	setLocalThemeConfig();
	setLocalThemeConfigStyle();
};
// Store layout configuration
const setLocalThemeConfig = () => {
	Local.remove('themeConfig');
	Local.set('themeConfig', getThemeConfig.value);
};
// Store layout configuration global theme style (html root tag)
const setLocalThemeConfigStyle = () => {
	Local.set('themeConfigStyle', document.documentElement.style.cssText);
};
// One-click copy configuration
const onCopyConfigClick = () => {
	let copyThemeConfig = Local.get('themeConfig');
	copyThemeConfig.isDrawer = false;
	copyText(JSON.stringify(copyThemeConfig)).then(() => {
		getThemeConfig.value.isDrawer = false;
	});
};
// Restore default with one click
const onResetConfigClick = () => {
	Local.clear();
	Session.clear();
	window.location.reload();
	// @ts-ignore
	Local.set('version', __NEXT_VERSION__);
};
// Initialize menu style, etc.
const initSetStyle = () => {
	// 2. Menu/Top Bar --> Top Bar Background Gradient
	onTopBarGradualChange();
	// 2. Menu/Top Bar --> Menu background gradient
	onMenuBarGradualChange();
	// 2. Menu/top bar --> Column menu background gradient
	onColumnsMenuBarGradualChange();
};
onMounted(() => {
	nextTick(() => {
		// Determine whether the current layout is different. If not, initialize the style of the current layout to prevent the layout configuration logo, menu background and other partial layout failures when the size of the monitoring window changes.
		if (!Local.get('frequency')) initLayoutChangeFun();
		Local.set('frequency', 1);
		// Monitor window size changes, non-default layout, set to default layout (adapted to mobile terminals)
		mittBus.on('layoutMobileResize', (res: LayoutMobileResize) => {
			getThemeConfig.value.layout = res.layout;
			getThemeConfig.value.isDrawer = false;
			initLayoutChangeFun();
			state.isMobile = other.isMobile();
		});
		setTimeout(() => {
			// Default style
			onColorPickerChange();
			// gray mode
			if (getThemeConfig.value.isGrayscale) onAddFilterChange('grayscale');
			// Color Weakness Mode
			if (getThemeConfig.value.isInvert) onAddFilterChange('invert');
			// dark mode
			if (getThemeConfig.value.isIsDark) onAddDarkChange();
			// Turn on watermark
			onWatermarkChange();
			// Set column logo height
			onColumnsLogoHeightChange();
			// Set column menu width
			onColumnsMenuWidthChange();
			// Set column menu height
			onColumnsMenuHeightChange();
			// language internationalization
			if (Local.get('themeConfig')) {
				window.$changeLang(Local.get('themeConfig').globalI18n)
			}
			// Initialize menu style, etc.
			initSetStyle();
		}, 100);
	});
});
onUnmounted(() => {
	mittBus.off('layoutMobileResize', () => {});
});

// exposure variables
defineExpose({
	openDrawer,
});
</script>

<style scoped lang="scss">
.layout-breadcrumb-seting-bar {
	height: calc(100vh - 50px);
	padding: 0 15px;
	:deep(.el-scrollbar__view) {
		overflow-x: hidden !important;
	}
	.layout-breadcrumb-seting-bar-flex {
		display: flex;
		align-items: center;
		margin-bottom: 5px;
		&-label {
			flex: 1;
			color: var(--el-text-color-primary);
		}
	}
	.layout-drawer-content-flex {
		overflow: hidden;
		display: flex;
		flex-wrap: wrap;
		align-content: flex-start;
		margin: 0 -5px;
		.layout-drawer-content-item {
			width: 50%;
			height: 70px;
			cursor: pointer;
			border: 1px solid transparent;
			position: relative;
			padding: 5px;
			.el-container {
				height: 100%;
				.el-aside-dark {
					background-color: var(--next-color-seting-header);
				}
				.el-aside {
					background-color: var(--next-color-seting-aside);
				}
				.el-header {
					background-color: var(--next-color-seting-header);
				}
				.el-main {
					background-color: var(--next-color-seting-main);
				}
			}
			.el-circular {
				border-radius: 2px;
				overflow: hidden;
				border: 1px solid transparent;
				transition: all 0.3s ease-in-out;
			}
			.drawer-layout-active {
				border: 1px solid;
				border-color: var(--el-color-primary);
			}
			.layout-tips-warp,
			.layout-tips-warp-active {
				transition: all 0.3s ease-in-out;
				position: absolute;
				left: 50%;
				top: 50%;
				transform: translate(-50%, -50%);
				border: 1px solid;
				border-color: var(--el-color-primary-light-5);
				border-radius: 100%;
				padding: 4px;
				.layout-tips-box {
					transition: inherit;
					width: 30px;
					height: 30px;
					z-index: 9;
					border: 1px solid;
					border-color: var(--el-color-primary-light-5);
					border-radius: 100%;
					.layout-tips-txt {
						transition: inherit;
						position: relative;
						top: 5px;
						font-size: 12px;
						line-height: 1;
						letter-spacing: 2px;
						white-space: nowrap;
						color: var(--el-color-primary-light-5);
						text-align: center;
						transform: rotate(30deg);
						left: -1px;
						background-color: var(--next-color-seting-main);
						width: 32px;
						height: 17px;
						line-height: 17px;
					}
				}
			}
			.layout-tips-warp-active {
				border: 1px solid;
				border-color: var(--el-color-primary);
				.layout-tips-box {
					border: 1px solid;
					border-color: var(--el-color-primary);
					.layout-tips-txt {
						color: var(--el-color-primary) !important;
						background-color: var(--next-color-seting-main) !important;
					}
				}
			}
			&:hover {
				.el-circular {
					transition: all 0.3s ease-in-out;
					border: 1px solid;
					border-color: var(--el-color-primary);
				}
				.layout-tips-warp {
					transition: all 0.3s ease-in-out;
					border-color: var(--el-color-primary);
					.layout-tips-box {
						transition: inherit;
						border-color: var(--el-color-primary);
						.layout-tips-txt {
							transition: inherit;
							color: var(--el-color-primary) !important;
							background-color: var(--next-color-seting-main) !important;
						}
					}
				}
			}
		}
	}
	.copy-config {
		margin: 10px 0;
		.copy-config-btn {
			width: 100%;
			margin-top: 15px;
		}
		.copy-config-btn-reset {
			width: 100%;
			margin: 10px 0 0;
		}
	}
}
</style>
