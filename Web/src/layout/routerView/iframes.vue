<template>
	<div class="layout-iframe w100 h100">
		<div class="layout-padding-view w100 h100">
			<div class="w100 h100"
                v-for="v in setIframeList" :key="v.path" 
                v-show="getRoutePath === v.path"
                v-loading="v.meta.loading" 
                element-loading-background="white"
            >
				<transition-group :name="name">
					<iframe
						:src="`${v.meta.isLink}${v.meta.isLink.indexOf('?') > 0 ? '&' : '?'}token=${getToken()}`"
						:key="v.path"
						frameborder="0"
						height="100%"
						width="100%"
						:data-url="v.path"
						ref="iframeRef"
					/>
				</transition-group>
			</div>
		</div>
	</div>
</template>

<script setup lang="ts" name="layoutIframeView">
import { computed, watch, ref, nextTick } from 'vue';
import { useRoute } from 'vue-router';
import { getToken } from '/@/utils/axios-utils';

// Define the value passed by the parent component
const props = defineProps({
	// refresh iframe
	refreshKey: {
		type: String,
		default: () => '',
	},
	// Transition animation name
	name: {
		type: String,
		default: () => 'slide-right',
	},
	// iframe list
	list: {
		type: Array,
		default: () => [],
	},
});

// Define variable content
const iframeRef = ref();
const route = useRoute();

// Process the list list and load it only when it is opened
const setIframeList = computed(() => {
    console.log(props.list);
	return (<RouteItems>props.list).filter((v: RouteItem) => v.meta?.isIframeOpen);
});
// Get iframe current routing path
const getRoutePath = computed(() => {
	return route.path;
});
// Close iframe loading
const closeIframeLoading = (val: string, item: RouteItem) => {
	nextTick(() => {
		if (!iframeRef.value) return false;
		iframeRef.value.forEach((v: HTMLElement) => {
			if (v.dataset.url === val) {
				v.onload = () => {
					if (item.meta?.isIframeOpen && item.meta.loading) item.meta.loading = false;
				};
			}
		});
	});
};
// Monitor route changes, initialize iframe data, and prevent switching from not taking effect when there are multiple iframes.
watch(
	() => route.fullPath,
	(val) => {
		const item: any = props.list.find((v: any) => v.path === val);
		if (!item) return false;
		if (!item.meta.isIframeOpen) item.meta.isIframeOpen = true;
		closeIframeLoading(val, item);
	},
	{
		immediate: true,
	}
);
// Monitor iframe refreshKey changes for tagsview right-click menu refresh
watch(
	() => props.refreshKey,
	() => {
		const item: any = props.list.find((v: any) => v.path === route.path);
		if (!item) return false;
		if (item.meta.isIframeOpen) item.meta.isIframeOpen = false;
		setTimeout(() => {
			item.meta.isIframeOpen = true;
			item.meta.loading = true;
			closeIframeLoading(route.fullPath, item);
		});
	},
	{
		deep: true,
	}
);
</script>
