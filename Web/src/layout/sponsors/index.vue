<template>
	<div class="sponsors-container" title="Click to experience" v-show="state.sponsors.isShow" @click="onSponsorsClick">
		<el-carousel height="240px" indicator-position="none" :arrow="setCarouselShow" @change="onCarouselChange">
			<el-carousel-item v-for="(v, k) in state.sponsors.list" :key="k">
				<img :src="v.url" class="sponsors-img" />
				<div class="sponsors-text" v-html="v.text"></div>
			</el-carousel-item>
		</el-carousel>
		<div class="sponsors-close">
			<SvgIcon name="ele-Close" :size="12" title="Close sponsor" @click.stop="onCloseSponsors" />
		</div>
	</div>
</template>

<script setup lang="ts" name="layoutSponsors">
import { reactive, computed, onMounted } from 'vue';
import sponsorsOne from '/@/assets/ccflowRightNextAdmin.png';

// Define variable content
const state = reactive({
	sponsors: {
		list: [
			{
				url: sponsorsOne,
				text: `Chicheng BPM system includes form engine + process engine + permission control, which is easy to integrate, flexible in configuration, powerful and suitable for China's national conditions. Demo: http:// demo.ccflow.org. Click star in the upper right corner to join the group: 1060674395`,
				link: 'http://www.ccflow.org/',
			},
		],
		isShow: false,
		index: 0,
	},
});

// Set the carousel arrow display
const setCarouselShow = computed(() => {
	return state.sponsors.list.length <= 1 ? 'never' : 'hover';
});
// Close sponsor
const onCloseSponsors = () => {
	state.sponsors.isShow = false;
};
// When the carousel changes
const onCarouselChange = (e: number) => {
	state.sponsors.index = e;
};
// Click on the current item content
const onSponsorsClick = () => {
	window.open(state.sponsors.list[state.sponsors.index].link);
};
// Delay display to prevent affecting the loading of other interfaces
const delayShow = () => {
	setTimeout(() => {
		state.sponsors.isShow = true;
	}, 3000);
};
// When the page loads
onMounted(() => {
	delayShow();
});
</script>

<style scoped lang="scss">
.sponsors-container {
	position: fixed;
	right: 15px;
	bottom: 15px;
	z-index: 3;
	width: 200px;
	background-color: var(--next-bg-main-color);
	box-shadow: var(--el-box-shadow-lighter);
	border-radius: 5px;
	overflow: hidden;
	cursor: pointer;
	.sponsors-img {
		width: 100%;
		height: 80px;
	}
	.sponsors-text {
		padding: 10px;
		color: var(--el-text-color-regular);
		font-size: var(--el-dialog-content-font-size);
	}
	.sponsors-close {
		width: 60px;
		height: 60px;
		border-radius: 100%;
		background: rgba(0, 0, 0, 0.05);
		transition: all 0.3s ease;
		position: absolute;
		right: -35px;
		bottom: -35px;
		:deep(i) {
			position: absolute;
			left: 9px;
			top: 9px;
			color: #afafaf;
			transition: all 0.3s ease;
		}
		&:hover {
			transition: all 0.3s ease;
			:deep(i) {
				color: var(--el-color-primary);
				transition: all 0.3s ease;
			}
		}
	}
}
</style>
