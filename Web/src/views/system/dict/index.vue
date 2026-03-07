<template>
	<div class="sys-dict-container">
		<el-row :gutter="5" style="width: 100%; height: 100%; flex: 1">
			<el-col :span="12" :xs="24" style="display: flex; height: 100%; flex: 1">
				<el-card class="full-table" shadow="hover" :body-style="{ height: 'calc(100% - 51px)' }">
					<template #header>
						<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"><ele-Collection /></el-icon>dictionary
					</template>
                    
					<el-form :model="state.queryDictTypeParams" ref="queryForm" :inline="true" @submit.native.prevent class="handle-form">
						<el-form-item label="name">
							<el-input v-model="state.queryDictTypeParams.name" @keyup.enter.native="handleDictTypeQuery" placeholder="Dictionary Name" clearable />
						</el-form-item>
						<el-form-item label="Encoding">
							<el-input v-model="state.queryDictTypeParams.code" @keyup.enter.native="handleDictTypeQuery" placeholder="Dictionary Encoding" clearable />
						</el-form-item>
						<el-form-item>
							<el-button-group>
								<el-button type="primary" icon="ele-Search" @click="handleDictTypeQuery" v-auth="'sysDictType:page'"> Query </el-button>
								<el-button icon="ele-Refresh" @click="resetDictTypeQuery"> reset </el-button>
							</el-button-group>
						</el-form-item>
						<el-form-item>
							<el-button type="primary" icon="ele-Plus" @click="openAddDictType" v-auth="'sysDictType:add'"> Add New </el-button>
						</el-form-item>
					</el-form>
                    
					<el-table :data="state.dictTypeData" style="width: 100%" v-loading="state.typeLoading" @row-click="handleDictType" highlight-current-row @sort-change="sortChangeDityType" border>
						<el-table-column type="index" label="No" width="55" align="center" sortable='custom' />
						<el-table-column prop="name" label="Dictionary Name" min-width="120" header-align="center" sortable='custom' show-overflow-tooltip />
						<el-table-column prop="code" label="Dictionary Encoding" min-width="140" header-align="center" sortable='custom' show-overflow-tooltip />
						<el-table-column prop="sysFlag" label="Built-in system" min-width="90" align="center" sortable='custom' show-overflow-tooltip v-if="userInfo.accountType === AccountTypeEnum.NUMBER_999">
							<template #default="scope">
                                <g-sys-dict v-model="scope.row.sysFlag" code="YesNoEnum" />
							</template>
						</el-table-column>
						<el-table-column prop="isTenant" label="tenant dictionary" min-width="90" align="center"  sortable='custom'  show-overflow-tooltip v-if="userInfo.accountType === AccountTypeEnum.NUMBER_999">
							<template #default="scope">
                                <g-sys-dict v-model="scope.row.isTenant" code="YesNoEnum" />
							</template>
						</el-table-column>
						<el-table-column prop="status" label="state" width="80" align="center"  sortable='custom'  show-overflow-tooltip>
							<template #default="scope">
                                <g-sys-dict v-model="scope.row.status" code="StatusEnum" />
							</template>
						</el-table-column>
						<el-table-column prop="orderNo" label="Sort" width="80" align="center"  sortable='custom'  show-overflow-tooltip />
						<el-table-column label="Modify records" width="100" align="center" show-overflow-tooltip>
							<template #default="scope">
								<ModifyRecord :data="scope.row" />
							</template>
						</el-table-column>
						<el-table-column label="Operation" width="120" fixed="right" align="center" v-if="auths(['sysDictType:update', 'sysDictType:delete'])">
							<template #default="scope">
								<el-tooltip content="Edit">
									<el-button icon="ele-Edit" size="small" text type="primary" :disabled="!hasPermission(scope.row)" @click="openEditDictType(scope.row)" v-auth="'sysDictType:update'"> </el-button>
								</el-tooltip>
								<el-tooltip content="Delete">
									<el-button icon="ele-Delete" size="small" text type="danger" :disabled="scope.row?.sysFlag === 1 || !hasPermission(scope.row)" @click="delDictType(scope.row)" v-auth="'sysDictType:delete'"> </el-button>
								</el-tooltip>
							</template>
						</el-table-column>
					</el-table>
					<el-pagination
						v-model:currentPage="state.tableDictTypeParams.page"
						v-model:page-size="state.tableDictTypeParams.pageSize"
						:total="state.tableDictTypeParams.total"
						:page-sizes="[10, 20, 50, 100]"
						size="small"
						background
						@size-change="handleDictTypeSizeChange"
						@current-change="handleDictTypeCurrentChange"
						layout="total, sizes, prev, pager, next, jumper"
					/>
				</el-card>
			</el-col>

			<el-col :span="12" :xs="24" style="display: flex; height: 100%; flex: 1">
				<el-card class="full-table" shadow="hover" :body-style="{ height: 'calc(100% - 51px)' }">
					<template #header>
						<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"><ele-Collection /></el-icon>Dictionary value [{{ state.editDictTypeName }}]
					</template>
					<el-form :model="state.queryDictDataParams" ref="queryForm" :inline="true" @submit.native.prevent class="handle-form">
						<el-form-item label="Display Text">
							<el-input v-model="state.queryDictDataParams.label" placeholder="Display Text" @keyup.enter="handleDictDataQuery" />
						</el-form-item>
						<el-form-item>
							<el-button-group>
								<el-button type="primary" icon="ele-Search" @click="handleDictDataQuery"> Query </el-button>
								<el-button icon="ele-Refresh" @click="resetDictDataQuery"> reset </el-button>
							</el-button-group>
						</el-form-item>
						<el-form-item>
							<el-button type="primary" icon="ele-Plus" @click="openAddDictData" :disabled="!hasPermission(state.selectDict)" v-auth="'sysDictData:add'"> Add New </el-button>
						</el-form-item>
					</el-form>

					<el-table :data="state.dictDataData" style="width: 100%" v-loading="state.loading" border>
						<el-table-column type="index" label="No" width="55" align="center" />
						<el-table-column prop="value" label="Display Text" header-align="center" min-width="120" show-overflow-tooltip>
							<template #default="scope">
								<el-tag :type="scope.row.tagType" :style="scope.row.styleSetting" :class="scope.row.classSetting">{{ scope.row.label }}</el-tag>
							</template>
						</el-table-column>
						<el-table-column prop="value" label="Dictionary value" header-align="center" min-width="120" show-overflow-tooltip />
						<el-table-column prop="code" label="Encoding" header-align="center" min-width="120" show-overflow-tooltip />
						<el-table-column prop="extData" label="Expand data" width="90" align="center">
							<template #default="scope">
								<el-tag type="warning" v-if="scope.row.extData == null || scope.row.extData == ''">null</el-tag>
								<el-tag type="success" v-else>Has value</el-tag>
							</template>
						</el-table-column>
						<el-table-column prop="status" label="state" width="70" align="center" show-overflow-tooltip>
							<template #default="scope">
                                <g-sys-dict v-model="scope.row.status" code="StatusEnum" />
							</template>
						</el-table-column>
						<el-table-column prop="orderNo" label="Sort" width="60" align="center" show-overflow-tooltip />
						<el-table-column label="Modify records" width="100" align="center" show-overflow-tooltip>
							<template #default="scope">
								<ModifyRecord :data="scope.row" />
							</template>
						</el-table-column>
						<el-table-column label="Operation" width="120" fixed="right" align="center" show-overflow-tooltip v-if="auths(['sysDictData:add', 'sysDictData:update', 'sysDictData:delete'])">
							<template #default="scope">
								<el-tooltip content="Edit">
									<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditDictData(scope.row)" v-auth="'sysDictData:update'"> </el-button>
								</el-tooltip>
								<el-tooltip content="Delete">
									<el-button icon="ele-Delete" size="small" text type="danger" :disabled="!hasPermission(state.selectDict)" @click="delDictData(scope.row)" v-auth="'sysDictData:delete'"> </el-button>
								</el-tooltip>
								<el-tooltip content="Copy">
									<el-button icon="ele-CopyDocument" size="small" text type="primary" :disabled="!hasPermission(state.selectDict)" @click="openCopyDictData(scope.row)" v-auth="'sysDictData:add'"> </el-button>
								</el-tooltip>
							</template>
						</el-table-column>
					</el-table>
					<el-pagination
						v-model:currentPage="state.tableDictDataParams.page"
						v-model:page-size="state.tableDictDataParams.pageSize"
						:total="state.tableDictDataParams.total"
						:page-sizes="[10, 20, 50, 100]"
						size="small"
						background
						@size-change="handleDictDataSizeChange"
						@current-change="handleDictDataCurrentChange"
						layout="total, sizes, prev, pager, next, jumper"
					/>
				</el-card>
			</el-col>
		</el-row>
		<EditDictType ref="editDictTypeRef" :title="state.editDictTypeTitle" @handleQuery="handleDictTypeQuery" @handleUpdate="updateDictSession" />
		<EditDictData ref="editDictDataRef" :title="state.editDictDataTitle" @handleQuery="handleDictDataQuery" @handleUpdate="updateDictSession" />
	</div>
</template>

<script lang="ts" setup name="sysDict">
import { onMounted, reactive, ref } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { useUserInfo } from '/@/stores/userInfo';
import { ElMessageBox, ElMessage } from 'element-plus';
import {SysDictType, SysDictData, AccountTypeEnum} from '/@/api-services/models';
import { SysDictTypeApi, SysDictDataApi } from '/@/api-services/api';
import EditDictType from '/@/views/system/dict/component/editDictType.vue';
import EditDictData from '/@/views/system/dict/component/editDictData.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';
import { auths } from "/@/utils/authFunction";

const userInfo = useUserInfo().userInfos;
const editDictTypeRef = ref<InstanceType<typeof EditDictType>>();
const editDictDataRef = ref<InstanceType<typeof EditDictData>>();
const state = reactive({
	loading: false,
	typeLoading: false,
	showMoveDictDialog: false,
	tenantList: [] as Array<any>,
	selectDict: {} as SysDictType,
	dictTypeData: [] as Array<SysDictType>,
	dictDataData: [] as Array<SysDictData>,
	queryDictTypeParams: {
		name: undefined,
		code: undefined,
	},
	tableDictTypeParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
		field: 'orderNo',
		order: 'ascending', // Sorting direction
		descStr: 'descending', // Key characters for sorting in descending order
	},
	queryDictDataParams: {
    label: undefined,
		dictTypeId: 0, // Dictionary typeId
	},
	tableDictDataParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	editDictTypeTitle: '',
	editDictDataTitle: '',
	editDictTypeName: '',
});

onMounted(async () => {
	handleDictTypeQuery();
});

// Query dictionary operations
const handleDictTypeQuery = async () => {
	state.typeLoading = true;
	let params = Object.assign(state.queryDictTypeParams, state.tableDictTypeParams);
	const res = await getAPI(SysDictTypeApi).apiSysDictTypePagePost(params);
	state.dictTypeData = res.data.result?.items ?? [];
	state.tableDictTypeParams.total = res.data.result?.total;
	state.typeLoading = false;
};

// Query dictionary value operation
const handleDictDataQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryDictDataParams, state.tableDictDataParams);
	const res = await getAPI(SysDictDataApi).apiSysDictDataPagePost(params);
	state.dictDataData = res.data.result?.items ?? [];
	state.tableDictDataParams.total = res.data.result?.total;
	state.loading = false;
};

// Column sort
const sortChangeDityType = async (column: any) => {
  state.tableDictTypeParams.field = column.prop;
  state.tableDictTypeParams.order = column.order;
  await handleDictTypeQuery();
};

// Click on the form
const handleDictType = (row: any, event: any, column: any) => {
	openDictDataDialog(row);
};

// Determine whether you have permission to operate
const hasPermission = (row: any) => {
	//if (row.code?.toLowerCase().endsWith('enum')) return false;
	return row?.sysFlag === 2 || userInfo.accountType === AccountTypeEnum.NUMBER_999;
};

// Reset dictionary operation
const resetDictTypeQuery = () => {
	state.queryDictTypeParams.name = undefined;
	state.queryDictTypeParams.code = undefined;
	handleDictTypeQuery();
};

// Reset dictionary value operation
const resetDictDataQuery = () => {
	state.queryDictDataParams.label = undefined;
	handleDictDataQuery();
};

// Open the new dictionary page
const openAddDictType = () => {
	state.editDictTypeTitle = 'Add dictionary';
	editDictTypeRef.value?.openDialog({ sysFlag: 2, status: 1, orderNo: 100 });
};

// Open the new dictionary value page
const openAddDictData = () => {
	if (!state.queryDictDataParams.dictTypeId) {
		ElMessage.warning('Please select a dictionary');
		return;
	}
	state.editDictDataTitle = 'Add dictionary value';
	editDictDataRef.value?.openDialog({ status: 1, orderNo: 100, dictTypeId: state.queryDictDataParams.dictTypeId });
};

// Open the edit dictionary page
const openEditDictType = (row: any) => {
	state.editDictTypeTitle = 'Edit Dictionary';
	editDictTypeRef.value?.openDialog(row);
};

// Open the Copy Dictionary Values ​​page
const openCopyDictData = (row: any) => {
	state.editDictDataTitle = 'copy dictionary value';
	const copyRow = JSON.parse(JSON.stringify(row));
	copyRow.id = 0;
	copyRow.dictType = null;
	editDictDataRef.value?.openDialog(copyRow);
};

// Open the edit dictionary value page
const openEditDictData = (row: any) => {
	row.dictType = state.selectDict;
	state.editDictDataTitle = 'Edit dictionary value';
	editDictDataRef.value?.openDialog(row);
};

// Open dictionary values ​​page
const openDictDataDialog = (row: any) => {
	state.selectDict = row;
	state.editDictTypeName = row.name;
	state.queryDictDataParams.dictTypeId = row.id;
	handleDictDataQuery();
};

// delete dictionary
const delDictType = (row: any) => {
	ElMessageBox.confirm(`Are you sure to delete the dictionary: [${row.name}]?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	}).then(async () => {
			await getAPI(SysDictTypeApi).apiSysDictTypeDeletePost({ id: row.id });
			handleDictTypeQuery();
			updateDictSession();
			ElMessage.success('Deleted successfully');
  }).catch(() => {});
};

// Delete dictionary value
const delDictData = (row: any) => {
	ElMessageBox.confirm(`Are you sure to delete the dictionary value: [${row.value}]?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	}).then(async () => {
			await getAPI(SysDictDataApi).apiSysDictDataDeletePost({ id: row.id });
			handleDictDataQuery();
			updateDictSession();
			ElMessage.success('Deleted successfully');
  }).catch(() => {});
};

// Change dictionary page capacity
const handleDictTypeSizeChange = (val: number) => {
	state.tableDictTypeParams.pageSize = val;
	handleDictTypeQuery();
};

// Change dictionary page number
const handleDictTypeCurrentChange = (val: number) => {
	state.tableDictTypeParams.page = val;
	handleDictTypeQuery();
};

// Change dictionary value page capacity
const handleDictDataSizeChange = (val: number) => {
	state.tableDictDataParams.pageSize = val;
	handleDictDataQuery();
};

// Change dictionary value page number
const handleDictDataCurrentChange = (val: number) => {
	state.tableDictDataParams.page = val;
	handleDictDataQuery();
};

// Update front-end dictionary cache
const updateDictSession = async () => {
	await useUserInfo().setDictList();
};
</script>

<style lang="scss" scoped>
.sys-dict-container {
    flex-direction: row !important;
}
.handle-form {
    margin-bottom: var(--el-card-padding);

    .el-form-item, .el-form-item:last-of-type {
        margin: 0 10px !important;
    }
}
:deep(.notice-bar) {
	position: absolute;
	display: inline-flex;
	height: 25px !important;
	width: 500px;
	overflow: hidden !important;
	div[data-slate-editor] {
		text-wrap-mode: nowrap;
	}
}
</style>
