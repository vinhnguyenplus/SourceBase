<template>
	<div class="sys-region-container">
		<el-splitter class="smallbar-el-splitter">
			<el-splitter-panel size="20%" :min="200">
				<RegionTree ref="regionTreeRef" @node-click="nodeClick" />
			</el-splitter-panel>
			<el-splitter-panel :min="200" style="overflow: auto; display: flex; flex-direction: column;">
				<el-card class="full-table" shadow="hover">
                    <template #header>
                        <el-form :model="state.queryParams" ref="queryForm" :inline="true">
                            <el-form-item label="Administrative name">
                                <el-input v-model="state.queryParams.name" placeholder="Administrative name" clearable />
                            </el-form-item>
                            <el-form-item label="Administrative code">
                                <el-input v-model="state.queryParams.code" placeholder="Administrative code" clearable />
                            </el-form-item>
                            <el-form-item>
                                <el-button-group>
                                    <el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysRegion:page'"> Query </el-button>
                                    <el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
                                </el-button-group>
                            </el-form-item>
                            <el-form-item>
                                <el-button type="primary" icon="ele-Plus" @click="openAddRegion" v-auth="'sysRegion:add'"> Add New </el-button>
                                <el-button type="danger" icon="ele-Lightning" @click="handlSync" v-auth="'sysRegion:sync'"> Synchronous Statistics Bureau </el-button>
                            </el-form-item>
                        </el-form>
                    </template>
					<el-table :data="state.regionData" style="width: 100%" v-loading="state.loading" row-key="id" default-expand-all :tree-props="{ children: 'children', hasChildren: 'hasChildren' }" border>
						<el-table-column prop="name" label="Administrative name" align="center" show-overflow-tooltip />
						<el-table-column prop="code" label="Administrative code" align="center" show-overflow-tooltip />
						<el-table-column prop="cityCode" label="Area code" align="center" show-overflow-tooltip />
						<el-table-column prop="orderNo" label="Sort" width="70" align="center" show-overflow-tooltip />
						<el-table-column prop="remark" label="Remarks" header-align="center" show-overflow-tooltip />
						<el-table-column label="Operation" width="140" fixed="right" align="center" show-overflow-tooltip>
							<template #default="scope">
								<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditRegion(scope.row)" v-auth="'sysRegion:update'"> Edit </el-button>
								<el-button icon="ele-Delete" size="small" text type="danger" @click="delRegion(scope.row)" v-auth="'sysRegion:delete'"> Delete </el-button>
							</template>
						</el-table-column>
					</el-table>
					<el-pagination
						v-model:currentPage="state.tableParams.page"
						v-model:page-size="state.tableParams.pageSize"
						:total="state.tableParams.total"
						:page-sizes="[10, 20, 50, 100]"
						size="small"
						background
						@size-change="handleSizeChange"
						@current-change="handleCurrentChange"
						layout="total, sizes, prev, pager, next, jumper"
					/>
				</el-card>
			</el-splitter-panel>
		</el-splitter>

		<EditRegion ref="editRegionRef" :title="state.editRegionTitle" @handleQuery="handleQuery" />
	</div>
</template>

<script lang="ts" setup name="sysRegion">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage, ElNotification } from 'element-plus';
//import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';

import RegionTree from '/@/views/system/region/component/regionTree.vue';
import EditRegion from '/@/views/system/region/component/editRegion.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysRegionApi } from '/@/api-services/api';
import { SysRegion } from '/@/api-services/models';

const editRegionRef = ref<InstanceType<typeof EditRegion>>();
const regionTreeRef = ref<InstanceType<typeof RegionTree>>();
const state = reactive({
	loading: false,
	regionData: [] as Array<SysRegion>, // List data
	queryParams: {
		id: -1,
		pid: undefined,
		name: undefined,
		code: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	editRegionTitle: '',
});

onMounted(async () => {
	await handleQuery();
});

// Query operation
const handleQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	let res = await getAPI(SysRegionApi).apiSysRegionPagePost(params);
	state.regionData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// reset operation
const resetQuery = async () => {
	state.queryParams.id = -1;
	state.queryParams.pid = undefined;
	state.queryParams.name = undefined;
	state.queryParams.code = undefined;
	await handleQuery();
};

// Open new page
const openAddRegion = () => {
	state.editRegionTitle = 'Add administrative area';
    const parent = regionTreeRef.value?.getCurrentNode() ?? { id: 0, name: "Top" };
    const parentPath = regionTreeRef.value?.getCurrentPath() ?? null;
	editRegionRef.value?.openDialog({ orderNo: 100, pid: parent.id }, parentPath ? parentPath.map(i => i.name).join(' / ') : 'Top');
};

// Open the edit page
const openEditRegion = (row: any) => {
	state.editRegionTitle = 'Edit administrative region';
	editRegionRef.value?.openDialog(row);
};

// delete
const delRegion = (row: any) => {
	ElMessageBox.confirm(`Are you sure to delete the administrative area: [${row.name}]?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysRegionApi).apiSysRegionDeletePost({ id: row.id });
			await handleQuery();
			// Update institution data after editing and deletion
			regionTreeRef.value?.initTreeData();
			ElMessage.success('Deleted successfully');
		})
		.catch(() => {});
};

// Tree component click
const nodeClick = async (node: any) => {
	state.queryParams.pid = node.id;
	state.queryParams.name = undefined;
	state.queryParams.code = undefined;
	await handleQuery();
};

// Synchronize National Bureau of Statistics operations
const handlSync = async () => {
	ElMessageBox.confirm('Confirm synchronization of National Bureau of Statistics administrative region data?', 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			ElNotification({
				title: 'Prompt',
				message: 'Background syncing in progress...',
				type: 'success',
				position: 'bottom-right',
			});
			await getAPI(SysRegionApi).apiSysRegionSyncPost({ timeout: 1000 * 60 * 30 });
		})
		.catch(() => {});
};

// Change page capacity
const handleSizeChange = async (val: number) => {
	state.tableParams.pageSize = val;
	await handleQuery();
};

// Change page number
const handleCurrentChange = async (val: number) => {
	state.tableParams.page = val;
	await handleQuery();
};
</script>
<style lang="scss" scoped>
.full-table {
    :deep(.el-card__header) {
        padding: 10px 20px;
    }
}
</style>