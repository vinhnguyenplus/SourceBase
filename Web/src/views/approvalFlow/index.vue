<template>
	<div class="labApprovalFlow-container">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="state.queryParams" ref="queryForm">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10">
						<el-form-item label="Keywords">
							<el-input v-model="state.queryParams.keyword" placeholder="Please enter fuzzy search keywords" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="showAdvanceQueryUI">
						<el-form-item label="Number">
							<el-input v-model="state.queryParams.code" placeholder="Please enter number" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="showAdvanceQueryUI">
						<el-form-item label="name">
							<el-input v-model="state.queryParams.name" placeholder="Please enter a name" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="showAdvanceQueryUI">
						<el-form-item label="Remarks">
							<el-input v-model="state.queryParams.remark" placeholder="Please enter a note" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb10">
						<el-form-item>
							<el-button-group>
								<el-button type="primary" icon="ele-Search" @click="handleQuery"> Query </el-button>
								<el-button icon="ele-Refresh" @click="() => (state.queryParams = {})">Reset </el-button>
							</el-button-group>
							<el-button type="primary" icon="ele-Plus" @click="openAddApprovalFlow" style="margin-left: 30px"> Add New </el-button>
							<el-button icon="ele-ArrowDown" @click="changeAdvanceQueryUI" v-if="!showAdvanceQueryUI" style="margin-left: 5px" text> </el-button>
							<el-button icon="ele-ArrowUp" @click="changeAdvanceQueryUI" v-if="showAdvanceQueryUI" style="margin-left: 5px" text> </el-button>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
		</el-card>
		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.tableData" style="width: 100%" v-loading="state.loading" row-key="id" border>
				<el-table-column type="index" label="No" width="55" align="center" />
				<el-table-column prop="code" label="Number" width="140" show-overflow-tooltip />
				<el-table-column prop="name" label="name" show-overflow-tooltip />
				<el-table-column prop="formJson" label="form" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditFormDialog(scope.row)"> form </el-button>
					</template>
				</el-table-column>
				<el-table-column prop="flowJson" label="Process" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditFlowDialog(scope.row)"> Process </el-button>
					</template>
				</el-table-column>
				<el-table-column label="Modify records" width="100" align="center" show-overflow-tooltip>
					<template #default="scope">
						<ModifyRecord :data="scope.row" />
					</template>
				</el-table-column>
				<el-table-column label="Operation" width="200" align="center" fixed="right" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-View" size="small" text type="primary" @click="openDetailDialog(scope.row)"> View </el-button>
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditApprovalFlow(scope.row)"> Edit </el-button>
						<el-button icon="ele-Delete" size="small" text type="primary" @click="delApprovalFlow(scope.row)"> Delete </el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				v-model:page-size="state.tableParams.pageSize"
        v-model:currentPage="state.tableParams.page"
				:page-sizes="[10, 20, 50, 100, 200, 500]"
        :total="state.tableParams.total"
        @current-change="handleCurrentChange"
        @size-change="handleSizeChange"
        layout="total, sizes, prev, pager, next, jumper"
				background
        small
			/>
		</el-card>

		<detailDialog ref="detailDialogRef" :title="state.dialogTitle" @reloadTable="handleQuery" />
		<printDialog ref="printDialogRef" :title="state.dialogTitle" @reloadTable="handleQuery" />
		<editDialog ref="editDialogRef" :title="state.dialogTitle" @reloadTable="handleQuery" />
		<editFormDialog ref="editFormDialogRef" :title="state.dialogTitle" @reloadTable="handleQuery" />
		<editFlowDialog ref="editFlowDialogRef" :title="state.dialogTitle" @updateFlow="handleFlow" @reloadTable="handleQuery" />
	</div>
</template>

<script lang="ts" setup name="approvalFlow">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
// import { auth } from '/@/utils/authFunction';

import printDialog from '/@/views/system/print/component/hiprint/preview.vue';
import editFormDialog from './component/editFormDialog.vue';
import detailDialog from './component/detailDialog.vue';
import editFlowDialog from './component/editFlowDialog.vue';
import editDialog from './component/editDialog.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';

import { getAPI } from '/@/utils/axios-utils';
import { ApprovalFlowApi } from '/@/api-plugins/approvalFlow/api';
import { ApprovalFlowInput, ApprovalFlowOutput } from '/@/api-plugins/approvalFlow/models';

const showAdvanceQueryUI = ref(false);

const detailDialogRef = ref();
const editFormDialogRef = ref();
const editFlowDialogRef = ref();
const printDialogRef = ref();
const editDialogRef = ref();

const state = reactive({
	loading: false,
	tableData: [] as Array<ApprovalFlowOutput>,
	queryParams: {} as ApprovalFlowInput,
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	dialogTitle: '',
});

onMounted(async () => {
	handleQuery();
});

// Change the display state of advanced query controls
const changeAdvanceQueryUI = () => {
	showAdvanceQueryUI.value = !showAdvanceQueryUI.value;
};

// Query operation
const handleQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	var res = await getAPI(ApprovalFlowApi).apiApprovalFlowPagePost(params);
	state.tableData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// Open new page
const openAddApprovalFlow = () => {
	state.dialogTitle = 'Add approval flow';
	editDialogRef.value.openDialog({ status: 1 });
};

// Open the edit page
const openEditApprovalFlow = (row: ApprovalFlowOutput) => {
	state.dialogTitle = 'Edit approval flow';
	editDialogRef.value.openDialog(row);
};

// Open print page
const openEditDialog = (row: ApprovalFlowOutput) => {
	state.dialogTitle = 'Edit approval flow';
	editDialogRef.value.openDialog(row);
};

// Open print page
const openDetailDialog = (row: ApprovalFlowOutput) => {
	state.dialogTitle = 'ViewApproval workflow';
	detailDialogRef.value.openDialog(row);
};

const openEditFormDialog = (row: ApprovalFlowOutput) => {
	state.dialogTitle = 'edit form';
	editFormDialogRef.value.openDialog(row);
};

const openEditFlowDialog = (row: ApprovalFlowOutput) => {
	state.dialogTitle = 'Editing process';
	editFlowDialogRef.value.openDialog(row);
};

const handleFlow = (json: string) => {
	console.log(JSON.stringify(json));
	handleQuery();
};

// delete
const delApprovalFlow = (row: ApprovalFlowOutput) => {
	ElMessageBox.confirm(`ConfirmwantDelete??`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			if (row.id) {
				await getAPI(ApprovalFlowApi).apiApprovalFlowDeletePost({ id: row.id });
				handleQuery();
				ElMessage.success('Deleted successfully');
			}
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

<style scoped>
:deep(.el-ipnut),
:deep(.el-select),
:deep(.el-input-number) {
	width: 100%;
}
</style>
