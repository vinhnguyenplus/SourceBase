<template>
	<el-card shadow="hover" header="Welcome">
		<template #header>
			<el-icon style="display: inline; vertical-align: middle"> <ele-Promotion /> </el-icon>
			<span> Welcome </span>
		</template>
		<div class="welcome">
			<div class="logo">
				<!-- <img src="/@/assets/logo.png" style="height: 150px;"/> -->
				<h2>Welcome to Admin Dashboard</h2>
			</div>
		</div>
	</el-card>
</template>

<script lang="ts">
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';

const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);

export default {
	title: 'Welcome',
	icon: 'ele-Promotion',
	description: 'Project Features and Documentation Links',
};
</script>

<script setup lang="ts" name="welcome">
const godoc = () => {
	window.open('https://adminnet.top/');
};
</script>

<style scoped>
.welcome .logo {
	text-align: center;
}

.welcome .logo img {
	vertical-align: bottom;
	width: 100px;
	height: 100px;
	margin-bottom: 20px;
}

.welcome .logo h2 {
	font-size: 30px;
	font-weight: normal;
	display: flex;
	align-items: center;
	justify-content: center;
}

.tips {
	margin-top: 20px;
	padding: 0 40px;
}

.tips-item {
	display: flex;
	align-items: center;
	justify-content: center;
	padding: 7.5px 0;
}

.tips-item-icon {
	width: 40px;
	height: 40px;
	display: flex;
	align-items: center;
	justify-content: center;
	border-radius: 50%;
	font-size: 18px;
	margin-right: 20px;
	color: var(--el-color-primary);
	background: rgba(180, 180, 180, 0.1);
}

.tips-item-message {
	flex: 1;
	font-size: 14px;
}

.actions {
	text-align: center;
	margin: 40px 0 20px 0;
}
</style>
