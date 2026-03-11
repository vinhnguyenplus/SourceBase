<template>
	<el-card shadow="hover" header="Current clock" class="item-background">
		<template #header>
			<el-icon style="display: inline; vertical-align: middle"> <ele-Clock /> </el-icon>
			<span> Currentclock </span>
		</template>
		<div class="time">
			<h2>{{ time }}</h2>
			<p>{{ day }}</p>
		</div>
	</el-card>
</template>

<script lang="ts">
export default {
	title: 'clock',
	icon: 'ele-Timer',
	description: 'Clock Atomic Component Demonstration',
};
</script>

<script setup lang="ts" name="timer">
import { formatDate } from '/@/utils/formatTime';
import { ref, onMounted, onUnmounted } from 'vue';
const time = ref<string>('');
const day = ref<string>('');
const timer = ref<any>(null);

onMounted(() => {
	showTime();
	timer.value = setInterval(() => {
		showTime();
	}, 1000);
});

onUnmounted(() => {
	clearInterval(timer.value);
});

const showTime = () => {
	time.value = formatDate(new Date(), 'HH:MM:SS');
	day.value = formatDate(new Date(), 'YYYY year mm month dd day');
};
</script>

<style scoped>
.item-background {
	background: var(--el-color-primary);
	color: #fff;
}
.time h2 {
	font-size: 40px;
}
.time p {
	font-size: 14px;
	margin-top: 13px;
	opacity: 0.7;
}
</style>
