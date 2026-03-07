<template>
	<div class="sys-jobCluster-container">
		<el-drawer v-model="state.isVisible" title="Job cluster" size="40%">
			<el-table :data="state.jobClusterList" style="width: 100%;" v-loading="state.loading" border>
				<el-table-column type="index" label="No" width="55" align="center" />
				<el-table-column prop="clusterId" label="cluster number" header-align="center" show-overflow-tooltip />
				<el-table-column prop="status" label="state" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag v-if="scope.row.status == 0"> Crash </el-tag>
						<el-tag v-if="scope.row.status == 1"> at work </el-tag>
						<el-tag v-if="scope.row.status == 2"> waiting to be awakened </el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="description" label="Description" header-align="center" show-overflow-tooltip />
				<el-table-column prop="updatedTime " label="Update Time" align="center" show-overflow-tooltip />
			</el-table>
		</el-drawer>
	</div>
</template>

<script lang="ts" setup name="sysJobCluster">
import { onMounted, reactive } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysJobApi } from '/@/api-services/api';
import { SysJobCluster } from '/@/api-services/models';

const state = reactive({
	loading: false,
	isVisible: false,
	jobClusterList: [] as Array<SysJobCluster>,
});

onMounted(async () => {
	handleQuery();
});

// Query operation
const handleQuery = async () => {
	state.loading = true;
	var res = await getAPI(SysJobApi).apiSysJobJobClusterListGet();
	state.jobClusterList = res.data.result ?? [];
	state.loading = false;
};

// open page
const openDrawer = () => {
	state.isVisible = true;
};

// Export object
defineExpose({ openDrawer });
</script>

<style lang="scss" scoped>
:deep(.el-drawer__body) {
    padding: 8px;
}
</style>