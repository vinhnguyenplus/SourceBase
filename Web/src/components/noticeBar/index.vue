<template>
	<div class="notice-bar" :style="{ background, height: `${height}px` }" v-show="!state.isMode">
		<div class="notice-bar-warp" :style="{ color, fontSize: `${size}px` }" ref="noticeBarWarpRef">
			<i v-if="leftIcon" class="notice-bar-warp-left-icon" :class="leftIcon"></i>
			<div class="notice-bar-warp-text-box">
				<div class="notice-bar-warp-text" ref="noticeBarTextRef">
					<div v-html="text" data-slate-editor />
				</div>
			</div>
			<!-- <SvgIcon :name="rightIcon" v-if="rightIcon" class="notice-bar-warp-right-icon" @click="onRightIconClick" /> -->
		</div>
	</div>
</template>

<script setup lang="ts" name="noticeBar">
import { reactive, ref, onMounted, nextTick } from 'vue';

const props = defineProps({
	mode: { type: String, default: '' }, // Notification bar mode, optional value is closeable link
	text: { type: String, default: 'Welcome to the PAYMENT COIN' }, // Notification text content
	color: { type: String, default: 'var(--el-color-warning)' }, // Notification text color
	background: { type: String, default: 'var(--el-color-warning-light-9)' }, // Notification background color
	size: { type: [Number, String], default: 14 }, // Font size, unit px
	height: { type: Number, default: 40 }, // Notification bar height, unit: px
	delay: { type: Number, default: 1 }, // Animation delay time (s)
	speed: { type: Number, default: 100 }, // Scroll rate (px/s)
	scrollable: { type: Boolean, default: false }, // Whether to enable vertical scrolling
	leftIcon: { type: String, default: 'iconfont icon-tongzhi2' }, // Customize left icon
	rightIcon: { type: String, default: '' }, // Customize the right icon
});

const emit = defineEmits(['close', 'link']);
const noticeBarWarpRef = ref<HTMLDivElement | null>(null);
const noticeBarTextRef = ref<HTMLDivElement | null>(null);
const state = reactive({
	isMode: false,
	warpOWidth: 0,
	textOWidth: 0,
	animationDuration: 0,
});

// Page initialization
onMounted(() => {
	if (!props.scrollable) {
		initAnimation();
	}
});

// Initialize animation
const initAnimation = () => {
	nextTick(() => {
		if (noticeBarWarpRef.value && noticeBarTextRef.value) {
			state.warpOWidth = noticeBarWarpRef.value.offsetWidth;
			state.textOWidth = noticeBarTextRef.value.scrollWidth;

			state.animationDuration = (state.textOWidth + state.warpOWidth) / props.speed;

			// Clear existing animation styles
			noticeBarTextRef.value.style.animation = 'none';
			noticeBarTextRef.value.offsetHeight; // Trigger reflow
			noticeBarTextRef.value.style.animation = `marquee ${state.animationDuration}s linear infinite`;

			// Define keyframes for marquee animation
			const keyframes = `
		  @keyframes marquee {
			0% { transform: translateX(${state.warpOWidth}px); }
			100% { transform: translateX(-${state.textOWidth}px); }
		  }
		`;
			const styleSheet = document.createElement('style');
			styleSheet.innerText = keyframes;
			document.head.appendChild(styleSheet);
		}
	});
};

// Click the icon on the right
const onRightIconClick = () => {
	if (!props.mode) return false;
	if (props.mode === 'closeable') {
		state.isMode = true;
		emit('close');
	} else if (props.mode === 'link') {
		emit('link');
	}
};
</script>

<style scoped lang="scss">
.notice-bar {
	padding: 0 15px;
	width: 100%;
	border-radius: 4px;
	.notice-bar-warp {
		display: flex;
		align-items: center;
		width: 100%;
		height: inherit;
		.notice-bar-warp-text-box {
			flex: 1;
			height: inherit;
			display: flex;
			align-items: center;
			overflow: hidden;
			position: relative;
			margin-right: 35px;
			// .notice-bar-warp-text {
			// 	white-space: nowrap;
			// 	position: absolute;
			// 	left: 0;
			// }
		}
		.notice-bar-warp-left-icon {
			width: 24px;
			font-size: inherit !important;
		}
		.notice-bar-warp-right-icon {
			width: 24px;
			text-align: right;
			font-size: inherit !important;
			&:hover {
				cursor: pointer;
			}
		}
	}
}
</style>
