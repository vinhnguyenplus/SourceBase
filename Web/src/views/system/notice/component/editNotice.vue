<template>
	<div class="sys-notice-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="900px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="title" prop="title" :rules="[{ required: true, message: 'Title cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.title" placeholder="title" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Type" prop="type" :rules="[{ required: true, message: 'Type cannot be empty', trigger: 'blur' }]">
              <g-sys-dict v-model="state.ruleForm.type" code="NoticeTypeEnum" render-as="select" class="w100" filterable allow-create default-first-option />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="content" prop="content" :rules="[{ required: true, message: 'Content cannot be empty', trigger: 'blur' }]">
							<Editor v-model:get-html="state.ruleForm.content" />
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

<script lang="ts" setup name="sysNoticeEdit">
import { reactive, ref } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysNoticeApi } from '/@/api-services/api';
import { UpdateNoticeInput } from '/@/api-services/models';
import Editor from '/@/components/editor/index.vue';

const props = defineProps({
	title: String,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as UpdateNoticeInput,
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
			await getAPI(SysNoticeApi).apiSysNoticeUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysNoticeApi).apiSysNoticeAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

// Export object
defineExpose({ openDialog });
</script>
