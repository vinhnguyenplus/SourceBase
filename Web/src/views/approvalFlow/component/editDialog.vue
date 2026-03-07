<template>
	<div class="labApprovalFlow-container">
		<el-dialog v-model="state.isShowDialog" :width="800" draggable>
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span>{{ props.title }}</span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto" :rules="rules">
				<el-tabs>
					<el-tab-pane label="Basic Information">
						<el-row :gutter="35">
							<el-form-item v-show="false">
								<el-input v-model="state.ruleForm.id" />
							</el-form-item>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Number" prop="code">
									<el-input v-model="state.ruleForm.code" placeholder="Please enter number" maxlength="32" show-word-limit clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="name" prop="name" :rules="[{ required: true, message: 'Name cannot be empty', trigger: 'blur' }]">
									<el-input v-model="state.ruleForm.name" placeholder="Please enter a name" maxlength="32" show-word-limit clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="state" prop="status" :rules="[{ required: true, message: 'Status cannot be empty', trigger: 'blur' }]">
									<g-sys-dict code="LabStatusEnum" v-model="state.ruleForm.status" render-as="select" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Remarks" prop="remark">
									<el-input v-model="state.ruleForm.remark" placeholder="Please enter a note" type="textarea" maxlength="255" show-word-limit clearable />
								</el-form-item>
							</el-col>
						</el-row>
					</el-tab-pane>
					<el-tab-pane label="Extended information">
						<el-row :gutter="35">
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="form" prop="formJson">
									<el-input v-model="state.ruleForm.formJson" placeholder="Please enter the form" type="textarea" maxlength="4096" show-word-limit clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Process" prop="flowJson">
									<el-input v-model="state.ruleForm.flowJson" placeholder="Please enter the process" type="textarea" maxlength="4096" show-word-limit clearable />
								</el-form-item>
							</el-col>
						</el-row>
					</el-tab-pane>
				</el-tabs>
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

<script setup lang="ts">
import { reactive, ref } from 'vue';
import { ElMessage } from 'element-plus';
import type { FormRules } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { ApprovalFlowApi } from '/@/api-plugins/approvalFlow/api';

// Parameters passed from parent
var props = defineProps({
	title: {
		type: String,
		default: '',
	},
	labStatus: {
		type: Array,
		default: () => [],
	},
});
// Function passed from parent for callback
const emit = defineEmits(['reloadTable']);

// Define variable content
const ruleFormRef = ref();
const state = reactive({
	loading: false,
	isShowDialog: false,
	ruleForm: {} as any,
});

// Add other rules yourself
const rules = ref<FormRules>({
	name: [
		{
			pattern: /^(?!^[0-9].*$).*/,
			message: 'cannot start with a number',
			trigger: 'blur',
		},
	],
});

// Open pop-up window
const openDialog = async (row: any) => {
	let rowData = JSON.parse(JSON.stringify(row));
	state.ruleForm = rowData.id ? (await getAPI(ApprovalFlowApi).apiApprovalFlowDetailGet(rowData.id)).data.result : rowData;
	state.isShowDialog = true;
};

// Close pop-up window
const closeDialog = () => {
	emit('reloadTable');
	state.isShowDialog = false;
};

// Cancel
const cancel = () => {
	state.isShowDialog = false;
};

// submit
const submit = async () => {
	ruleFormRef.value.validate(async (isValid: boolean, fields?: any) => {
		if (isValid) {
			if (state.ruleForm.id == undefined || state.ruleForm.id == null || state.ruleForm.id == 0) {
				await getAPI(ApprovalFlowApi).apiApprovalFlowAddPost(state.ruleForm);
			} else {
				await getAPI(ApprovalFlowApi).apiApprovalFlowUpdatePost(state.ruleForm);
			}
			closeDialog();
		} else {
			ElMessage({
				message: `The form failed to verify at ${Object.keys(fields).length}, please modify it before submitting.`,
				type: 'error',
			});
		}
	});
};

// Expose properties or functions to parent components
defineExpose({ openDialog });
</script>

<style scoped lang="scss">
:deep(.el-select),
:deep(.el-input-number) {
	width: 100%;
}
</style>
