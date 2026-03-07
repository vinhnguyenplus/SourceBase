<template>
	<div class="sys-dbColumn-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> Add column </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="List" prop="dbColumnName" :rules="[{ required: true, message: 'Name cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.dbColumnName" placeholder="Column name" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Description" prop="columnDescription" :rules="[{ required: true, message: 'Description cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.columnDescription" placeholder="Description" clearable type="textarea" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Primary Key">
							<el-select v-model="state.ruleForm.isPrimarykey" class="w100">
								<el-option v-for="item in yesNoSelect" :key="item.value" :label="item.label" :value="item.value" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Auto-increment">
							<el-select v-model="state.ruleForm.isIdentity" class="w100">
								<el-option v-for="item in yesNoSelect" :key="item.value" :label="item.label" :value="item.value" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Type">
							<el-select v-model="state.ruleForm.dataType" class="w100">
								<el-option v-for="item in dataTypeList" :key="item.value" :label="item.value" :value="item.value" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Nullable">
							<el-select v-model="state.ruleForm.isNullable" class="w100">
								<el-option v-for="item in yesNoSelect" :key="item.value" :label="item.label" :value="item.value" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="length">
							<el-input-number v-model="state.ruleForm.length" class="w100" controls-position="right" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Decimal places">
							<el-input-number v-model="state.ruleForm.decimalDigits" class="w100" controls-position="right" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Default value" prop="defaultValue">
							<el-input v-model="state.ruleForm.defaultValue" placeholder="Default value" clearable />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">Cancel</el-button>
					<el-button type="primary" @click="submit">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysAddColumn">
import { reactive, ref } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysDatabaseApi } from '/@/api-services/api';
import { DbColumnInput } from '/@/api-services/models';
import { dataTypeList, yesNoSelect } from '../database';

const emits = defineEmits(['handleQueryColumn']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as DbColumnInput,
});

// Open pop-up window
const openDialog = (addRow: DbColumnInput) => {
	state.ruleForm = addRow;
	if (state.ruleForm.length === 0) {
		state.ruleForm.length = 32;
	}
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
};

// Close pop-up window
const closeDialog = () => {
	emits('handleQueryColumn');
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
		await getAPI(SysDatabaseApi).apiSysDatabaseAddColumnPost(state.ruleForm);
		closeDialog();
	});
};

// Export object
defineExpose({ openDialog });
</script>
