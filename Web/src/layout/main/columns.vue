<template>
	<el-container class="layout-container">
		<ColumnsAside />
		<el-container class="layout-columns-warp layout-container-view h100">
			<LayoutAside />
			<el-scrollbar ref="layoutScrollbarRef" class="layout-backtop">
				<LayoutHeader />
				<LayoutMain ref="layoutMainRef" />
			</el-scrollbar>
		</el-container>
	</el-container>
</template>

<script setup lang="ts" name="layoutColumns">
import { defineAsyncComponent, watch, onMounted, nextTick, ref } from 'vue';
import { useRoute } from 'vue-router';
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';

// Introduce components
const LayoutAside = defineAsyncComponent(() => import('/@/layout/component/aside.vue'));
const LayoutHeader = defineAsyncComponent(() => import('/@/layout/component/header.vue'));
const LayoutMain = defineAsyncComponent(() => import('/@/layout/component/main.vue'));
const ColumnsAside = defineAsyncComponent(() => import('/@/layout/component/columnsAside.vue'));

// Define variable content
const layoutScrollbarRef = ref<RefType>('');
const layoutMainRef = ref<InstanceType<typeof LayoutMain>>();
const route = useRoute();
const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);

// Reset scroll bar height
const updateScrollbar = () => {
	// Update parent scrollbar
	layoutScrollbarRef.value.update();
	// Update child scrollbar
	layoutMainRef.value && layoutMainRef.value!.layoutMainScrollbarRef.update();
};
// Reset the scroll bar height because the component is introduced asynchronously
const initScrollBarHeight = () => {
	nextTick(() => {
		setTimeout(() => {
			updateScrollbar();
			layoutScrollbarRef.value.wrapRef.scrollTop = 0;
			if (layoutMainRef.value != undefined) layoutMainRef.value!.layoutMainScrollbarRef.wrapRef.scrollTop = 0;
		}, 500);
	});
};
// When the page loads
onMounted(() => {
	initScrollBarHeight();
});
// Monitor routing changes and move the scroll bar to the top when switching interfaces
watch(
	() => route.path,
	() => {
		initScrollBarHeight();
	}
);
// Monitor changes in the themeConfig configuration file and update the height of the menu el-scrollbar
watch(
	() => [themeConfig.value.isTagsview, themeConfig.value.isFixedHeader],
	() => {
		nextTick(() => {
			updateScrollbar();
		});
	},
	{
		deep: true,
	}
);
</script>
