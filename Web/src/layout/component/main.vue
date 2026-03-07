<template>
	<el-main class="layout-main" :style="isFixedHeader ? `height: calc(100% - ${setMainHeight})` : `minHeight: calc(100% - ${setMainHeight})`">
		<el-scrollbar ref="layoutMainScrollbarRef" 
            class="layout-main-scroll layout-backtop-header-fixed" 
            wrap-class="layout-main-scroll" 
            view-class="layout-main-scroll overflow-bug" 
        >
			<LayoutParentView />
			<LayoutFooter v-if="isFooter" />
		</el-scrollbar>
		<el-backtop :target="setBacktopClass" />
	</el-main>
</template>

<script setup lang="ts" name="layoutMain">
import { defineAsyncComponent, onMounted, computed, ref } from 'vue';
import { useRoute } from 'vue-router';
import { storeToRefs } from 'pinia';
import { useTagsViewRoutes } from '/@/stores/tagsViewRoutes';
import { useThemeConfig } from '/@/stores/themeConfig';
import { NextLoading } from '/@/utils/loading';

// Introduce components
const LayoutParentView = defineAsyncComponent(() => import('/@/layout/routerView/parent.vue'));
const LayoutFooter = defineAsyncComponent(() => import('/@/layout/footer/index.vue'));

// Define variable content
const layoutMainScrollbarRef = ref();
const route = useRoute();
const storesTagsViewRoutes = useTagsViewRoutes();
const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);
const { isTagsViewCurrenFull } = storeToRefs(storesTagsViewRoutes);

// Set footer to show/hide
const isFooter = computed(() => {
	return themeConfig.value.isFooter && !route.meta.isIframe;
});
// Set header fixed
const isFixedHeader = computed(() => {
	return themeConfig.value.isFixedHeader;
});
// Set Backtop Return to top
const setBacktopClass = computed(() => {
	if (themeConfig.value.isFixedHeader) return `.layout-backtop-header-fixed .el-scrollbar__wrap`;
	else return `.layout-backtop .el-scrollbar__wrap`;
});
// Set the height of the main content area
const setMainHeight = computed(() => {
	if (isTagsViewCurrenFull.value) return '0px';
	const { isTagsview, layout } = themeConfig.value;
	if (isTagsview && layout !== 'classic') return '85px';
	else return '51px';
});
// Before page loads
onMounted(() => {
	NextLoading.done(600);
});

// exposure variables
defineExpose({
	layoutMainScrollbarRef,
});
</script>
