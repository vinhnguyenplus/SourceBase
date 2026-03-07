<template>
	<div class="sys-dbEntity-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Cpu /> </el-icon>
					<span> Generate seed data </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto" :rules="state.rules">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Table Name" prop="tableName" :rules="[{ required: true, message: 'Table name cannot be empty', trigger: 'blur' }]">
							<el-input disabled v-model="state.ruleForm.tableName" placeholder="Table Name" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Suffix" prop="suffix">
							<el-input v-model="state.ruleForm.suffix" placeholder="Suffix" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Storage location" prop="position">
							<!-- <el-input v-model="state.ruleForm.position" placeholder="storage location" clearable >Admin.NET.Core</el-input> -->
							<el-select v-model="state.ruleForm.position" filterable clearable class="w100" placeholder="Storage location">
								<el-option v-for="(item, index) in props.applicationNamespaces" :key="index" :label="item" :value="item" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Filter duplicate data" prop="filterExistingData">
							<el-switch v-model="state.ruleForm.filterExistingData"></el-switch>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel" :disabled="state.loading">Cancel</el-button>
					<el-button type="primary" v-reclick="3000" @click="submit" :disabled="state.loading">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysGenEntity">
import { reactive, ref } from 'vue';
import { ElMessage } from "element-plus";
import { getAPI } from '/@/utils/axios-utils';
import { SysDatabaseApi } from '/@/api-services/api';

const emits = defineEmits(['handleQueryColumn']);

const props = defineProps({
	applicationNamespaces: { type: Array },
});

const ruleFormRef = ref();
const state = reactive({
  loading: false,
	isShowDialog: false,
	ruleForm: {} as any,
	rules: { position: [{ required: true, message: 'Please select a storage location', trigger: 'blur' }] },
});

// Open pop-up window
const openDialog = (row: any) => {
	state.ruleForm.configId = row.configId;
	state.ruleForm.tableName = row.tableName;
	state.ruleForm.position = row.position;
	state.ruleForm.filterExistingData = false;
	state.isShowDialog = true;
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
    state.loading = true;
		try {
      await getAPI(SysDatabaseApi).apiSysDatabaseCreateSeedDataPost(state.ruleForm);
      closeDialog();
      ElMessage.success('Generated successfully');
    } catch (e) { /* empty */ }
    state.loading = false;
	});
};

// Export object
defineExpose({ openDialog });
</script>
