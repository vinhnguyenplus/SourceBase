<template>
	<el-card shadow="hover" header="Version information">
		<template #header>
			<el-icon style="display: inline; vertical-align: middle"> <ele-InfoFilled /> </el-icon>
			<span> Information </span>
		</template>
	</el-card>
</template>

<script lang="ts">
export default {
	title: 'Version information',
	icon: 'ele-InfoFilled',
	description: 'Version Information Atomic Component Demo',
};
</script>

<script setup lang="ts" name="version">
import { ref, onMounted } from 'vue';
import verSvg from '/@/assets/img/ver.svg';

const version = ref<string>('loading...');

onMounted(() => {
	version.value = 'v2.0.0';
});

const getVer = () => {
	version.value = 'v2.0.0';
};

const golog = () => {
	window.open('https://gitee.com/zuohuaijun/Admin.NET/issues');
};

const gogit = () => {
	window.open('https://gitee.com/zuohuaijun/Admin.NET.git');
};
</script>
