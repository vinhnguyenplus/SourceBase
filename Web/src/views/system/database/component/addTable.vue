<template>
	<div class="sys-dbTable-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="1400px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> Add table </span>
				</div>
			</template>
			<el-divider content-position="left">Datasheet information</el-divider>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Table Name" prop="tableName" :rules="[{ required: true, message: 'Name cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.tableName" placeholder="Table Name" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Description" prop="description" :rules="[{ required: true, message: 'Description cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.description" placeholder="Description" clearable type="textarea" />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<el-divider content-position="left">Data column information</el-divider>
			<el-table :data="state.tableData" style="width: 100%" max-height="400">
				<el-table-column prop="dbColumnName" label="Field Name" width="200" fixed>
					<template #default="scope">
						<el-input v-model="scope.row.dbColumnName" autocomplete="off" />
					</template>
				</el-table-column>
				<el-table-column prop="columnDescription" label="Description" width="220">
					<template #default="scope">
						<el-input v-model="scope.row.columnDescription" autocomplete="off" />
					</template>
				</el-table-column>
				<el-table-column prop="isPrimarykey" label="Primary Key" width="100">
					<template #default="scope">
						<el-select v-model="scope.row.isPrimarykey" class="m-2" placeholder="Select">
							<el-option v-for="item in yesNoSelect" :key="item.value" :label="item.label" :value="item.value" />
						</el-select>
					</template>
				</el-table-column>
				<el-table-column prop="isIdentity" label="Auto-increment" width="100">
					<template #default="scope">
						<el-select v-model="scope.row.isIdentity" class="m-2" placeholder="Select">
							<el-option v-for="item in yesNoSelect" :key="item.value" :label="item.label" :value="item.value" />
						</el-select>
					</template>
				</el-table-column>
				<el-table-column prop="dataType" label="Type" width="150">
					<template #default="scope">
						<el-select v-model="scope.row.dataType" class="m-2" placeholder="Select" @change="handleColTypeChange(scope.row)">
							<el-option v-for="item in dataTypeList" :key="item.value" :label="item.value" :value="item.value" />
						</el-select>
					</template>
				</el-table-column>
				<el-table-column prop="isNullable" label="Nullable" width="100">
					<template #default="scope">
						<el-select v-model="scope.row.isNullable" class="m-2" placeholder="Select">
							<el-option v-for="item in yesNoSelect" :key="item.value" :label="item.label" :value="item.value" />
						</el-select>
					</template>
				</el-table-column>
				<el-table-column prop="length" label="length" width="100">
					<template #default="scope">
						<el-input-number v-model="scope.row.length" controls-position="right" class="w100" />
					</template>
				</el-table-column>
				<el-table-column prop="decimalDigits" label="Decimal places" width="100">
					<template #default="scope">
						<el-input-number v-model="scope.row.decimalDigits" controls-position="right" class="w100" />
					</template>
				</el-table-column>				
				<el-table-column prop="defaultValue" label="Default value" width="90">
					<template #default="scope">
						<el-input v-model="scope.row.defaultValue" autocomplete="off" />
					</template>
				</el-table-column>
				<el-table-column label="Operation" min-width="200" align="center" fixed="right">
					<template #default="scope">
						<el-button link type="primary" icon="el-icon-delete" @click.prevent="handleColDelete(scope.$index)">Delete</el-button>
						<el-button v-if="state.tableData.length > 1" link type="primary" icon="ele-Top" @click.prevent="handleColUp(scope.row, scope.$index)">Move up</el-button>
						<el-button v-if="state.tableData.length > 1" link type="primary" icon="ele-Bottom" @click.prevent="handleColDown(scope.row, scope.$index)">Move down</el-button>
					</template>
				</el-table-column>
			</el-table>
			<div style="text-align: left; margin-top: 10px">
				<el-button icon="ele-Plus" @click="addPrimaryColumn">Add primary key field</el-button>
				<el-button icon="ele-Plus" @click="addColumn">Add a regular field</el-button>
				<el-button icon="ele-Plus" @click="addTenantColumn">Added tenant field</el-button>
				<el-button icon="ele-Plus" @click="addOrgColumn">Add organization field</el-button>
				<el-button icon="ele-Plus" @click="addBaseColumn">Add new basic fields</el-button>
				<el-button icon="ele-Plus" @click="addDeleteColumn">Add a soft delete field</el-button>
			</div>

			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">Cancel</el-button>
					<el-button type="primary" @click="submit">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysAddTable">
import { reactive, ref } from 'vue';
import { ElMessage } from 'element-plus';

import { getAPI } from '/@/utils/axios-utils';
import { SysDatabaseApi } from '/@/api-services/api';
import { UpdateDbTableInput } from '/@/api-services/models';
import { dataTypeList, EditRecordRow, yesNoSelect } from '../database';

var colIndex = 0;
const emits = defineEmits(['addTableSubmitted']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as UpdateDbTableInput,
	tableData: [] as any,
});

// Open pop-up window
const openDialog = (row: any) => {
	state.ruleForm = row;
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
};

// Close pop-up window
const closeDialog = () => {
	emits('addTableSubmitted', state.ruleForm.tableName ?? '');
	state.tableData = [];
	state.isShowDialog = false;
};

// Cancel
const cancel = () => {
	state.isShowDialog = false;
};

// submit
const submit = () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		if (state.tableData.length === 0) {
			ElMessage({
				type: 'error',
				message: `PleaseAdd tocolumn!`,
			});
			return;
		}
		const params: any = {
			dbColumnInfoList: state.tableData,
			...state.ruleForm,
		};
		await getAPI(SysDatabaseApi).apiSysDatabaseAddTablePost(params);
		closeDialog();
	});
};

// Add primary key column
function addPrimaryColumn() {
	state.tableData.push({
		columnDescription: 'Primary keyId',
		dataType: 'bigint',
		dbColumnName: 'Id',
		decimalDigits: 0,
		isIdentity: 0,
		isNullable: 0,
		isPrimarykey: 1,
		length: 0,
		key: colIndex,
		editable: true,
		isNew: true,
	});
	colIndex++;
}

// Add normal column
function addColumn() {
	state.tableData.push({
		columnDescription: '',
		//dataType: 'varchar',
		dbColumnName: '',
		decimalDigits: 0,
		isIdentity: 0,
		isNullable: 1,
		isPrimarykey: 0,
		//length: 32,
		key: colIndex,
		editable: true,
		isNew: true,
	});
	colIndex++;
}

// Add tenant column
function addTenantColumn() {
	state.tableData.push({
		columnDescription: 'Tenant ID',
		dataType: 'bigint',
		dbColumnName: 'TenantId',
		decimalDigits: 0,
		isIdentity: 0,
		isNullable: 1,
		isPrimarykey: 0,
		length: 0,
		key: colIndex,
		editable: true,
		isNew: true,
	});
	colIndex++;
}

// Add organization column
function addOrgColumn() {
	state.tableData.push({
		columnDescription: 'Organization ID',
		dataType: 'bigint',
		dbColumnName: 'OrgId',
		decimalDigits: 0,
		isIdentity: 0,
		isNullable: 1,
		isPrimarykey: 0,
		length: 0,
		key: colIndex,
		editable: true,
		isNew: true,
	});
	colIndex++;
}

// Add common base columns
function addBaseColumn() {
	const fileds = [
		{
			dataType: 'datetime',
			name: 'CreateTime',
			desc: 'Creation Time',
		},
		{
			dataType: 'datetime',
			name: 'UpdateTime',
			desc: 'Update Time',
		},
		{
			dataType: 'bigint',
			name: 'CreateUserId',
			desc: 'CreatorId',
		},
		{
			dataType: 'varchar',
			name: 'CreateUserName',
			desc: 'Creator name',
			length: 64,
		},
		{
			dataType: 'bigint',
			name: 'UpdateUserId',
			desc: 'Modifier ID',
		},
		{
			dataType: 'varchar',
			name: 'UpdateUserName',
			desc: 'Modifier name',
			length: 64,
		},
	];

	fileds.forEach((m: any) => {
		state.tableData.push({
			columnDescription: m.desc,
			dataType: m.dataType,
			dbColumnName: m.name,
			decimalDigits: 0,
			isIdentity: 0,
			isNullable: m.isNullable === 0 ? 0 : 1,
			isPrimarykey: 0,
			length: m.length || 0,
			key: colIndex,
			editable: true,
			isNew: true,
		});
		colIndex++;
	});
}

// Add soft delete column
function addDeleteColumn() {
	state.tableData.push({
		columnDescription: 'soft delete',
		dataType: 'bit',
		dbColumnName: 'IsDelete',
		decimalDigits: 0,
		isIdentity: 0,
		isNullable: 0,
		isPrimarykey: 0,
		length: 0,
		key: colIndex,
		editable: true,
		isNew: true,
	});
	colIndex++;
}

function handleColDelete(index: number) {
	state.tableData.splice(index, 1);
}

// Column type selection changes
function handleColTypeChange(record: EditRecordRow) {
    if (['varchar', 'char', 'nvarchar', 'nchar'].includes(record.dataType as string)) {
        if ([0, undefined, null].includes(record.length)) {
            record.length = 32;
        }
    } else {
        record.length = 0;
    }
}

// move up
function handleColUp(record: EditRecordRow, index: number) {
	if (record.isNew) {
		var data1 = ChangeExForArray(index, index - 1, state.tableData);
		return data1;
	}
}

// move down
function handleColDown(record: EditRecordRow, index: number) {
	if (record.isNew) {
		return ChangeExForArray(index, index + 1, state.tableData);
	}
}

function ChangeExForArray(index1: number, index2: number, array: Array<EditRecordRow>) {
	let maxIndex = state.tableData.length - 1; // maximum index
	if (index2 > maxIndex) {
		index2 = 0;
	}
	if (index2 < 0) {
		index2 = maxIndex;
	}
	let temp = array[index1];
	array[index1] = array[index2];
	array[index2] = temp;
	return array;
}

// Export object
defineExpose({ openDialog });
</script>
