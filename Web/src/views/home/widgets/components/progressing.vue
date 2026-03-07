<template>
	<el-card shadow="hover" header="Project progress">
		<template #header>
			<el-icon style="display: inline; vertical-align: middle"> <ele-Odometer /> </el-icon>
			<span> Project progress </span>
		</template>
		<div class="progress">
			<el-progress type="dashboard" :percentage="99.9" :width="160" color="var(--el-color-primary)">
				<template #default="{ percentage }">
					<div class="percentage-value">{{ percentage }}%</div>
					<div class="percentage-label">Current progress</div>
				</template>
			</el-progress>
		</div>
	</el-card>
</template>

<script lang="ts">
export default {
	title: 'progress loop',
	icon: 'ele-Odometer',
	description: 'Progress Ring Atomic Component Demonstration',
};
</script>

<script setup lang="ts" name="progressing"></script>

<style scoped>
.progress {
	text-align: center;
}
.progress .percentage-value {
	font-size: 28px;
}
.progress .percentage-label {
	font-size: 12px;
	margin-top: 10px;
}
</style>
