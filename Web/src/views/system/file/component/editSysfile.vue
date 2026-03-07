<template>
	<div class="sys-file-container">
		<el-dialog v-model="state.isShowDialog" draggable overflow destroy-on-close width="500px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="File name" prop="fileName" :rules="[{ required: true, message: 'File name cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.fileName" placeholder="File name" clearable />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="File type" prop="fileType">
							<el-select v-model="state.ruleForm.fileType" placeholder="Please select the file type" style="margin-bottom: 10px">
								<el-option label="Related documents" value="Related documents" />
								<el-option label="Archived File" value="Archived File" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Is it public?">
							<el-radio-group v-model="state.ruleForm.isPublic">
								<el-radio :value="false">no</el-radio>
								<el-radio :value="true">Yes</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Associated Object Name" prop="relationName">
							<el-input v-model="state.ruleForm.relationName" placeholder="Associated Object Name" clearable />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Associated object ID" prop="relationId">
							<el-input v-model="state.ruleForm.relationId" placeholder="Associated Object ID" clearable />
						</el-form-item>
					</el-col>
				</el-row>
				<el-row>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Associated ID" prop="fileName">
							<el-input v-model="state.ruleForm.belongId" placeholder="Associated ID" clearable />
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

<script lang="ts" setup name="sysEditFile">
import { reactive, ref } from 'vue';
import { ElMessage } from 'element-plus';

import { getAPI } from '/@/utils/axios-utils';
import { SysFileApi } from '/@/api-services/api';
import { SysFile } from '/@/api-services/models';

const props = defineProps({
	title: String,
	sysFileId: Number,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as SysFile,
});

// Open pop-up window
const openDialog = (row: any) => {
	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
};

// Close pop-up window
const closeDialog = () => {
	emits('handleQuery');
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
		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await getAPI(SysFileApi)
				.apiSysFileUpdatePost(state.ruleForm)
				.then((rsp: any) => {
					if (rsp.data.code == 200) {
						ElMessage.success('File information modified successfully!');
					} else {
						ElMessage.error('Failed to modify file information:' + rsp.data.message);
					}
				});
		}
		closeDialog();
	});
};

// Export object
defineExpose({ openDialog });
</script>
