<template>
	<div class="sys-open-access-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="Identity mark">
					<el-input v-model="state.queryParams.accessKey" placeholder="Identity mark" clearable />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysOpenAccess:page'"> Query </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openAddOpenAccess" v-auth="'sysOpenAccess:add'"> Add New </el-button>
					<el-button icon="ele-QuestionFilled" @click="openHelp"> illustrate </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.openAccessData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="No" width="55" align="center" />
				<el-table-column prop="accessKey" label="Identity mark" header-align="center" show-overflow-tooltip />
				<el-table-column prop="accessSecret" label="key" header-align="center" show-overflow-tooltip />
				<el-table-column prop="bindUserAccount" label="Bind userAccount number" header-align="center" show-overflow-tooltip />
				<el-table-column prop="bindTenantName" label="Bind tenant name" header-align="center" show-overflow-tooltip />
				<el-table-column label="Modify records" width="100" align="center" show-overflow-tooltip>
					<template #default="scope">
						<ModifyRecord :data="scope.row" />
					</template>
				</el-table-column>
				<el-table-column label="Operation" width="200" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditOpenAccess(scope.row)" v-auth="'sysOpenAccess:update'" :disabled="scope.row.status === 1"> Edit </el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="delOpenAccess(scope.row)" v-auth="'sysOpenAccess:delete'" :disabled="scope.row.status === 1"> Delete </el-button>
						<el-button size="small" text @click="openGenerateSign(scope.row)"> Generate signature </el-button>
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

		<EditOpenAccess ref="editOpenAccessRef" :title="state.editOpenAccessTitle" @handleQuery="handleQuery" />
		<HelpView ref="helpViewRef" />
		<GenerateSign ref="generateSignRef" />
	</div>
</template>

<script lang="ts" setup name="sysOpenAccess">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import EditOpenAccess from '/@/views/system/openAccess/component/editOpenAccess.vue';
import HelpView from '/@/views/system/openAccess/component/helpView.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';
import GenerateSign from '/@/views/system/openAccess/component/generateSign.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysOpenAccessApi } from '/@/api-services/api';
import { OpenAccessOutput } from '/@/api-services/models';

const editOpenAccessRef = ref<InstanceType<typeof EditOpenAccess>>();
const helpViewRef = ref<InstanceType<typeof HelpView>>();
const generateSignRef = ref<InstanceType<typeof GenerateSign>>();
const state = reactive({
	loading: false,
	openAccessData: [] as Array<OpenAccessOutput>,
	queryParams: {
		accessKey: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	editOpenAccessTitle: '',
});

onMounted(async () => {
	handleQuery();
});

// Query operation
const handleQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	var res = await getAPI(SysOpenAccessApi).apiSysOpenAccessPagePost(params);
	state.openAccessData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// reset operation
const resetQuery = () => {
	state.queryParams.accessKey = undefined;
	handleQuery();
};

// Open new page
const openAddOpenAccess = () => {
	state.editOpenAccessTitle = 'Add open interface identity';
	editOpenAccessRef.value?.openDialog({ type: 1 });
};

// Open the edit page
const openEditOpenAccess = (row: any) => {
	state.editOpenAccessTitle = 'Edit open interface identity';
	editOpenAccessRef.value?.openDialog(row);
};

// delete
const delOpenAccess = (row: any) => {
	ElMessageBox.confirm(`Are you sure to delete the open interface identity: [${row.accessKey}]?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysOpenAccessApi).apiSysOpenAccessDeletePost({ id: row.id });
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

// Open help page
const openHelp = () => {
	helpViewRef.value?.openDialog();
};

// Open generate signature
const openGenerateSign = (row: any) => {
	generateSignRef.value?.openDialog(row);
};
</script>
