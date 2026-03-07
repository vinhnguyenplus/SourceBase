<template>
	<div class="sys-database-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true" v-loading="state.loading">
				<el-form-item label="Library name">
					<el-select v-model="state.configId" placeholder="Library name" filterable @change="handleQueryTable">
						<el-option v-for="item in state.dbData" :key="item.configId" :label="item.dbNickName" :value="item.configId" />
					</el-select>
				</el-form-item>
				<el-form-item label="Table Name">
					<el-select v-model="state.tableName" placeholder="Table Name" filterable clearable @change="handleQueryColumn">
                        <template #label="{ label, value }">
                            <div class="flex flex-items-center">
                                <span>{{ value }}</span>
                                <span class="desc">{{ label }}</span>
                            </div>
                        </template>
						<el-option v-for="item in state.tableData" :key="item.name" :data="item" :label="item.description" :value="item.name">
                            <div class="flex flex-items-center">
                                <span style="flex: 1">{{ item.name }}</span>
                                <el-tag type="info" size="small">{{ item.description }}</el-tag>
                            </div>
                        </el-option>
					</el-select>
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button icon="ele-Plus" type="primary" @click="openAddTable"> Add table </el-button>
						<el-button icon="ele-Edit" @click="openEditTable"> Edit Table </el-button>
						<el-button icon="ele-Delete" type="danger" @click="delTable" disabled> Delete table </el-button>
						<el-button icon="ele-View" @click="visualTable"> Visualization </el-button>
					</el-button-group>
					<el-button-group style="padding-left: 10px">
						<el-button icon="ele-Plus" @click="openAddColumn"> Add column </el-button>
						<el-button icon="ele-Plus" @click="openGenDialog"> Generate entity </el-button>
						<el-popover placement="bottom" title="Warm reminder" :width="200" trigger="hover" content="If it is a newly generated entity, please restart the service before generating the seed again.">
							<template #reference>
								<el-button icon="ele-Plus" @click="openGenSeedDataDialog"> Generate Seed </el-button>
							</template>
						</el-popover>
					</el-button-group>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.columnData" style="width: 100%" v-loading="state.loading1" border>
				<el-table-column type="index" label="No" width="55" align="center" />
				<el-table-column prop="dbColumnName" label="Field Name" show-overflow-tooltip />
				<el-table-column prop="dataType" label="data type" align="center" show-overflow-tooltip />
				<el-table-column prop="isPrimarykey" label="Primary Key" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success" v-if="scope.row.isPrimarykey === true">Yes</el-tag>
						<el-tag type="info" v-else>no</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="isIdentity" label="Auto-increment" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success" v-if="scope.row.isIdentity === true">Yes</el-tag>
						<el-tag type="info" v-else>no</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="isNullable" label="Nullable" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag v-if="scope.row.isNullable === true">Yes</el-tag>
						<el-tag type="info" v-else>no</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="length" label="length" width="70" align="center" show-overflow-tooltip />
				<el-table-column prop="decimalDigits" label="Accuracy" width="70" align="center" show-overflow-tooltip />
				<el-table-column prop="defaultValue" label="Default value" align="center" show-overflow-tooltip />
				<el-table-column prop="columnDescription" label="Description" header-align="center" show-overflow-tooltip />
				<el-table-column label="Operation" width="195" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Top" size="small" text type="primary" @click="moveColumn(scope.row, 'up')" :disabled="scope.$index === 0" title="move up"></el-button>
						<el-button icon="ele-Bottom" size="small" text type="primary" @click="moveColumn(scope.row, 'down')" :disabled="scope.$index === state.columnData.length - 1" title="Move down"></el-button>
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditColumn(scope.row)">Edit</el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="delColumn(scope.row)">Delete</el-button>
					</template>
				</el-table-column>
			</el-table>
		</el-card>

		<EditTable ref="editTableRef" @handleQueryTable="handleQueryTable" />
		<EditColumn ref="editColumnRef" @handleQueryColumn="handleQueryColumn" />
		<AddTable ref="addTableRef" @addTableSubmitted="addTableSubmitted" />
		<AddColumn ref="addColumnRef" @handleQueryColumn="handleQueryColumn" />
		<GenEntity ref="genEntityRef" @handleQueryColumn="handleQueryColumn" :application-namespaces="state.appNamespaces" />
		<GenSeedData ref="genSeedDataRef" :application-namespaces="state.appNamespaces" />
	</div>
</template>

<script lang="ts" setup name="sysDatabase">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { useRouter } from 'vue-router';
import EditTable from '/@/views/system/database/component/editTable.vue';
import EditColumn from '/@/views/system/database/component/editColumn.vue';
import AddTable from '/@/views/system/database/component/addTable.vue';
import AddColumn from '/@/views/system/database/component/addColumn.vue';
import GenEntity from '/@/views/system/database/component/genEntity.vue';
import GenSeedData from '/@/views/system/database/component/genSeedData.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysDatabaseApi, SysCodeGenApi } from '/@/api-services/api';
import { DbColumnOutput, DbTableInfo, DbColumnInput, DeleteDbTableInput, DeleteDbColumnInput, MoveDbColumnInput } from '/@/api-services/models';

const editTableRef = ref<InstanceType<typeof EditTable>>();
const editColumnRef = ref<InstanceType<typeof EditColumn>>();
const addTableRef = ref<InstanceType<typeof AddTable>>();
const addColumnRef = ref<InstanceType<typeof AddColumn>>();
const genEntityRef = ref<InstanceType<typeof GenEntity>>();
const genSeedDataRef = ref<InstanceType<typeof GenSeedData>>();
const router = useRouter();
const state = reactive({
	loading: false,
	loading1: false,
	dbData: [] as any,
	configId: '',
	tableData: [] as Array<DbTableInfo>,
	tableName: '',
	columnData: [] as Array<DbColumnOutput>,
	queryParams: {
		name: undefined,
		code: undefined,
	},
	appNamespaces: [] as Array<String>, // storage location
});

onMounted(async () => {
	state.loading = true;
	var res = await getAPI(SysDatabaseApi).apiSysDatabaseListGet();
	state.dbData = res.data.result;
	state.loading = false;

	let appNamesRes = await getAPI(SysCodeGenApi).apiSysCodeGenApplicationNamespacesGet();
	state.appNamespaces = appNamesRes.data.result as Array<string>;
});

// Add table
const addTableSubmitted = (e: any) => {
	handleQueryTable();
	state.tableName = e;
	handleQueryColumn();
};

// Table query operations
const handleQueryTable = async () => {
	state.tableName = '';
	state.columnData = [];
	state.loading = true;

	var res = await getAPI(SysDatabaseApi).apiSysDatabaseTableListConfigIdGet(state.configId);
	let tableData = res.data.result ?? [];
	state.tableData = [];
	tableData.forEach((element: any) => {
		//Exclude tables starting with zero_
		if (!element.name.startsWith('zero_')) {
			state.tableData.push(element);
		}
	});
	state.loading = false;
};

// Column query operations
const handleQueryColumn = async () => {
	state.columnData = [];
	if (state.tableName == '' || typeof state.tableName == 'undefined') return;

	state.loading1 = true;
	var res = await getAPI(SysDatabaseApi).apiSysDatabaseColumnListTableNameConfigIdGet(state.tableName, state.configId);
	state.columnData = res.data.result ?? [];
	state.loading1 = false;
};

// Open the table editing page
const openEditTable = () => {
	if (state.configId == '' || state.tableName == '') {
		ElMessage({
			type: 'error',
			message: `Please select a database name and table name!`,
		});
		return;
	}
	var res = state.tableData.filter((u: any) => u.name == state.tableName);
	var table: any = {
		configId: state.configId,
		tableName: state.tableName,
		oldTableName: state.tableName,
		description: res[0].description,
	};
	editTableRef.value?.openDialog(table);
};

// Open the entity generation page
const openGenDialog = () => {
	if (state.configId == '' || state.tableName == '') {
		ElMessage({
			type: 'error',
			message: `Please select a database name and table name!`,
		});
		return;
	}
	// var res = state.tableData.filter((u: any) => u.name == state.tableName);
	var table: any = {
		configId: state.configId,
		tableName: state.tableName,
		position: state.appNamespaces[0],
	};
	genEntityRef.value?.openDialog(table);
};

// Generate seed data page
const openGenSeedDataDialog = () => {
	if (state.configId == '' || state.tableName == '') {
		ElMessage({
			type: 'error',
			message: `Please select a database name and table name!`,
		});
		return;
	}
	var table: any = {
		configId: state.configId,
		tableName: state.tableName,
		position: state.appNamespaces[0],
	};
	genSeedDataRef.value?.openDialog(table);
};

// Open the table add page
const openAddTable = () => {
	if (state.configId == '') {
		ElMessage({
			type: 'error',
			message: `Please select the library name!`,
		});
		return;
	}
	var table: any = {
		configId: state.configId,
		tableName: '',
		oldTableName: '',
		description: '',
	};
	addTableRef.value?.openDialog(table);
};

// Open the column editing page
const openEditColumn = (row: any) => {
	var column: any = {
		configId: state.configId,
		tableName: row.tableName,
		columnName: row.dbColumnName,
		oldColumnName: row.dbColumnName,
		description: row.columnDescription,
		defaultValue: row.defaultValue,
	};
	editColumnRef.value?.openDialog(column);
};

// Open the column addition page
const openAddColumn = () => {
	if (state.configId == '' || state.tableName == '') {
		ElMessage({
			type: 'error',
			message: `Please select a database name and table name!`,
		});
		return;
	}
	const addRow: DbColumnInput = {
		configId: state.configId,
		tableName: state.tableName,
		columnDescription: '',
		dataType: '',
		dbColumnName: '',
		decimalDigits: 0,
		isIdentity: 0,
		isNullable: 0,
		isPrimarykey: 0,
		length: 0,
		// key: 0,
		// editable: true,
		// isNew: true,
	};
	addColumnRef.value?.openDialog(addRow);
};

// Delete table
const delTable = () => {
	if (state.tableName == '') {
		ElMessage({
			type: 'error',
			message: `Please select a table name!`,
		});
		return;
	}
	ElMessageBox.confirm(`Are you sure you want to delete the table: 【${state.tableName}】?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			const deleteDbTableInput: DeleteDbTableInput = {
				configId: state.configId,
				tableName: state.tableName,
			};
			await getAPI(SysDatabaseApi).apiSysDatabaseDeleteTablePost(deleteDbTableInput);
			handleQueryTable();
			ElMessage.success('Table deleted successfully');
		})
		.catch(() => {});
};

// Delete column
const delColumn = (row: any) => {
	ElMessageBox.confirm(`Are you sure to delete the column: [${row.dbColumnName}]?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			const eleteDbColumnInput: DeleteDbColumnInput = {
				configId: state.configId,
				tableName: state.tableName,
				dbColumnName: row.dbColumnName,
			};
			await getAPI(SysDatabaseApi).apiSysDatabaseDeleteColumnPost(eleteDbColumnInput);
			handleQueryColumn();
			ElMessage.success('Column deleted successfully');
		})
		.catch(() => {});
};

const moveColumn = (row: any, direction: 'up' | 'down') => {
	const { columnData, tableName, configId } = state;
	const currentIndex = columnData.findIndex((item) => item.dbColumnName === row.dbColumnName);

	// Boundary checking and feedback
	if (direction === 'up' && currentIndex === 0) {
		ElMessage.warning('Already at the top, cannot move up');
		return;
	}
	if (direction === 'down' && currentIndex === columnData.length - 1) {
		ElMessage.warning('Already at the bottom，NoneLawMove down');
		return;
	}

	// Calculate target position
	const targetIndex = direction === 'up' ? currentIndex - 1 : currentIndex + 1;
	const targetColumn = columnData[targetIndex];
	const columnName = direction === 'up' ? targetColumn.dbColumnName : row.dbColumnName;
	const afterColumnName = direction === 'up' ? row.dbColumnName : targetColumn.dbColumnName;

	ElMessageBox.confirm(`Are you sure to move the column [${row.dbColumnName}]${direction === 'up' ? 'Move up' : 'Move down'}?`, 'Operation confirmation', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	}).then(async () => {
			try {
				const moveParams: MoveDbColumnInput = {
					configId,
					tableName,
					columnName,
					afterColumnName,
				};

				// Call API
				await getAPI(SysDatabaseApi).apiSysDatabaseMoveColumnPost(moveParams);

				handleQueryColumn();
				ElMessage.success('Column positions have been updated');
			} catch (error: any) {
				ElMessage.error(`Operation failed: ${error.message || 'Unknown error'}`);
			}
		})
		.catch(() => {});
};

// Visualization table
const visualTable = () => {
	if (state.configId == '') {
		ElMessage({
			type: 'error',
			message: `Please select the library name!`,
		});
		return;
	}
	router.push(`/develop/database/visual?configId=${state.configId}`);
};
</script>

<style lang="scss" scoped>
.el-select__placeholder {
    .desc {
        color: var(--el-color-info); 
        font-size: var(--el-font-size-extra-small);
        //font-style: italic;
        margin-left: 5px;
    }
}
</style>