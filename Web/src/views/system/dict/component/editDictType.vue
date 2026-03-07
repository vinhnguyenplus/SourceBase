<template>
	<div class="sys-dictType-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Dictionary Name" prop="name" :rules="[{ required: true, message: 'Dictionary name cannot be empty', trigger: 'blur' }]">							
							<g-multi-lang-Input entityName="SysDictType" fieldName="Name" :entityId="state.ruleForm.id" v-model="state.ruleForm.name" placeholder="Dictionary Name" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Dictionary Encoding" prop="code" :rules="[{ required: true, message: 'Dictionary encoding cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.code" placeholder="Dictionary Encoding" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12" class="mb20" v-if="userInfo.accountType === AccountTypeEnum.NUMBER_999">
						<el-form-item label="built-in parameters" prop="sysFlag" :rules="[{ required: true, message: 'The built-in parameter cannot be empty', trigger: 'blur' }]">
							<el-radio-group v-model="state.ruleForm.sysFlag" :disabled="state.ruleForm.sysFlag == 1 && state.ruleForm.id != undefined">
								<el-radio :value="1">Yes</el-radio>
								<el-radio :value="2">no</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12" class="mb20" v-if="userInfo.accountType === AccountTypeEnum.NUMBER_999">
						<el-form-item label="tenant dictionary" prop="isTenant" :rules="[{ required: true, message: 'Tenant dictionary cannot be empty', trigger: 'blur' }]">
							<el-radio-group v-model="state.ruleForm.isTenant" :disabled="state.ruleForm.id">
								<el-radio :value="1">Yes</el-radio>
								<el-radio :value="2">no</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="state">
							<el-radio-group v-model="state.ruleForm.status">
								<el-radio :value="1">enable</el-radio>
								<el-radio :value="2">Disable</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Sort">
							<el-input-number v-model="state.ruleForm.orderNo" placeholder="Sort" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Remarks">
							<el-input v-model="state.ruleForm.remark" placeholder="Please enter the remark content" clearable type="textarea" />
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

<script lang="ts" setup name="sysEditDictType">
import { reactive, ref } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysDictTypeApi } from '/@/api-services/api';
import {AccountTypeEnum, UpdateDictTypeInput} from '/@/api-services/models';
import {useUserInfo} from "/@/stores/userInfo";

const props = defineProps({
	title: String,
});
const userInfo = useUserInfo().userInfos;
const emits = defineEmits(['handleQuery', 'handleUpdate']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as UpdateDictTypeInput,
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
			await getAPI(SysDictTypeApi).apiSysDictTypeUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysDictTypeApi).apiSysDictTypeAddPost(state.ruleForm);
		}
		emits('handleUpdate');
		closeDialog();
	});
};

// Export object
defineExpose({ openDialog });
</script>
