<template>
	<i v-if="isShowIconEle" class="el-icon" :style="setIconSvgStyle">
		<component :is="getIconName" />
	</i>
    <i v-else-if="isShowIconSvg" class="el-icon" :style="setIconSvgStyle">
        <svg aria-hidden="true">
            <use :xlink:href="'#' + getIconName"></use>
        </svg>
    </i>
	<div v-else-if="isShowIconImg" :style="setIconImgOutStyle">
		<img :src="getIconName" :style="setIconSvgInsStyle" />
	</div>
	<i v-else :class="getIconName" :style="setIconSvgStyle" />
</template>

<script setup lang="ts" name="svgIcon">
import { computed } from 'vue';

// Define the value passed by the parent component
const props = defineProps({
	// svg icon component name
	name: {
		type: String,
	},
	// svg size
	size: {
		type: Number,
		//default: () => 16,
	},
	// svg color
	color: {
		type: String,
	},
});

// Online link, local import address prefix
// https://gitee.com/lyt-top/vue-next-admin/issues/I62OVL
const linesString = ['https', 'http', '/src', '/assets', 'data:image', window.__env__.VITE_PUBLIC_PATH];

// Get icon icon name
const getIconName = computed(() => {
	return props?.name;
});
// Used to determine the display and hiding of the svg icon that comes with element plus
const isShowIconEle = computed(() => {
	return props?.name?.startsWith('ele-');
});
// Used to determine the display and hiding of custom svg icons
const isShowIconSvg = computed(() => {
	return props?.name?.startsWith('svg-');
});
// Used to determine whether to display or hide icons such as online links and local imports.
const isShowIconImg = computed(() => {
	return linesString.find((str) => props.name?.startsWith(str));
});
// Set icon style
const setIconSvgStyle = computed(() => {
	return `font-size: ${props.size}px;color: ${props.color};`;
});
// Set image style
const setIconImgOutStyle = computed(() => {
	return `width: ${props.size}px;height: ${props.size}px;display: inline-block;overflow: hidden;`;
});
// Set image style
// https://gitee.com/lyt-top/vue-next-admin/issues/I59ND0
const setIconSvgInsStyle = computed(() => {
	const filterStyle: string[] = [];
	const compatibles: string[] = ['-webkit', '-ms', '-o', '-moz'];
	compatibles.forEach((j) => filterStyle.push(`${j}-filter: drop-shadow(${props.color} ${props.size}px 0);`));
	return `width: ${props.size}px;height: ${props.size}px;position: relative;left: -${props.size}px;${filterStyle.join('')}`;
});
</script>
