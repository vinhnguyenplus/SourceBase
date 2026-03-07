<template>
	<div class="sys-plugin-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="tenant" v-if="userStore.userInfos.accountType == 999">
					<TenantSelect v-model="state.queryParams.tenantId" clearable />
				</el-form-item>
				<el-form-item label="Function Name">
					<el-input v-model="state.queryParams.name" placeholder="Function Name" clearable />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysPlugin:page'"> Query </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openAddPlugin" v-auth="'sysPlugin:add'"> Add New </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.pluginData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="No" width="55" align="center" fixed />
				<el-table-column prop="name" label="Function Name" header-align="center" show-overflow-tooltip />
				<el-table-column prop="assemblyName" label="Assembly Name" header-align="center" show-overflow-tooltip />
				<el-table-column prop="orderNo" label="Sort" width="70" align="center" show-overflow-tooltip />
				<el-table-column label="state" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
            <g-sys-dict v-model="scope.row.status" code="StatusEnum" />
					</template>
				</el-table-column>
				<el-table-column label="Modify records" width="100" align="center" show-overflow-tooltip>
					<template #default="scope">
						<ModifyRecord :data="scope.row" />
					</template>
				</el-table-column>
				<el-table-column label="Operation" width="140" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditPlugin(scope.row)" v-auth="'sysPlugin:update'"> Edit </el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="delPlugin(scope.row)" v-auth="'sysPlugin:delete'"> Delete </el-button>
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

		<EditPlugin ref="editPluginRef" :title="state.editPluginTitle" @handleQuery="handleQuery" />
	</div>
</template>

<script lang="ts" setup name="sysPlugin">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import EditPlugin from '/@/views/system/plugin/component/editPlugin.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysPluginApi } from '/@/api-services/api';
import { SysPlugin } from '/@/api-services/models';
import { useUserInfo } from "/@/stores/userInfo";
import TenantSelect from '/@/views/system/tenant/component/tenantSelect.vue';

const userStore = useUserInfo();
const editPluginRef = ref<InstanceType<typeof EditPlugin>>();
const state = reactive({
	loading: false,
	pluginData: [] as Array<SysPlugin>,
	queryParams: {
		tenantId: undefined,
		name: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	editPluginTitle: '',
});

onMounted(async () => {
	if (userStore.userInfos.accountType == 999) {
		state.queryParams.tenantId = userStore.userInfos.currentTenantId as any;
	}
	handleQuery();
});

// Query operation
const handleQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	var res = await getAPI(SysPluginApi).apiSysPluginPagePost(params);
	state.pluginData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// reset operation
const resetQuery = () => {
	state.queryParams.name = undefined;
	handleQuery();
};

// Open new page
const openAddPlugin = () => {
	state.editPluginTitle = 'Add dynamic plug-in';
	editPluginRef.value?.openDialog({ orderNo: 100, tenantId: state.queryParams.tenantId, status: 1 });
};

// Open the edit page
const openEditPlugin = (row: any) => {
	state.editPluginTitle = 'Edit dynamic plugin';
	editPluginRef.value?.openDialog(row);
};

// Delete current row
const delPlugin = (row: any) => {
	ElMessageBox.confirm(`Are you sure to delete the dynamic plug-in: [${row.name}]?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysPluginApi).apiSysPluginDeletePost({ id: row.id });
			handleQuery();
			ElMessage.success('Deleted successfully');
		})
		.catch(() => {});
};

// Change page capacity
const handleSizeChange = (val: number) => {
	state.tableParams.pageSize = val;
	handleQuery();
};

// Change page number
const handleCurrentChange = (val: number) => {
	state.tableParams.page = val;
	handleQuery();
};
</script>
