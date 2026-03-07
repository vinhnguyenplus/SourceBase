<template>
	<div class="sys-print-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="tenant" v-if="userStore.userInfos.accountType == 999">
                    <TenantSelect v-model="state.queryParams.tenantId" clearable />
				</el-form-item>
				<el-form-item label="Template Name">
					<el-input v-model="state.queryParams.name" placeholder="Template Name" clearable />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysPrint:page'"> Query </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openAddPrint" v-auth="'sysPrint:add'"> Add New </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.printData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="No" width="55" align="center" fixed />
				<el-table-column prop="name" label="name" header-align="center" show-overflow-tooltip />
				<!-- <el-table-column prop="template" label="template" show-overflow-tooltip /> -->
				<el-table-column prop="orderNo" label="Sort" align="center" show-overflow-tooltip />
				<el-table-column label="state" align="center" show-overflow-tooltip>
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
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditPrint(scope.row)" v-auth="'sysPrint:update'"> Edit </el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="delPrint(scope.row)" v-auth="'sysPrint:delete'"> Delete </el-button>
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

		<EditPrint ref="editPrintRef" :title="state.editPrintTitle" @handleQuery="handleQuery" />
	</div>
</template>

<script lang="ts" setup name="sysPrint">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import EditPrint from '/@/views/system/print/component/editPrint.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysPrintApi } from '/@/api-services/api';
import { SysPrint } from '/@/api-services/models';
import { useUserInfo } from "/@/stores/userInfo";
import TenantSelect from '/@/views/system/tenant/component/tenantSelect.vue';

const userStore = useUserInfo();
const editPrintRef = ref<InstanceType<typeof EditPrint>>();
const state = reactive({
	loading: false,
	printData: [] as Array<SysPrint>,
	queryParams: {
		tenantId: undefined,
		name: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	editPrintTitle: '',
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
	var res = await getAPI(SysPrintApi).apiSysPrintPagePost(params);
	state.printData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// reset operation
const resetQuery = () => {
	state.queryParams.name = undefined;
	handleQuery();
};

// Open new page
const openAddPrint = () => {
	state.editPrintTitle = 'Add toPrint template';
	editPrintRef.value?.openDialog({ tenantId: state.queryParams.tenantId });
};

// Open the edit page
const openEditPrint = (row: any) => {
	state.editPrintTitle = 'EditPrint template';
	editPrintRef.value?.openDialog(row);
};

// Delete current row
const delPrint = (row: any) => {
	ElMessageBox.confirm(`Are you sure to delete the printing template: [${row.name}]?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysPrintApi).apiSysPrintDeletePost({ id: row.id });
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
