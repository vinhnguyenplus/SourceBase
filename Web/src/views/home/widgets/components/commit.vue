<template>
	<card-pro title="Update Log" prefix-icon="ele-DocumentCopy" shadow="hover">
        
    </card-pro>
</template>

<script lang="ts">
export default {
	title: 'Update Log',
	icon: 'ele-DocumentCopy',
	description: 'Current project update record',
};
</script>

<script setup lang="ts" name="commit">
import { reactive, onMounted } from 'vue';
import { formatDate } from '/@/utils/formatTime';
import CardPro from '/@/components/CardPro/index.vue';

const state = reactive({
	loading: false,
	list: [] as any,
});

const getList = () => {
	axios({
		method: 'get',
		url: 'https://gitee.com/api/v5/repos/zuohuaijun/Admin.NET/commits',
		params: {
			page: 1,
			per_page: 10,
		},
	}).then((res: any) => {
		state.list = res.data;
		state.loading = false;
	});
};

const refresh = () => {
	state.loading = true;
	getList();
};

onMounted(() => {
	state.loading = true;
	getList();
});
</script>

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
.commit {
	max-height: 500px;
	overflow: auto;
}
</style>
